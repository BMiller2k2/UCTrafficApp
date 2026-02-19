using UCTrafficApp.Pages;
using UCTrafficApp.Services;

namespace UCTrafficApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            bool isSignedIn = false;

            Routing.RegisterRoute(nameof(ReportIssuePage), typeof(ReportIssuePage));

            Routing.RegisterRoute("Email_verification", typeof(Email_verification));

            Routing.RegisterRoute("Email_verified", typeof(Email_verified));

            Routing.RegisterRoute("Account_lock", typeof(Account_lock));
        }
    }
}
