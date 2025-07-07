using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using RadioControlApp.Models;
using RadioControlApp.Services;
using System.Collections.ObjectModel;

namespace RadioControlApp.ViewModels;

/// <summary>
/// ViewModel dla ekranu konfiguracji kodów IR.
/// </summary>
public partial class IRConfigurationViewModel : ObservableObject
{
    private readonly IRadioDeviceService _radioService;
    private readonly ILocalStorageService _storageService;
    private readonly IValidator<IRCode> _validator;

    [ObservableProperty]
    private ObservableCollection<IRCode> _storedCodes = new();

    [ObservableProperty]
    private IRCode _newCode = new();

    [ObservableProperty]
    private IRCode? _selectedCode;

    [ObservableProperty]
    private string _deviceId = "default-device";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public IRConfigurationViewModel(
        IRadioDeviceService radioService, 
        ILocalStorageService storageService,
        IValidator<IRCode> validator)
    {
        _radioService = radioService;
        _storageService = storageService;
        _validator = validator;
        
        LoadCodesCommand.Execute(null);
    }

    /// <summary>
    /// Ładuje kody IR z urządzenia i lokalnego magazynu.
    /// </summary>
    [RelayCommand]
    private async Task LoadCodes()
    {
        IsLoading = true;
        HasError = false;
        
        try
        {
            // Pobierz kody z urządzenia
            var deviceCodes = await _radioService.GetStoredIRCodesAsync(DeviceId);
            
            // Pobierz kody z lokalnego magazynu
            var localCodes = await _storageService.LoadAsync<List<IRCode>>("ir_codes") ?? new List<IRCode>();
            
            // Połącz i usuń duplikaty
            var allCodes = deviceCodes.Concat(localCodes)
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .OrderBy(c => c.Name)
                .ToList();

            StoredCodes.Clear();
            foreach (var code in allCodes)
            {
                StoredCodes.Add(code);
            }

            StatusMessage = $"Załadowano {StoredCodes.Count} kodów IR";
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas ładowania kodów: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Dodaje nowy kod IR.
    /// </summary>
    [RelayCommand]
    private async Task AddCode()
    {
        HasError = false;
        
        // Walidacja
        var validationResult = await _validator.ValidateAsync(NewCode);
        if (!validationResult.IsValid)
        {
            HasError = true;
            StatusMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return;
        }

        IsLoading = true;
        
        try
        {
            // Sprawdź czy kod już istnieje
            if (StoredCodes.Any(c => c.HexCode.Equals(NewCode.HexCode, StringComparison.OrdinalIgnoreCase)))
            {
                HasError = true;
                StatusMessage = "Kod o takiej wartości już istnieje";
                return;
            }

            // Zaprogramuj w urządzeniu
            var success = await _radioService.ProgramIRCodeAsync(DeviceId, NewCode);
            
            if (success)
            {
                // Dodaj do kolekcji
                StoredCodes.Add(new IRCode
                {
                    Id = NewCode.Id,
                    Name = NewCode.Name,
                    HexCode = NewCode.HexCode.ToUpper(),
                    Description = NewCode.Description,
                    Category = NewCode.Category,
                    CreatedAt = DateTime.Now
                });

                // Zapisz lokalnie
                await SaveCodesLocally();

                // Wyczyść formularz
                NewCode = new IRCode();
                
                StatusMessage = "Kod IR został dodany pomyślnie";
            }
            else
            {
                HasError = true;
                StatusMessage = "Nie udało się zaprogramować kodu w urządzeniu";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas dodawania kodu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Usuwa wybrany kod IR.
    /// </summary>
    [RelayCommand]
    private async Task DeleteCode()
    {
        if (SelectedCode == null) return;

        IsLoading = true;
        HasError = false;

        try
        {
            var success = await _radioService.DeleteIRCodeAsync(DeviceId, SelectedCode.Id);
            
            if (success)
            {
                StoredCodes.Remove(SelectedCode);
                await SaveCodesLocally();
                
                StatusMessage = "Kod IR został usunięty";
                SelectedCode = null;
            }
            else
            {
                HasError = true;
                StatusMessage = "Nie udało się usunąć kodu z urządzenia";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas usuwania kodu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Wysyła wybrany kod IR.
    /// </summary>
    [RelayCommand]
    private async Task SendCode()
    {
        if (SelectedCode == null) return;

        IsLoading = true;
        HasError = false;

        try
        {
            var success = await _radioService.SendIRCodeAsync(DeviceId, SelectedCode.HexCode);
            
            if (success)
            {
                // Zwiększ licznik użycia
                SelectedCode.UsageCount++;
                await SaveCodesLocally();
                
                StatusMessage = $"Kod '{SelectedCode.Name}' został wysłany";
            }
            else
            {
                HasError = true;
                StatusMessage = "Nie udało się wysłać kodu IR";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas wysyłania kodu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Testuje kod IR wprowadzony ręcznie.
    /// </summary>
    [RelayCommand]
    private async Task TestCode(string hexCode)
    {
        if (string.IsNullOrWhiteSpace(hexCode)) return;

        IsLoading = true;
        HasError = false;

        try
        {
            var success = await _radioService.SendIRCodeAsync(DeviceId, hexCode);
            StatusMessage = success ? "Kod testowy został wysłany" : "Nie udało się wysłać kodu testowego";
            HasError = !success;
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Błąd podczas testowania kodu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveCodesLocally()
    {
        try
        {
            var codesList = StoredCodes.ToList();
            await _storageService.SaveAsync("ir_codes", codesList);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving codes locally: {ex.Message}");
        }
    }
}

/// <summary>
/// Walidator dla kodów IR.
/// </summary>
public class IRCodeValidator : AbstractValidator<IRCode>
{
    public IRCodeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa jest wymagana")
            .MinimumLength(2).WithMessage("Nazwa musi mieć przynajmniej 2 znaki")
            .MaximumLength(50).WithMessage("Nazwa nie może przekraczać 50 znaków");

        RuleFor(x => x.HexCode)
            .NotEmpty().WithMessage("Kod HEX jest wymagany")
            .Matches(@"^[0-9A-Fa-f]+$").WithMessage("Kod musi zawierać tylko znaki HEX (0-9, A-F)")
            .MinimumLength(2).WithMessage("Kod musi mieć przynajmniej 2 znaki")
            .MaximumLength(16).WithMessage("Kod nie może przekraczać 16 znaków");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Kategoria jest wymagana");
    }
}