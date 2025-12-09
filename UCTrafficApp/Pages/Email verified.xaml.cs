using Microsoft.Maui.Controls;

namespace UCTrafficApp.Pages
{
    public partial class Email_verified : ContentPage
    {
        public Email_verified()
        {
            InitializeComponent();
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            // Return to the Sign In page inside the Account tab.
            await Shell.Current.GoToAsync("//Account/SignInPage");
        }
    }
}
