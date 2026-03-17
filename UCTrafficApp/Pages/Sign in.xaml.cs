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
                    // 1. Update the local state
                    Preferences.Set("IsSignedIn", true);

                    // ADDED: Save the email so the AccountPage can display it
                    Preferences.Set("UserEmail", emailOrUser);

                    // 2. Cast Shell.Current to access SettingsTab visibility
                    if (Shell.Current is AppShell mainShell)
                    {
                        mainShell.SettingsTab.IsVisible = true;
                    }

                    await Shell.Current.GoToAsync("//Home/HomePage");
                    return;
                }

                // ?? verification required ?
                if (result.RequiresEmailVerification)
                {
                    await Shell.Current.GoToAsync($"EmailVerificationPage?email={Uri.EscapeDataString(emailOrUser)}");
                    return;
                }

                // ? lockout ?
                DateTimeOffset until;
                if (result is { LockoutUntilUtc: not null } &&
                    DateTimeOffset.TryParse(result.LockoutUntilUtc!.Value.ToString("o"), out until))
                {
                    var untilIso = Uri.EscapeDataString(until.UtcDateTime.ToString("o"));
                    await Shell.Current.GoToAsync(
                        $"//Account/AccountLockPage?email={Uri.EscapeDataString(emailOrUser)}&until={untilIso}");
                    return;
                }
                else if ((result.ErrorMessage ?? string.Empty).Contains("lock", StringComparison.OrdinalIgnoreCase))
                {
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

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Account/SignUpPage");
            return;
        }
    }
}