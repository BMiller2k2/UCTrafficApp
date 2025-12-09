using Microsoft.Extensions.DependencyInjection;
using UCTrafficApp.Services;

namespace UCTrafficApp.Pages
{
    [QueryProperty(nameof(Email), "email")]
    [QueryProperty(nameof(UntilIso), "until")]
    public partial class Account_lock : ContentPage
    {
        private readonly IAuthService _auth;
        private DateTimeOffset _lockoutUntilUtc;

        public string Email { get; set; } = "";
        public string UntilIso
        {
            get => _lockoutUntilUtc.ToString("o");
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && DateTimeOffset.TryParse(value, out var dto))
                    _lockoutUntilUtc = dto.ToUniversalTime();
            }
        }

        public Account_lock()
        {
            InitializeComponent();
            _auth = App.Services.GetRequiredService<IAuthService>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessageLabel.Text = "Password or Username is incorrect!";
            StartTimer();
        }

        private void StartTimer()
        {
            UpdateCountdown();
            Application.Current!.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                UpdateCountdown();
                return DateTimeOffset.UtcNow < _lockoutUntilUtc;
            });
        }

        private void UpdateCountdown()
        {
            var now = DateTimeOffset.UtcNow;
            var remaining = _lockoutUntilUtc - now;
            if (remaining <= TimeSpan.Zero)
            {
                CountdownLabel.Text = "You can try again now.";
                ContinueButton.IsEnabled = true;
            }
            else
            {
                CountdownLabel.Text = $"Try again in {remaining.Minutes:D2}:{remaining.Seconds:D2}";
                ContinueButton.IsEnabled = false;
            }
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Account/SignInPage");
        }
    }
}
