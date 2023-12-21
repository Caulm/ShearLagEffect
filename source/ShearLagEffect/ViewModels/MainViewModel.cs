using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using AppShearLagEffect.Models;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppShearLagEffect.ViewModels;

//主窗体视图模型
public partial class MainViewModel : ObservableObject
{
    //Bindings
    [ObservableProperty]
    string windowTitle;
    [ObservableProperty]
    ExplorerViewModel explorerViewModel;
    [ObservableProperty]
    UserControl? workView;

    //Commands
    //计算设置
    public ICommand SettingCommand { get; }
    //状态栏
    public ICommand StatusBarCommand { get; }
    //窗口置顶
    public ICommand TopmostCommand { get; }

    public MainViewModel()
    {
        document = Document.GetCurrent();
        windowTitle = Resource1.WindowTitle + " - " + document.Projects.First().Name;
        explorerViewModel = new(this);
        SettingCommand = new RelayCommand(Setting);
        StatusBarCommand = new RelayCommand<object>(StatusBar);
        TopmostCommand = new RelayCommand<object>(Topmost);
    }

    ~MainViewModel()
    {
        Document.GetCurrent()?.Close();
    }

    private void Setting()
    {
        //NOT IMPLEMENT
    }

    private void StatusBar(object? parameter)
    {
        if (parameter is not MainWindow mainWindow)
            return;
        if (mainWindow.statusBarCheckBox.IsChecked ?? false)
            mainWindow.statusBar.Visibility = System.Windows.Visibility.Visible;
        else
            mainWindow.statusBar.Visibility = System.Windows.Visibility.Collapsed;
    }

    private void Topmost(object? parameter)
    {
        if (parameter is not MainWindow mainWindow)
            return;
        mainWindow.Topmost = mainWindow.topmostCheckBox.IsChecked ?? false;
    }

    readonly Document? document;

}
