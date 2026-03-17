using UCTrafficApp.Pages;
using UCTrafficApp.Services;

namespace UCTrafficApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // 1. Register Routes
            Routing.RegisterRoute("AccountPage", typeof(AccountPage));
            Routing.RegisterRoute(nameof(ReportIssuePage), typeof(ReportIssuePage));
            Routing.RegisterRoute("Email_verification", typeof(Email_verification));
            Routing.RegisterRoute("Email_verified", typeof(Email_verified));
            Routing.RegisterRoute("Account_lock", typeof(Account_lock));

            // 2. Check login state on startup
            // This reads the value you set in Sign_in.xaml.cs
            bool isSignedIn = Preferences.Get("IsSignedIn", false);

            // Set the visibility of the public Tab we named in XAML
            SettingsTab.IsVisible = isSignedIn;
        }
    }
}