# Casa - Smart Home Control App

## Product Requirements Document (PRD)

### Problem Statement
Homeowners with multiple smart devices from different brands are forced to juggle multiple vendor apps, creating a fragmented and frustrating user experience. There's a need for a unified, fast, and privacy-focused mobile app that brings all smart home controls together in one place.

### Goals
1. **Unified Interface**: One fast, consistent UI to control devices from different brands
2. **Privacy First**: Local-first approach with minimal cloud dependency, prioritizing user privacy
3. **Household Friendly**: Multi-user roles (Owner/Admin/Member/Guest) with shared favorites and safe controls
4. **Powerful Automations**: Safe, testable automations with audit logs

### Target Users

#### Primary User
A homeowner who is tired of managing multiple vendor apps and wants a single, unified interface for all smart home devices.

#### Secondary User
Family members and roommates who need simple, daily controls without complexity.

#### Advanced User
Power users with Home Assistant who want a clean, mobile-first UX for their existing setup.

### Scope (MVP)

#### In Scope
- Home Assistant integration via REST/WebSocket
- Core device types: lights, switches, plugs, climate, sensors, locks, media players
- Dashboard with status summary and favorites
- Room-based device organization
- Scene management and execution
- Basic automation builder with If/Then logic
- Alert system for critical events
- Multi-user support with role-based permissions
- Biometric security for sensitive controls
- Offline mode with cached state

#### Out of Scope (Future Phases)
- Direct HomeKit/Google Home/SmartThings integration
- Advanced automation templates and AI suggestions
- Energy analytics and insights
- Voice control integration
- Custom dashboards and widgets
- Third-party plugin system
- Video camera streams
- Advanced scheduling and calendar integration

### Success Metrics
1. **Performance**: Home screen renders in < 300ms from cache
2. **Adoption**: 80% of users complete onboarding successfully
3. **Engagement**: Users control at least 3 devices daily
4. **Reliability**: 99% uptime for core features
5. **User Satisfaction**: NPS score > 50

### Non-Functional Requirements

#### Performance
- Home screen < 300ms to render from cache
- Background refresh every 30 seconds
- Smooth animations at 60fps

#### Reliability
- Automatic retries with exponential backoff
- Clear offline indicators
- Queue actions when offline

#### Privacy
- No data selling
- Optional analytics (off by default)
- Encrypted local storage

#### Observability
- Debug logs with export option
- Crash reporting (opt-in)

---

## UX Flows

### 1. Onboarding Flow
```
Start → Welcome Screen → Enter HA URL → Enter Token → Test Connection → Success/Retry → Dashboard
```

**Steps:**
1. User launches app for first time
2. Welcome screen explains app benefits
3. User enters Home Assistant URL
4. User enters long-lived access token
5. App tests connection
6. On success: Navigate to dashboard
7. On failure: Show error and allow retry
8. Option to skip for later

### 2. Dashboard Flow
```
Dashboard → Quick Actions (Scenes) → Status Cards → Favorites
```

**Interactions:**
- Pull to refresh
- Tap quick scene to execute
- Tap status card for detail
- Toggle favorite devices
- Tap "See All" to view room

### 3. Room Control Flow
```
Rooms → Select Room → Room Detail → Device List → Device Control
```

**Steps:**
1. User taps Rooms tab
2. Grid of room tiles displayed
3. User taps a room
4. Room detail shows all devices
5. User controls device (toggle, slider, etc.)
6. Quick actions (All On/All Off)

### 4. Automation Creation Flow
```
Automate → Create → Choose Trigger → Add Conditions → Add Actions → Safety Rules → Save
```

**Steps:**
1. User taps "Create Automation"
2. Choose trigger type (time, device, location)
3. Configure trigger details
4. Add optional conditions
5. Add actions (device controls)
6. Set safety rules (cooldown, quiet hours)
7. Test/simulate automation
8. Save and enable

### 5. Alert Response Flow
```
Alert Notification → Alerts Page → View Detail → Take Action → Dismiss/Acknowledge
```

**Steps:**
1. Critical alert appears as push notification
2. User opens app to Alerts page
3. Alert detail shows source and severity
4. Recommended actions displayed
5. User takes action or dismisses
6. Alert logged in activity history

---

## Screen Wireframes Description

### Home Screen (Dashboard)
**Layout:**
- Header: Welcome message + current home name
- Quick Scenes: Horizontal scrollable cards (3-4 visible)
- Status Cards: 2-column grid (Energy, Security)
- Climate Card: Full-width card
- Favorites: Vertical list of favorite devices

**Components:**
- Scene cards with icon, name, and tap gesture
- Status cards showing current value and trend
- Device cards with toggle switches or status display
- Pull-to-refresh indicator

### Rooms Screen
**Layout:**
- Header: "Rooms" title
- Room Grid: 2-column grid of room tiles
- Each tile shows: Icon, name, device count, active devices

**Interactions:**
- Tap room tile → Navigate to room detail
- Shows visual indicator for active devices

### Room Detail Screen
**Layout:**
- Header: Room icon + name + device count
- Quick Actions: "All On" and "All Off" buttons
- Device List: Vertical list of all devices in room
- Device card variations by type:
  - Lights: Toggle + brightness slider
  - Switches: Simple toggle
  - Sensors: Read-only display
  - Climate: Temperature display + controls

**Components:**
- Collapsible device cards
- Inline controls (no navigation required)
- Last updated timestamp

### Automate Screen
**Layout:**
- Header: "Automations" title
- Active Automations: List with toggle switches
- Inactive Automations: Collapsed list
- Add Button: Fixed at bottom

**Components:**
- Automation cards with icon, name, description
- Toggle to enable/disable
- Tap card for edit/detail
- FAB for "Create Automation"

### Alerts Screen
**Layout:**
- Header: "Alerts & Activity"
- Critical Alerts: Red-bordered cards at top
- Warnings: Orange-bordered cards
- Recent Activity: Gray cards with timeline

**Components:**
- Alert cards with severity indicator
- Timestamp and relative time
- Action buttons for critical alerts
- Dismissible warnings

### Settings Screen
**Layout:**
- Sections: Homes, Users, Integrations, Notifications, Security, Other
- List of settings items with navigation arrows
- Toggle switches for boolean settings

**Components:**
- Section headers
- Settings rows with icons
- Disclosure indicators (›)
- Toggle switches

### Onboarding Screen
**Layout:**
- Centered logo/icon
- Welcome text
- Form fields for URL and token
- Help text with instructions
- Connect button
- Skip option at bottom

**Components:**
- Input fields with validation
- Secure entry for token
- Loading indicator during connection test
- Error message display

---

## API Contract (Home Assistant)

### Authentication
```
POST /api/
Headers: Authorization: Bearer {token}
Response: 200 OK {"message": "API running."}
```

### Get All Entities
```
GET /api/states
Headers: Authorization: Bearer {token}
Response: Array of entity objects
```

**Entity Object:**
```json
{
  "entity_id": "light.living_room",
  "state": "on",
  "attributes": {
    "friendly_name": "Living Room Light",
    "brightness": 255,
    "color_temp": 370
  },
  "last_changed": "2024-01-15T10:30:00Z"
}
```

### Call Service
```
POST /api/services/{domain}/{service}
Headers: Authorization: Bearer {token}
Body: {
  "entity_id": "light.living_room",
  "brightness": 128
}
Response: Array of updated entity states
```

**Common Services:**
- `light.turn_on` / `light.turn_off`
- `switch.turn_on` / `switch.turn_off`
- `climate.set_temperature`
- `lock.lock` / `lock.unlock`
- `scene.turn_on`

### WebSocket API
```
Connect: ws://{host}:8123/api/websocket
Auth: {"type": "auth", "access_token": "{token}"}
Subscribe: {"type": "subscribe_events", "event_type": "state_changed"}
```

**State Changed Event:**
```json
{
  "event_type": "state_changed",
  "data": {
    "entity_id": "light.living_room",
    "old_state": {...},
    "new_state": {...}
  }
}
```

---

## MVP Roadmap

### Phase 1: MVP (8-10 weeks)

#### Week 1-2: Foundation
- ✅ Project setup and architecture
- ✅ MAUI app structure with Shell navigation
- ✅ Basic models and services
- ✅ Onboarding flow UI
- Home Assistant connection testing

#### Week 3-4: Core Features
- Dashboard implementation
- Room listing and detail views
- Device controls (lights, switches)
- Scene execution
- Basic settings

#### Week 5-6: Automation & Alerts
- Automation listing
- Basic automation builder
- Alert system
- Activity log

#### Week 7-8: Polish & Security
- Biometric authentication
- Role-based permissions
- Offline mode and caching
- Error handling and retry logic

#### Week 9-10: Testing & Launch
- End-to-end testing
- Performance optimization
- Beta testing
- App store submission

### Phase 2: Enhanced Features (6-8 weeks)
- Advanced automation templates
- Dry-run simulation for automations
- Device history graphs
- Custom dashboard widgets
- Enhanced scene builder
- Notification customization
- Backup and restore

### Phase 3: Advanced Integration (8-10 weeks)
- HomeKit bridge integration
- Google Home integration
- Energy monitoring and analytics
- Video camera integration
- Voice control (Siri shortcuts)
- Location-based automations
- Advanced scheduling

---

## Acceptance Criteria

### Onboarding
- [ ] User can enter Home Assistant URL and token
- [ ] App validates connection and shows success/error
- [ ] User can skip onboarding and return later
- [ ] Help text clearly explains how to get token
- [ ] Connection test completes within 5 seconds
- [ ] Invalid credentials show helpful error message

### Dashboard
- [ ] Dashboard loads in < 300ms from cache
- [ ] Quick scenes are tappable and execute correctly
- [ ] Status cards show current values
- [ ] Favorites list shows user's favorite devices
- [ ] Pull-to-refresh updates all data
- [ ] Offline indicator shows when not connected

### Rooms
- [ ] All rooms from HA are displayed
- [ ] Tapping room navigates to room detail
- [ ] Device count is accurate
- [ ] Active device count updates in real-time
- [ ] Empty rooms show appropriate message

### Room Detail
- [ ] All devices in room are listed
- [ ] Device controls work (toggle, slider)
- [ ] "All On" and "All Off" buttons work
- [ ] Device state updates in real-time via WebSocket
- [ ] Last updated timestamp is accurate

### Automations
- [ ] Active and inactive automations are separated
- [ ] Toggle switch enables/disables automation
- [ ] Tapping automation shows details
- [ ] Create button opens automation builder
- [ ] Automation executes according to trigger

### Alerts
- [ ] Critical alerts appear at top
- [ ] Alerts sorted by severity
- [ ] Action buttons work on critical alerts
- [ ] Activity log shows recent device changes
- [ ] Timestamps use relative time (5m ago, 1h ago)

### Settings
- [ ] User can view current home
- [ ] Integration status shows connected/disconnected
- [ ] Notification toggles work
- [ ] Biometric lock can be enabled/disabled
- [ ] About page shows correct version

### Security
- [ ] Biometric authentication works on app launch
- [ ] Lock/unlock requires confirmation
- [ ] Sensitive data encrypted at rest
- [ ] Token stored securely in keychain
- [ ] Role permissions enforced correctly

### Performance
- [ ] Home screen renders < 300ms
- [ ] No frame drops during scrolling
- [ ] Background refresh every 30 seconds
- [ ] WebSocket reconnects automatically
- [ ] Offline mode queues actions

### Accessibility
- [ ] Touch targets minimum 44x44 points
- [ ] VoiceOver labels on all controls
- [ ] High contrast mode supported
- [ ] Dynamic type supported
- [ ] Color is not the only indicator
