using SmartHouse.Models;
using SmartHouse.ViewModels;

namespace SmartHouse.Views;

public partial class RoomDetailPage : ContentPage
{
    public RoomDetailPage(Room room)
    {
        InitializeComponent();
        BindingContext = room;
        Title = room.Name;
    }
}
