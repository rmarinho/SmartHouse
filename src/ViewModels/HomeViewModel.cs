using SmartHouse.Models;
using SmartHouse.Services;
using System.Collections.ObjectModel;
using DeviceModel = SmartHouse.Models.Device;

namespace SmartHouse.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IHomeAssistantService _homeAssistantService;

    public ObservableCollection<Scene> QuickScenes { get; } = new();
    public ObservableCollection<DeviceModel> FavoriteDevices { get; } = new();

    private string _energyUsage = "2.4 kWh";
    public string EnergyUsage
    {
        get => _energyUsage;
        set => SetProperty(ref _energyUsage, value);
    }

    private string _securityStatus = "Armed";
    public string SecurityStatus
    {
        get => _securityStatus;
        set => SetProperty(ref _securityStatus, value);
    }

    private string _temperature = "21°C";
    public string Temperature
    {
        get => _temperature;
        set => SetProperty(ref _temperature, value);
    }

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
