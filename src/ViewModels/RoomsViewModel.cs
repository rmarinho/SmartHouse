using SmartHouse.Models;
using SmartHouse.Services;
using System.Collections.ObjectModel;

namespace SmartHouse.ViewModels;

public partial class RoomsViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    public ObservableCollection<Room> Rooms { get; } = new();

    public RoomsViewModel(IDataService dataService)
    {
        _dataService = dataService;
        LoadRooms();
    }

    private async void LoadRooms()
    {
        var home = await _dataService.GetCurrentHomeAsync();
        if (home != null)
        {
            foreach (var room in home.Rooms)
            {
                Rooms.Add(room);
            }
        }
    }
}
