using System.Net.Http.Json;
using System.Text.Json;
using UCTrafficApp.Models;

namespace UCTrafficApp.Services
{
    public interface ITrafficService
    {
        Task<List<TrafficDataModel>> GetTrafficDataAsync();
        Task RefreshTrafficDataAsync();
    }

    public class TrafficService : ITrafficService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://your-api-endpoint.com/api"; // TODO: Replace with your actual API endpoint
        private const string TrafficEndpoint = "/traffic";

        public TrafficService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<TrafficDataModel>> GetTrafficDataAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}{TrafficEndpoint}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var trafficData = JsonSerializer.Deserialize<List<TrafficDataModel>>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return trafficData ?? new List<TrafficDataModel>();
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current?.MainPage?.DisplayAlert("Error", 
                            $"Failed to fetch traffic data: {response.StatusCode}", "OK");
                    });
                    return new List<TrafficDataModel>();
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current?.MainPage?.DisplayAlert("Error", 
                        $"Error fetching traffic data: {ex.Message}", "OK");
                });
                return new List<TrafficDataModel>();
            }
        }

        public async Task RefreshTrafficDataAsync()
        {
            await GetTrafficDataAsync();
        }
    }
}
