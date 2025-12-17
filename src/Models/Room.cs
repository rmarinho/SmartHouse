namespace SmartHouse.Models;

public class Room
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "🏠";
    public List<Device> Devices { get; set; } = new();
    public int DeviceCount => Devices.Count;
    public int ActiveDevices => Devices.Count(d => d.State == "on");
}
