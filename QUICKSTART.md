# Quick Start Guide - Casa Smart Home App

## What You Have

A fully structured .NET MAUI smart home control application with:
- ✅ Complete UI for all 5 main screens
- ✅ Navigation shell with bottom tabs
- ✅ Data models and services
- ✅ MVVM architecture
- ✅ Home Assistant integration foundation
- ✅ Onboarding flow
- ✅ Comprehensive documentation

## How to Run

### Prerequisites
```bash
# Check .NET version (needs 10.0+)
dotnet --version

# On macOS, you need Xcode installed
xcode-select --install
```

### Build and Run

```bash
# Navigate to project
cd /Users/ruimarinho/Projects/SmartHouse

# Restore packages
dotnet restore src/SmartHouse.csproj

# Build for iOS Simulator
dotnet build src/SmartHouse.csproj -f net10.0-ios -c Debug

# Run on iOS Simulator
dotnet build src/SmartHouse.csproj -t:Run -f net10.0-ios
```

### For Android
```bash
dotnet build src/SmartHouse.csproj -f net10.0-android -c Debug
dotnet build src/SmartHouse.csproj -t:Run -f net10.0-android
```

## What You'll See

When you run the app, you'll experience:

1. **Onboarding Page** (First launch)
   - Welcome screen with logo
   - Home Assistant connection form
   - "Connect" button (currently does basic validation)
   - "Skip for now" option

2. **Home Tab**
   - Welcome header
   - Quick Scenes (Good Night, Good Morning, Leave Home)
   - Status Cards (Energy, Security)
   - Climate Card
   - Favorite Devices

3. **Rooms Tab**
   - 2-column grid of rooms
   - 6 sample rooms (Living Room, Kitchen, Bedroom, Bathroom, Office, Garage)
   - Device counts per room

4. **Automate Tab**
   - Active automations (3 examples)
   - Inactive automations (1 example)
   - "Create Automation" button

5. **Alerts Tab**
   - Critical alerts section (1 smoke alert)
   - Warnings section (2 warnings)
   - Recent activity log (3 items)

6. **Settings Tab**
   - Homes section
   - Users & Access
   - Integrations (Home Assistant, HomeKit)
   - Notifications toggles
   - Security settings
   - About & Help

## Current Limitations

### What Works
- ✅ All UI is visible and styled
- ✅ Navigation between tabs
- ✅ App builds successfully
- ✅ Onboarding form accepts input

### What's Mock/In Progress
- ⏳ Device controls (toggles, sliders) - UI only
- ⏳ Home Assistant connection - validates but doesn't connect yet
- ⏳ Real-time updates - WebSocket not implemented
- ⏳ Data persistence - no local storage yet
- ⏳ Scene activation - button taps have no effect
- ⏳ Room detail navigation - not wired up yet

## Quick Customization

### Change App Name
Edit `src/SmartHouse.csproj`:
```xml
<ApplicationTitle>Your App Name</ApplicationTitle>
<ApplicationId>com.yourcompany.appname</ApplicationId>
```

### Add a New Room
Edit `src/Services/DataService.cs`, in `GetCurrentHomeAsync()`:
```csharp
new Room { Id = "new_room", Name = "New Room", Icon = "🏠" }
```

### Change Colors
Edit `src/Resources/Styles/Colors.xaml` to customize the color scheme.

### Add a Scene
Edit `src/ViewModels/HomeViewModel.cs`, in `LoadQuickScenes()`:
```csharp
QuickScenes.Add(new Scene { 
    Id = "my_scene", 
    Name = "My Scene", 
    Icon = "✨" 
});
```

## Development Workflow

### Recommended Order
1. **Connect to Real HA** - Implement full HomeAssistantService methods
2. **Add Navigation** - Wire up room taps, detail views
3. **Make Controls Work** - Connect toggle/slider events to service calls
4. **Add Persistence** - SQLite for local cache, SecureStorage for tokens
5. **WebSocket** - Real-time state updates
6. **Polish** - Animations, error handling, loading states

### File You'll Edit Most
- `Services/HomeAssistantService.cs` - Add real API calls
- `ViewModels/*.cs` - Add commands for button clicks
- `Views/*.xaml` - Bind controls to ViewModel commands
- `Models/*.cs` - Extend as needed for your features

## Testing with Home Assistant

To connect to a real Home Assistant instance:

1. Make sure you have Home Assistant running
2. Get your long-lived access token:
   - Profile → Long-Lived Access Tokens → Create Token
3. Run the app and enter:
   - URL: `http://your-ha-ip:8123`
   - Token: (paste your token)
4. Implement the service methods to parse responses

## Debugging

### Enable More Logging
In `MauiProgram.cs`:
```csharp
#if DEBUG
    builder.Logging.AddDebug();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
#endif
```

### View Console Output
- **iOS Simulator**: Check Xcode console or VS output window
- **Android**: Use `adb logcat` or Android Studio logcat

### Common Issues

**Build Errors:**
- Clean: `dotnet clean src/SmartHouse.csproj`
- Rebuild: `dotnet build src/SmartHouse.csproj`

**XAML Errors:**
- Check for unescaped characters (`&` should be `and`)
- Verify all DynamicResource keys exist in Colors.xaml

**Navigation Not Working:**
- Ensure route is registered in AppShell.xaml
- Check page is registered in MauiProgram.cs

## Next Steps

### Immediate Tasks
1. Implement `GetDevicesAsync()` in HomeAssistantService
2. Add command bindings for scene buttons
3. Create RoomDetailPage navigation
4. Add loading indicators

### Learning Resources
- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [Home Assistant API](https://developers.home-assistant.io/docs/api/rest/)
- [MVVM Pattern](https://learn.microsoft.com/dotnet/architecture/maui/mvvm)

## Project Files Reference

```
Key Files to Know:
├── src/App.xaml.cs              - App entry point
├── src/AppShell.xaml            - Navigation structure
├── src/MauiProgram.cs           - Dependency injection setup
├── src/Services/                - Backend logic
├── src/ViewModels/              - Screen logic
└── src/Views/                   - UI screens

Documentation:
├── README.md                    - Full project guide
├── PRD.md                       - Product requirements
├── IMPLEMENTATION.md            - This implementation summary
└── prompt.md                    - Original requirements
```

## Getting Help

The codebase is well-documented with:
- XML comments on public methods
- Clear folder structure
- Descriptive variable names
- Separation of concerns

If you need to understand:
- **How navigation works** → Check `AppShell.xaml`
- **How data flows** → Check ViewModels
- **How to call HA** → Check `HomeAssistantService.cs`
- **What models exist** → Check `Models/` folder

## Success Checklist

After running the app, you should be able to:
- [ ] See the onboarding page
- [ ] Navigate to all 5 tabs
- [ ] See sample data on each screen
- [ ] View room grid on Rooms tab
- [ ] See automation list on Automate tab
- [ ] View alerts on Alerts tab
- [ ] Access settings on Settings tab

**You're ready to build!** 🚀

The foundation is solid. Now it's time to connect real devices and make it interactive.
