
using UCTrafficApp.Models;
using Microsoft.Maui.Storage;
using SQLite;
using System.IO;
using UCTrafficApp.Data;

namespace UCTrafficApp
{
    public partial class App : Application
    {
        private readonly sqliteConnection _sqliteConnection;
        public static IServiceProvider Services { get; set; }

        public App(IServiceProvider provider)
        {
            InitializeComponent();
            Services = provider;

            MainPage = new AppShell();

        }

        //protected override async void OnStart()
        //{
        //    ISQLiteAsyncConnection database = _sqliteConnection.CreateConnection();

        //    await database.CreateTableAsync<UserDto>();

        //    base.OnStart();
        //}

        // 🔹 Add this reset helper method
        public static void ResetDatabase()
        {
            try
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "users.db3");

                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    Application.Current?.MainPage?.DisplayAlert("Database Reset", "The local database was cleared successfully.", "OK");
                }
                else
                {
                    Application.Current?.MainPage?.DisplayAlert("Database Reset", "No database found to delete.", "OK");
                }
            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlert("Error", $"Failed to reset database: {ex.Message}", "OK");
            }
        }
    }
}
