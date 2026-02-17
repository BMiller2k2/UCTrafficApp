using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;   // for App.Services.GetService<T>()
using UCTrafficApp.Services;

namespace UCTrafficApp.Pages
{
    public partial class Sign_up : ContentPage
    {
        private readonly IAuthService _authService;

        public Sign_up()
        {
            InitializeComponent();

            // Resolve the authentication service (falls back to mock if not registered)
            _authService = App.Services.GetService<IAuthService>() ?? new MockAuthService();
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            try
            {
                button!.IsEnabled = false;

                string email = (EmailEntry?.Text ?? "").Trim();       // <-- matches XAML x:Name
                string password = PasswordEntry?.Text ?? "";

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Missing Info", "Please enter an email and password.", "OK");
                    return;
                }

                // Create the user via your auth service
                var signUpResult = await _authService.SignUpAsync(email, email, password);
                if (!signUpResult.Success)
                {
                    await DisplayAlert("Sign Up Failed", signUpResult.ErrorMessage ?? "An error occurred.", "OK");
                    return;
                }

                // Send verification code
                var sendResult = await _authService.SendEmailVerificationCodeAsync(email);
                if (!sendResult.Success)
                {
                    await DisplayAlert("Email Error", sendResult.ErrorMessage ?? "Could not send verification code.", "OK");
                    return;
                }

                // Navigate to verification page and pass the email
                await Shell.Current.GoToAsync($"Email_verification?email={Uri.EscapeDataString(email)}");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            }
            finally
            {
                if (button != null) button.IsEnabled = true;
            }
        }
    }
}
