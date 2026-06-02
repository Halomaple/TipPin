using System;
using System.Drawing;
using System.Windows.Forms;

namespace TipPin
{
    internal class MenuColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected
        {
            get { return Color.FromArgb(173, 214, 255); } // 更浅的蓝色hover背景
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(173, 214, 255); }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb(173, 214, 255); }
        }

        public override Color MenuItemBorder
        {
            get { return Color.FromArgb(100, 150, 200); } // 中等蓝色边框
        }
    }
    public class TrayIcon : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;
        
        public TrayIcon(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            
            // 创建系统托盘图标
            _notifyIcon = new NotifyIcon
            {
                Icon = CreateTrayIcon(),
                Text = "便利贴",
                Visible = true
            };
            
            // 设置上下文菜单
            _notifyIcon.ContextMenuStrip = CreateContextMenu();
            _notifyIcon.DoubleClick += OnShowMainWindow;
        }
        
        private Icon CreateTrayIcon()
        {
            // 简单的TP字母重叠图标
            Bitmap bitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(System.Drawing.Color.Transparent);
                using (var font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold))
                {
                    g.DrawString("T", font, System.Drawing.Brushes.White, 2, 2);
                    g.DrawString("P", font, System.Drawing.Brushes.White, 6, 6);
                }
                return Icon.FromHandle(bitmap.GetHicon());
            }
        }
        
        private void OnShowMainWindow(object? sender, EventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow != null)
            {
                System.Windows.Application.Current.MainWindow.Show();
                System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void OnAbout(object? sender, EventArgs e)
        {
            // 简化版本，只显示消息框
            MessageBox.Show("便利贴工具 v1.0\n\n一个简洁高效的桌面便利贴应用。", "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnExit(object? sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Font = new System.Drawing.Font("微软雅黑", 10); // 增大字体
            menu.RenderMode = ToolStripRenderMode.Professional; // 使用专业渲染模式
            menu.BackColor = System.Drawing.Color.White; // 设置背景色
            menu.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64); // 使用深灰色文字
            menu.ShowImageMargin = false; // 隐藏图像边距，避免灰色区域
            menu.ShowCheckMargin = false; // 隐藏复选边距
            menu.Margin = new Padding(0); // 设置菜单边距为0
            
            // 自定义hover效果
            menu.Renderer = new ToolStripProfessionalRenderer(new MenuColorTable());
            menu.Items.Add("显示主界面", null, OnShowMainWindow);
            menu.Items.Add("关于", null, OnAbout);
            menu.Items.Add("退出", null, OnExit);
            return menu;
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
    }
}
