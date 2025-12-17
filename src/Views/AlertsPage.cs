using SmartHouse.ViewModels;

namespace SmartHouse.Views;

public partial class AlertsPage : ContentPage
{
    public AlertsPage(AlertsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
