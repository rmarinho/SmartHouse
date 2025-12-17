using SmartHouse.Models;
using DeviceModel = SmartHouse.Models.Device;

namespace SmartHouse.Services;

public interface IDataService
{
    Task<Home?> GetCurrentHomeAsync();
    Task SaveHomeAsync(Home home);
    Task<List<DeviceModel>> GetFavoriteDevicesAsync();
    Task AddFavoriteDeviceAsync(string deviceId);
    Task RemoveFavoriteDeviceAsync(string deviceId);
}

public class DataService : IDataService
{
    private Home? _currentHome;
    private List<string> _favoriteDeviceIds = new();

    public Task<Home?> GetCurrentHomeAsync()
    {
        if (_currentHome == null)
        {
            _currentHome = new Home
            {
                Id = "home1",
                Name = "My Home",
                Timezone = "UTC",
                Rooms = new List<Room>
                {
                    new Room { Id = "living_room", Name = "Living Room", Icon = "🛋️" },
                    new Room { Id = "kitchen", Name = "Kitchen", Icon = "🍳" },
                    new Room { Id = "bedroom", Name = "Bedroom", Icon = "🛏️" },
                    new Room { Id = "bathroom", Name = "Bathroom", Icon = "🚿" },
                    new Room { Id = "office", Name = "Office", Icon = "💼" },
                    new Room { Id = "garage", Name = "Garage", Icon = "🚗" }
                }
            };
        }
        return Task.FromResult<Home?>(_currentHome);
    }

    public Task SaveHomeAsync(Home home)
    {
        _currentHome = home;
        return Task.CompletedTask;
    }

    public Task<List<DeviceModel>> GetFavoriteDevicesAsync()
    {
        return Task.FromResult(new List<DeviceModel>());
    }

    public Task AddFavoriteDeviceAsync(string deviceId)
    {
        if (!_favoriteDeviceIds.Contains(deviceId))
            _favoriteDeviceIds.Add(deviceId);
        return Task.CompletedTask;
    }

    public Task RemoveFavoriteDeviceAsync(string deviceId)
    {
        _favoriteDeviceIds.Remove(deviceId);
        return Task.CompletedTask;
    }
}
