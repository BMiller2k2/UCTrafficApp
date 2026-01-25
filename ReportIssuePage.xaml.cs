using System.Text.Json;
using Microsoft.Maui.Controls;

namespace SettingsApp;

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

        // Example: Create JSON object
        var feedbackData = new
        {
            message = feedback,
            timestamp = DateTime.Now.ToString("u")
        };

        // Serialize feedback to JSON string
        string json = JsonSerializer.Serialize(feedbackData, new JsonSerializerOptions { WriteIndented = true });

        // Show JSON as confirmation (for now)
        await DisplayAlert("Feedback Submitted", $"Your feedback has been captured:\n\n{json}", "OK");

        // Clear editor
        FeedbackEditor.Text = string.Empty;
    }
}
