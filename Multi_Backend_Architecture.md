# Multi-Backend Architecture Guide

How to support multiple smart home backends (Home Assistant, HomeKit, Google Home, etc.) in Casa.

## Architecture Pattern

### Interface-Based Design

Create a common interface that all backends implement:

```
ISmartHomeBackend (interface)
├── HomeAssistantBackend (implementation)
├── HomeKitBackend (implementation)
├── GoogleHomeBackend (implementation)
└── SmartThingsBackend (implementation)
```

---

## Core Interface

```csharp
public interface ISmartHomeBackend
{
    // Connection
    string Name { get; }
    string Id { get; }
    Task<bool> TestConnectionAsync();
    Task<bool> ConnectAsync(BackendConfig config);
    Task DisconnectAsync();
    bool IsConnected { get; }
    
    // Devices
    Task<List<Device>> GetDevicesAsync();
    Task<Device?> GetDeviceAsync(string deviceId);
    
    // Control
    Task<bool> TurnOnAsync(string deviceId, Dictionary<string, object>? parameters = null);
    Task<bool> TurnOffAsync(string deviceId);
    Task<bool> ToggleAsync(string deviceId);
    Task<bool> SetValueAsync(string deviceId, string property, object value);
    
    // Scenes
    Task<List<Scene>> GetScenesAsync();
    Task<bool> ActivateSceneAsync(string sceneId);
    
    // Real-time updates
    event EventHandler<DeviceStateChangedEventArgs>? DeviceStateChanged;
    Task SubscribeToUpdatesAsync();
    Task UnsubscribeFromUpdatesAsync();
}
```

---

## Backend Configuration

```csharp
public class BackendConfig
{
    public string BackendType { get; set; } = string.Empty;
    public Dictionary<string, string> Settings { get; set; } = new();
}

// Home Assistant config
new BackendConfig
{
    BackendType = "HomeAssistant",
    Settings = new()
    {
        ["url"] = "http://192.168.1.100:8123",
        ["token"] = "YOUR_TOKEN"
    }
}

// HomeKit config
new BackendConfig
{
    BackendType = "HomeKit",
    Settings = new()
    {
        ["bridge_ip"] = "192.168.1.101",
        ["pin"] = "123-45-678"
    }
}
```

---

## Backend Manager

Manages multiple backends simultaneously:

```csharp
public class BackendManager
{
    private readonly List<ISmartHomeBackend> _backends = new();
    private readonly Dictionary<string, ISmartHomeBackend> _deviceToBackend = new();
    
    public void RegisterBackend(ISmartHomeBackend backend)
    {
        _backends.Add(backend);
        backend.DeviceStateChanged += OnDeviceStateChanged;
    }
    
    public async Task<List<Device>> GetAllDevicesAsync()
    {
        var allDevices = new List<Device>();
        
        foreach (var backend in _backends.Where(b => b.IsConnected))
        {
            var devices = await backend.GetDevicesAsync();
            
            // Map devices to backend
            foreach (var device in devices)
            {
                _deviceToBackend[device.Id] = backend;
                allDevices.Add(device);
            }
        }
        
        return allDevices;
    }
    
    public async Task<bool> ControlDeviceAsync(string deviceId, string action)
    {
        if (!_deviceToBackend.TryGetValue(deviceId, out var backend))
            return false;
            
        return action switch
        {
            "turn_on" => await backend.TurnOnAsync(deviceId),
            "turn_off" => await backend.TurnOffAsync(deviceId),
            "toggle" => await backend.ToggleAsync(deviceId),
            _ => false
        };
    }
    
    private void OnDeviceStateChanged(object? sender, DeviceStateChangedEventArgs e)
    {
        // Update UI or notify observers
        DeviceStateChanged?.Invoke(this, e);
    }
    
    public event EventHandler<DeviceStateChangedEventArgs>? DeviceStateChanged;
}
```

---

## Home Assistant Backend Implementation

```csharp
public class HomeAssistantBackend : ISmartHomeBackend
{
    private readonly HttpClient _httpClient;
    private string _baseUrl = string.Empty;
    private string _token = string.Empty;
    private bool _isConnected;
    
    public string Name => "Home Assistant";
    public string Id => "home_assistant";
    public bool IsConnected => _isConnected;
    
    public event EventHandler<DeviceStateChangedEventArgs>? DeviceStateChanged;
    
    public HomeAssistantBackend(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<bool> ConnectAsync(BackendConfig config)
    {
        _baseUrl = config.Settings["url"];
        _token = config.Settings["token"];
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
        
        _isConnected = await TestConnectionAsync();
        return _isConnected;
    }
    
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<List<Device>> GetDevicesAsync()
    {
        // Implementation from previous guide
        // ...
    }
    
    public async Task<bool> TurnOnAsync(string deviceId, Dictionary<string, object>? parameters = null)
    {
        var domain = deviceId.Split('.')[0];
        var payload = new Dictionary<string, object> { ["entity_id"] = deviceId };
        
        if (parameters != null)
            foreach (var kvp in parameters)
                payload[kvp.Key] = kvp.Value;
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}/api/services/{domain}/turn_on",
            payload
        );
        
        return response.IsSuccessStatusCode;
    }
    
    // ... other methods
}
```

---

## HomeKit Backend (Example Skeleton)

```csharp
public class HomeKitBackend : ISmartHomeBackend
{
    public string Name => "HomeKit";
    public string Id => "homekit";
    public bool IsConnected { get; private set; }
    
    public event EventHandler<DeviceStateChangedEventArgs>? DeviceStateChanged;
    
    public async Task<bool> ConnectAsync(BackendConfig config)
    {
        // Use HomeKit API/Bridge
        // This would require platform-specific code or a bridge
        return false;
    }
    
    public async Task<List<Device>> GetDevicesAsync()
    {
        // Query HomeKit accessories
        return new List<Device>();
    }
    
    public async Task<bool> TurnOnAsync(string deviceId, Dictionary<string, object>? parameters = null)
    {
        // Control HomeKit accessory
        return false;
    }
    
    // ... other methods
}
```

---

## Device ID Mapping

Since different backends have different ID formats, use prefixes:

```csharp
public static class DeviceIdHelper
{
    public static string CreateId(string backendId, string nativeId)
    {
        return $"{backendId}:{nativeId}";
    }
    
    public static (string BackendId, string NativeId) ParseId(string deviceId)
    {
        var parts = deviceId.Split(':', 2);
        return (parts[0], parts[1]);
    }
}

// Examples:
// "home_assistant:light.living_room"
// "homekit:12345678-ABCD-1234-EFGH-1234567890AB"
// "google_home:devices/123456789"
```

---

## Service Registration

Update `MauiProgram.cs`:

```csharp
// Register backends
builder.Services.AddSingleton<ISmartHomeBackend, HomeAssistantBackend>(sp =>
{
    var httpClient = new HttpClient();
    return new HomeAssistantBackend(httpClient);
});

builder.Services.AddSingleton<ISmartHomeBackend, HomeKitBackend>();
builder.Services.AddSingleton<ISmartHomeBackend, GoogleHomeBackend>();

// Register backend manager
builder.Services.AddSingleton<BackendManager>(sp =>
{
    var manager = new BackendManager();
    var backends = sp.GetServices<ISmartHomeBackend>();
    
    foreach (var backend in backends)
    {
        manager.RegisterBackend(backend);
    }
    
    return manager;
});
```

---

## Usage in ViewModels

```csharp
public class HomeViewModel : BaseViewModel
{
    private readonly BackendManager _backendManager;
    
    public HomeViewModel(BackendManager backendManager)
    {
        _backendManager = backendManager;
        _backendManager.DeviceStateChanged += OnDeviceStateChanged;
    }
    
    public async Task LoadDevicesAsync()
    {
        var allDevices = await _backendManager.GetAllDevicesAsync();
        
        // Group by room
        var byRoom = allDevices.GroupBy(d => d.Area);
        
        // Update UI
        foreach (var group in byRoom)
        {
            // Add to rooms collection
        }
    }
    
    public async Task ToggleDeviceAsync(string deviceId)
    {
        await _backendManager.ControlDeviceAsync(deviceId, "toggle");
    }
}
```

---

## Backend Settings UI

Let users configure multiple backends in Settings:

```xaml
<CollectionView ItemsSource="{Binding ConfiguredBackends}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Frame>
                <Grid ColumnDefinitions="Auto,*,Auto">
                    <Label Grid.Column="0" Text="{Binding Icon}" FontSize="32"/>
                    <VerticalStackLayout Grid.Column="1">
                        <Label Text="{Binding Name}" FontAttributes="Bold"/>
                        <Label Text="{Binding Status}"/>
                    </VerticalStackLayout>
                    <Switch Grid.Column="2" IsToggled="{Binding IsEnabled}"/>
                </Grid>
            </Frame>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>

<Button Text="+ Add Backend" Command="{Binding AddBackendCommand}"/>
```

---

## Backend Priority

If a device exists in multiple backends, define priority:

```csharp
public class BackendManager
{
    private readonly List<ISmartHomeBackend> _backends = new();
    
    public void SetPriority(string backendId, int priority)
    {
        // Reorder backends by priority
        // Higher priority backends are queried first
    }
    
    public async Task<Device?> GetDeviceAsync(string deviceId)
    {
        // Try each backend in priority order
        foreach (var backend in _backends.OrderByDescending(b => b.Priority))
        {
            var device = await backend.GetDeviceAsync(deviceId);
            if (device != null)
                return device;
        }
        return null;
    }
}
```

---

## Capability Mapping

Different backends support different features:

```csharp
public enum DeviceCapability
{
    OnOff,
    Brightness,
    ColorTemperature,
    RGB,
    Temperature,
    Humidity,
    Lock,
    Position
}

public interface ISmartHomeBackend
{
    Task<List<DeviceCapability>> GetCapabilitiesAsync(string deviceId);
}

// Use this to show/hide controls in UI
if (capabilities.Contains(DeviceCapability.Brightness))
{
    // Show brightness slider
}
```

---

## Data Synchronization

Keep local cache in sync with all backends:

```csharp
public class DeviceSyncService
{
    private readonly BackendManager _backendManager;
    private readonly IDataService _dataService;
    
    public async Task SyncAllAsync()
    {
        var allDevices = await _backendManager.GetAllDevicesAsync();
        await _dataService.SaveDevicesAsync(allDevices);
    }
    
    public async Task StartRealtimeSyncAsync()
    {
        foreach (var backend in _backendManager.Backends)
        {
            await backend.SubscribeToUpdatesAsync();
        }
    }
}
```

---

## Implementation Roadmap

### Phase 1: Foundation (Current)
- [x] Home Assistant REST API
- [ ] Multi-backend interface design
- [ ] Backend manager

### Phase 2: Home Assistant Complete
- [ ] WebSocket for real-time updates
- [ ] Full service support
- [ ] Area/room mapping

### Phase 3: Additional Backends
- [ ] HomeKit bridge integration
- [ ] Google Home API
- [ ] SmartThings API
- [ ] Amazon Alexa

### Phase 4: Advanced Features
- [ ] Cross-backend scenes
- [ ] Unified automation
- [ ] Device capability detection
- [ ] Priority and fallback

---

## Testing Multi-Backend

```csharp
[Test]
public async Task TestMultipleBackends()
{
    var manager = new BackendManager();
    
    // Add Home Assistant
    var ha = new HomeAssistantBackend(new HttpClient());
    await ha.ConnectAsync(new BackendConfig { /* ... */ });
    manager.RegisterBackend(ha);
    
    // Add HomeKit
    var hk = new HomeKitBackend();
    await hk.ConnectAsync(new BackendConfig { /* ... */ });
    manager.RegisterBackend(hk);
    
    // Get all devices from both
    var devices = await manager.GetAllDevicesAsync();
    
    Assert.IsTrue(devices.Count > 0);
    Assert.IsTrue(devices.Any(d => d.Id.StartsWith("home_assistant:")));
    Assert.IsTrue(devices.Any(d => d.Id.StartsWith("homekit:")));
}
```

---

## Best Practices

1. **Abstract Common Patterns** - Use interface for all backends
2. **Unique Device IDs** - Prefix with backend identifier
3. **Capability Detection** - Don't assume all devices support all features
4. **Error Handling** - One backend failing shouldn't crash the app
5. **Priority System** - Let users choose preferred backend
6. **Offline Support** - Cache last known state
7. **Real-time Sync** - Use WebSocket/events where available

---

## Summary

By using this architecture:
- ✅ Easy to add new backends
- ✅ Consistent API across all backends
- ✅ Users can connect multiple systems
- ✅ Devices from different platforms work together
- ✅ Maintainable and testable code

**Start with Home Assistant, then add others incrementally!**
