using CATui.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CATui.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly INavigationService _navService;

    public HomeViewModel(INavigationService navigationService)
    {
        _navService = navigationService;
    }

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to the Dashboard!";

    [RelayCommand]
    private void NavigateSettings() => _navService.NavigateTo<SettingsViewModel>();
}
