using System;

namespace AppShearLagEffect.Models;

//局部稳定折减
public class LocalStability
{
    #region 输入条件
    //Id
    public Int64 Id { get; set; } = -1;
    //名称
    public string Name { get; set; } = string.Empty;
    //横向加劲肋间距(mm)
    public double TransverseStiffenerSpacing { get; set; } = double.NaN;
    //受压板宽度(mm)
    public double Width { get; set; } = double.NaN;
    //受压板厚度(mm)
    public double Thickness { get; set; } = double.NaN;
    //纵向加劲肋数量
    public Int64 LongitudinalStiffenerCount { get; set; } = -1;
    //纵向加劲类型
    public StiffenerType LongitudinalStiffenerType { get; set; } = StiffenerType.IShape;
    //板肋肋高(mm)
    public double IShapeHeight { get; set; } = double.NaN;
    //板肋肋厚(mm)
    public double IShapeThickness { get; set; } = double.NaN;
    //U肋肋高(mm)
    public double UShapeHeight { get; set; } = double.NaN;
    //U肋开口宽度(mm)
    public double UShapeOpeningWidth { get; set; } = double.NaN;
    //U肋底板宽度(mm)
    public double UShapeBottomWidth { get; set; } = double.NaN;
    //U肋肋厚(mm)
    public double UShapeThickness { get; set; } = double.NaN;
    #endregion

    #region 输出结果
    //错误信息
    public string ErrorMessage { get; set; } = string.Empty;
    //刚性判断
    public string RigidityState {  get; set; } = string.Empty;
    //验算报告
    public string AnalyzeReport { get; set; } = string.Empty;
    #endregion

    public LocalStability(Int64 id)
    {
        Id = id;
        TransverseStiffenerSpacing = 2000;
        Width = 3000;
        Thickness = 16;
        LongitudinalStiffenerCount = 5;
        IShapeHeight = 140;
        IShapeThickness = 12;
        UShapeHeight = 300;
        UShapeOpeningWidth = 300;
        UShapeBottomWidth = 180;
        UShapeThickness = 8;
    }

    public void Clear()
    {
        ErrorMessage = string.Empty;
        RigidityState = string.Empty;
        AnalyzeReport = string.Empty;
    }

}

//加劲肋类型
public enum StiffenerType : Int64
{
    IShape = 0,
    UShape = 1,
    //TShape = 2,
}

public static class StiffenerTypeExtensions
{
    public static string Name(this StiffenerType value)
    {
        return value switch
        {
            StiffenerType.IShape => "板肋",
            StiffenerType.UShape => "U肋",
            //StiffenerType.TShape => "T形肋",
            _ => "未知",
        };
    }
}
