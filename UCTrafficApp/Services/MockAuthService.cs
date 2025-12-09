using System.Threading.Tasks;

namespace UCTrafficApp.Services
{
    public class MockAuthService : IAuthService
    {
        public Task<AuthResult> SignInAsync(string usernameOrEmail, string password)
        {
            if (usernameOrEmail == "test" && password == "password")
            {
                return Task.FromResult(new AuthResult { Success = true });
            }

            if (usernameOrEmail == "needsverify")
            {
                return Task.FromResult(new AuthResult
                {
                    Success = false,
                    RequiresEmailVerification = true,
                    ErrorMessage = "Email not verified."
                });
            }

            return Task.FromResult(new AuthResult
            {
                Success = false,
                ErrorMessage = "Invalid username or password."
            });
        }

        public Task<AuthResult> SignUpAsync(string email, string username, string password)
        {
            return Task.FromResult(new AuthResult
            {
                Success = true,
                RequiresEmailVerification = true
            });
        }

        public Task<AuthResult> SendEmailVerificationCodeAsync(string email)
        {
            return Task.FromResult(new AuthResult
            {
                Success = true
            });
        }

        public Task<AuthResult> VerifyEmailCodeAsync(string email, string code)
        {
            if (code == "123456")
            {
                return Task.FromResult(new AuthResult { Success = true });
            }

            return Task.FromResult(new AuthResult
            {
                Success = false,
                ErrorMessage = "Invalid verification code."
            });
        }
    }
}
