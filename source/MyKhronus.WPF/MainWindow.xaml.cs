namespace MyKhronus.WPF;

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        ((HwndSource)PresentationSource.FromVisual(this)).AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == 0x0083 && wParam.ToInt32() == 1) // WM_NCCALCSIZE when maximizing
        {
            handled = true;
            return IntPtr.Zero;
        }

        if (msg == 0x0086) // WM_NCACTIVATE — stop Windows drawing the inactive frame edge
        {
            handled = true;
            return new IntPtr(1);
        }

        if (msg == 0x0024) // WM_GETMINMAXINFO
        {
            var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            var monitor = NativeMethods.MonitorFromWindow(hwnd, 0x0002); // MONITOR_DEFAULTTONEAREST
            var info = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
            NativeMethods.GetMonitorInfo(monitor, ref info);

            var monitorRect = info.rcMonitor;
            var workArea = info.rcWork;

            mmi.ptMaxPosition = new POINT
            {
                x = Math.Abs(workArea.left - monitorRect.left),
                y = Math.Abs(workArea.top - monitorRect.top)
            };

            mmi.ptMaxSize = new POINT
            {
                x = Math.Abs(workArea.right - workArea.left),
                y = Math.Abs(workArea.bottom - workArea.top)
            };

            Marshal.StructureToPtr(mmi, lParam, true);
            handled = true;
        }

        return IntPtr.Zero;
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left) return;

        var source = e.OriginalSource as DependencyObject;
        while (source != null)
        {
            if (source is Button) return;
            source = VisualTreeHelper.GetParent(source);
        }

        if (e.ClickCount == 2)
        {
            Maximize_Click(sender, e);
            return;
        }

        if (WindowState == WindowState.Maximized)
        {
            var cursorPos = e.GetPosition(this);
            var cursorScreenPos = PointToScreen(cursorPos);
            var ratio = cursorPos.X / ActualWidth;
            var restoreWidth = RestoreBounds.Width;

            WindowState = WindowState.Normal;
            Left = cursorScreenPos.X - (restoreWidth * ratio);
            Top = cursorScreenPos.Y - 10;
        }

        DragMove();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void Maximize_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
}

file static class NativeMethods
{
    [DllImport("user32.dll")]
    internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
}

[StructLayout(LayoutKind.Sequential)]
file struct MINMAXINFO
{
    public POINT ptReserved;
    public POINT ptMaxSize;
    public POINT ptMaxPosition;
    public POINT ptMinTrackSize;
    public POINT ptMaxTrackSize;
}

[StructLayout(LayoutKind.Sequential)]
file struct POINT
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
file struct RECT
{
    public int left, top, right, bottom;
}

[StructLayout(LayoutKind.Sequential)]
file struct MONITORINFO
{
    public int cbSize;
    public RECT rcMonitor;
    public RECT rcWork;
    public uint dwFlags;
}
