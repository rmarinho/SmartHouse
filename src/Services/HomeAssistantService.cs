using SmartHouse.Models;
using DeviceModel = SmartHouse.Models.Device;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHouse.Services;

public interface IHomeAssistantService
{
    void Configure(string url, string token);
    Task<bool> TestConnectionAsync(string url, string token);
    Task<HAConfig?> GetConfigAsync();
    Task<List<DeviceModel>> GetDevicesAsync();
    Task<DeviceModel?> GetDeviceAsync(string entityId);
    Task<bool> CallServiceAsync(string domain, string service, string entityId, Dictionary<string, object>? data = null);
    Task<bool> TurnOnAsync(string entityId, Dictionary<string, object>? data = null);
    Task<bool> TurnOffAsync(string entityId);
    Task<bool> ToggleAsync(string entityId);
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

            var json = await response.Content.ReadAsStringAsync();
            var entities = JsonSerializer.Deserialize<List<HAEntity>>(json);

            if (entities == null) return new List<DeviceModel>();

            return entities.Select(e => new DeviceModel
            {
                Id = e.EntityId,
                Name = GetAttributeString(e.Attributes, "friendly_name") ?? e.EntityId,
                Domain = e.EntityId.Split('.')[0],
                Area = GetAttributeString(e.Attributes, "area_id") ?? string.Empty,
                State = e.State,
                LastChanged = e.LastChanged,
                Attributes = e.Attributes.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)kvp.Value
                )
            }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting devices: {ex.Message}");
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
            var payload = new Dictionary<string, object>
            {
                ["entity_id"] = entityId
            };

            if (data != null)
            {
                foreach (var kvp in data)
                {
                    payload[kvp.Key] = kvp.Value;
                }
            }

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/services/{domain}/{service}", payload);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<HAConfig?> GetConfigAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/config");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<HAConfig>(json);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> TurnOnAsync(string entityId, Dictionary<string, object>? data = null)
    {
        var domain = entityId.Split('.')[0];
        return await CallServiceAsync(domain, "turn_on", entityId, data);
    }

    public async Task<bool> TurnOffAsync(string entityId)
    {
        var domain = entityId.Split('.')[0];
        return await CallServiceAsync(domain, "turn_off", entityId);
    }

    public async Task<bool> ToggleAsync(string entityId)
    {
        var domain = entityId.Split('.')[0];
        return await CallServiceAsync(domain, "toggle", entityId);
    }

    public async Task<List<Scene>> GetScenesAsync()
    {
        var devices = await GetDevicesAsync();
        return devices
            .Where(d => d.Domain == "scene")
            .Select(d => new Scene { Id = d.Id, Name = d.Name, Icon = "✨" })
            .ToList();
    }

    public async Task<bool> ActivateSceneAsync(string sceneId)
    {
        return await CallServiceAsync("scene", "turn_on", sceneId);
    }

    private static string? GetAttributeString(Dictionary<string, JsonElement> attributes, string key)
    {
        if (attributes.TryGetValue(key, out var value))
        {
            if (value.ValueKind == JsonValueKind.String)
                return value.GetString();
        }
        return null;
    }
}

// Models for deserializing Home Assistant responses
public class HAEntity
{
    [JsonPropertyName("entity_id")]
    public string EntityId { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public Dictionary<string, JsonElement> Attributes { get; set; } = new();

    [JsonPropertyName("last_changed")]
    public DateTime LastChanged { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }
}

public class HAConfig
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("location_name")]
    public string LocationName { get; set; } = string.Empty;

    [JsonPropertyName("time_zone")]
    public string TimeZone { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("components")]
    public List<string> Components { get; set; } = new();
}
