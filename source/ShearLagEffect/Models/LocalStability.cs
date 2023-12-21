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
    //加劲肋类型
    public StiffenerType StiffenerType { get; set; } = StiffenerType.IShape;

    #endregion

    #region 输出结果


    //错误信息
    public string ErrorMessage { get; set; } = string.Empty;
    //验算报告
    public string AnalyzeReport { get; set; } = string.Empty;
    #endregion

    public LocalStability(Int64 id)
    {
        Id = id;


    }

    public void Clear()
    {
        ErrorMessage = string.Empty;


        AnalyzeReport = string.Empty;
    }

}

//加劲肋类型
public enum StiffenerType : Int64
{
    IShape = 0,
    UShape = 1,
    TShape = 2,
}

public static class StiffenerTypeExtensions
{
    public static string Name(this StiffenerType value)
    {
        return value switch
        {
            StiffenerType.IShape => "板肋",
            StiffenerType.UShape => "U肋",
            StiffenerType.TShape => "T形肋",
            _ => "未知",
        };
    }
}
