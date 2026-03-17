using Microsoft.Maui.Controls;

namespace UCTrafficApp.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnAccountClicked(object sender, EventArgs e)
    {
        // This replaces the DisplayAlert with the actual navigation command
        await Shell.Current.GoToAsync("AccountPage");
    }

    private async void OnPrivacyClicked(object sender, EventArgs e)
        => await DisplayAlert("Privacy", "Privacy settings will be added soon.", "OK");

    private async void OnPreferencesClicked(object sender, EventArgs e)
        => await DisplayAlert("Preferences", "Preferences settings will be added soon.", "OK");

    private async void OnReportIssueClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(ReportIssuePage));
}