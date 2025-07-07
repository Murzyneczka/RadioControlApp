# RadioControlApp

Aplikacja mobilna do zdalnego sterowania i monitorowania sprzętu radiowego, zbudowana w .NET MAUI.

## Wymagania
- .NET 9 SDK
- Visual Studio Code
- Rozszerzenia VS Code:
  - C# (powered by OmniSharp)
  - .NET MAUI Extension
- Android SDK (dla Androida)
- Xcode (dla iOS, tylko na macOS)

## Instalacja
1. Sklonuj repozytorium: `git clone <repo-url>`
2. Otwórz projekt w VS Code: `code RadioControlApp`
3. Zainstaluj zależności: `dotnet restore`
4. Skompiluj projekt: `dotnet build`
5. Uruchom aplikację:
   - Android: `dotnet run --framework net9.0-android`
   - iOS: `dotnet run --framework net9.0-ios`

## Konfiguracja
- Edytuj `appsettings.json`, aby ustawić adres API urządzenia radiowego.
- Upewnij się, że urządzenie radiowe jest dostępne w sieci.

## Struktura projektu
- `Models/`: Modele domenowe
- `ViewModels/`: ViewModele dla MVVM
- `Views/`: Widoki XAML
- `Services/`: Serwisy (np. komunikacja z API)
- `Commands/`, `Queries/`: Logika CQRS