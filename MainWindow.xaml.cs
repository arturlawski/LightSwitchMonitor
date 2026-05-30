using System.Windows;

namespace LightSwitchMonitor
{
    public partial class MainWindow : Window
    {
        private readonly AppSettings _settings;
        private readonly MonitorDimmerService _dimmerService;
        private bool _isRefreshing;
        private bool _closeForExit;

        public event EventHandler? SettingsChanged;

        public MainWindow(AppSettings settings, MonitorDimmerService dimmerService)
        {
            _settings = settings;
            _dimmerService = dimmerService;

            InitializeComponent();
            RefreshFromSettings();
        }

        public void RefreshFromSettings()
        {
            _isRefreshing = true;
            EnabledCheckBox.IsChecked = _settings.IsEnabled;
            DimLevelSlider.Value = _settings.DimPercent;
            DimLevelTextBlock.Text = $"{_settings.DimPercent:0}%";
            _isRefreshing = false;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_closeForExit)
            {
                e.Cancel = true;
                Hide();
            }
        }

        public void CloseForExit()
        {
            _closeForExit = true;
            Close();
        }

        private void EnabledCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
            {
                return;
            }

            _settings.IsEnabled = EnabledCheckBox.IsChecked == true;
            _settings.Save();
            _dimmerService.ApplySettings();
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void DimLevelSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isRefreshing)
            {
                return;
            }

            _settings.DimPercent = (int)Math.Round(DimLevelSlider.Value);
            DimLevelTextBlock.Text = $"{_settings.DimPercent:0}%";
            _settings.Save();
            _dimmerService.ApplySettings();
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
