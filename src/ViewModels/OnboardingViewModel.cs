using CommunityToolkit.Mvvm.ComponentModel;
using SmartHouse.Services;

namespace SmartHouse.ViewModels;

public partial class OnboardingViewModel : BaseViewModel
{
    private readonly IHomeAssistantService _homeAssistantService;

    [ObservableProperty]
    private string _url = string.Empty;

    [ObservableProperty]
    private string _token = string.Empty;

    [ObservableProperty]
    private bool _isConnecting;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

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
