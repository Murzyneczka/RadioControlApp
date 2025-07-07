# Radio Control App - Release Notes

## Version 1.1 (Latest)
**Release Date:** July 2025

### � Bug Fixes
- **CRITICAL FIX**: Resolved ClassNotFoundException for MainActivity
  - Fixed corrupted CRC hash in AndroidManifest.xml (`crc641fb321c08285b0` instead of `crc64e1fb321c08285b90`)
  - Corrected "MainActivityu" to "MainActivity" class reference issue
  - Updated RadioMonitoringService CRC hash to match
  
### 🔧 Technical Updates  
- Downgraded project from .NET 9 to .NET 8 for better compatibility
- Updated MAUI package versions to stable .NET 8 releases:
  - Microsoft.Maui.Controls: 8.0.91
  - Microsoft.Maui.Controls.Compatibility: 8.0.91
  - Microsoft.Extensions.Logging.Debug: 8.0.0
  - Microsoft.Extensions.Http: 8.0.0

### 📱 Version Information
- Application Version: 2
- Display Version: 1.1
- Android Version Code: 2

### 🚀 Deployment
- App should now launch successfully without ClassNotFoundException
- Clean installation recommended for existing users experiencing the crash

---

## 🚀 Pierwsze wydanie - Kompletna aplikacja sterowania urządzeniami radiowymi

### 📅 Data wydania: Styczeń 2025

---

## ⭐ Główne funkcjonalności

### 📊 Dashboard Monitoringu
- **Monitoring w czasie rzeczywistym** siły sygnału i temperatury urządzenia
- **Interaktywne wykresy** z wykorzystaniem LiveChartsCore
- **Automatyczne alerty** przy niskim sygnale (< 30%) i wysokiej temperaturze (> 70°C)
- **Kontrola monitoringu** - start/stop z przyciskami

### 🎛️ Konfiguracja Kodów IR
- **Dodawanie i usuwanie** kodów IR z walidacją HEX
- **Testowanie kodów** przed zapisaniem
- **Kategoryzacja kodów** (Power, Volume, Channel, itp.)
- **Śledzenie użycia** kodów IR
- **Lokalne przechowywanie** w bazie SQLite

### 📻 Konfiguracja Stacji FM
- **Ręczne strojenie** z zakresem 87.5-108.0 MHz
- **Kontrola głośności** z suwakami
- **Zapisywanie ulubionych stacji**
- **Szybkie strojenie** popularnych częstotliwości
- **Walidacja częstotliwości** z komunikatami błędów

---

## 🛠️ Technologie i Architektura

### Frontend
- **.NET MAUI 9.0** - Wieloplatformowy framework UI
- **XAML + Material Design** - Nowoczesny interfejs użytkownika
- **LiveChartsCore** - Zaawansowane wykresy w czasie rzeczywistym

### Backend
- **MVVM Pattern** - Czysty podział logiki i interfejsu
- **CommunityToolkit.Mvvm** - Implementacja wzorca MVVM
- **FluentValidation** - Walidacja danych wejściowych
- **SQLite** - Lokalna baza danych

### Komunikacja
- **HTTP Client** - RESTful API komunikacja z urządzeniem
- **Dependency Injection** - Czysta architektura
- **Async/Await** - Asynchroniczne operacje

---

## 📱 Wymagania Systemowe

- **Android:** 5.0+ (API Level 21)
- **Docelowy Android:** 15 (API Level 35)
- **Rozmiar aplikacji:** 41 MB
- **Uprawnienia:** Internet, WiFi, Vibrate, Wake Lock

---

## 🔧 Konfiguracja

### API Endpoint
Domyślny URL API: `https://radio-device-api.example.com/`
(można zmienić w kodzie źródłowym)

### Lokalny Storage
- Automatyczne tworzenie bazy SQLite
- Synchronizacja z urządzeniem przy połączeniu
- Offline functionality dla zapisanych danych

---

## 📦 Pliki do pobrania

### Aplikacja Android
- **RadioControlApp-v1.0.apk** (41 MB)
  - Podpisana wersja Release
  - Gotowa do instalacji
  - Optymalizowana dla urządzeń produkcyjnych

---

## 🚀 Instrukcja instalacji

### Android
1. Pobierz plik `RadioControlApp-v1.0.apk`
2. Włącz "Nieznane źródła" w ustawieniach Androida
3. Dotknij pliku APK aby zainstalować
4. Lub użyj ADB: `adb install RadioControlApp-v1.0.apk`

### Pierwsze uruchomienie
1. Uruchom aplikację
2. Skonfiguruj adres IP urządzenia radiowego (jeśli potrzeba)
3. Sprawdź połączenie na dashboardzie
4. Rozpocznij konfigurację kodów IR i stacji FM

---

## 🐛 Znane problemy i ograniczenia

- Demo API endpoint - wymaga konfiguracji dla rzeczywistego urządzenia
- Niektóre ostrzeżenia XAML binding (nie wpływają na funkcjonalność)
- Brak automatycznego wykrywania urządzeń w sieci

---

## 🔮 Planowane funkcjonalności (przyszłe wersje)

- [ ] Automatyczne wykrywanie urządzeń w sieci lokalnej
- [ ] Eksport/import konfiguracji
- [ ] Tematy ciemne/jasne
- [ ] Powiadomienia push
- [ ] Backup do chmury
- [ ] Wsparcie dla wielu urządzeń
- [ ] Widget na ekran główny

---

## 👥 Wsparcie

W przypadku problemów lub sugestii:
1. Sprawdź dokumentację w README.md
2. Sprawdź BUILD_SUMMARY.md dla detali technicznych
3. Przejrzyj logi aplikacji w przypadku błędów

---

## 📄 Licencja

Projekt open source - szczegóły w pliku LICENSE

---

**✨ Dziękujemy za używanie RadioControlApp! ✨**