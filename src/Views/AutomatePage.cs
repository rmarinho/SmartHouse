using SmartHouse.ViewModels;

namespace SmartHouse.Views;

public partial class AutomatePage : ContentPage
{
    public AutomatePage(AutomateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
