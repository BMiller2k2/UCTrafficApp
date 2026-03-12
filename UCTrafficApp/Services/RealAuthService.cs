using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UCTrafficApp.Data;
using UCTrafficApp.Models;

namespace UCTrafficApp.Services
{
    public class RealAuthService : IAuthService
    {
        private readonly DatabaseService _db;
        private readonly EmailService _email;

        private const int MaxFailedAttempts = 3;
        private const int LockMinutes = 15;

        public RealAuthService(DatabaseService db, EmailService email)
        {
            _db = db;
            _email = email;
        }

        private static string Hash(string s) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)));

        public async Task<AuthResult> SignUpAsync(string email, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new AuthResult { Success = false, ErrorMessage = "Email and password are required." };

            var existing = await _db.GetUserByEmailAsync(email);
            var user = existing ?? new UserModel { Email = email };

            user.PasswordHash = Hash(password);
            user.IsVerified = false;
            user.FailedAttempts = 0;
            user.LockoutUntil = null;

            if (existing == null) await _db.SaveUserAsync(user);
            else await _db.UpdateUserAsync(user);

            return new AuthResult { Success = true, RequiresEmailVerification = true };
        }

        public async Task<AuthResult> SendEmailVerificationCodeAsync(string email)
        {
            var user = await _db.GetUserByEmailAsync(email);
            if (user == null)
                return new AuthResult { Success = false, ErrorMessage = "User not found." };

            var code = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = code;
            user.CodeGeneratedAt = DateTime.UtcNow;
            await _db.UpdateUserAsync(user);

            try
            {
                await _email.SendVerificationCodeAsync(email, code);
                return new AuthResult { Success = true };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, ErrorMessage = $"Email failed: {ex.Message}" };
            }
        }

        public async Task<AuthResult> VerifyEmailCodeAsync(string email, string code)
        {
            var normalizedEmail = (email ?? string.Empty).Trim();
            var normalizedCode = (code ?? string.Empty).Trim();

            var user = await _db.GetUserByEmailAsync(normalizedEmail);
            if (user == null)
                return new AuthResult { Success = false, ErrorMessage = "User not found." };

            if (string.IsNullOrEmpty(user.VerificationCode))
                return new AuthResult { Success = false, ErrorMessage = "No code on record. Please resend." };

            // compare safely
            if (string.Equals(user.VerificationCode.Trim(), normalizedCode, StringComparison.OrdinalIgnoreCase))
            {
                user.IsVerified = true;
                user.VerificationCode = null;
                await _db.UpdateUserAsync(user);
                return new AuthResult { Success = true };
            }

            return new AuthResult { Success = false, ErrorMessage = "Incorrect code." };
        }


        public async Task<AuthResult> SignInAsync(string usernameOrEmail, string password)
        {
            var email = usernameOrEmail?.Trim();
            var user = await _db.GetUserByEmailAsync(email);

            if (user == null)
                return new AuthResult { Success = false, ErrorMessage = "Account not found." };

            // If already locked, report remaining time
            if (user.LockoutUntil.HasValue && user.LockoutUntil.Value > DateTime.UtcNow)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Account locked.",
                    LockoutUntilUtc = new DateTimeOffset(user.LockoutUntil.Value, TimeSpan.Zero)
                };
            }

            // wrong password path …
            if (Hash(password) != user.PasswordHash)
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 3)
                {
                    user.FailedAttempts = 0;
                    user.LockoutUntil = DateTime.UtcNow.AddMinutes(1);
                    await _db.UpdateUserAsync(user);

                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Account locked.",
                        LockoutUntilUtc = new DateTimeOffset(user.LockoutUntil.Value, TimeSpan.Zero)
                    };
                }

                await _db.UpdateUserAsync(user);
                return new AuthResult { Success = false, ErrorMessage = "Invalid credentials." };
            }

            // success
            user.FailedAttempts = 0;
            user.LockoutUntil = null;
            await _db.UpdateUserAsync(user);
            return new AuthResult { Success = true };
        }

    }
}

