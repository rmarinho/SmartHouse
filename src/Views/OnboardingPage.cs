using SmartHouse.ViewModels;

namespace SmartHouse.Views;

public partial class OnboardingPage : ContentPage
{
    private readonly OnboardingViewModel _viewModel;

    public OnboardingPage(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnConnectClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.IsEnabled = false;
            button.Text = "Connecting...";
        }
        
        var success = await _viewModel.TestConnectionAsync();
        
        if (sender is Button btn)
        {
            btn.IsEnabled = true;
            btn.Text = "Connect";
        }
        
        if (success)
        {
            // Save that onboarding is complete
            Preferences.Set("HasCompletedOnboarding", true);
            
            // Wait a moment for user to see success message
            await Task.Delay(1000);
            
            // Navigate to main app using Window
            if (Window != null)
            {
                Window.Page = new AppShell();
            }
        }
    }

    private void OnSkipClicked(object sender, EventArgs e)
    {
        // Save that onboarding is complete
        Preferences.Set("HasCompletedOnboarding", true);
        
        // Navigate to main app using Window
        if (Window != null)
        {
            Window.Page = new AppShell();
        }
    }
}
