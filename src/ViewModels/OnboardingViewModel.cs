using SmartHouse.Services;

namespace SmartHouse.ViewModels;

public class OnboardingViewModel : BaseViewModel
{
    private readonly IHomeAssistantService _homeAssistantService;

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

    private bool _isConnecting;
    public bool IsConnecting
    {
        get => _isConnecting;
        set => SetProperty(ref _isConnecting, value);
    }

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public OnboardingViewModel(IHomeAssistantService homeAssistantService)
    {
        _homeAssistantService = homeAssistantService;
    }

    public async Task<bool> TestConnectionAsync()
    {
        if (string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(Token))
        {
            StatusMessage = "Please enter both URL and token";
            return false;
        }

        IsConnecting = true;
        StatusMessage = "Testing connection...";

        var success = await _homeAssistantService.TestConnectionAsync(Url, Token);

        if (success)
        {
            StatusMessage = "Connected successfully!";
        }
        else
        {
            StatusMessage = "Connection failed. Please check your URL and token.";
        }

        IsConnecting = false;
        return success;
    }
}
