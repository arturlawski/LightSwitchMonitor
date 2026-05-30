using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace LightSwitchMonitor;

public sealed class DimOverlayWindow : Window
{
    private const int GwlExStyle = -20;
    private const int SwpNoZOrder = 0x0004;
    private const int SwpNoActivate = 0x0010;
    private const int SwpShowWindow = 0x0040;
    private const int WsExTransparent = 0x00000020;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;

    private readonly Forms.Screen _screen;

    public DimOverlayWindow(Forms.Screen screen)
    {
        _screen = screen;

        AllowsTransparency = true;
        Background = System.Windows.Media.Brushes.Black;
        Focusable = false;
        IsHitTestVisible = false;
        ResizeMode = ResizeMode.NoResize;
        ShowActivated = false;
        ShowInTaskbar = false;
        Topmost = true;
        WindowStyle = WindowStyle.None;
        Width = Math.Max(1, screen.Bounds.Width);
        Height = Math.Max(1, screen.Bounds.Height);
        Left = screen.Bounds.Left;
        Top = screen.Bounds.Top;
    }

    public string DeviceName => _screen.DeviceName;

    public void SetDimPercent(int dimPercent)
    {
        Opacity = Math.Clamp(dimPercent, 0, 100) / 100.0;
    }

    public void Reposition()
    {
        if (!IsLoaded)
        {
            return;
        }

        var handle = new WindowInteropHelper(this).Handle;
        SetWindowPos(
            handle,
            IntPtr.Zero,
            _screen.Bounds.Left,
            _screen.Bounds.Top,
            _screen.Bounds.Width,
            _screen.Bounds.Height,
            SwpNoZOrder | SwpNoActivate | SwpShowWindow);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var handle = new WindowInteropHelper(this).Handle;
        var currentStyle = GetWindowLong(handle, GwlExStyle);
        SetWindowLong(handle, GwlExStyle, currentStyle | WsExTransparent | WsExToolWindow | WsExNoActivate);
        Reposition();
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        IntPtr hwnd,
        IntPtr hwndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        int flags);
}
