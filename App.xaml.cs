using System.Drawing;
using System.Windows;
using LightSwitchMonitor.Configuration;
using LightSwitchMonitor.Services;
using LightSwitchMonitor.Views;
using Forms = System.Windows.Forms;

namespace LightSwitchMonitor
{
    public partial class App : System.Windows.Application
    {
        private Forms.NotifyIcon? _notifyIcon;
        private Forms.ToolStripMenuItem? _enabledItem;
        private MonitorDimmerService? _dimmerService;
        private AppSettings? _settings;
        private MainWindow? _settingsWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _settings = AppSettings.Load();
            _dimmerService = new MonitorDimmerService(_settings);
            _dimmerService.Start();

            _notifyIcon = new Forms.NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "LightSwitch Monitor",
                Visible = true,
                ContextMenuStrip = BuildContextMenu()
            };

            _notifyIcon.MouseUp += (_, args) =>
            {
                if (args.Button == Forms.MouseButtons.Left)
                {
                    ShowSettingsWindow();
                }
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            _dimmerService?.Dispose();
            base.OnExit(e);
        }

        private Forms.ContextMenuStrip BuildContextMenu()
        {
            var menu = new Forms.ContextMenuStrip();

            var settingsItem = new Forms.ToolStripMenuItem("Ustawienia");
            settingsItem.Click += (_, _) => ShowSettingsWindow();

            _enabledItem = new Forms.ToolStripMenuItem("Przyciemnianie aktywne")
            {
                Checked = _settings?.IsEnabled ?? true,
                CheckOnClick = true
            };
            _enabledItem.CheckedChanged += (_, _) =>
            {
                if (_settings is null)
                {
                    return;
                }

                _settings.IsEnabled = _enabledItem.Checked;
                _settings.Save();
                _dimmerService?.ApplySettings();
                _settingsWindow?.RefreshFromSettings();
            };

            var exitItem = new Forms.ToolStripMenuItem("Zamknij");
            exitItem.Click += (_, _) => ExitApplication();

            menu.Items.Add(settingsItem);
            menu.Items.Add(_enabledItem);
            menu.Items.Add(new Forms.ToolStripSeparator());
            menu.Items.Add(exitItem);

            return menu;
        }

        private void ShowSettingsWindow()
        {
            if (_settings is null || _dimmerService is null)
            {
                return;
            }

            if (_settingsWindow is null)
            {
                _settingsWindow = new MainWindow(_settings, _dimmerService);
                _settingsWindow.SettingsChanged += (_, _) => SyncTrayState();
                _settingsWindow.Closed += (_, _) => _settingsWindow = null;
            }

            _settingsWindow.Show();
            _settingsWindow.WindowState = WindowState.Normal;
            _settingsWindow.Activate();
        }

        private void SyncTrayState()
        {
            if (_enabledItem is not null && _settings is not null)
            {
                _enabledItem.Checked = _settings.IsEnabled;
            }
        }

        private void ExitApplication()
        {
            _settingsWindow?.CloseForExit();
            Shutdown();
        }
    }
}
