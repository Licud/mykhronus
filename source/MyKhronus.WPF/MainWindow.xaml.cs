namespace MyKhronus.WPF;

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

public partial class MainWindow : Window
{
    [DllImport("user32.DLL", EntryPoint="ReleaseCapture")]
    private static extern void ReleaseCapture();

    [DllImport("user32.DLL", EntryPoint = "SendMessage")]
    private static extern void SendMessage(IntPtr one, int two, int three, int four);

    public MainWindow()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var windowHandle = new WindowInteropHelper(this).Handle;

        ReleaseCapture();
        SendMessage(windowHandle, 0x112, 0xf012, 0);
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

    private void Maximize_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    
    private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

}
