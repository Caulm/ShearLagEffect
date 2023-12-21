using AppShearLagEffect.ViewModels;
using AppShearLagEffect.Views;
using System.Collections.Generic;
using System.Windows.Controls;

namespace AppShearLagEffect.Utils;

public static class WorkViewFactory
{
    public static UserControl? CreateView(MainViewModel mainViewModel, string workViewName, Dictionary<string, object>? parameter)
    {
        UserControl? workView = null;
        if (workViewName == "ShearLagEffect")
        {
            workView = new ShearLagEffectView();
            var workViewModel = new ShearLagEffectViewModel(mainViewModel, parameter);
            workView.DataContext = workViewModel;
        }
        else if (workViewName == "LocalStability")
        {
            workView = new LocalStabilityView();
            var workViewModel = new LocalStabilityViewModel(mainViewModel, parameter);
            workView.DataContext = workViewModel;
        }
        return workView;
    }

}
