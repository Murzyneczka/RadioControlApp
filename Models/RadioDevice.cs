using System.ComponentModel.DataAnnotations;

namespace RadioControlApp.Models;

/// <summary>
/// Model reprezentujący urządzenie radiowe.
/// </summary>
public class RadioDevice
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; }
    public bool IsOnline { get; set; }
    public DeviceStatus Status { get; set; } = new();
    public List<IRCode> StoredIRCodes { get; set; } = new();
    public List<FMStation> FMStations { get; set; } = new();
}

/// <summary>
/// Status urządzenia radiowego.
/// </summary>
public class DeviceStatus
{
    public int SignalStrength { get; set; } // 0-100
    public double CurrentFrequency { get; set; } // MHz
    public int Volume { get; set; } // 0-100
    public double Temperature { get; set; } // °C
    public bool IsTransmitting { get; set; }
    public string CurrentMode { get; set; } = "FM"; // FM, AM, IR
    public DateTime LastUpdate { get; set; }
}

/// <summary>
/// Model kodu podczerwieni.
/// </summary>
public class IRCode
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required(ErrorMessage = "Nazwa jest wymagana")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Kod HEX jest wymagany")]
    [RegularExpression(@"^[0-9A-Fa-f]+$", ErrorMessage = "Kod musi być w formacie HEX")]
    public string HexCode { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int UsageCount { get; set; }
}

/// <summary>
/// Model stacji FM.
/// </summary>
public class FMStation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required(ErrorMessage = "Nazwa stacji jest wymagana")]
    public string Name { get; set; } = string.Empty;
    
    [Range(87.5, 108.0, ErrorMessage = "Częstotliwość musi być między 87.5 a 108.0 MHz")]
    public double Frequency { get; set; }
    
    public string Description { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int SignalQuality { get; set; } // 0-100
}

/// <summary>
/// Dane monitorowania w czasie rzeczywistym.
/// </summary>
public class MonitoringData
{
    public DateTime Timestamp { get; set; }
    public int SignalStrength { get; set; }
    public double Frequency { get; set; }
    public double Temperature { get; set; }
    public string Status { get; set; } = string.Empty;
}