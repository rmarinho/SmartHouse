using SmartHouse.ViewModels;

namespace SmartHouse.Views;

public partial class HomeAssistantTestPage : ContentPage
{
    private readonly HomeAssistantTestViewModel _viewModel;

    public HomeAssistantTestPage(HomeAssistantTestViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
