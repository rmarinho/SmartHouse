using SmartHouse.Models;
using System.Collections.ObjectModel;

namespace SmartHouse.ViewModels;

public partial class AutomateViewModel : BaseViewModel
{
    public ObservableCollection<Automation> ActiveAutomations { get; } = new();
    public ObservableCollection<Automation> InactiveAutomations { get; } = new();

    public AutomateViewModel()
    {
        LoadAutomations();
    }

    private void LoadAutomations()
    {
        ActiveAutomations.Add(new Automation
        {
            Id = "night_mode",
            Name = "Night Mode",
            Icon = "🌙",
            Enabled = true
        });

        ActiveAutomations.Add(new Automation
        {
            Id = "motion_lights",
            Name = "Motion Lights",
            Icon = "🏃",
            Enabled = true
        });

        ActiveAutomations.Add(new Automation
        {
            Id = "climate_control",
            Name = "Climate Control",
            Icon = "🌡️",
            Enabled = true
        });

        InactiveAutomations.Add(new Automation
        {
            Id = "lock_night",
            Name = "Lock at Night",
            Icon = "🚪",
            Enabled = false
        });
    }
}
