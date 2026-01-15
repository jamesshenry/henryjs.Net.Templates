using System.ComponentModel;
using CATui.Binding;
using CATui.Core.Interfaces;
using CATui.Core.ViewModels;
using CATui.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace CATui.Views;

public class MainShell : Window
{
    private readonly MainViewModel _viewModel;
    private readonly INavigationService _navService;
    private readonly IServiceProvider _serviceProvider;
    private readonly View _contentContainer;
    private readonly Label _statusLabel;
    private readonly BindingContext _bindingContext;

    public MainShell(MainViewModel viewModel, INavigationService navService, IServiceProvider sp)
    {
        _viewModel = viewModel;
        _navService = navService;
        _serviceProvider = sp;
        _bindingContext = new BindingContext();

        Title = _viewModel.AppTitle;

        _statusLabel = new Label { Y = Pos.AnchorEnd(1), Width = Dim.Fill() };

        _contentContainer = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 3,
        };

        Add(_contentContainer, _statusLabel);

        // Set up bindings
        _bindingContext.AddBinding(
            _statusLabel.BindTextOneWay(
                _viewModel,
                () => _viewModel.StatusText,
                nameof(_viewModel.StatusText)
            )
        );

        _navService.PropertyChanged += OnNavServicePropertyChanged;

        _viewModel.NavigateHomeCommand.Execute(null);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bindingContext.Dispose();
        }
        base.Dispose(disposing);
    }

    private void OnNavServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(INavigationService.CurrentViewModel))
        {
            UpdateContent(_navService.CurrentViewModel);
        }
    }

    private void UpdateContent(ObservableObject? viewModel)
    {
        if (viewModel == null)
            return;

        _contentContainer.RemoveAll();

        var view = ViewLocator.GetView(_serviceProvider, viewModel);

        view.Width = Dim.Fill();
        view.Height = Dim.Fill();

        _contentContainer.Add(view);

        view.SetFocus();
    }
}
