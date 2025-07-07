using System.Net.Http;
using System.Text;
using System.Text.Json;
using RadioControlApp.Models;

namespace RadioControlApp.Services;

/// <summary>
/// Serwis do komunikacji z urządzeniem radiowym przez API.
/// </summary>
public class RadioApiService : IRadioDeviceService
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, CancellationTokenSource> _monitoringTasks = new();

    public RadioApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RadioDevice?> GetDeviceInfoAsync(string deviceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/devices/{deviceId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RadioDevice>(json);
            }
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Error getting device info: {ex.Message}");
        }
        return null;
    }

    public async Task<DeviceStatus?> GetDeviceStatusAsync(string deviceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/devices/{deviceId}/status");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DeviceStatus>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting device status: {ex.Message}");
        }
        return null;
    }

    /// <summary>
    /// Wysyła kod IR do urządzenia.
    /// </summary>
    public async Task<bool> SendIRCodeAsync(string deviceId, string codeValue)
    {
        try
        {
            var payload = new { DeviceId = deviceId, IRCode = codeValue };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/ir/send", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending IR code: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProgramIRCodeAsync(string deviceId, IRCode irCode)
    {
        try
        {
            var payload = new { DeviceId = deviceId, Code = irCode };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/ir/program", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error programming IR code: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteIRCodeAsync(string deviceId, string codeId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/devices/{deviceId}/ir/{codeId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting IR code: {ex.Message}");
            return false;
        }
    }

    public async Task<List<IRCode>> GetStoredIRCodesAsync(string deviceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/devices/{deviceId}/ir");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<IRCode>>(json) ?? new List<IRCode>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting IR codes: {ex.Message}");
        }
        return new List<IRCode>();
    }

    public async Task<bool> SetFMFrequencyAsync(string deviceId, double frequency)
    {
        try
        {
            var payload = new { DeviceId = deviceId, Frequency = frequency };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/fm/frequency", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting FM frequency: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AddFMStationAsync(string deviceId, FMStation station)
    {
        try
        {
            var payload = new { DeviceId = deviceId, Station = station };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/fm/stations", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding FM station: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteFMStationAsync(string deviceId, string stationId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/devices/{deviceId}/fm/{stationId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting FM station: {ex.Message}");
            return false;
        }
    }

    public async Task<List<FMStation>> GetFMStationsAsync(string deviceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/devices/{deviceId}/fm");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<FMStation>>(json) ?? new List<FMStation>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting FM stations: {ex.Message}");
        }
        return new List<FMStation>();
    }

    public async Task<bool> SetVolumeAsync(string deviceId, int volume)
    {
        try
        {
            var payload = new { DeviceId = deviceId, Volume = volume };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/devices/volume", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting volume: {ex.Message}");
            return false;
        }
    }

    public async Task StartMonitoringAsync(string deviceId, Action<MonitoringData> onDataReceived)
    {
        if (_monitoringTasks.ContainsKey(deviceId))
            return;

        var cts = new CancellationTokenSource();
        _monitoringTasks[deviceId] = cts;

        _ = Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var status = await GetDeviceStatusAsync(deviceId);
                    if (status != null)
                    {
                        var monitoringData = new MonitoringData
                        {
                            Timestamp = DateTime.Now,
                            SignalStrength = status.SignalStrength,
                            Frequency = status.CurrentFrequency,
                            Temperature = status.Temperature,
                            Status = status.IsTransmitting ? "Transmitting" : "Idle"
                        };
                        onDataReceived(monitoringData);
                    }

                    await Task.Delay(1000, cts.Token); // Update every second
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in monitoring: {ex.Message}");
                    await Task.Delay(5000, cts.Token); // Retry after 5 seconds
                }
            }
        }, cts.Token);
    }

    public async Task StopMonitoringAsync(string deviceId)
    {
        if (_monitoringTasks.TryGetValue(deviceId, out var cts))
        {
            cts.Cancel();
            _monitoringTasks.Remove(deviceId);
        }
        await Task.CompletedTask;
    }
}