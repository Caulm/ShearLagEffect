using AppShearLagEffect.Models;
using AppShearLagEffect.Utils;
using System;

namespace AppShearLagEffect.Analysts;

//剪力滞效应分析器
public class ShearLagEffectAnalyst : IAnalyst
{
    public ShearLagEffectAnalyst(ShearLagEffect shearLagEffect)
    {
        this.shearLagEffect = shearLagEffect;
    }

    public void Analyze()
    {
        var spanLength = shearLagEffect.SpanLength;
        var spanIndex = shearLagEffect.SpanIndex;
        var location = shearLagEffect.Location;
        if (spanLength.Length == 0)
        {
            shearLagEffect.ErrorMessage = $"桥梁跨径输入有误";
            return;
        }
        if (spanIndex < 1 || spanIndex > spanLength.Length)
        {
            shearLagEffect.ErrorMessage = $"断面跨号输入有误，应该在{1}到{spanLength.Length}之间";
            return;
        }
        while(spanIndex < spanLength.Length && location >= spanLength[spanIndex - 1])
        {
            location -= spanLength[spanIndex - 1];
            ++spanIndex;
        }
        if (location < 0 || location > spanLength[spanIndex - 1])
        {
            shearLagEffect.ErrorMessage = $"断面位置输入有误，应该在{0}到{spanLength[spanIndex - 1]:0}之间";
            return;
        }
        var widthUpper = shearLagEffect.WidthUpper;
        for(var i = 1; i < widthUpper.Length - 1; ++i)
            widthUpper[i] = widthUpper[i] * 0.5;
        var widthLower = shearLagEffect.WidthLower;
        for (var i = 1; i < widthLower.Length - 1; ++i)
            widthLower[i] = widthLower[i] * 0.5;

        shearLagEffect.Clear();

        var effectiveSpanLeft = double.NaN;
        var effectiveSpanLeftFormula = string.Empty;
        var effectiveSpanRight = double.NaN;
        var effectiveSpanRightFormula = string.Empty;
        var effectiveWidthUpper = new double[widthUpper.Length];
        var effectiveWidthUpperFormula = new string[widthUpper.Length];
        var effectiveWidthLower = new double[widthLower.Length];
        var effectiveWidthLowerFormula = new string[widthLower.Length];

        //简支梁1
        if (spanLength.Length == 1)
        {
            effectiveSpanLeftFormula = $"{spanLength[spanIndex - 1]}";
            effectiveSpanLeft = spanLength[spanIndex - 1];
            for (var i = 0; i < widthUpper.Length; ++i)
            {
                var b = widthUpper[i];
                effectiveWidthUpper[i] = CalculateFormula5183(b, effectiveSpanLeft, out var formula);
                effectiveWidthUpperFormula[i] = formula;
            }
            for (var i = 0; i < widthLower.Length; ++i)
            {
                var b = widthLower[i];
                effectiveWidthLower[i] = CalculateFormula5183(b, effectiveSpanLeft, out var formula);
                effectiveWidthLowerFormula[i] = formula;
            }
        }
        //连续梁1
        else if (spanLength.Length > 1 && (
            (spanIndex == 1 && location <= spanLength[spanIndex - 1] * 0.8) ||
            (spanIndex == spanLength.Length && location >= spanLength[spanIndex - 1] * 0.2)
            ))
        {
            effectiveSpanLeftFormula = $"0.8*{spanLength[spanIndex - 1]}";
            effectiveSpanLeft = 0.8 * spanLength[spanIndex - 1];
            for (var i = 0; i < widthUpper.Length; ++i)
            {
                var b = widthUpper[i];
                effectiveWidthUpper[i] = CalculateFormula5183(b, effectiveSpanLeft, out var formula);
                effectiveWidthUpperFormula[i] = formula;
            }
            for (var i = 0; i < widthLower.Length; ++i)
            {
                var b = widthLower[i];
                effectiveWidthLower[i] = CalculateFormula5183(b, effectiveSpanLeft, out var formula);
                effectiveWidthLowerFormula[i] = formula;
            }
        }
        //连续梁5
        else if (spanLength.Length > 2 &&
            spanIndex > 1 && spanIndex < spanLength.Length &&
            location >= spanLength[spanIndex - 1] * 0.2 && location <= spanLength[spanIndex - 1] * 0.8)
        {
            effectiveSpanLeftFormula = $"0.6*{spanLength[spanIndex - 1]}";
            effectiveSpanLeft = 0.6 * spanLength[spanIndex - 1];
            for (var i = 0; i < widthUpper.Length; ++i)
            {
                var b = widthUpper[i];
                effectiveWidthUpper[i] = CalculateFormula5183(b, effectiveSpanLeft, out var formula);
                effectiveWidthUpperFormula[i] = formula;
            }
            for (var i = 0; i < widthLower.Length; ++i)
            {
                var b = widthLower[i];
                effectiveWidthLower[i] = CalculateFormula5183(b, effectiveSpanLeft, out var formula);
                effectiveWidthLowerFormula[i] = formula;
            }
        }
        //连续梁3,7
        else if (spanLength.Length > 2 &&
            spanIndex > 1 && spanIndex < spanLength.Length &&
            location == 0)
        {
            effectiveSpanLeftFormula = $"0.2*({spanLength[spanIndex - 2]}+{spanLength[spanIndex - 1]})";
            effectiveSpanLeft = 0.2 * (spanLength[spanIndex - 2] + spanLength[spanIndex - 1]);
            for (var i = 0; i < widthUpper.Length; ++i)
            {
                var b = widthUpper[i];
                effectiveWidthUpper[i] = CalculateFormula5184(b, effectiveSpanLeft, out var formula);
                effectiveWidthUpperFormula[i] = formula;
            }
            for (var i = 0; i < widthLower.Length; ++i)
            {
                var b = widthLower[i];
                effectiveWidthLower[i] = CalculateFormula5184(b, effectiveSpanLeft, out var formula);
                effectiveWidthLowerFormula[i] = formula;
            }
        }
        //连续梁2
        else if (spanLength.Length > 1 &&
            spanIndex == 1 && location > spanLength[spanIndex - 1] * 0.8)
        {
            var ratio = (location - spanLength[spanIndex - 1] * 0.8) / (spanLength[spanIndex - 1] * 0.2);
            effectiveSpanLeftFormula = $"0.8*{spanLength[spanIndex - 1]}";
            effectiveSpanLeft = 0.8 * spanLength[spanIndex - 1];
            effectiveSpanRightFormula = $"0.2*({spanLength[spanIndex - 1]}+{spanLength[spanIndex]})";
            effectiveSpanRight = 0.2 * (spanLength[spanIndex - 1] + spanLength[spanIndex]);
            for (var i = 0; i < widthUpper.Length; ++i)
            {
                var b = widthUpper[i];
                var interValue3 = CalculateFormula5183(b, effectiveSpanLeft, out var interFormula3);
                var interValue4 = CalculateFormula5184(b, effectiveSpanRight, out var interFormula4);
                effectiveWidthUpper[i] = InterpolateUtil.Linear(interValue3, interValue4, ratio, interFormula3, interFormula4, out var formula);
                effectiveWidthUpperFormula[i] = formula;
            }
            for (var i = 0; i < widthLower.Length; ++i)
            {
                var b = widthLower[i];
                var interValue3 = CalculateFormula5183(b, effectiveSpanLeft, out var interFormula3);
                var interValue4 = CalculateFormula5184(b, effectiveSpanRight, out var interFormula4);
                effectiveWidthLower[i] = InterpolateUtil.Linear(interValue3, interValue4, ratio, interFormula3, interFormula4, out var formula);
                effectiveWidthLowerFormula[i] = formula;
            }
        }
        //连续梁4
        else if (spanLength.Length > 2 &&
            spanIndex > 1 && spanIndex < spanLength.Length &&
            location < spanLength[spanIndex - 1] * 0.2)
        {
            var ratio = location / (spanLength[spanIndex - 1] * 0.2);
            effectiveSpanLeftFormula = $"0.2*({spanLength[spanIndex - 2]}+{spanLength[spanIndex - 1]})";
            effectiveSpanLeft = 0.2 * (spanLength[spanIndex - 2] + spanLength[spanIndex - 1]);
            effectiveSpanRightFormula = $"0.6*{spanLength[spanIndex - 1]}";
            effectiveSpanRight = 0.6 * spanLength[spanIndex - 1];
            for (var i = 0; i < widthUpper.Length; ++i)
            {
                var b = widthUpper[i];
                var interValue3 = CalculateFormula5184(b, effectiveSpanLeft, out var interFormula3);
                var interValue4 = CalculateFormula5183(b, effectiveSpanRight, out var interFormula4);
                effectiveWidthUpper[i] = InterpolateUtil.Linear(interValue3, interValue4, ratio, interFormula3, interFormula4, out var formula);
                effectiveWidthUpperFormula[i] = formula;
            }
            for (var i = 0; i < widthLower.Length; ++i)
            {
                var b = widthLower[i];
                var interValue3 = CalculateFormula5184(b, effectiveSpanLeft, out var interFormula3);
                var interValue4 = CalculateFormula5183(b, effectiveSpanRight, out var interFormula4);
                effectiveWidthLower[i] = InterpolateUtil.Linear(interValue3, interValue4, ratio, interFormula3, interFormula4, out var formula);
                effectiveWidthLowerFormula[i] = formula;
            }
        }
        //连续梁6
        else if (spanLength.Length > 2 &&
            spanIndex > 1 && spanIndex < spanLength.Length &&
            location > spanLength[spanIndex - 1] * 0.8)
        {
            var ratio = (location - spanLength[spanIndex - 1] * 0.8) / (spanLength[spanIndex - 1] * 0.2);
            effectiveSpanLeftFormula = $"0.6*{spanLength[spanIndex - 1]}";
            effectiveSpanLeft = 0.6 * spanLength[spanIndex - 1];
            effectiveSpanRightFormula = $"0.2*({spanLength[spanIndex - 1]}+{spanLength[spanIndex]})";
            effectiveSpanRight = 0.2 * (spanLength[spanIndex - 1] + spanLength[spanIndex]);
            for (var i = 0; i < widthUpper.Length; ++i)
            {
                var b = widthUpper[i];
                var interValue3 = CalculateFormula5183(b, effectiveSpanLeft, out var interFormula3);
                var interValue4 = CalculateFormula5184(b, effectiveSpanRight, out var interFormula4);
                effectiveWidthUpper[i] = InterpolateUtil.Linear(interValue3, interValue4, ratio, interFormula3, interFormula4, out var formula);
                effectiveWidthUpperFormula[i] = formula;
            }
            for (var i = 0; i < widthLower.Length; ++i)
            {
                var b = widthLower[i];
                var interValue3 = CalculateFormula5183(b, effectiveSpanLeft, out var interFormula3);
                var interValue4 = CalculateFormula5184(b, effectiveSpanRight, out var interFormula4);
                effectiveWidthLower[i] = InterpolateUtil.Linear(interValue3, interValue4, ratio, interFormula3, interFormula4, out var formula);
                effectiveWidthLowerFormula[i] = formula;
            }
        }
        //连续梁8
        else if (spanLength.Length > 1 &&
            spanIndex == spanLength.Length && location < spanLength[spanIndex - 1] * 0.8)
        {
            var ratio = location / (spanLength[spanIndex - 1] * 0.2);
            effectiveSpanLeftFormula = $"0.2*({spanLength[spanIndex - 2]}+{spanLength[spanIndex - 1]})";
            effectiveSpanLeft = 0.2 * (spanLength[spanIndex - 2] + spanLength[spanIndex - 1]);
            effectiveSpanRightFormula = $"0.8*{spanLength[spanIndex - 1]}";
            effectiveSpanRight = 0.8 * spanLength[spanIndex - 1];
            for (var i = 0; i < widthUpper.Length; ++i)
            {
                var b = widthUpper[i];
                var interValue3 = CalculateFormula5184(b, effectiveSpanLeft, out var interFormula3);
                var interValue4 = CalculateFormula5183(b, effectiveSpanRight, out var interFormula4);
                effectiveWidthUpper[i] = InterpolateUtil.Linear(interValue3, interValue4, ratio, interFormula3, interFormula4, out var formula);
                effectiveWidthUpperFormula[i] = formula;
            }
            for (var i = 0; i < widthLower.Length; ++i)
            {
                var b = widthLower[i];
                var interValue3 = CalculateFormula5184(b, effectiveSpanLeft, out var interFormula3);
                var interValue4 = CalculateFormula5183(b, effectiveSpanRight, out var interFormula4);
                effectiveWidthLower[i] = InterpolateUtil.Linear(interValue3, interValue4, ratio, interFormula3, interFormula4, out var formula);
                effectiveWidthLowerFormula[i] = formula;
            }
        }
        else
        {
            throw new NotImplementedException();
        }

        for (var i = 1; i < effectiveWidthUpper.Length - 1; ++i)
        {
            effectiveWidthUpper[i] = effectiveWidthUpper[i] * 2;
            effectiveWidthUpperFormula[i] = $"2*({effectiveWidthUpperFormula[i]})";
        }
        for (var i = 1; i < effectiveWidthLower.Length - 1; ++i)
        {
            effectiveWidthLower[i] = effectiveWidthLower[i] * 2;
            effectiveWidthLowerFormula[i] = $"2*({effectiveWidthLowerFormula[i]})";
        }

        shearLagEffect.SpanIndex = spanIndex;
        shearLagEffect.Location = location;
        shearLagEffect.EffectiveSpanLeft = effectiveSpanLeft;
        shearLagEffect.EffectiveSpanLeftFormula = effectiveSpanLeftFormula;
        shearLagEffect.EffectiveSpanRight = effectiveSpanRight;
        shearLagEffect.EffectiveSpanRightFormula = effectiveSpanRightFormula;
        shearLagEffect.EffectiveWidthUpper = effectiveWidthUpper;
        shearLagEffect.EffectiveWidthUpperFormula = effectiveWidthUpperFormula;
        shearLagEffect.EffectiveWidthLower = effectiveWidthLower;
        shearLagEffect.EffectiveWidthLowerFormula = effectiveWidthLowerFormula;
        CreateReport();
    }

    private readonly ShearLagEffect shearLagEffect;

    private void CreateReport()
    {
        var effectiveSpanLeft = shearLagEffect.EffectiveSpanLeft;
        var effectiveSpanLeftFormula = shearLagEffect.EffectiveSpanLeftFormula;
        var effectiveSpanRight = shearLagEffect.EffectiveSpanRight;
        var effectiveSpanRightFormula = shearLagEffect.EffectiveSpanRightFormula;
        var effectiveWidthUpper = shearLagEffect.EffectiveWidthUpper;
        var effectiveWidthUpperFormula = shearLagEffect.EffectiveWidthUpperFormula;
        var effectiveWidthLower = shearLagEffect.EffectiveWidthLower;
        var effectiveWidthLowerFormula = shearLagEffect.EffectiveWidthLowerFormula;

        var report = string.Empty;

        report += $"等效跨径(mm)：\n";
        if (double.IsNaN(effectiveSpanRight))
        {
            report += $"l={effectiveSpanLeftFormula}={effectiveSpanLeft:0}\n";
        }
        else
        {
            report += $"l1={effectiveSpanLeftFormula}={effectiveSpanLeft:0}\n";
            report += $"l2={effectiveSpanRightFormula}={effectiveSpanRight:0}\n";
        }
        if (effectiveWidthUpper.Length > 0)
        {
            report += $"等效上翼缘宽(mm)：\n";
            for (var i = 0; i < effectiveWidthUpper.Length; ++i)
            {
                report += $"bue{i + 1}={effectiveWidthUpperFormula[i]}={effectiveWidthUpper[i]:0}\n";
            }
        }
        if (effectiveWidthLower.Length > 0)
        {
            report += $"等效下翼缘宽(mm)：\n";
            for (var i = 0; i < effectiveWidthLower.Length; ++i)
            {
                report += $"ble{i + 1}={effectiveWidthLowerFormula[i]}={effectiveWidthLower[i]:0}\n";
            }
        }

        shearLagEffect.AnalyzeReport = report;
    }

    private static double CalculateFormula5183(double b, double l, out string formula)
    {
        var r = b / l;
        if (r <= 0.05)
        {
            formula = $"{b}";
            return b;
        }
        else if (r < 0.3)
        {
            formula = $"(1.1-2*{b}/({l}))*{b}";
            return (1.1 - 2 * r) * b;
        }
        else
        {
            formula = $"0.15*({l})";
            return 0.15 * l;
        }
    }

    private static double CalculateFormula5184(double b, double l, out string formula)
    {
        var r = b / l;
        if (r <= 0.02)
        {
            formula = $"{b}";
            return b;
        }
        else if (r < 0.3)
        {
            formula = $"(1.06-3.2*{b}/({l})+4.5*({b}/({l}))^2)*{b}";
            return (1.06 - 3.2 * r + 4.5 * r * r) * b;
        }
        else
        {
            formula = $"0.15*({l})";
            return 0.15 * l;
        }
    }

}
