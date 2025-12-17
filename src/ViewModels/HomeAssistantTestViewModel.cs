using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHouse.Models;
using SmartHouse.Services;
using DeviceModel = SmartHouse.Models.Device;

namespace SmartHouse.ViewModels;

public partial class HomeAssistantTestViewModel : BaseViewModel
{
    private readonly IHomeAssistantService _homeAssistantService;

    public ObservableCollection<DeviceModel> Devices { get; } = new();
    public ObservableCollection<Scene> Scenes { get; } = new();

    [ObservableProperty]
    private string _url = string.Empty;

    [ObservableProperty]
    private string _token = string.Empty;

    [ObservableProperty]
    private string _testEntityId = string.Empty;

    [ObservableProperty]
    private string _configStatus = string.Empty;

    [ObservableProperty]
    private bool _isConfigSaved;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasConnectionResult))]
    private string _connectionResult = string.Empty;

    public bool HasConnectionResult => !string.IsNullOrEmpty(ConnectionResult);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasConfigResult))]
    private string _configResult = string.Empty;

    public bool HasConfigResult => !string.IsNullOrEmpty(ConfigResult);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDevices))]
    private string _deviceCount = string.Empty;

    public bool HasDevices => Devices.Count > 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasScenes))]
    private string _sceneCount = string.Empty;

    public bool HasScenes => Scenes.Count > 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasControlResult))]
    private string _controlResult = string.Empty;

    public bool HasControlResult => !string.IsNullOrEmpty(ControlResult);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public HomeAssistantTestViewModel(IHomeAssistantService homeAssistantService)
    {
        _homeAssistantService = homeAssistantService;

        // Load saved config
        Url = Preferences.Get("HA_Url", "");
        Token = Preferences.Get("HA_Token", "");
    }

    [RelayCommand]
    private void SaveConfig()
    {
        if (string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(Token))
        {
            ConfigStatus = "Please enter both URL and token";
            IsConfigSaved = false;
            return;
        }

        // Save to preferences
        Preferences.Set("HA_Url", Url);
        Preferences.Set("HA_Token", Token);

        // Configure service
        _homeAssistantService.Configure(Url, Token);

        ConfigStatus = "✓ Configuration saved";
        IsConfigSaved = true;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            ConnectionResult = "Testing...";

            if (string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(Token))
            {
                ConnectionResult = "❌ Please save configuration first";
                return;
            }

            var success = await _homeAssistantService.TestConnectionAsync(Url, Token);

            if (success)
            {
                ConnectionResult = "✓ Connection successful!";
            }
            else
            {
                ConnectionResult = "❌ Connection failed";
            }
        }
        catch (Exception ex)
        {
            ConnectionResult = $"❌ Error: {ex.Message}";
            ErrorMessage = ex.ToString();
        }
    }

    [RelayCommand]
    private async Task GetConfigAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            ConfigResult = "Loading...";

            var config = await _homeAssistantService.GetConfigAsync();

            if (config != null)
            {
                ConfigResult = $"✓ Home Assistant Config:\n" +
                              $"Location: {config.LocationName}\n" +
                              $"Version: {config.Version}\n" +
                              $"Timezone: {config.TimeZone}\n" +
                              $"Components: {config.Components.Count}";
            }
            else
            {
                ConfigResult = "❌ Failed to get config";
            }
        }
        catch (Exception ex)
        {
            ConfigResult = $"❌ Error: {ex.Message}";
            ErrorMessage = ex.ToString();
        }
    }

    [RelayCommand]
    private async Task GetDevicesAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            DeviceCount = "Loading...";
            Devices.Clear();

            var devices = await _homeAssistantService.GetDevicesAsync();

            foreach (var device in devices)
            {
                Devices.Add(device);
            }

            DeviceCount = $"✓ Found {devices.Count} devices";
            OnPropertyChanged(nameof(HasDevices));

            if (devices.Count == 0)
            {
                DeviceCount = "No devices found. Check your Home Assistant setup.";
            }
        }
        catch (Exception ex)
        {
            DeviceCount = $"❌ Error: {ex.Message}";
            ErrorMessage = ex.ToString();
        }
    }

    [RelayCommand]
    private async Task GetScenesAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            SceneCount = "Loading...";
            Scenes.Clear();

            var scenes = await _homeAssistantService.GetScenesAsync();

            foreach (var scene in scenes)
            {
                Scenes.Add(scene);
            }

            SceneCount = $"✓ Found {scenes.Count} scenes";
            OnPropertyChanged(nameof(HasScenes));

            if (scenes.Count == 0)
            {
                SceneCount = "No scenes found.";
            }
        }
        catch (Exception ex)
        {
            SceneCount = $"❌ Error: {ex.Message}";
            ErrorMessage = ex.ToString();
        }
    }

    [RelayCommand]
    private async Task TurnOnAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(TestEntityId))
            {
                ControlResult = "❌ Please enter an entity ID";
                return;
            }

            ControlResult = $"Turning on {TestEntityId}...";

            var success = await _homeAssistantService.TurnOnAsync(TestEntityId);

            if (success)
            {
                ControlResult = $"✓ {TestEntityId} turned ON";
            }
            else
            {
                ControlResult = $"❌ Failed to turn on {TestEntityId}";
            }
        }
        catch (Exception ex)
        {
            ControlResult = $"❌ Error: {ex.Message}";
            ErrorMessage = ex.ToString();
        }
    }

    [RelayCommand]
    private async Task TurnOffAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(TestEntityId))
            {
                ControlResult = "❌ Please enter an entity ID";
                return;
            }

            ControlResult = $"Turning off {TestEntityId}...";

            var success = await _homeAssistantService.TurnOffAsync(TestEntityId);

            if (success)
            {
                ControlResult = $"✓ {TestEntityId} turned OFF";
            }
            else
            {
                ControlResult = $"❌ Failed to turn off {TestEntityId}";
            }
        }
        catch (Exception ex)
        {
            ControlResult = $"❌ Error: {ex.Message}";
            ErrorMessage = ex.ToString();
        }
    }

    [RelayCommand]
    private async Task ToggleAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(TestEntityId))
            {
                ControlResult = "❌ Please enter an entity ID";
                return;
            }

            ControlResult = $"Toggling {TestEntityId}...";

            var success = await _homeAssistantService.ToggleAsync(TestEntityId);

            if (success)
            {
                ControlResult = $"✓ {TestEntityId} toggled";
            }
            else
            {
                ControlResult = $"❌ Failed to toggle {TestEntityId}";
            }
        }
        catch (Exception ex)
        {
            ControlResult = $"❌ Error: {ex.Message}";
            ErrorMessage = ex.ToString();
        }
    }

    [RelayCommand]
    private async Task ActivateSceneAsync(string? sceneId)
    {
        try
        {
            if (string.IsNullOrEmpty(sceneId)) return;

            ErrorMessage = string.Empty;
            ControlResult = $"Activating scene {sceneId}...";

            var success = await _homeAssistantService.ActivateSceneAsync(sceneId);

            if (success)
            {
                ControlResult = $"✓ Scene activated: {sceneId}";
            }
            else
            {
                ControlResult = $"❌ Failed to activate scene";
            }
        }
        catch (Exception ex)
        {
            ControlResult = $"❌ Error: {ex.Message}";
            ErrorMessage = ex.ToString();
        }
    }
}
