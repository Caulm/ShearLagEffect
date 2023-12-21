using AppShearLagEffect.Models;

namespace AppShearLagEffect.Analysts;

public class LocalStabilityAnalyst : IAnalyst
{
    public LocalStabilityAnalyst(LocalStability localStability)
    {
        this.localStability = localStability;
    }

    public void Analyze()
    {
        
    }

    private readonly LocalStability localStability;
}
