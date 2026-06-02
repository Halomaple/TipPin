using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace TipPin
{
    public partial class MainWindow : Window
    {
        private const int WM_NCHITTEST = 0x0084;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int RESIZE_HANDLE_SIZE = 10;

        public MainWindow()
        {
            InitializeComponent();
            
            // 初始化窗口位置和大小
            Left = SystemParameters.WorkArea.Width / 2 - Width / 2;
            Top = SystemParameters.WorkArea.Height / 2 - Height / 2;
            Width = 400;
            Height = 600;
            
            // 添加鼠标事件处理
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseMove += OnMouseMove;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseLeave += OnMouseLeave;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 保留空方法，以防有其他用途
        }

        private bool _isDragging;
        private Point _dragStartPoint;
        private bool _isResizing;
        private ResizeDirection _resizeDirection;
        private Point _resizeStartPoint;
        private Rect _resizeStartRect;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var hitTest = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                if (hitTest != null)
                {
                    // 检查是否点击在窗口边缘用于调整大小
                    var point = e.GetPosition(this);
                    var rect = new Rect(0, 0, Width, Height);
                    var resizeDirection = GetResizeDirection(point, rect);
                    if (resizeDirection != ResizeDirection.None)
                    {
                        _isResizing = true;
                        _resizeDirection = resizeDirection;
                        _resizeStartPoint = e.GetPosition(this);
                        _resizeStartRect = new Rect(Left, Top, Width, Height);
                        CaptureMouse();
                        e.Handled = true;
                    }
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                Vector diff = _dragStartPoint - currentPosition;
                
                // 如果窗口被磁吸，不进行移动操作
                if (!IsWindowSnapped())
                {
                    Left -= diff.X;
                    Top -= diff.Y;
                }
                
                // 磁吸到边界
                SnapToEdges();
            }
            else if (_isResizing)
            {
                Point currentPosition = e.GetPosition(this);
                Vector diff = _resizeStartPoint - currentPosition;
                
                switch (_resizeDirection)
                {
                    case ResizeDirection.Left:
                        Width = _resizeStartRect.Width + diff.X;
                        Left = _resizeStartRect.Left - diff.X;
                        break;
                    case ResizeDirection.Right:
                        Width = _resizeStartRect.Width - diff.X;
                        break;
                    case ResizeDirection.Top:
                        Height = _resizeStartRect.Height + diff.Y;
                        Top = _resizeStartRect.Top - diff.Y;
                        break;
                    case ResizeDirection.Bottom:
                        Height = _resizeStartRect.Height - diff.Y;
                        break;
                    case ResizeDirection.TopLeft:
                        Width = _resizeStartRect.Width + diff.X;
                        Left = _resizeStartRect.Left - diff.X;
                        Height = _resizeStartRect.Height + diff.Y;
                        Top = _resizeStartRect.Top - diff.Y;
                        break;
                    case ResizeDirection.TopRight:
                        Width = _resizeStartRect.Width - diff.X;
                        Height = _resizeStartRect.Height + diff.Y;
                        Top = _resizeStartRect.Top - diff.Y;
                        break;
                    case ResizeDirection.BottomLeft:
                        Width = _resizeStartRect.Width + diff.X;
                        Left = _resizeStartRect.Left - diff.X;
                        Height = _resizeStartRect.Height - diff.Y;
                        break;
                    case ResizeDirection.BottomRight:
                        Width = _resizeStartRect.Width - diff.X;
                        Height = _resizeStartRect.Height - diff.Y;
                        break;
                }
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                ReleaseMouseCapture();
                SaveWindowPosition();
            }
            else if (_isResizing)
            {
                _isResizing = false;
                ReleaseMouseCapture();
                SaveWindowPosition();
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_isDragging || _isResizing)
            {
                _isDragging = false;
                _isResizing = false;
                ReleaseMouseCapture();
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // 检查是否点击在标题栏上，而不是窗口边缘
                var hitTest = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                if (hitTest != null && hitTest.VisualHit is Border border)
                {
                    // 简单检查是否是标题栏（通过背景颜色判断）
                    if (border.Background == null || border.Background.ToString().Contains("Transparent"))
                    {
                        _isDragging = true;
                        _dragStartPoint = e.GetPosition(this);
                        CaptureMouse();
                    }
                }
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                ReleaseMouseCapture();
                SaveWindowPosition();
            }
        }

        private void SaveWindowPosition()
        {
            // 暂时注释掉保存设置代码
        }

        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnMaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            // 最小化到系统托盘
            Hide();
        }

        private void OnButtonMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(0, 100, 200)); // 深蓝色背景
            }
        }

        private void OnCloseButtonMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(220, 0, 0)); // 红色背景
            }
        }

        private void OnButtonMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Background = new SolidColorBrush(Colors.Transparent); // 恢复透明背景
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // 确保应用程序正确退出
            Application.Current.Shutdown();
            base.OnClosed(e);
        }

        private ContextMenu CreateContextMenu()
        {
            var menu = new ContextMenu();
            
            // 透明度菜单项
            var opacityMenu = new MenuItem { Header = "透明度" };
            for (int i = 10; i <= 100; i += 10)
            {
                int opacity = i;
                var item = new MenuItem 
                { 
                    Header = $"{i}%",
                    IsChecked = i == 100
                };
                item.Click += (s, e) => 
                {
                    Opacity = opacity / 100.0;
                    foreach (MenuItem mi in opacityMenu.Items)
                        mi.IsChecked = mi == item;
                };
                opacityMenu.Items.Add(item);
            }
            
            menu.Items.Add(opacityMenu);
            return menu;
        }

        private ResizeDirection GetResizeDirection(Point point, Rect rect)
        {
            if (point.X >= rect.Left && point.X <= rect.Left + RESIZE_HANDLE_SIZE && point.Y >= rect.Top && point.Y <= rect.Top + RESIZE_HANDLE_SIZE)
                return ResizeDirection.TopLeft;
            if (point.X >= rect.Right - RESIZE_HANDLE_SIZE && point.X <= rect.Right && point.Y >= rect.Top && point.Y <= rect.Top + RESIZE_HANDLE_SIZE)
                return ResizeDirection.TopRight;
            if (point.X >= rect.Left && point.X <= rect.Left + RESIZE_HANDLE_SIZE && point.Y >= rect.Bottom - RESIZE_HANDLE_SIZE && point.Y <= rect.Bottom)
                return ResizeDirection.BottomLeft;
            if (point.X >= rect.Right - RESIZE_HANDLE_SIZE && point.X <= rect.Right && point.Y >= rect.Bottom - RESIZE_HANDLE_SIZE && point.Y <= rect.Bottom)
                return ResizeDirection.BottomRight;
            if (point.X >= rect.Left && point.X <= rect.Left + RESIZE_HANDLE_SIZE)
                return ResizeDirection.Left;
            if (point.X >= rect.Right - RESIZE_HANDLE_SIZE && point.X <= rect.Right)
                return ResizeDirection.Right;
            if (point.Y >= rect.Top && point.Y <= rect.Top + RESIZE_HANDLE_SIZE)
                return ResizeDirection.Top;
            if (point.Y >= rect.Bottom - RESIZE_HANDLE_SIZE && point.Y <= rect.Bottom)
                return ResizeDirection.Bottom;
            return ResizeDirection.None;
        }

        private bool IsWindowSnapped()
        {
            var screenBounds = SystemParameters.WorkArea;
            var windowBounds = new Rect(Left, Top, Width, Height);
            
            // 检查窗口是否被吸附到边界
            return Math.Abs(windowBounds.Left - screenBounds.Left) < 10 ||
                   Math.Abs(windowBounds.Right - screenBounds.Right) < 10 ||
                   Math.Abs(windowBounds.Top - screenBounds.Top) < 10 ||
                   Math.Abs(windowBounds.Bottom - screenBounds.Bottom) < 10;
        }

        private void SnapToEdges()
        {
            var screenBounds = SystemParameters.WorkArea;
            var windowBounds = new Rect(Left, Top, Width, Height);
            
            // 磁吸到屏幕边缘
            if (Math.Abs(windowBounds.Left - screenBounds.Left) < 10)
                Left = screenBounds.Left;
            if (Math.Abs(windowBounds.Right - screenBounds.Right) < 10)
                Left = screenBounds.Right - Width;
            if (Math.Abs(windowBounds.Top - screenBounds.Top) < 10)
                Top = screenBounds.Top;
            if (Math.Abs(windowBounds.Bottom - screenBounds.Bottom) < 10)
                Top = screenBounds.Bottom - Height;
        }

        // 定义调整大小方向枚举
        private enum ResizeDirection
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }
    }
}
