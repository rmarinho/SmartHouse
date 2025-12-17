namespace SmartHouse.Models;

public class Alert
{
    public string Id { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "active";
    
    public Color SeverityColor => Severity switch
    {
        "critical" => Colors.Red,
        "warning" => Colors.Orange,
        "info" => Colors.Blue,
        _ => Colors.Gray
    };
}
