using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using RadioControlApp.Models;
using RadioControlApp.Services;
using System.Collections.ObjectModel;

namespace RadioControlApp.ViewModels;

/// <summary>
/// ViewModel dla ekranu konfiguracji stacji FM.
/// </summary>
public partial class FMConfigurationViewModel : ObservableObject
{
    private readonly IRadioDeviceService _radioService;
    private readonly ILocalStorageService _storageService;
    private readonly IValidator<FMStation> _validator;

    [ObservableProperty]
    private ObservableCollection<FMStation> _stations = new();

    [ObservableProperty]
    private FMStation _newStation = new();

    [ObservableProperty]
    private FMStation? _selectedStation;

    [ObservableProperty]
    private string _deviceId = "default-device";

    [ObservableProperty]
    private double _currentFrequency = 100.0;

    [ObservableProperty]
    private int _currentVolume = 50;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private DeviceStatus? _deviceStatus;

    public IRangeList<double> FrequencyRange { get; } = new IRangeList<double>();

    public FMConfigurationViewModel(
        IRadioDeviceService radioService,
        ILocalStorageService storageService,
        IValidator<FMStation> validator)
    {
        _radioService = radioService;
        _storageService = storageService;
        _validator = validator;

        // Inicjalizacja zakresu częstotliwości FM (87.5 - 108.0 MHz)
        for (double freq = 87.5; freq <= 108.0; freq += 0.1)
        {
            FrequencyRange.Add(Math.Round(freq, 1));
        }

        LoadStationsCommand.Execute(null);
        LoadDeviceStatusCommand.Execute(null);
    }

    /// <summary>
    /// Ładuje stacje FM z urządzenia i lokalnego magazynu.
    /// </summary>
    [RelayCommand]
    private async Task LoadStations()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            // Pobierz stacje z urządzenia
            var deviceStations = await _radioService.GetFMStationsAsync(DeviceId);

            // Pobierz stacje z lokalnego magazynu
            var localStations = await _storageService.LoadAsync<List<FMStation>>("fm_stations") ?? new List<FMStation>();

            // Połącz i usuń duplikaty
            var allStations = deviceStations.Concat(localStations)
                .GroupBy(s => s.Id)
                .Select(g => g.First())
                .OrderBy(s => s.Frequency)
                .ToList();

            Stations.Clear();
            foreach (var station in allStations)
            {
                Stations.Add(station);
            }

            StatusMessage = $"Załadowano {Stations.Count} stacji FM";
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas ładowania stacji: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Ładuje aktualny status urządzenia.
    /// </summary>
    [RelayCommand]
    private async Task LoadDeviceStatus()
    {
        try
        {
            DeviceStatus = await _radioService.GetDeviceStatusAsync(DeviceId);
            if (DeviceStatus != null)
            {
                CurrentFrequency = DeviceStatus.CurrentFrequency;
                CurrentVolume = DeviceStatus.Volume;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading device status: {ex.Message}");
        }
    }

    /// <summary>
    /// Dodaje nową stację FM.
    /// </summary>
    [RelayCommand]
    private async Task AddStation()
    {
        HasError = false;

        // Walidacja
        var validationResult = await _validator.ValidateAsync(NewStation);
        if (!validationResult.IsValid)
        {
            HasError = true;
            StatusMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return;
        }

        IsLoading = true;

        try
        {
            // Sprawdź czy stacja już istnieje
            if (Stations.Any(s => Math.Abs(s.Frequency - NewStation.Frequency) < 0.01))
            {
                HasError = true;
                StatusMessage = "Stacja o takiej częstotliwości już istnieje";
                return;
            }

            // Dodaj do urządzenia
            var success = await _radioService.AddFMStationAsync(DeviceId, NewStation);

            if (success)
            {
                // Dodaj do kolekcji
                var station = new FMStation
                {
                    Id = NewStation.Id,
                    Name = NewStation.Name,
                    Frequency = NewStation.Frequency,
                    Description = NewStation.Description,
                    IsFavorite = NewStation.IsFavorite,
                    CreatedAt = DateTime.Now
                };

                Stations.Add(station);

                // Posortuj stacje według częstotliwości
                var sortedStations = Stations.OrderBy(s => s.Frequency).ToList();
                Stations.Clear();
                foreach (var s in sortedStations)
                {
                    Stations.Add(s);
                }

                // Zapisz lokalnie
                await SaveStationsLocally();

                // Wyczyść formularz
                NewStation = new FMStation();

                StatusMessage = "Stacja FM została dodana pomyślnie";
            }
            else
            {
                HasError = true;
                StatusMessage = "Nie udało się dodać stacji do urządzenia";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas dodawania stacji: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Usuwa wybraną stację FM.
    /// </summary>
    [RelayCommand]
    private async Task DeleteStation()
    {
        if (SelectedStation == null) return;

        IsLoading = true;
        HasError = false;

        try
        {
            var success = await _radioService.DeleteFMStationAsync(DeviceId, SelectedStation.Id);

            if (success)
            {
                Stations.Remove(SelectedStation);
                await SaveStationsLocally();

                StatusMessage = "Stacja FM została usunięta";
                SelectedStation = null;
            }
            else
            {
                HasError = true;
                StatusMessage = "Nie udało się usunąć stacji z urządzenia";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas usuwania stacji: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Stroi urządzenie na wybraną stację.
    /// </summary>
    [RelayCommand]
    private async Task TuneToStation()
    {
        if (SelectedStation == null) return;

        IsLoading = true;
        HasError = false;

        try
        {
            var success = await _radioService.SetFMFrequencyAsync(DeviceId, SelectedStation.Frequency);

            if (success)
            {
                CurrentFrequency = SelectedStation.Frequency;
                StatusMessage = $"Nastrojono na {SelectedStation.Name} ({SelectedStation.Frequency:F1} MHz)";
                
                // Odśwież status urządzenia
                await LoadDeviceStatus();
            }
            else
            {
                HasError = true;
                StatusMessage = "Nie udało się nastroić urządzenia";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas strojenia: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Ustawia częstotliwość ręcznie.
    /// </summary>
    [RelayCommand]
    private async Task SetFrequency()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            var success = await _radioService.SetFMFrequencyAsync(DeviceId, CurrentFrequency);

            if (success)
            {
                StatusMessage = $"Częstotliwość ustawiona na {CurrentFrequency:F1} MHz";
                await LoadDeviceStatus();
            }
            else
            {
                HasError = true;
                StatusMessage = "Nie udało się ustawić częstotliwości";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas ustawiania częstotliwości: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Ustawia głośność urządzenia.
    /// </summary>
    [RelayCommand]
    private async Task SetVolume()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            var success = await _radioService.SetVolumeAsync(DeviceId, CurrentVolume);

            if (success)
            {
                StatusMessage = $"Głośność ustawiona na {CurrentVolume}%";
                await LoadDeviceStatus();
            }
            else
            {
                HasError = true;
                StatusMessage = "Nie udało się ustawić głośności";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas ustawiania głośności: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Przełącza status ulubionej dla stacji.
    /// </summary>
    [RelayCommand]
    private async Task ToggleFavorite(FMStation station)
    {
        if (station == null) return;

        station.IsFavorite = !station.IsFavorite;
        await SaveStationsLocally();
        StatusMessage = station.IsFavorite ? "Dodano do ulubionych" : "Usunięto z ulubionych";
    }

    private async Task SaveStationsLocally()
    {
        try
        {
            var stationsList = Stations.ToList();
            await _storageService.SaveAsync("fm_stations", stationsList);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving stations locally: {ex.Message}");
        }
    }
}

/// <summary>
/// Walidator dla stacji FM.
/// </summary>
public class FMStationValidator : AbstractValidator<FMStation>
{
    public FMStationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa stacji jest wymagana")
            .MinimumLength(2).WithMessage("Nazwa musi mieć przynajmniej 2 znaki")
            .MaximumLength(50).WithMessage("Nazwa nie może przekraczać 50 znaków");

        RuleFor(x => x.Frequency)
            .InclusiveBetween(87.5, 108.0).WithMessage("Częstotliwość musi być między 87.5 a 108.0 MHz");
    }
}

/// <summary>
/// Lista zakresów dla kontrolki Slider/Picker.
/// </summary>
public class IRangeList<T> : List<T> { }