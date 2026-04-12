using Microsoft.Maui.Controls;

namespace SettingsApp;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnAccountClicked(object sender, EventArgs e)
        => await DisplayAlert("Account", "Account settings will be added soon.", "OK");

    private async void OnPrivacyClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(PrivacyPage));
        
    private async void OnPreferencesClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(PreferencesPage));

    private async void OnAboutClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(AboutPage));

    private async void OnReportIssueClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(ReportIssuePage));

}
