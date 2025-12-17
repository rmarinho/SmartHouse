using SmartHouse.ViewModels;

namespace SmartHouse.Views;

public partial class RoomsPage : ContentPage
{
    public RoomsPage(RoomsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
