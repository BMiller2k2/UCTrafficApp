using SQLite;
using UCTrafficApp.Models;

namespace UCTrafficApp.Data
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string dbPath)
        {
            // Initialize SQLite Tables and connection
            SQLitePCL.Batteries_V2.Init();
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<UserModel>();
            _db.CreateTableAsync<UserDto>();

        }

        public Task<UserModel> GetUserByEmailAsync(string email) =>
            _db.Table<UserModel>().Where(u => u.Email == email).FirstOrDefaultAsync();

        public Task<int> SaveUserAsync(UserModel user) =>
            _db.InsertOrReplaceAsync(user);

        public Task<int> UpdateUserAsync(UserModel user) =>
            _db.UpdateAsync(user);


    }
}
