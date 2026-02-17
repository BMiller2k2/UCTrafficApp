using SQLite;
using UCTrafficApp.Models;

namespace UCTrafficApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string dbPath)
        {
            // Initialize SQLite
            SQLitePCL.Batteries_V2.Init();
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<UserModel>().Wait();
            _db.CreateTableAsync<UserDto>().Wait();

        }

        public Task<UserModel> GetUserByEmailAsync(string email) =>
            _db.Table<UserModel>().Where(u => u.Email == email).FirstOrDefaultAsync();

        public Task<int> SaveUserAsync(UserModel user) =>
            _db.InsertOrReplaceAsync(user);

        public Task<int> UpdateUserAsync(UserModel user) =>
            _db.UpdateAsync(user);


    }
}
