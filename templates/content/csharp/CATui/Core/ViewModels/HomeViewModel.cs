using CATui.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace CATui.Core.ViewModels;

public sealed partial class HomeViewModel : ObservableObject, IBindableView
{
    private readonly INavigationService _navService;
    private readonly ILogger<HomeViewModel> _logger;

    public HomeViewModel(INavigationService navigationService, ILogger<HomeViewModel> logger)
    {
        _navService = navigationService;
        _logger = logger;
    }

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to the Dashboard!";

    [RelayCommand]
    private void NavigateSettings() => _navService.NavigateTo<SettingsViewModel>();

    public void OnNavigatedFrom()
    {
        // Override in derived classes if needed
    }

    public void OnNavigatedTo()
    {
        // Override in derived classes if needed
    }
}
