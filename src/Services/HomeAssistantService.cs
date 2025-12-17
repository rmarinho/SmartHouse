using SmartHouse.Models;
using DeviceModel = SmartHouse.Models.Device;
using System.Net.Http.Json;

namespace SmartHouse.Services;

public interface IHomeAssistantService
{
    Task<bool> TestConnectionAsync(string url, string token);
    Task<List<DeviceModel>> GetDevicesAsync();
    Task<DeviceModel?> GetDeviceAsync(string entityId);
    Task<bool> CallServiceAsync(string domain, string service, string entityId, Dictionary<string, object>? data = null);
    Task<List<Scene>> GetScenesAsync();
    Task<bool> ActivateSceneAsync(string sceneId);
}

public class HomeAssistantService : IHomeAssistantService
{
    private readonly HttpClient _httpClient;
    private string _baseUrl = string.Empty;
    private string _token = string.Empty;

    public HomeAssistantService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void Configure(string url, string token)
    {
        _baseUrl = url.TrimEnd('/');
        _token = token;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }

    public async Task<bool> TestConnectionAsync(string url, string token)
    {
        try
        {
            Configure(url, token);
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<DeviceModel>> GetDevicesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/states");
            if (!response.IsSuccessStatusCode)
                return new List<DeviceModel>();

            return new List<DeviceModel>();
        }
        catch
        {
            return new List<DeviceModel>();
        }
    }

    public async Task<DeviceModel?> GetDeviceAsync(string entityId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/states/{entityId}");
            if (!response.IsSuccessStatusCode)
                return null;

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CallServiceAsync(string domain, string service, string entityId, Dictionary<string, object>? data = null)
    {
        try
        {
            var payload = new
            {
                entity_id = entityId,
                data = data ?? new Dictionary<string, object>()
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/services/{domain}/{service}", payload);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Scene>> GetScenesAsync()
    {
        return new List<Scene>();
    }

    public async Task<bool> ActivateSceneAsync(string sceneId)
    {
        return await CallServiceAsync("scene", "turn_on", sceneId);
    }
}
