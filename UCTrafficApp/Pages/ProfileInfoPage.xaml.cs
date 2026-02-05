namespace UCTrafficApp.Pages;

public partial class ProfileInfoPage : ContentPage
{
    public ProfileInfoPage()
    {
        InitializeComponent();
    }

    private async void Settings_Tapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(SettingsPage));

    private async void MyProfile_Tapped(object sender, EventArgs e)
        => await DisplayAlert("My Profile", "Placeholder", "OK");

    private async void Notifications_Tapped(object sender, EventArgs e)
        => await DisplayAlert("Notifications", "Placeholder", "OK");

    private async void FAQ_Tapped(object sender, EventArgs e)
        => await DisplayAlert("FAQ", "Placeholder", "OK");

    private async void Logout_Clicked(object sender, EventArgs e)
        => await DisplayAlert("Logout", "Placeholder", "OK");
}

