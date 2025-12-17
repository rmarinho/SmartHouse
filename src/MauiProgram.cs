using Microsoft.Extensions.Logging;
using SmartHouse.Services;
using SmartHouse.ViewModels;
using SmartHouse.Views;
using System.Net.Http.Json;

namespace SmartHouse;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register Services
		builder.Services.AddSingleton<IDataService, DataService>();
		builder.Services.AddSingleton<IHomeAssistantService>(sp =>
		{
			var httpClient = new HttpClient();
			return new HomeAssistantService(httpClient);
		});

		// Register ViewModels
		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<RoomsViewModel>();
		builder.Services.AddTransient<AutomateViewModel>();
		builder.Services.AddTransient<AlertsViewModel>();
		builder.Services.AddTransient<OnboardingViewModel>();

		// Register Views
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<RoomsPage>();
		builder.Services.AddTransient<AutomatePage>();
		builder.Services.AddTransient<AlertsPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<OnboardingPage>();

		return builder.Build();
	}
}
