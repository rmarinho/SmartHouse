namespace SmartHouse.Models;

public class Scene
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "⚡";
    public List<string> Actions { get; set; } = new();
}
