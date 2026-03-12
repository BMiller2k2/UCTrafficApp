using SQLite;
using UCTrafficApp.Models;

namespace UCTrafficApp.Data
{
    public class DatabaseCrudOperations
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseCrudOperations(string dbPath)
        {
            // Initialize SQLite Tables and connection
            SQLitePCL.Batteries_V2.Init();
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<UserModel>();
            _db.CreateTableAsync<UserDto>();
            _db.CreateTableAsync<IssueDTO>();

        }

        public Task<UserModel> GetUserByEmailAsync(string email) =>
            _db.Table<UserModel>().Where(u => u.Email == email).FirstOrDefaultAsync();

        public Task<int> SaveUserAsync(UserModel user) =>
            _db.InsertOrReplaceAsync(user);

        public Task<int> UpdateUserAsync(UserModel user) =>
            _db.UpdateAsync(user);
        public async Task<int> SaveIssueAsync(string text)
        {
            var issue = new IssueDTO
            {
                IssueText = text
            };

            return await _db.InsertAsync(issue);
        }

    }
}
