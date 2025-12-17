using CommunityToolkit.Mvvm.ComponentModel;
using SmartHouse.Models;
using SmartHouse.Services;
using System.Collections.ObjectModel;
using DeviceModel = SmartHouse.Models.Device;

namespace SmartHouse.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IHomeAssistantService _homeAssistantService;

    public ObservableCollection<Scene> QuickScenes { get; } = new();
    public ObservableCollection<DeviceModel> FavoriteDevices { get; } = new();

    [ObservableProperty]
    private string _energyUsage = "2.4 kWh";

    [ObservableProperty]
    private string _securityStatus = "Armed";

    [ObservableProperty]
    private string _temperature = "21°C";

    public HomeViewModel(IDataService dataService, IHomeAssistantService homeAssistantService)
    {
        _dataService = dataService;
        _homeAssistantService = homeAssistantService;

        LoadQuickScenes();
        LoadFavorites();
    }

    private void LoadQuickScenes()
    {
        QuickScenes.Add(new Scene { Id = "good_night", Name = "Good Night", Icon = "💡" });
        QuickScenes.Add(new Scene { Id = "good_morning", Name = "Good Morning", Icon = "☀️" });
        QuickScenes.Add(new Scene { Id = "leave_home", Name = "Leave Home", Icon = "🏡" });
    }

    private async void LoadFavorites()
    {
        var favorites = await _dataService.GetFavoriteDevicesAsync();
        foreach (var device in favorites)
        {
            FavoriteDevices.Add(device);
        }
    }
}
