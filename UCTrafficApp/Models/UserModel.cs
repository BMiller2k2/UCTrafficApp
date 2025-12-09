using SQLite;

namespace UCTrafficApp.Models
{
    public class UserModel
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Unique, NotNull] public string Email { get; set; }
        [NotNull] public string PasswordHash { get; set; }

        public bool IsVerified { get; set; }
        public string VerificationCode { get; set; }
        public DateTime CodeGeneratedAt { get; set; }

        public int FailedAttempts { get; set; }
        public DateTime? LockoutUntil { get; set; }
    }
}
