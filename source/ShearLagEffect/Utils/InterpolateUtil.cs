namespace AppShearLagEffect.Utils;

public static class InterpolateUtil
{
    public static double Linear(double value1, double value2, double ratio, string formula1, string formula2, out string formula)
    {
        if(ratio == 0)
        {
            formula = formula1;
            return value1;
        }
        if (ratio == 1)
        {
            formula = formula2;
            return value2;
        }
        formula = $"{1-ratio:F3}*({formula1})+{ratio:F3}*({formula2})";
        return (1 - ratio) * value1 + ratio * value2;
    }

}
