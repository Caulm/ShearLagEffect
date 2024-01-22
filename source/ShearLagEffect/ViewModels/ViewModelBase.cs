using CommunityToolkit.Mvvm.ComponentModel;
using AppShearLagEffect.Models;

namespace AppShearLagEffect.ViewModels;

//ViewModel的基类
[ObservableObject]
public partial class ViewModelBase
{
    public ViewModelBase(MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel;
        document = Document.GetCurrent();
    }

    public virtual void Refresh()
    {
    }

    protected readonly MainViewModel mainViewModel;
    protected readonly Document? document;
}
