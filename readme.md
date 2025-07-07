# RadioControlApp

🎛️ **Aplikacja mobilna do zdalnego sterowania i monitorowania sprzętu radiowego**

Nowoczesna aplikacja zbudowana w .NET MAUI umożliwiająca zdalne zarządzanie urządzeniami radiowymi przez programowanie kodów IR, konfigurację stacji FM oraz monitorowanie w czasie rzeczywistym.

## ✨ Funkcjonalności

### 📡 Dashboard z monitorowaniem w czasie rzeczywistym
- **Wykres siły sygnału** w czasie rzeczywistym z użyciem LiveCharts
- **Status urządzenia** (temperatura, częstotliwość, głośność, tryb)
- **Alerty** przy niskiej sile sygnału i wysokiej temperaturze
- **Automatyczne powiadomienia** o stanie urządzenia
- **Kontrola monitorowania** (start/stop) z wizualną informacją zwrotną

### 🎯 Konfiguracja kodów IR
- **Programowanie nowych kodów IR** z walidacją HEX
- **Kategoryzacja kodów** (TV, Audio, Klimatyzacja, Oświetlenie, itp.)
- **Zarządzanie zapisanymi kodami** (wysyłanie, usuwanie)
- **Szybki test kodów** przed zapisaniem
- **Licznik użycia** dla każdego kodu
- **Lokalne przechowywanie** z synchronizacją z urządzeniem

### 📻 Konfiguracja stacji FM
- **Ręczne strojenie** z suwakiem częstotliwości (87.5-108.0 MHz)
- **Kontrola głośności** w czasie rzeczywistym
- **Zarządzanie listą stacji** z funkcją ulubionych
- **Szybkie nastrajanie** na zapisane stacje
- **Walidacja częstotliwości** zgodnie ze standardem FM
- **Automatyczna synchronizacja** statusu urządzenia

### 🏗️ Architektura
- **MVVM Pattern** z CommunityToolkit.Mvvm
- **Dependency Injection** z Microsoft.Extensions.DependencyInjection
- **Walidacja** z FluentValidation
- **Lokalna baza danych** SQLite
- **HTTP API** do komunikacji z urządzeniem
- **Reactive UI** z binding i konwerterami

## 🛠️ Wymagania techniczne

### Środowisko deweloperskie
- **.NET 9 SDK**
- **Visual Studio 2022** lub **Visual Studio Code**
- **Android SDK** (dla platform Android)
- **Xcode** (dla iOS, tylko na macOS)

### Rozszerzenia VS Code (opcjonalne)
- C# (powered by OmniSharp)
- .NET MAUI Extension
- XAML Language Support

## 🚀 Instalacja i uruchomienie

### 1. Klonowanie repozytorium
```bash
git clone <repo-url>
cd RadioControlApp
```

### 2. Przywrócenie zależności
```bash
dotnet restore
```

### 3. Konfiguracja
Edytuj `appsettings.json`:
```json
{
    "RadioDeviceApi": {
        "BaseUrl": "https://your-radio-device-api.com/",
        "TimeoutSeconds": 30
    }
}
```

### 4. Kompilacja
```bash
dotnet build
```

### 5. Uruchomienie
```bash
# Android
dotnet run --framework net9.0-android

# iOS (tylko na macOS)
dotnet run --framework net9.0-ios
```

## 📱 Interfejs użytkownika

### Dashboard
- **Nagłówek urządzenia** z statusem połączenia i kontrolą monitorowania
- **Status urządzenia** z kluczowymi metrykami
- **Wykres w czasie rzeczywistym** siły sygnału
- **Podsumowanie** liczby kodów IR i stacji FM
- **Szybka nawigacja** do konfiguracji

### Konfiguracja IR
- **Formularz dodawania** nowych kodów z walidacją
- **Szybki test** kodów przed zapisaniem
- **Lista zapisanych kodów** z akcjami (wyślij, usuń)
- **Kategoryzacja** i statystyki użycia

### Konfiguracja FM
- **Aktualne informacje** o częstotliwości i głośności
- **Ręczne strojenie** z suwakami
- **Formularz dodawania** nowych stacji
- **Lista stacji** z funkcją ulubionych i szybkim strojeniem

## 🏗️ Struktura projektu

```
RadioControlApp/
├── Models/                    # Modele danych
│   ├── RadioDevice.cs
│   ├── IRCode.cs
│   ├── FMStation.cs
│   └── MonitoringData.cs
├── ViewModels/               # ViewModele MVVM
│   ├── DashboardViewModel.cs
│   ├── IRConfigurationViewModel.cs
│   └── FMConfigurationViewModel.cs
├── Views/                    # Widoki XAML
│   ├── DashboardPage.xaml
│   ├── IRConfigurationPage.xaml
│   └── FMConfigurationPage.xaml
├── Services/                 # Serwisy biznesowe
│   ├── IRadioDeviceService.cs
│   ├── RadioApiService.cs
│   ├── ILocalStorageService.cs
│   └── LocalStorageService.cs
├── Converters/              # Konwertery XAML
│   └── ValueConverters.cs
├── Resources/               # Zasoby aplikacji
│   └── Styles/
│       ├── Colors.xaml
│       └── Styles.xaml
└── AppShell.xaml           # Nawigacja Shell
```

## 🔌 API urządzenia radiowego

Aplikacja oczekuje REST API z następującymi endpointami:

### Informacje o urządzeniu
- `GET /api/devices/{deviceId}` - Informacje o urządzeniu
- `GET /api/devices/{deviceId}/status` - Aktualny status

### Kody IR
- `GET /api/devices/{deviceId}/ir` - Lista kodów IR
- `POST /api/ir/program` - Programowanie nowego kodu
- `POST /api/ir/send` - Wysłanie kodu IR
- `DELETE /api/devices/{deviceId}/ir/{codeId}` - Usunięcie kodu

### Stacje FM
- `GET /api/devices/{deviceId}/fm` - Lista stacji FM
- `POST /api/fm/stations` - Dodanie stacji
- `POST /api/fm/frequency` - Ustawienie częstotliwości
- `DELETE /api/devices/{deviceId}/fm/{stationId}` - Usunięcie stacji

### Kontrola urządzenia
- `POST /api/devices/volume` - Ustawienie głośności

## 🔧 Zależności NuGet

- **CommunityToolkit.Mvvm** (8.3.0) - MVVM framework
- **Microsoft.Extensions.Http** (9.0.0) - HTTP client factory
- **SQLite-net-pcl** (1.9.172) - Lokalna baza danych
- **LiveChartsCore.SkiaSharpView.Maui** (2.0.0-rc2) - Wykresy
- **FluentValidation** (11.9.0) - Walidacja modeli
- **Serilog.AspNetCore** (8.0.1) - Logowanie

## 🧪 Testowanie

### Uruchomienie testów jednostkowych
```bash
dotnet test
```

### Debugowanie
1. Ustaw breakpointy w ViewModelach
2. Uruchom aplikację w trybie debug
3. Użyj hot reload dla zmian XAML

## 🐛 Rozwiązywanie problemów

### Częste problemy
1. **Błąd połączenia z API** - Sprawdź URL w appsettings.json
2. **Błędy SQLite** - Usuń i zainstaluj ponownie aplikację
3. **Problemy z wykresami** - Sprawdź LiveCharts dependencies

### Logi
Aplikacja używa Serilog do logowania. Logi są zapisywane w:
- Android: `/storage/emulated/0/Android/data/{app}/files/`
- iOS: `Documents/`

## 🤝 Wkład w rozwój

1. Fork repository
2. Stwórz branch funkcjonalności (`git checkout -b feature/AmazingFeature`)
3. Commit zmian (`git commit -m 'Add some AmazingFeature'`)
4. Push do branch (`git push origin feature/AmazingFeature`)
5. Otwórz Pull Request

## 📝 Licencja

Ten projekt jest licencjonowany na zasadach MIT License - zobacz plik [LICENSE](LICENSE) dla szczegółów.

## 👥 Autorzy

- **Główny developer** - [Twoje imię]

## 🙏 Podziękowania

- Zespół .NET MAUI za framework
- Społeczność open source za biblioteki
- Testerzy beta za feedback