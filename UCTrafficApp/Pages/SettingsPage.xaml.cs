namespace UCTrafficApp.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        
    }

    private async void ReportIssue_Clicked(object sender, EventArgs e)
{
    await Shell.Current.GoToAsync(nameof(ReportIssuePage));
}

}
