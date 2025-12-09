using System.Threading.Tasks;

namespace UCTrafficApp.Services
{
    public interface IAuthService
    {
        Task<AuthResult> SignInAsync(string usernameOrEmail, string password);
        Task<AuthResult> SignUpAsync(string email, string username, string password);
        Task<AuthResult> SendEmailVerificationCodeAsync(string email);
        Task<AuthResult> VerifyEmailCodeAsync(string email, string code);
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public bool RequiresEmailVerification { get; set; }
        public string? ErrorMessage { get; set; }

        // NEW: when locked, set this to the UTC time the lock ends
        public DateTimeOffset? LockoutUntilUtc { get; set; }
    }

}
