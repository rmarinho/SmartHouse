namespace SmartHouse.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void OnResetOnboardingTapped(object sender, EventArgs e)
    {
        // Clear onboarding flag
        Preferences.Remove("HasCompletedOnboarding");
        
        // Navigate back to onboarding using Window
        if (Window != null)
        {
            var onboardingPage = Handler?.MauiContext?.Services
                .GetService<OnboardingPage>();
            
            if (onboardingPage != null)
            {
                Window.Page = onboardingPage;
            }
        }
    }
}
