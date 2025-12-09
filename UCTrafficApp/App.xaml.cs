using Microsoft.Maui.Storage;
using System.IO;

namespace UCTrafficApp
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; set; }

        public App(IServiceProvider provider)
        {
            InitializeComponent();
            Services = provider;

            MainPage = new AppShell();
        }

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
