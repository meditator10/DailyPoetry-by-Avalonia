using CommunityToolkit.Mvvm.ComponentModel;

namespace Dpa.Library.ViewModels;

public class ViewModelBase : ObservableObject
{
    public virtual void SetParameter(object parameter) { }
}