using Microsoft.Maui.Storage;

namespace SettingsApp;

public partial class PreferencesPage : ContentPage
{
    public PreferencesPage()
    {
        InitializeComponent();

        // Load saved values
        DarkModeSwitch.IsToggled = Preferences.Get("DarkMode", false);
        NotificationSwitch.IsToggled = Preferences.Get("Notifications", true);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        Preferences.Set("DarkMode", DarkModeSwitch.IsToggled);
        Preferences.Set("Notifications", NotificationSwitch.IsToggled);

        // Apply Dark Mode instantly
        Application.Current.UserAppTheme =
            DarkModeSwitch.IsToggled ? AppTheme.Dark : AppTheme.Light;

        await DisplayAlert("Saved", "Preferences updated.", "OK");
    }
}
