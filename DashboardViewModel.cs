using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace RadioControlApp.ViewModels;

/// <summary>
/// ViewModel dla ekranu Dashboardu, wyświetlający status urządzenia i dane w czasie rzeczywistym.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string _deviceName = "Radio Device";

    [ObservableProperty]
    private string _connectionStatus = "Połączono";

    [ObservableProperty]
    private bool _hasNotification;

    [ObservableProperty]
    private string _notificationMessage;

    [ObservableProperty]
    private ISeries[] _signalStrengthSeries;

    [ObservableProperty]
    private Axis[] _timeAxis;

    public DashboardViewModel()
    {
        // Inicjalizacja przykładowych danych dla wykresu
        SignalStrengthSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = new double[] { 50, 60, 55, 70, 65 },
                Name = "Siła sygnału"
            }
        };
        TimeAxis = new Axis[]
        {
            new Axis { Name = "Czas", Labels = new[] { "10:00", "10:01", "10:02", "10:03", "10:04" } }
        };
    }

    /// <summary>
    /// Nawiguje do ekranu konfiguracji kodów IR.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToIRConfig()
    {
        await Shell.Current.GoToAsync("//IRConfigurationPage");
    }

    /// <summary>
    /// Nawiguje do ekranu konfiguracji stacji FM.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToFMConfig()
    {
        await Shell.Current.GoToAsync("//FMConfigurationPage");
    }
}