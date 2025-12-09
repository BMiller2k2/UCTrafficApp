using Microsoft.Extensions.DependencyInjection;
using UCTrafficApp.Services;

namespace UCTrafficApp.Pages
{
    [QueryProperty(nameof(Email), "email")]
    public partial class Email_verification : ContentPage
    {
        private readonly IAuthService _auth;

        private string _email = string.Empty;
        public string Email            // receives the query param
        {
            get => _email;
            set => _email = value?.Trim() ?? string.Empty;
        }

        public Email_verification()
        {
            InitializeComponent();
            _auth = App.Services.GetRequiredService<IAuthService>();
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            var code = CodeEntry.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_email))
            {
                await DisplayAlert("Invalid Code", "User not found (no email provided).", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                await DisplayAlert("Invalid Code", "Please enter the code.", "OK");
                return;
            }

            var result = await _auth.VerifyEmailCodeAsync(_email, code);
            if (!result.Success)
            {
                await DisplayAlert("Invalid Code", result.ErrorMessage ?? "Incorrect code.", "OK");
                return;
            }

            await Shell.Current.GoToAsync("//Account/EmailVerifiedPage");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Account/SignInPage");
        }
    }
}
