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
    [ObservableProperty]
    private string param;
    //加筋类型
    public IEnumerable<StiffenerTypeDisplayItem> StiffenerTypeValues { get; }

    //Commands
    //计算
    public ICommand CalculateCommand { get; }
    //清空
    public ICommand ClearAllCommand { get; }

    public LocalStabilityViewModel(MainViewModel mainViewModel, Dictionary<string, object>? parameter = null)
        : base(mainViewModel)
    {
        if (parameter is not null && parameter.ContainsKey("Id") && parameter["Id"] is Int64 id)
            dbEntity = document?.LocalStabilities.Find(id);
        param = "VAL";
        StiffenerTypeValues = Enum.GetValues(typeof(StiffenerType))
            .Cast<StiffenerType>()
            .Select(value => new StiffenerTypeDisplayItem(value));
        ClearAllCommand = new RelayCommand(ClearAll);
        CalculateCommand = new RelayCommand(Calculate);
    }

    public override void Refresh()
    {
        OnPropertyChanged(nameof(DbEntity));

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
