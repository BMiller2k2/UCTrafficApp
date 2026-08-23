using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using UCTrafficApp.Models;
using UCTrafficApp.Services;

namespace UCTrafficApp.Pages
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        private readonly ITrafficService _trafficService;
        private ObservableCollection<TrafficDataModel> _trafficDataList;
        private bool _isLoading;
        private string _statusMessage;

        public ObservableCollection<TrafficDataModel> TrafficDataList
        {
            get => _trafficDataList;
            set
            {
                if (_trafficDataList != value)
                {
                    _trafficDataList = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public IAsyncRelayCommand RefreshCommand { get; }

        public HomePageViewModel(ITrafficService trafficService)
        {
            _trafficService = trafficService;
            _trafficDataList = new ObservableCollection<TrafficDataModel>();
            RefreshCommand = new AsyncRelayCommand(LoadTrafficDataAsync);
        }

        public async Task LoadTrafficDataAsync()
        {
            if (IsLoading)
                return;

            try
            {
                IsLoading = true;
                StatusMessage = "Loading traffic data...";

                var trafficData = await _trafficService.GetTrafficDataAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TrafficDataList.Clear();
                    foreach (var item in trafficData)
                    {
                        TrafficDataList.Add(item);
                    }

                    if (trafficData.Count == 0)
                    {
                        StatusMessage = "No traffic data available";
                    }
                    else
                    {
                        StatusMessage = $"Updated: {DateTime.Now:HH:mm:ss}";
                    }
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Generic AsyncRelayCommand implementation for MAUI
    public class AsyncRelayCommand : IAsyncRelayCommand
    {
        private readonly Func<Task> _execute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<Task> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => !_isExecuting;

        public async void Execute(object parameter)
        {
            try
            {
                _isExecuting = true;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public interface IAsyncRelayCommand : ICommand
    {
    }
}
