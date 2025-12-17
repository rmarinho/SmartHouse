using SmartHouse.Models;
using System.Collections.ObjectModel;

namespace SmartHouse.ViewModels;

public class AlertsViewModel : BaseViewModel
{
    public ObservableCollection<Alert> CriticalAlerts { get; } = new();
    public ObservableCollection<Alert> Warnings { get; } = new();
    public ObservableCollection<ActivityLog> RecentActivity { get; } = new();

    public AlertsViewModel()
    {
        LoadAlerts();
    }

    private void LoadAlerts()
    {
        CriticalAlerts.Add(new Alert
        {
            Id = "smoke1",
            Severity = "critical",
            Source = "Kitchen Smoke Detector",
            Message = "Smoke Detected",
            CreatedAt = DateTime.Now.AddMinutes(-2)
        });

        Warnings.Add(new Alert
        {
            Id = "door1",
            Severity = "warning",
            Source = "Front Door",
            Message = "Front Door Open",
            CreatedAt = DateTime.Now.AddMinutes(-15)
        });

        Warnings.Add(new Alert
        {
            Id = "battery1",
            Severity = "warning",
            Source = "Motion Sensor",
            Message = "Low Battery",
            CreatedAt = DateTime.Now.AddHours(-1)
        });

        RecentActivity.Add(new ActivityLog
        {
            DeviceName = "Living Room Lights",
            Action = "Turned on",
            Timestamp = DateTime.Now.AddMinutes(-5)
        });

        RecentActivity.Add(new ActivityLog
        {
            DeviceName = "Thermostat",
            Action = "Temperature set to 21°C",
            Timestamp = DateTime.Now.AddMinutes(-12)
        });

        RecentActivity.Add(new ActivityLog
        {
            DeviceName = "Front Door",
            Action = "Locked by automation",
            Timestamp = DateTime.Now.AddHours(-1)
        });
    }
}

public class ActivityLog
{
    public string DeviceName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string TimeAgo
    {
        get
        {
            var diff = DateTime.Now - Timestamp;
            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes}m ago";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours}h ago";
            return $"{(int)diff.TotalDays}d ago";
        }
    }
}
