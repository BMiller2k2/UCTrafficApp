using Microsoft.Maui.Storage;

namespace UCTrafficApp.Pages;

public partial class AccountPage : ContentPage
{
    public AccountPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Retrieve the saved email and update the label
        // "UserEmailLabel" matches the x:Name we put in the XAML
        string email = Preferences.Get("UserEmail", "Unknown User");
        UserEmailLabel.Text = email;
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        // 1. Confirm with the user
        bool confirm = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");

        if (confirm)
        {
            // 2. Clear persistent login state and the saved email
            Preferences.Set("IsSignedIn", false);
            Preferences.Remove("UserEmail");

            // 3. Update AppShell to hide the Settings tab
            if (Shell.Current is AppShell mainShell)
            {
                mainShell.SettingsTab.IsVisible = false;
            }

            // 4. Redirect to Sign In page
            // The "///" prefix resets the navigation stack
            await Shell.Current.GoToAsync("///Account/SignInPage");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Navigate back to the previous page in the stack (SettingsPage)
        await Shell.Current.GoToAsync("..");
    }
}