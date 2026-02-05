namespace UCTrafficApp.Pages;

public partial class ReportIssueThanksPage : ContentPage
{
    public ReportIssueThanksPage()
    {
        InitializeComponent();
    }

    private async void Return_Clicked(object sender, EventArgs e)
    {
        // Pops back to where you came from (Settings/Profile flow)
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync("..");
    }
}
