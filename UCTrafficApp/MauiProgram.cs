using Microsoft.Extensions.Logging;
using UCTrafficApp.Services;
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using UCTrafficApp.Data;

namespace UCTrafficApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>() // Removed .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // SQLite database path (cross-platform safe)

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<sqliteConnection>();//database connection
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "UCTrafficApp.db3");
            builder.Services.AddSingleton(new DatabaseService(dbPath));   // Database
            builder.Services.AddSingleton<EmailService>();                // Email sender
            builder.Services.AddSingleton<IAuthService, RealAuthService>(); // Real authentication with DB + lockout
            var app = builder.Build();

            // Make services globally accessible via App.Services
            App.Services = app.Services;

            return app;
        }
    }
}
