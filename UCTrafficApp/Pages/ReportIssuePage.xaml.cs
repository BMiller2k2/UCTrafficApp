namespace UCTrafficApp.Pages;

public partial class ReportIssuePage : ContentPage
{
    public ReportIssuePage()
    {
        InitializeComponent();
    }

    private async void Submit_Clicked(object sender, EventArgs e)
    {
        var text = IssueEditor.Text?.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            await DisplayAlert("Missing Input", "Please enter an issue before submitting.", "OK");
            return;
        }

        await Shell.Current.GoToAsync(nameof(ReportIssueThanksPage));
    }
}
