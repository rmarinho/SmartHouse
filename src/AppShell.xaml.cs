using SmartHouse.Views;

namespace SmartHouse;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Register routes for navigation
		Routing.RegisterRoute(nameof(HomeAssistantTestPage), typeof(HomeAssistantTestPage));
		Routing.RegisterRoute(nameof(RoomDetailPage), typeof(RoomDetailPage));
	}
}
