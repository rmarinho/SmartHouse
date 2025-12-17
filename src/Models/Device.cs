namespace SmartHouse.Models;

public class Device
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime LastChanged { get; set; }
    public Dictionary<string, object> Attributes { get; set; } = new();
}
