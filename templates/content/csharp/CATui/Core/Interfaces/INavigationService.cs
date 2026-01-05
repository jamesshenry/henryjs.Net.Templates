using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CATui.Core.Interfaces;

public interface INavigationService : INotifyPropertyChanged
{
    ObservableObject CurrentViewModel { get; }
    void NavigateTo<TViewModel>()
        where TViewModel : ObservableObject;
    TResult? ShowModal<TViewModel, TResult>(Action<TViewModel>? configure = null)
        where TViewModel : class, IModalViewModel<TResult>;
}
