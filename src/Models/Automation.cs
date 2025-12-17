namespace SmartHouse.Models;

public class Automation
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "⚡";
    public bool Enabled { get; set; } = true;
    public AutomationTrigger Trigger { get; set; } = new();
    public List<AutomationCondition> Conditions { get; set; } = new();
    public List<AutomationAction> Actions { get; set; } = new();
    public SafetyRules SafetyRules { get; set; } = new();
}

public class AutomationTrigger
{
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Config { get; set; } = new();
}

public class AutomationCondition
{
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Config { get; set; } = new();
}

public class AutomationAction
{
    public string Service { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}

public class SafetyRules
{
    public int? CooldownSeconds { get; set; }
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
    public bool RequireConfirmation { get; set; }
}
