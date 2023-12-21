using AppShearLagEffect.Utils;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppShearLagEffect.Models;

//剪力滞折减
public class ShearLagEffect
{
    #region 输入条件
    //Id
    public Int64 Id { get; set; } = -1;
    //名称
    public string Name { get; set; } = string.Empty;
    //跨径(mm)
    public byte[] SpanLengthInter { get; set; } = Array.Empty<byte>();
    [NotMapped]
    public double[] SpanLength
    {
        get { return ConvertUtil.BytesToArray<double>(SpanLengthInter); }
        set { SpanLengthInter = ConvertUtil.ArrayToBytes(value); }
    }
    //始段悬臂
    public bool InitialCantilever { get; set; } = false;
    //末段悬臂
    public bool FinalCantilever { get; set; } = false;
    //断面跨号
    public Int64 SpanIndex { get; set; } = -1;
    //断面位置(mm)
    public double Location { get; set; } = double.NaN;
    //上翼缘宽(mm)
    public byte[] WidthUpperInter { get; set; } = Array.Empty<byte>();
    [NotMapped]
    public double[] WidthUpper
    {
        get { return ConvertUtil.BytesToArray<double>(WidthUpperInter); }
        set { WidthUpperInter = ConvertUtil.ArrayToBytes(value); }
    }
    //下翼缘宽(mm)
    public byte[] WidthLowerInter { get; set; } = Array.Empty<byte>();
    [NotMapped]
    public double[] WidthLower
    {
        get { return ConvertUtil.BytesToArray<double>(WidthLowerInter); }
        set { WidthLowerInter = ConvertUtil.ArrayToBytes(value); }
    }
    #endregion

    #region 输出结果
    //左侧断面位置等效跨径(mm)
    public double EffectiveSpanLeft { get; set; } = 0d;
    //左侧断面位置等效跨径公式
    public string EffectiveSpanLeftFormula { get; set; } = string.Empty;
    //右侧断面位置等效跨径(mm)
    public double EffectiveSpanRight { get; set; } = 0d;
    //右侧断面位置等效跨径公式
    public string EffectiveSpanRightFormula { get; set; } = string.Empty;
    //有效上翼缘宽(mm)
    public byte[] EffectiveWidthUpperInter { get; set; } = Array.Empty<byte>();
    [NotMapped]
    public double[] EffectiveWidthUpper
    {
        get { return ConvertUtil.BytesToArray<double>(EffectiveWidthUpperInter); }
        set { EffectiveWidthUpperInter = ConvertUtil.ArrayToBytes(value); }
    }
    //有效上翼缘宽公式
    public string EffectiveWidthUpperFormulaInter { get; set; } = string.Empty;
    [NotMapped]
    public string[] EffectiveWidthUpperFormula
    {
        get { return EffectiveWidthUpperFormulaInter.Split(';'); }
        set { EffectiveWidthUpperFormulaInter = string.Join(';', value); }
    }
    //有效下翼缘宽(mm)
    public byte[] EffectiveWidthLowerInter { get; set; } = Array.Empty<byte>();
    [NotMapped]
    public double[] EffectiveWidthLower
    {
        get { return ConvertUtil.BytesToArray<double>(EffectiveWidthLowerInter); }
        set { EffectiveWidthLowerInter = ConvertUtil.ArrayToBytes(value); }
    }
    //有效下翼缘宽公式
    public string EffectiveWidthLowerFormulaInter { get; set; } = string.Empty;
    [NotMapped]
    public string[] EffectiveWidthLowerFormula
    {
        get { return EffectiveWidthLowerFormulaInter.Split(';'); }
        set { EffectiveWidthLowerFormulaInter = string.Join(';', value); }
    }
    //错误信息
    public string ErrorMessage { get; set; } = string.Empty;
    //验算报告
    public string AnalyzeReport { get; set; } = string.Empty;
    #endregion

    public ShearLagEffect(Int64 id = -1)
    {
        Id = id;
        SpanLength = new double[3] { 30000, 45000, 30000 };
        SpanIndex = 1;
        Location = 0;
        WidthUpper = new double[3] { 2000, 3000, 2000 };
        WidthLower = new double[3] { 150, 2300, 150 };
    }

    public void Clear()
    {
        ErrorMessage = string.Empty;

        EffectiveSpanLeft = 0d;
        EffectiveSpanLeftFormula = string.Empty;
        EffectiveSpanRight = 0d;
        EffectiveSpanRightFormula = string.Empty;

        EffectiveWidthUpper = Array.Empty<double>();
        EffectiveWidthUpperFormula = Array.Empty<string>();
        EffectiveWidthLower = Array.Empty<double>();
        EffectiveWidthLowerFormula = Array.Empty<string>();

        AnalyzeReport = string.Empty;
    }

}
