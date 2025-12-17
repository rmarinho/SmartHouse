namespace SmartHouse.Models;

public class Home
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Timezone { get; set; } = "UTC";
    public List<Room> Rooms { get; set; } = new();
    public List<User> Users { get; set; } = new();
}
