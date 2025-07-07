using RadioControlApp.Models;

namespace RadioControlApp.Services;

/// <summary>
/// Interfejs serwisu do komunikacji z urządzeniem radiowym.
/// </summary>
public interface IRadioDeviceService
{
    /// <summary>
    /// Pobiera informacje o urządzeniu.
    /// </summary>
    Task<RadioDevice?> GetDeviceInfoAsync(string deviceId);
    
    /// <summary>
    /// Pobiera status urządzenia.
    /// </summary>
    Task<DeviceStatus?> GetDeviceStatusAsync(string deviceId);
    
    /// <summary>
    /// Wysyła kod IR do urządzenia.
    /// </summary>
    Task<bool> SendIRCodeAsync(string deviceId, string codeValue);
    
    /// <summary>
    /// Programuje nowy kod IR w urządzeniu.
    /// </summary>
    Task<bool> ProgramIRCodeAsync(string deviceId, IRCode irCode);
    
    /// <summary>
    /// Usuwa kod IR z urządzenia.
    /// </summary>
    Task<bool> DeleteIRCodeAsync(string deviceId, string codeId);
    
    /// <summary>
    /// Pobiera listę zaprogramowanych kodów IR.
    /// </summary>
    Task<List<IRCode>> GetStoredIRCodesAsync(string deviceId);
    
    /// <summary>
    /// Ustawia częstotliwość FM.
    /// </summary>
    Task<bool> SetFMFrequencyAsync(string deviceId, double frequency);
    
    /// <summary>
    /// Dodaje stację FM do listy.
    /// </summary>
    Task<bool> AddFMStationAsync(string deviceId, FMStation station);
    
    /// <summary>
    /// Usuwa stację FM z listy.
    /// </summary>
    Task<bool> DeleteFMStationAsync(string deviceId, string stationId);
    
    /// <summary>
    /// Pobiera listę stacji FM.
    /// </summary>
    Task<List<FMStation>> GetFMStationsAsync(string deviceId);
    
    /// <summary>
    /// Ustawia głośność urządzenia.
    /// </summary>
    Task<bool> SetVolumeAsync(string deviceId, int volume);
    
    /// <summary>
    /// Rozpoczyna monitorowanie urządzenia w czasie rzeczywistym.
    /// </summary>
    Task StartMonitoringAsync(string deviceId, Action<MonitoringData> onDataReceived);
    
    /// <summary>
    /// Zatrzymuje monitorowanie urządzenia.
    /// </summary>
    Task StopMonitoringAsync(string deviceId);
}