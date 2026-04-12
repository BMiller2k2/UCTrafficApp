using Microsoft.Maui.Storage;

namespace SettingsApp;

public partial class PrivacyPage : ContentPage
{
    public PrivacyPage()
    {
        InitializeComponent();

        // Load saved values
        LocationSwitch.IsToggled = Preferences.Get("LocationAccess", false);
        AnalyticsSwitch.IsToggled = Preferences.Get("AnalyticsData", false);
        ProfileVisibleSwitch.IsToggled = Preferences.Get("ProfileVisible", true);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        Preferences.Set("LocationAccess", LocationSwitch.IsToggled);
        Preferences.Set("AnalyticsData", AnalyticsSwitch.IsToggled);
        Preferences.Set("ProfileVisible", ProfileVisibleSwitch.IsToggled);

        await DisplayAlert("Saved", "Privacy settings updated.", "OK");
    }
}
