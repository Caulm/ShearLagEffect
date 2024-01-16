using AppShearLagEffect.Analysts;
using AppShearLagEffect.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AppShearLagEffect.ViewModels;

//局部稳定折减视图模型
public partial class LocalStabilityViewModel : ViewModelBase
{
    //Bindings
    [ObservableProperty]
    private LocalStability? dbEntity;
    //加劲类型
    public IEnumerable<StiffenerTypeDisplayItem> StiffenerTypeValues { get; }
    [ObservableProperty]
    private Visibility panelVisibility0;
    [ObservableProperty]
    private Visibility panelVisibility1;

    //Commands
    //切换纵向加劲类型
    public ICommand StiffenerTypeSelectionChangedCommand { get; }
    //计算
    public ICommand CalculateCommand { get; }
    //清空
    public ICommand ClearAllCommand { get; }

    public LocalStabilityViewModel(MainViewModel mainViewModel, Dictionary<string, object>? parameter = null)
        : base(mainViewModel)
    {
        if (parameter is not null && parameter.ContainsKey("Id") && parameter["Id"] is Int64 id)
            dbEntity = document?.LocalStabilities.Find(id);
        StiffenerTypeValues = Enum.GetValues(typeof(StiffenerType))
            .Cast<StiffenerType>()
            .Select(value => new StiffenerTypeDisplayItem(value));
        PanelVisibility0 = (dbEntity.LongitudinalStiffenerType == StiffenerType.IShape) ? Visibility.Visible : Visibility.Collapsed;
        PanelVisibility1 = (dbEntity.LongitudinalStiffenerType == StiffenerType.UShape) ? Visibility.Visible : Visibility.Collapsed;
        StiffenerTypeSelectionChangedCommand = new RelayCommand(StiffenerTypeSelectionChanged);
        ClearAllCommand = new RelayCommand(ClearAll);
        CalculateCommand = new RelayCommand(Calculate);
    }

    public override void Refresh()
    {
        OnPropertyChanged(nameof(DbEntity));
    }

    private void StiffenerTypeSelectionChanged()
    {
        PanelVisibility0 = (dbEntity.LongitudinalStiffenerType == StiffenerType.IShape) ? Visibility.Visible : Visibility.Collapsed;
        PanelVisibility1 = (dbEntity.LongitudinalStiffenerType == StiffenerType.UShape) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Calculate()
    {
        if (dbEntity is null)
            return;
        new LocalStabilityAnalyst(dbEntity).Analyze();
        if (dbEntity.ErrorMessage.Length > 0)
            MessageBox.Show(dbEntity.ErrorMessage, "错误");
        Refresh();
    }

    private void ClearAll()
    {
        dbEntity?.Clear();
        Refresh();
    }

}

public class StiffenerTypeDisplayItem
{
    public StiffenerType Value { get; }
    public string Name { get; }

    public StiffenerTypeDisplayItem(StiffenerType value)
    {
        Value = value;
        Name = value.Name();
    }
}
