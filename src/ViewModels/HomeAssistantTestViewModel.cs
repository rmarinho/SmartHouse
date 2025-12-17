using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartHouse.Models;
using SmartHouse.Services;
using DeviceModel = SmartHouse.Models.Device;

namespace SmartHouse.ViewModels;

public class HomeAssistantTestViewModel : BaseViewModel
{
    private readonly IHomeAssistantService _homeAssistantService;

    public ObservableCollection<DeviceModel> Devices { get; } = new();
    public ObservableCollection<Scene> Scenes { get; } = new();

    private string _url = string.Empty;
    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    private string _token = string.Empty;
    public string Token
    {
        get => _token;
        set => SetProperty(ref _token, value);
    }

    private string _testEntityId = string.Empty;
    public string TestEntityId
    {
        get => _testEntityId;
        set => SetProperty(ref _testEntityId, value);
    }

    private string _configStatus = string.Empty;
    public string ConfigStatus
    {
        get => _configStatus;
        set => SetProperty(ref _configStatus, value);
    }

    private bool _isConfigSaved;
    public bool IsConfigSaved
    {
        get => _isConfigSaved;
        set => SetProperty(ref _isConfigSaved, value);
    }

    private string _connectionResult = string.Empty;
    public string ConnectionResult
    {
        get => _connectionResult;
        set
        {
            SetProperty(ref _connectionResult, value);
            OnPropertyChanged(nameof(HasConnectionResult));
        }
    }

    public bool HasConnectionResult => !string.IsNullOrEmpty(ConnectionResult);

    private string _configResult = string.Empty;
    public string ConfigResult
    {
        get => _configResult;
        set
        {
            SetProperty(ref _configResult, value);
            OnPropertyChanged(nameof(HasConfigResult));
        }
    }

    public bool HasConfigResult => !string.IsNullOrEmpty(ConfigResult);

    private string _deviceCount = string.Empty;
    public string DeviceCount
    {
        get => _deviceCount;
        set
        {
            SetProperty(ref _deviceCount, value);
            OnPropertyChanged(nameof(HasDevices));
        }
    }

    public bool HasDevices => Devices.Count > 0;

    private string _sceneCount = string.Empty;
    public string SceneCount
    {
        get => _sceneCount;
        set
        {
            SetProperty(ref _sceneCount, value);
            OnPropertyChanged(nameof(HasScenes));
        }
    }

    public bool HasScenes => Scenes.Count > 0;

    private string _controlResult = string.Empty;
    public string ControlResult
    {
        get => _controlResult;
        set
        {
            SetProperty(ref _controlResult, value);
            OnPropertyChanged(nameof(HasControlResult));
        }
    }

    public bool HasControlResult => !string.IsNullOrEmpty(ControlResult);

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ICommand SaveConfigCommand { get; }
    public ICommand TestConnectionCommand { get; }
    public ICommand GetConfigCommand { get; }
    public ICommand GetDevicesCommand { get; }
    public ICommand GetScenesCommand { get; }
    public ICommand TurnOnCommand { get; }
    public ICommand TurnOffCommand { get; }
    public ICommand ToggleCommand { get; }
    public ICommand ActivateSceneCommand { get; }

    public HomeAssistantTestViewModel(IHomeAssistantService homeAssistantService)
    {
        _homeAssistantService = homeAssistantService;

        // Load saved config
        Url = Preferences.Get("HA_Url", "");
        Token = Preferences.Get("HA_Token", "");

        SaveConfigCommand = new Command(OnSaveConfig);
        TestConnectionCommand = new Command(async () => await OnTestConnection());
        GetConfigCommand = new Command(async () => await OnGetConfig());
        GetDevicesCommand = new Command(async () => await OnGetDevices());
        GetScenesCommand = new Command(async () => await OnGetScenes());
        TurnOnCommand = new Command(async () => await OnTurnOn());
        TurnOffCommand = new Command(async () => await OnTurnOff());
        ToggleCommand = new Command(async () => await OnToggle());
        ActivateSceneCommand = new Command<string>(async (sceneId) => await OnActivateScene(sceneId));
    }

    private void OnSaveConfig()
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

    private async Task OnTestConnection()
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

    private async Task OnGetConfig()
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

    private async Task OnGetDevices()
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

    private async Task OnGetScenes()
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

    private async Task OnTurnOn()
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

    private async Task OnTurnOff()
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

    private async Task OnToggle()
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

    private async Task OnActivateScene(string? sceneId)
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
