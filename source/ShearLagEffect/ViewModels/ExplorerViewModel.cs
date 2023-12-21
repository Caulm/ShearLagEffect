using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppShearLagEffect.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace AppShearLagEffect.ViewModels;

public partial class ExplorerViewModel : ViewModelBase
{
    //Bindings
    [ObservableProperty]
    private ObservableCollection<ExplorerViewTreeNode> treeRoots;

    //Commands
    //选中节点
    public ICommand NodeSelectedChangedCommand { get; }

    public ExplorerViewModel(MainViewModel mainViewModel, Dictionary<string, object>? parameter = null)
        : base(mainViewModel)
    {
        treeRoots = new();
        NodeSelectedChangedCommand = new RelayCommand<object>(NodeSelectedChange);
        BuildTree();
    }

    private void BuildTree()
    {
        //剪力滞折减
        treeRoots.Add(new ExplorerViewTreeNode() { Name = "剪力滞折减", IsExpanded = true, });
        foreach (var shearLagEffect in document.ShearLagEffects.AsEnumerable())
        {
            treeRoots.Last().Children.Add(new()
            {
                Name = shearLagEffect.Name,
                NodeSelectCommand = new RelayCommand<Dictionary<string, object>>(ShowWorkView),
                NodeSelectCommandParameter = new() { { "ViewName", "ShearLagEffect" }, { "Id", shearLagEffect.Id } },
            });
        }
        //局部稳定折减
        treeRoots.Add(new ExplorerViewTreeNode() { Name = "局部稳定折减", IsExpanded = true, });
        foreach (var localStability in document.LocalStabilities.AsEnumerable())
        {
            treeRoots.Last().Children.Add(new()
            {
                Name = localStability.Name,
                NodeSelectCommand = new RelayCommand<Dictionary<string, object>>(ShowWorkView),
                NodeSelectCommandParameter = new() { { "ViewName", "LocalStability" }, { "Id", localStability.Id } },
            });
        }
    }

    private void NodeSelectedChange(object? parameter)
    {
        if (parameter is not ExplorerViewTreeNode treeNode || treeNode.NodeSelectCommand is null)
            return;
        treeNode.NodeSelectCommand.Execute(treeNode.NodeSelectCommandParameter);
    }

    private void ShowWorkView(Dictionary<string, object>? parameter)
    {
        if (parameter is null || !parameter.ContainsKey("ViewName") || parameter["ViewName"] is not string viewName)
            return;
        var workView = WorkViewFactory.CreateView(mainViewModel, viewName, parameter);
        mainViewModel.WorkView = workView;
    }

}

[ObservableObject]
public partial class ExplorerViewTreeNode
{
    //Bindings
    [ObservableProperty]
    string name;
    [ObservableProperty]
    bool isExpanded;
    [ObservableProperty]
    bool isSelected;
    [ObservableProperty]
    ObservableCollection<ExplorerViewTreeNode> children;

    //Command
    //选中节点
    public ICommand? NodeSelectCommand { get; set; }
    public Dictionary<string, object>? NodeSelectCommandParameter { get; set; }

    public ExplorerViewTreeNode()
    {
        name = "NULL";
        isExpanded = false;
        isSelected = false;
        children = new();
    }

}
