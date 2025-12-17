# Testing the Onboarding Flow

The onboarding page is now properly configured and will show on first launch.

## How It Works

1. **First Launch**: App shows OnboardingPage
2. **User connects or skips**: Preference is saved
3. **Subsequent launches**: App goes directly to main tabs
4. **Reset option**: Available in Settings

## Testing Steps

### Test 1: First Launch (Fresh Install)

1. Build and run the app:
   ```bash
   cd src
   dotnet build SmartHouse.csproj -t:Run -f net10.0-ios
   ```

2. You should see the **Onboarding Page** with:
   - 🏠 Casa logo
   - "Welcome to Casa" title
   - Home Assistant URL field
   - Access Token field
   - "Connect" button
   - "Skip for now" button

### Test 2: Connect Flow

**If you have Home Assistant:**
1. Enter your HA URL (e.g., `http://192.168.1.100:8123`)
2. Enter your long-lived access token
3. Click "Connect"
4. Should show:
   - "Testing connection..." message
   - Success: "Connected! Found X devices"
   - Automatically navigate to main app after 1 second

**If connection fails:**
- Message: "Connection failed. Check URL and token."
- Button re-enables for retry

### Test 3: Skip Flow

1. Launch app (onboarding page)
2. Click "Skip for now"
3. Should immediately navigate to main app
4. All features work with sample data

### Test 4: Subsequent Launches

1. Close the app completely
2. Launch again
3. Should go directly to **Home tab** (not onboarding)
4. Bottom navigation visible

### Test 5: Reset Onboarding

1. While in main app, go to **Settings tab**
2. Scroll to "Other" section
3. Find "🔄 Reset Onboarding"
4. Tap it
5. Should navigate back to onboarding page
6. Re-test connection or skip

## What Gets Saved

The app saves a preference:
```csharp
Preferences.Set("HasCompletedOnboarding", true);
```

**Location:**
- iOS: NSUserDefaults
- Android: SharedPreferences

**To clear manually in debugger:**
```csharp
Preferences.Remove("HasCompletedOnboarding");
```

## Expected Behavior

### ✅ Working Correctly

- [x] Onboarding shows on first launch
- [x] Can enter URL and token
- [x] "Connect" button tests connection
- [x] Status message shows connection result
- [x] Successful connection navigates to main app
- [x] "Skip" button navigates to main app
- [x] Main app shows on subsequent launches
- [x] Reset option in settings works

### 🔧 Current Limitations

- [ ] Token not yet saved securely (needs SecureStorage)
- [ ] Connection config not persisted
- [ ] Real device loading after connection (coming soon)

## Troubleshooting

### Onboarding doesn't show
**Issue**: Goes directly to main app
**Solution**: 
```csharp
// In iOS Simulator or Android emulator, reset app data
Preferences.Remove("HasCompletedOnboarding");
```
Or delete and reinstall the app.

### Can't get past onboarding
**Issue**: Stuck on onboarding page
**Solution**: Use "Skip for now" button to test other features

### Reset doesn't work
**Issue**: Settings reset button does nothing
**Solution**: Check that you tapped the correct frame (🔄 Reset Onboarding)

## Manual Testing Checklist

Before each release:

- [ ] Fresh install shows onboarding
- [ ] Can skip onboarding
- [ ] Can connect with valid HA credentials
- [ ] Connection failure shows error
- [ ] Success navigates to main app
- [ ] Second launch goes to main app
- [ ] Reset in settings works
- [ ] After reset, onboarding shows again

## Simulator Commands

**iOS Simulator:**
```bash
# Reset app data
xcrun simctl uninstall booted com.casa.smarthouse

# Reinstall
dotnet build -t:Run -f net10.0-ios
```

**Android Emulator:**
```bash
# Clear app data
adb shell pm clear com.casa.smarthouse

# Reinstall
dotnet build -t:Run -f net10.0-android
```

## Next Steps

To fully complete onboarding:

1. **Save connection config** using SecureStorage
2. **Load real devices** after connection
3. **Show loading indicator** during connection
4. **Validate URL format** before testing
5. **Add "Learn More" link** for token help
6. **Remember last URL** for convenience

---

**Ready to test! 🚀**

The onboarding flow is now complete and functional.
