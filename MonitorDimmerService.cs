using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace LightSwitchMonitor;

public sealed class MonitorDimmerService : IDisposable
{
    private readonly AppSettings _settings;
    private readonly DispatcherTimer _timer;
    private readonly Dictionary<string, DimOverlayWindow> _overlays = new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;

    public MonitorDimmerService(AppSettings settings)
    {
        _settings = settings;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(120)
        };
        _timer.Tick += (_, _) => UpdateOverlays();
    }

    public void Start()
    {
        SyncScreens();
        ApplySettings();
        _timer.Start();
    }

    public void ApplySettings()
    {
        foreach (var overlay in _overlays.Values)
        {
            overlay.SetDimPercent(_settings.DimPercent);
        }

        UpdateOverlays();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _timer.Stop();

        foreach (var overlay in _overlays.Values)
        {
            overlay.Close();
        }

        _overlays.Clear();
    }

    private void SyncScreens()
    {
        var screens = Forms.Screen.AllScreens;
        var activeDeviceNames = screens.Select(screen => screen.DeviceName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var removedDeviceName in _overlays.Keys.Where(deviceName => !activeDeviceNames.Contains(deviceName)).ToArray())
        {
            _overlays[removedDeviceName].Close();
            _overlays.Remove(removedDeviceName);
        }

        foreach (var screen in screens)
        {
            if (_overlays.ContainsKey(screen.DeviceName))
            {
                continue;
            }

            var overlay = new DimOverlayWindow(screen);
            overlay.SetDimPercent(_settings.DimPercent);
            _overlays.Add(screen.DeviceName, overlay);
        }
    }

    private void UpdateOverlays()
    {
        if (_disposed)
        {
            return;
        }

        SyncScreens();

        if (!_settings.IsEnabled || Forms.Screen.AllScreens.Length < 2)
        {
            HideAll();
            return;
        }

        var cursorPoint = Forms.Cursor.Position;
        var activeScreen = Forms.Screen.FromPoint(cursorPoint);

        foreach (var (deviceName, overlay) in _overlays)
        {
            if (string.Equals(deviceName, activeScreen.DeviceName, StringComparison.OrdinalIgnoreCase))
            {
                overlay.Hide();
                continue;
            }

            overlay.SetDimPercent(_settings.DimPercent);
            overlay.Reposition();

            if (!overlay.IsVisible)
            {
                overlay.Show();
            }
        }
    }

    private void HideAll()
    {
        foreach (var overlay in _overlays.Values)
        {
            overlay.Hide();
        }
    }
}
