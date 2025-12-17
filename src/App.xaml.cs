using Microsoft.Extensions.DependencyInjection;

namespace SmartHouse;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Check if user has completed onboarding
		var hasCompletedOnboarding = Preferences.Get("HasCompletedOnboarding", false);
		
		if (hasCompletedOnboarding)
		{
			// Go to main app
			return new Window(new AppShell());
		}
		else
		{
			// Show onboarding first
			var onboardingPage = Handler?.MauiContext?.Services.GetService<Views.OnboardingPage>();
			if (onboardingPage != null)
			{
				return new Window(onboardingPage);
			}
			return new Window(new AppShell());
		}
	}
}