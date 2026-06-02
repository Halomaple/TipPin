# TP 便利贴 - Windows 桌面便利贴工具

## 功能特性

### 核心功能

- 现代化简约设计风格
- 支持多便利贴管理
- 系统托盘图标和菜单
- 窗口磁吸和透明度调节
- 多种布局和排序方式
- 配置自动保存

### 详细功能说明

1. **窗口操作**：

   - 拖动标题栏可移动窗口
   - 点击"─"按钮最小化窗口
   - 点击"×"按钮隐藏窗口到系统托盘
   - 双击系统托盘图标恢复窗口显示

2. **便利贴管理**：

   - 自动创建 2 个示例便利贴
   - 右键菜单可添加新便利贴
   - 每个便利贴可独立编辑和删除

3. **系统托盘功能**：
   - 右键点击托盘图标显示菜单
   - 双击托盘图标恢复主窗口
   - 支持透明度调节(10%-100%)

## 开发环境

- .NET 8.0+
- Visual Studio 2022 或 VS Code
- Hardcodet.NotifyIcon.Wpf 1.0.5

## 编译与运行

### 安装依赖

1. 安装.NET SDK 8.0 或更高版本
2. 确保已安装 Visual Studio 2022 或 VS Code

### 运行步骤

```bash
# 恢复NuGet包
dotnet restore

# 编译项目
dotnet build

# 运行应用
dotnet run
```

### 调试指南

1. **窗口不显示问题**：

   - 检查系统托盘是否有应用图标
   - 双击托盘图标尝试恢复窗口
   - 确保没有其他实例正在运行

2. **常见错误**：
   - NU1701 警告：可忽略，不影响功能
   - 窗口透明问题：修改 MainWindow.xaml 中的 AllowsTransparency 属性

## 项目结构

```
StickyNoteApp/
├── App.xaml.cs          # 应用入口和全局配置
├── AppConfig.cs         # 配置管理类
├── TrayIcon.cs          # 系统托盘管理
├── MainWindow.xaml      # 主窗口界面
├── MainWindow.xaml.cs   # 主窗口逻辑
├── NoteControl.xaml     # 便利贴控件界面
├── NoteControl.xaml.cs  # 便利贴控件逻辑
├── NoteWindow.xaml      # 单独窗口界面
└── NoteWindow.xaml.cs   # 单独窗口逻辑
```

## 配置文件位置

所有便利贴内容和窗口设置保存在：
`%USERPROFILE%\Documents\StickyNotesConfig.json`

## 后续开发计划

- 实现磁吸功能
- 完善布局和排序功能
- 添加更多自定义选项
- 支持主题切换
- 增加数据备份功能

## 常见问题

Q: 窗口启动后看不到？
A: 请检查系统托盘区域，双击图标恢复窗口

Q: 如何添加新便利贴？
A: 右键点击主窗口空白处，选择"添加便利贴"

Q: 透明度调节无效？
A: 确保 MainWindow.xaml 中 AllowsTransparency="True"
