using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using RadioControlApp.Models;
using RadioControlApp.Services;
using System.Collections.ObjectModel;

namespace RadioControlApp.ViewModels;

/// <summary>
/// ViewModel dla ekranu Dashboardu, wyświetlający status urządzenia i dane w czasie rzeczywistym.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly IRadioDeviceService _radioService;
    private readonly ILocalStorageService _storageService;
    private readonly ObservableCollection<double> _signalData = new();
    private readonly ObservableCollection<string> _timeLabels = new();

    [ObservableProperty]
    private string _deviceName = "Radio Device";

    [ObservableProperty]
    private string _deviceId = "default-device";

    [ObservableProperty]
    private string _connectionStatus = "Rozłączono";

    [ObservableProperty]
    private bool _isOnline;

    [ObservableProperty]
    private DeviceStatus? _deviceStatus;

    [ObservableProperty]
    private bool _hasNotification;

    [ObservableProperty]
    private string _notificationMessage = string.Empty;

    [ObservableProperty]
    private ISeries[] _signalStrengthSeries;

    [ObservableProperty]
    private Axis[] _timeAxis;

    [ObservableProperty]
    private bool _isMonitoring;

    [ObservableProperty]
    private int _totalIRCodes;

    [ObservableProperty]
    private int _totalFMStations;

    [ObservableProperty]
    private DateTime _lastUpdate;

    public DashboardViewModel(IRadioDeviceService radioService, ILocalStorageService storageService)
    {
        _radioService = radioService;
        _storageService = storageService;

        InitializeCharts();
        LoadDeviceDataCommand.Execute(null);
    }

    private void InitializeCharts()
    {
        // Inicjalizacja wykresu siły sygnału
        SignalStrengthSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = _signalData,
                Name = "Siła sygnału (%)",
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        };

        TimeAxis = new Axis[]
        {
            new Axis 
            { 
                Name = "Czas", 
                Labels = _timeLabels,
                TextSize = 10
            }
        };

        // Dodaj początkowe dane
        AddInitialData();
    }

    private void AddInitialData()
    {
        var now = DateTime.Now;
        for (int i = 9; i >= 0; i--)
        {
            _signalData.Add(0);
            _timeLabels.Add(now.AddMinutes(-i).ToString("HH:mm"));
        }
    }

    /// <summary>
    /// Ładuje dane urządzenia i rozpoczyna monitorowanie.
    /// </summary>
    [RelayCommand]
    private async Task LoadDeviceData()
    {
        try
        {
            // Pobierz informacje o urządzeniu
            var device = await _radioService.GetDeviceInfoAsync(DeviceId);
            if (device != null)
            {
                DeviceName = device.Name;
                IsOnline = device.IsOnline;
                ConnectionStatus = device.IsOnline ? "Połączono" : "Rozłączono";
                TotalIRCodes = device.StoredIRCodes?.Count ?? 0;
                TotalFMStations = device.FMStations?.Count ?? 0;
            }

            // Pobierz aktualny status
            DeviceStatus = await _radioService.GetDeviceStatusAsync(DeviceId);
            LastUpdate = DateTime.Now;

            // Pobierz liczby z lokalnej pamięci jako backup
            var localCodes = await _storageService.LoadAsync<List<IRCode>>("ir_codes") ?? new List<IRCode>();
            var localStations = await _storageService.LoadAsync<List<FMStation>>("fm_stations") ?? new List<FMStation>();
            
            if (TotalIRCodes == 0) TotalIRCodes = localCodes.Count;
            if (TotalFMStations == 0) TotalFMStations = localStations.Count;

            ShowNotification("Dane urządzenia zostały załadowane", false);
        }
        catch (Exception ex)
        {
            ConnectionStatus = "Błąd połączenia";
            IsOnline = false;
            ShowNotification($"Błąd ładowania danych: {ex.Message}", true);
        }
    }

    /// <summary>
    /// Rozpoczyna/zatrzymuje monitorowanie urządzenia w czasie rzeczywistym.
    /// </summary>
    [RelayCommand]
    private async Task ToggleMonitoring()
    {
        if (IsMonitoring)
        {
            await StopMonitoring();
        }
        else
        {
            await StartMonitoring();
        }
    }

    private async Task StartMonitoring()
    {
        try
        {
            IsMonitoring = true;
            ShowNotification("Rozpoczęto monitorowanie w czasie rzeczywistym", false);

            await _radioService.StartMonitoringAsync(DeviceId, OnMonitoringDataReceived);
        }
        catch (Exception ex)
        {
            IsMonitoring = false;
            ShowNotification($"Błąd rozpoczęcia monitorowania: {ex.Message}", true);
        }
    }

    private async Task StopMonitoring()
    {
        try
        {
            IsMonitoring = false;
            await _radioService.StopMonitoringAsync(DeviceId);
            ShowNotification("Zatrzymano monitorowanie", false);
        }
        catch (Exception ex)
        {
            ShowNotification($"Błąd zatrzymania monitorowania: {ex.Message}", true);
        }
    }

    private void OnMonitoringDataReceived(MonitoringData data)
    {
        // Aktualizuj wykres siły sygnału
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_signalData.Count >= 10)
            {
                _signalData.RemoveAt(0);
                _timeLabels.RemoveAt(0);
            }

            _signalData.Add(data.SignalStrength);
            _timeLabels.Add(data.Timestamp.ToString("HH:mm"));

            // Aktualizuj status urządzenia
            if (DeviceStatus != null)
            {
                DeviceStatus.SignalStrength = data.SignalStrength;
                DeviceStatus.CurrentFrequency = data.Frequency;
                DeviceStatus.Temperature = data.Temperature;
                DeviceStatus.LastUpdate = data.Timestamp;
            }

            LastUpdate = data.Timestamp;
            ConnectionStatus = "Połączono (Live)";
            IsOnline = true;

            // Sprawdź alerty
            CheckAlerts(data);
        });
    }

    private void CheckAlerts(MonitoringData data)
    {
        // Alert niskiej siły sygnału
        if (data.SignalStrength < 20)
        {
            ShowNotification("Uwaga: Bardzo niska siła sygnału!", true);
        }
        // Alert wysokiej temperatury
        else if (data.Temperature > 80)
        {
            ShowNotification("Uwaga: Wysoka temperatura urządzenia!", true);
        }
        // Wyczysć powiadomienia jeśli wszystko w porządku
        else if (HasNotification && !NotificationMessage.Contains("Błąd"))
        {
            ClearNotification();
        }
    }

    /// <summary>
    /// Nawiguje do ekranu konfiguracji kodów IR.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToIRConfig()
    {
        await Shell.Current.GoToAsync("IRConfigurationPage");
    }

    /// <summary>
    /// Nawiguje do ekranu konfiguracji stacji FM.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToFMConfig()
    {
        await Shell.Current.GoToAsync("FMConfigurationPage");
    }

    /// <summary>
    /// Odświeża status urządzenia.
    /// </summary>
    [RelayCommand]
    private async Task RefreshStatus()
    {
        await LoadDeviceData();
    }

    private void ShowNotification(string message, bool isError)
    {
        NotificationMessage = message;
        HasNotification = true;

        // Auto-hide notification after 5 seconds if not an error
        if (!isError)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000);
                if (NotificationMessage == message) // Check if still the same message
                {
                    MainThread.BeginInvokeOnMainThread(ClearNotification);
                }
            });
        }
    }

    private void ClearNotification()
    {
        HasNotification = false;
        NotificationMessage = string.Empty;
    }

    // Cleanup when ViewModel is disposed
    ~DashboardViewModel()
    {
        if (IsMonitoring)
        {
            _radioService.StopMonitoringAsync(DeviceId);
        }
    }
}