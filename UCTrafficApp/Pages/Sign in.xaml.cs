using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using UCTrafficApp.Services;
using System;

namespace UCTrafficApp.Pages
{
    public partial class Sign_in : ContentPage
    {
        private readonly IAuthService _authService;

        public Sign_in()
        {
            InitializeComponent();
            _authService = App.Services.GetRequiredService<IAuthService>();
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            var emailOrUser = UsernameEntry.Text?.Trim() ?? string.Empty;
            var password = PasswordEntry.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(emailOrUser) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Missing info", "Please enter both username and password.", "OK");
                return;
            }

            var btn = (Button)sender;
            btn.IsEnabled = false;

            try
            {
                var result = await _authService.SignInAsync(emailOrUser, password);

                // ? success ? go to your Home tab/page
                if (result.Success)
                {
                    await Shell.Current.GoToAsync("//Home/HomePage");
                    return;
                }

                // ?? verification required ? go to email verification and pass the email
                if (result.RequiresEmailVerification)
                {
                    await Shell.Current.GoToAsync(
                        $"//Account/EmailVerificationPage?email={Uri.EscapeDataString(emailOrUser)}");
                    return;
                }

                // ? lockout ? redirect to lock page with countdown if available
                // Prefer a typed property (LockoutUntilUtc); if you haven't added it yet, we also check the message text.
                DateTimeOffset until;
                if (result is { LockoutUntilUtc: not null } &&
                    DateTimeOffset.TryParse(result.LockoutUntilUtc!.Value.ToString("o"), out until))
                {
                    var untilIso = Uri.EscapeDataString(until.UtcDateTime.ToString("o"));
                    await Shell.Current.GoToAsync(
                        $"//Account/AccountLockPage?email={Uri.EscapeDataString(emailOrUser)}&until={untilIso}");
                    return;
                }
                else if ((result.ErrorMessage ?? string.Empty)
                            .Contains("lock", StringComparison.OrdinalIgnoreCase))
                {
                    // Fallback if you didn't add LockoutUntilUtc to AuthResult
                    await Shell.Current.GoToAsync("//Account/AccountLockPage");
                    return;
                }

                // default: show error
                await DisplayAlert("Sign in failed", result.ErrorMessage ?? "Unable to sign in.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }
    }
}
