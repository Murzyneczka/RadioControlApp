using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RadioControlApp.Services;

/// <summary>
/// Serwis do komunikacji z urządzeniem radiowym przez API.
/// </summary>
public class RadioApiService : IRadioDeviceService
{
    private readonly HttpClient _httpClient;

    public RadioApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Wysyła kod IR do urządzenia.
    /// </summary>
    /// <param name="deviceId">ID urządzenia.</param>
    /// <param name="codeValue">Kod IR w formacie HEX.</param>
    /// <returns>True, jeśli operacja się powiodła.</returns>
    public async Task<bool> SendIRCodeAsync(string deviceId, string codeValue)
    {
        var payload = new { DeviceId = deviceId, IRCode = codeValue };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/ir/send", content);
        return response.IsSuccessStatusCode;
    }
}