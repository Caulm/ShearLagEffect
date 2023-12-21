using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppShearLagEffect.Analysts;
using AppShearLagEffect.Models;
using AppShearLagEffect.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AppShearLagEffect.ViewModels;

//剪力滞折减视图模型
public partial class ShearLagEffectViewModel : ViewModelBase
{
    //Bindings
    [ObservableProperty]
    private ShearLagEffect? dbEntity;
    public string SpanLength
    {
        get
        {
            return ConvertUtil.ArrayToString(dbEntity.SpanLength);
        }
        set
        {
            try { dbEntity.SpanLength = ConvertUtil.StringToArray<double>(value); } catch { }
            OnPropertyChanged(nameof(SpanLength));
        }
    }
    public string WidthUpper
    {
        get
        {
            return ConvertUtil.ArrayToString(dbEntity.WidthUpper);
        }
        set
        {
            try { dbEntity.WidthUpper = ConvertUtil.StringToArray<double>(value); } catch { }
            OnPropertyChanged(nameof(WidthUpper));
        }
    }
    public string WidthLower
    {
        get
        {
            return ConvertUtil.ArrayToString(dbEntity.WidthLower);
        }
        set
        {
            try { dbEntity.WidthLower = ConvertUtil.StringToArray<double>(value); } catch { }
            OnPropertyChanged(nameof(WidthLower));
        }
    }
    public string EffectiveWidthUpper
    {
        get
        {
            return ConvertUtil.ArrayToString(dbEntity.EffectiveWidthUpper);
        }
    }
    public string EffectiveWidthLower
    {
        get
        {
            return ConvertUtil.ArrayToString(dbEntity.EffectiveWidthLower);
        }
    }

    //Commands
    //计算
    public ICommand CalculateCommand { get; }
    //清空
    public ICommand ClearAllCommand { get; }

    public ShearLagEffectViewModel(MainViewModel mainViewModel, Dictionary<string, object>? parameter = null)
        : base(mainViewModel)
    {
        if (parameter is not null && parameter.ContainsKey("Id") && parameter["Id"] is Int64 id)
            dbEntity = document?.ShearLagEffects.Find(id);
        //SectionTypeValues = Enum.GetValues(typeof(SectionType))
        //    .Cast<SectionType>()
        //    .Select(value => new SectionTypeDisplayItem(value));
        ClearAllCommand = new RelayCommand(ClearAll);
        CalculateCommand = new RelayCommand(Calculate);
    }

    public override void Refresh()
    {
        OnPropertyChanged(nameof(DbEntity));
        OnPropertyChanged(nameof(SpanLength));
        OnPropertyChanged(nameof(WidthUpper));
        OnPropertyChanged(nameof(WidthLower));
        OnPropertyChanged(nameof(EffectiveWidthUpper));
        OnPropertyChanged(nameof(EffectiveWidthLower));
    }

    private void Calculate()
    {
        if (dbEntity is null)
            return;
        new ShearLagEffectAnalyst(dbEntity).Analyze();
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
