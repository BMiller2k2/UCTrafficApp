using UCTrafficApp.Pages;

namespace UCTrafficApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            bool isSignedIn = false;

            Routing.RegisterRoute(nameof(ReportIssuePage), typeof(ReportIssuePage));

        }
    }
}
