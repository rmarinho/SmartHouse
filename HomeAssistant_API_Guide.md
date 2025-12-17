# Home Assistant API Integration Guide

Complete guide for connecting Casa to Home Assistant.

## What You Need

1. **Home Assistant instance** running (local or remote)
2. **Long-lived access token** from Home Assistant
3. **Network access** to your HA instance

## Getting Your Token

1. Open Home Assistant web interface
2. Click your profile (bottom left corner)
3. Scroll to **"Long-Lived Access Tokens"**
4. Click **"Create Token"**
5. Name it "Casa Mobile App"
6. **Copy immediately** (can't see it again!)

## Connection Test

```bash
curl http://192.168.1.100:8123/api/ \
  -H "Authorization: Bearer YOUR_TOKEN"
```

Expected: `{"message": "API running."}`

---

## REST API Overview

### Authentication Header
```
Authorization: Bearer YOUR_LONG_LIVED_ACCESS_TOKEN
```

### Key Endpoints

**Get all devices:**
```
GET /api/states
```

**Control device:**
```
POST /api/services/light/turn_on
{"entity_id": "light.living_room", "brightness": 128}
```

**Get single device:**
```
GET /api/states/light.living_room
```

---

## Common Service Calls

### Lights
```bash
# Turn on with brightness
POST /api/services/light/turn_on
{"entity_id": "light.living_room", "brightness": 255}

# Turn off
POST /api/services/light/turn_off
{"entity_id": "light.living_room"}

# Toggle
POST /api/services/light/toggle
{"entity_id": "light.living_room"}
```

### Switches & Plugs
```bash
POST /api/services/switch/turn_on
{"entity_id": "switch.coffee_maker"}
```

### Climate/Thermostats
```bash
POST /api/services/climate/set_temperature
{"entity_id": "climate.bedroom", "temperature": 22}
```

### Locks
```bash
POST /api/services/lock/lock
{"entity_id": "lock.front_door"}
```

### Scenes
```bash
POST /api/services/scene/turn_on
{"entity_id": "scene.movie_time"}
```

---

## C# Implementation

The project includes a complete `HomeAssistantService.cs` with methods:

```csharp
// Test connection
await service.TestConnectionAsync(url, token);

// Get all devices
var devices = await service.GetDevicesAsync();

// Turn on light with brightness
await service.TurnOnAsync("light.living_room", 
    new Dictionary<string, object> { ["brightness"] = 128 });

// Turn off
await service.TurnOffAsync("light.living_room");

// Toggle
await service.ToggleAsync("switch.coffee_maker");

// Activate scene
await service.ActivateSceneAsync("scene.movie_time");
```

---

## Entity Types Reference

| Domain | Description | States | Example ID |
|--------|-------------|--------|------------|
| `light` | Lights | `on`, `off` | `light.living_room` |
| `switch` | Switches/Plugs | `on`, `off` | `switch.coffee_maker` |
| `climate` | Thermostats | `heat`, `cool`, `off`, `auto` | `climate.bedroom` |
| `lock` | Smart locks | `locked`, `unlocked` | `lock.front_door` |
| `cover` | Blinds/Doors | `open`, `closed` | `cover.garage_door` |
| `fan` | Fans | `on`, `off` | `fan.bedroom_fan` |
| `sensor` | Sensors | Any value | `sensor.temperature` |
| `binary_sensor` | Binary sensors | `on`, `off` | `binary_sensor.motion` |
| `scene` | Scenes | N/A | `scene.movie_time` |

---

## WebSocket API (Real-time Updates)

For real-time device state updates without polling:

### Connection Steps
1. Connect: `ws://your-ha-ip:8123/api/websocket`
2. Authenticate with token
3. Subscribe to `state_changed` events
4. Receive real-time updates

### Message Examples

**Server sends on connect:**
```json
{"type": "auth_required", "ha_version": "2024.12.0"}
```

**Client authenticates:**
```json
{"type": "auth", "access_token": "YOUR_TOKEN"}
```

**Subscribe to changes:**
```json
{"id": 1, "type": "subscribe_events", "event_type": "state_changed"}
```

**Receive update:**
```json
{
  "type": "event",
  "event": {
    "entity_id": "light.living_room",
    "new_state": {"state": "on", "attributes": {"brightness": 255}}
  }
}
```

---

## Testing Your Integration

### 1. Test from Onboarding Page

1. Run the app
2. Enter your HA URL: `http://192.168.1.100:8123`
3. Enter your token
4. Click "Connect"
5. Should show "Connected! Found X devices"

### 2. Load Real Devices

Once connected, the app will:
- Load all entities from your HA
- Display them by room
- Show current states
- Enable device controls

### 3. Control Devices

Toggle switches and lights - they should actually control your devices!

---

## Implementation Checklist

- [x] Create `HomeAssistantService.cs` with REST API methods
- [x] Add authentication headers
- [x] Parse entity responses
- [ ] Wire up device controls in UI
- [ ] Add WebSocket for real-time updates
- [ ] Implement error handling and retries
- [ ] Add offline mode with caching

---

## Troubleshooting

### Connection Issues

**401 Unauthorized**
- Invalid or expired token
- Solution: Create new token in HA

**Connection Refused**
- HA not running
- Wrong IP or port
- Solution: Verify HA is accessible

**Timeout**
- Network issue
- Firewall blocking
- Solution: Check network connectivity

### No Devices Showing

- Token might not have permissions
- No entities in HA
- Check HA has devices configured

### Controls Not Working

- Service call syntax error
- Entity doesn't support the service
- Check HA logs for errors

---

## Security Best Practices

1. **Use HTTPS** in production
2. **Store tokens securely** (SecureStorage/Keychain)
3. **Never commit tokens** to git
4. **Rotate tokens** regularly
5. **Use separate tokens** per app

---

## Next Steps

1. **Update `HomeAssistantService.cs`** with full implementation
2. **Test connection** from app
3. **Load real devices** into UI
4. **Wire up controls** to call services
5. **Add WebSocket** for real-time sync
6. **Implement other backends** (HomeKit, Google Home, etc.)

---

## Resources

- 📚 [Home Assistant REST API Docs](https://developers.home-assistant.io/docs/api/rest/)
- 📚 [WebSocket API Docs](https://developers.home-assistant.io/docs/api/websocket/)
- 📚 [Service Call Reference](https://www.home-assistant.io/docs/scripts/service-calls/)
- 📚 [Entity State Object](https://www.home-assistant.io/docs/configuration/state_object/)

---

**Ready to connect! 🏠✨**
