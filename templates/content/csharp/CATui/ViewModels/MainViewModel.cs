using CATui.Navigation;
using CATui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CATui.ViewModels;

public partial class MainViewModel : ObservableObject, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _appTitle = "CATui";

    [ObservableProperty]
    private string _statusText = "Ready";

    public MainViewModel(INavigationService navigationService, IDialogService dialogService)
    {
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public void OnNavigatedFrom()
    {
        throw new NotImplementedException();
    }

    public void OnNavigatedTo()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void NavigateHome() => _navigationService.NavigateTo<HomeViewModel>();

    [RelayCommand]
    private void NavigateSettings() => _navigationService.NavigateTo<SettingsViewModel>();

    [RelayCommand]
    private void ShowAbout()
    {
        _dialogService.ShowError("About", "CATui: A Terminal.Gui v2 MVVM Demo");
    }
}
