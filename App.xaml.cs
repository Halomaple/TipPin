using System;
using System.Windows;
using System.Runtime.InteropServices;

namespace TipPin
{
    public partial class App : Application
    {
        private MainWindow _mainWindow = null!;
        private TrayIcon _trayIcon = null!;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 隐藏控制台窗口
            var handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_HIDE);
            }

            base.OnStartup(e);
            
            // 创建主窗口但不显示它
            _mainWindow = new MainWindow();
            
            // 创建系统托盘图标
            _trayIcon = new TrayIcon(_mainWindow);
        }
    }
}
