using AppShearLagEffect.Models;
using System;

namespace AppShearLagEffect.Analysts;

public class LocalStabilityAnalyst : IAnalyst
{
    public LocalStabilityAnalyst(LocalStability localStability)
    {
        this.d = localStability;
    }

    public void Analyze()
    {
        #region 数据校验
        d.ErrorMessage = string.Empty;
        if (d.TransverseStiffenerSpacing < 1)
            d.ErrorMessage = "横向加劲肋间距非法";
        if (d.Width < 1)
            d.ErrorMessage = "受压板宽度非法";
        if (d.Thickness < 1)
            d.ErrorMessage = "受压板厚度非法";
        if (d.LongitudinalStiffenerCount < 1 ||
            d.LongitudinalStiffenerCount > d.Width)
            d.ErrorMessage = "纵向加劲肋数量非法";
        if (d.LongitudinalStiffenerType == StiffenerType.IShape)
        {
            if (d.IShapeHeight < 1 || d.IShapeThickness < 1)
                d.ErrorMessage = "纵向加劲尺寸非法";
            if (d.IShapeHeight > d.IShapeThickness * 12)
                d.ErrorMessage = "纵向加劲宽厚比不满足 hs/ts<=12";
        }
        else if (d.LongitudinalStiffenerType == StiffenerType.UShape)
        {
            if (d.UShapeHeight < 1 || d.UShapeThickness < 1 ||
                d.UShapeOpeningWidth < 1 || d.UShapeBottomWidth < 1)
                d.ErrorMessage = "纵向加劲尺寸非法";
            if (d.UShapeBottomWidth > d.UShapeThickness * 30)
                d.ErrorMessage = "纵向加劲宽厚比不满足 bb/ts<=30。";
            if (Math.Pow(d.UShapeHeight, 2) +
                0.25 * Math.Pow(d.UShapeOpeningWidth - d.UShapeBottomWidth, 2) >
                Math.Pow(d.UShapeThickness * 40, 2))
                d.ErrorMessage = "纵向加劲宽厚比不满足 斜腿长/肋厚<=40。";
        }
        else
            d.ErrorMessage = "不支持的纵向加劲类型";
        if (d.ErrorMessage.Length > 0)
            return; 
        #endregion

        d.Clear();
        var report = string.Empty;
        //长宽比
        var alpha = d.TransverseStiffenerSpacing / d.Width;
        report += $"受压板长宽比：\nα={d.TransverseStiffenerSpacing:0}/{d.Width:0}={alpha:F3}\n";
        //单宽板刚度
        var D = E * Math.Pow(d.Thickness, 3) / (12 * (1 - Math.Pow(V, 2)));
        report += $"单宽板刚度(mm^4)：\nD={E}*{d.Thickness:0}^3/(12*(1-{V}^2))={D:0}\n";
        //加劲肋截面积
        var Asl = 0d;
        //加劲肋惯性矩
        var Isl = 0d;
        if (d.LongitudinalStiffenerType == StiffenerType.IShape)
        {
            report += $"纵向加劲类型：板肋\n";
            Asl = d.IShapeHeight * d.IShapeThickness;
            report += $"加劲肋截面积(mm^2)：\nAsl={d.IShapeHeight:0}*{d.IShapeThickness:0}={Asl:0}\n";
            Isl = Math.Pow(d.IShapeHeight, 3) * d.IShapeThickness / 3;
            report += $"加劲肋惯性矩(mm^4)：\nIsl={d.IShapeHeight:0}^3*{d.IShapeThickness}/3={Isl:0}\n";
        }
        else if (d.LongitudinalStiffenerType == StiffenerType.UShape)
        {
            report += $"纵向加劲类型：U肋\n";
            Asl = (2 * Math.Sqrt(Math.Pow(d.UShapeHeight, 2) +
                0.25 * Math.Pow(d.UShapeOpeningWidth - d.UShapeBottomWidth, 2)) +
                d.UShapeBottomWidth) * d.UShapeThickness;
            report += $"加劲肋截面积(mm^2)：\nAsl=(2*√({d.UShapeHeight:0}^2+({d.UShapeOpeningWidth:0}-{d.UShapeBottomWidth:0})^2/4+{d.UShapeBottomWidth:0})*{d.UShapeThickness:0}={Asl:0}\n";
            Isl = 2 * Math.Pow(d.UShapeHeight, 3) * d.UShapeThickness / 3 +
                Math.Pow(d.UShapeThickness, 3) * d.UShapeBottomWidth / 12 +
                0.25 * d.UShapeBottomWidth * d.UShapeThickness * Math.Pow(d.UShapeHeight - 0.5 * d.UShapeThickness, 2);
            report += $"加劲肋惯性矩(mm^4)：\nIsl=2/3*{d.UShapeHeight:0}^3*{d.UShapeThickness:0}+{d.UShapeThickness:0}^3*{d.UShapeBottomWidth:0}/12+{d.UShapeBottomWidth:0}*{d.UShapeThickness:0}*({d.UShapeHeight:0}-{d.UShapeThickness}/2)^2/4={Isl:0}\n";
        }
        else
            throw new NotImplementedException();
        var count = d.LongitudinalStiffenerCount + 1;
        report += $"n={d.LongitudinalStiffenerCount}+1={count}\n";
        //相对刚度
        var relativeRigidity = E * Isl / (d.Width * D);
        report += $"相对刚度：\nγl={E}*{Isl:0}/({d.Width:0}*{D:0})={relativeRigidity:F3}\n";
        var alpha0 = Math.Pow(1 + count * relativeRigidity, 0.25);
        report += $"临界长宽比：\nα0=4√(1+{count}*{relativeRigidity:F3})={alpha0:F3}\n";
        var dl = Asl / (d.Width * d.Thickness);
        report += $"加劲截面积比：\nδl={Asl:0}/({d.Width:0}*{d.Thickness:0})={dl:F3}\n";
        //临界刚度
        var criticalRigidity = 0d;
        if (alpha > alpha0)
        {
            report += $"α>α0\n";
            criticalRigidity = (Math.Pow(2 * Math.Pow(count, 2) * (1 + count * dl) - 1, 2) - 1) / count;
            report += $"临界刚度：\nγl*=((2*{count}^2*(1+{count}*{dl:F3})-1)^2-1)/{count}={criticalRigidity:F3}\n";
        }
        else
        {
            report += $"α<=α0\n";
            criticalRigidity = (4 * Math.Pow(count, 2) * (1 + count * dl) * Math.Pow(alpha, 2) -
                Math.Pow(Math.Pow(alpha, 2) + 1, 2)) / count;
            report += $"临界刚度：\nγl*=(4*{count}^2*(1+{count}*{dl:F3})*{alpha:F3}^2-({alpha:F3}^2+1)^2)/{count}={criticalRigidity:F3}\n";
        }
        bool rigidity = true;
        if (relativeRigidity >= criticalRigidity)
            report += $"γl>=γl*\n";
        else
        {
            rigidity = false;
            report += $"γl<γl*\n";
        }
        if (Asl >= d.Width * d.Thickness / (10 * count))
            report += $"Asl>=b*t/(10*n)\n";
        else
        {
            rigidity = false;
            report += $"Asl<b*t/(10*n)\n";
        }

        d.RigidityState = rigidity ? "刚性" : "柔性";
        report += $"纵向加劲肋是{d.RigidityState}";
        d.AnalyzeReport = report;
    }

    const double E = 2.06e5;
    const double V = 0.3;
    private readonly LocalStability d;
}
