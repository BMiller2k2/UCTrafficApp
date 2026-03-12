using System.Text.Json;
using Microsoft.Maui.Controls;
using UCTrafficApp.Data;

namespace UCTrafficApp.Pages;

public partial class ReportIssuePage : ContentPage
{
    public ReportIssuePage()
    {
        InitializeComponent();
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        string feedback = FeedbackEditor.Text?.Trim();

        if (string.IsNullOrWhiteSpace(feedback))
        {
            await DisplayAlert("Error", "Please enter your feedback before submitting.", "OK");
            return;
        }


        var repo = App.Services.GetService<DatabaseCrudOperations>();

        string text = FeedbackEditor.Text;

        await repo.SaveIssueAsync(text);

        await DisplayAlert("Saved", "Your text was saved to the database.", "OK");

        // Clear editor
        FeedbackEditor.Text = string.Empty;
    }
}
