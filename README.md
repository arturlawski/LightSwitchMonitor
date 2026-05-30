# LightSwitch Monitor

LightSwitch Monitor to lekka aplikacja dla Windows 11, która automatycznie przyciemnia wszystkie monitory poza tym, na którym aktualnie znajduje się kursor myszy. Program działa w tle, jest dostępny z obszaru powiadomień systemu Windows i pozwala szybko ustawić poziom przyciemnienia suwakiem.

## Najważniejsze funkcje

- Automatyczne wykrywanie monitora z aktywnym kursorem.
- Przyciemnianie nieaktywnych ekranów za pomocą przezroczystych nakładek.
- Praca w tle z ikoną w trayu.
- Kompaktowe okno ustawień otwierane kliknięciem w ikonę aplikacji.
- Regulacja poziomu przyciemnienia w zakresie od 5% do 85%.
- Zapisywanie ustawień między uruchomieniami.
- Ukrywanie okna ustawień po zamknięciu bez wyłączania aplikacji.

## Jak działa aplikacja

Aplikacja tworzy niewielkie, bezramkowe okna WPF nad nieaktywnymi monitorami. Okna te są czarne, półprzezroczyste i skonfigurowane jako klik-przezroczyste, dzięki czemu nie przechwytują myszy, nie zabierają fokusu i nie przeszkadzają w pracy.

Pozycja kursora jest sprawdzana cyklicznie. Gdy kursor przejdzie na inny monitor, dotychczas aktywny ekran zostaje przyciemniony, a nowy ekran aktywny wraca do normalnego wyglądu.

## Wymagania

- Windows 11
- .NET SDK zgodny z `net10.0-windows`
- Co najmniej dwa monitory, aby efekt przyciemniania był widoczny

## Budowanie projektu

W katalogu projektu uruchom:

```powershell
dotnet build .\LightSwitchMonitor.csproj
```

Plik wykonywalny w konfiguracji debug zostanie utworzony tutaj:

```text
bin\Debug\net10.0-windows\LightSwitchMonitor.exe
```

## Uruchamianie

Uruchom aplikację z pliku:

```text
bin\Debug\net10.0-windows\LightSwitchMonitor.exe
```

Po uruchomieniu program pojawi się w obszarze powiadomień systemu Windows.

- Lewy klik w ikonę otwiera okno ustawień.
- Prawy klik w ikonę otwiera menu kontekstowe.
- Suwak w oknie ustawień zmienia poziom przyciemnienia.
- Menu kontekstowe pozwala włączyć, wyłączyć lub zamknąć aplikację.

## Ustawienia

Ustawienia użytkownika są zapisywane w pliku:

```text
%APPDATA%\LightSwitchMonitor\settings.json
```

Aktualnie zapisywane są:

- stan włączenia przyciemniania,
- wybrany poziom przyciemnienia.

## Struktura projektu

```text
LightSwitchMonitor
├── App.xaml / App.xaml.cs              start aplikacji i integracja z trayem
├── MainWindow.xaml / MainWindow.xaml.cs
│                                       okno ustawień i obsługa suwaka
├── MonitorDimmerService.cs             wykrywanie monitorów i sterowanie nakładkami
├── DimOverlayWindow.cs                 klik-przezroczysta nakładka przyciemniająca
├── AppSettings.cs                      zapis i odczyt ustawień użytkownika
└── LightSwitchMonitor.csproj           konfiguracja projektu WPF
```

## Uwagi techniczne

- Aplikacja nie zmienia sprzętowej jasności monitorów.
- Przyciemnianie jest realizowane wizualnie przez półprzezroczyste nakładki.
- Przy jednym podłączonym monitorze nakładki są automatycznie ukrywane.
- Zamknięcie okna ustawień nie kończy działania aplikacji; proces pozostaje aktywny w trayu.

