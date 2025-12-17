You are a senior product designer + mobile architect. Design a mobile app called "Casa" that unifies smart-home control across multiple ecosystems (Home Assistant first-class; optionally HomeKit/Google Home/SmartThings).

IMPORTANT: Always use the MAUI Expert agent located at `agents/dotnet-maui.agent.md` when working on .NET MAUI code. This agent contains critical rules and best practices that must be followed.

GOALS
- One fast, consistent UI to control devices from different brands.
- Local-first + privacy by default (prefer LAN; minimal cloud).
- Household-friendly: multi-user roles (Owner/Admin/Member/Guest), shared favorites, and safe controls for locks/alarms.
- Automations that are powerful but safe, with testing and audit logs.

TARGET USERS
- Primary: a homeowner who wants one app instead of many vendor apps.
- Secondary: family/roommates who need simple daily controls.
- Advanced: power user with Home Assistant who wants clean mobile UX.

PLATFORMS & TECH CONSTRAINTS
- Mobile: iOS + Android (assume .NET MAUI).
- Offline behavior: app should show cached device state and queue actions when possible.
- Accessibility: large touch targets, high-contrast option, voiceover-friendly labels.

INTEGRATIONS (MVP)
- Home Assistant via REST/WebSocket (entities, services, events).
- Support core device types: lights, switches, plugs, climate, sensors, locks (with extra safety), media players.
- Scenes: map to HA scenes/scripts; allow "App scenes" that trigger multiple services across domains.

INFORMATION ARCHITECTURE
Bottom nav tabs:
1) Home (dashboard + favorites + quick scenes)
2) Rooms (room tiles -> room detail)
3) Automate (automation list + builder)
4) Alerts (critical alerts + activity log)
5) Settings (homes/users, integrations, notifications, security)

KEY FEATURES (MVP)
- Onboarding: connect to Home Assistant (URL + token), test connection, select default home.
- Home dashboard: status summary + Quick Scenes row + cards for Energy/Security/Climate + Favorites list.
- Rooms: auto-group entities by area; allow custom ordering and hiding.
- Device detail: consistent controls per device type, plus "last updated", and a compact history graph if available.
- Automations: If/And/Then builder, templates (e.g., "motion -> light"), dry-run simulation, cooldown, quiet hours.
- Alerts: show actionable alerts (leak, smoke, door open), with severity and recommended action.
- Security: biometric lock, encrypted local storage, role-based permissions, confirm/hold-to-act for locks.

NON-FUNCTIONAL REQUIREMENTS
- Performance: home screen < 300ms to render from cache; background refresh.
- Reliability: retries, exponential backoff, clear offline indicators.
- Privacy: no selling data; optional analytics off by default.
- Observability: logs for debugging and an export option.

DATA MODEL (HIGH LEVEL)
- Home: id, name, timezone
- User: id, role, allowed domains (locks/alarm restricted)
- Device(Entity): id, name, domain, area, capabilities, state, lastChanged
- Scene: id, name, icon, actions[]
- Automation: id, name, enabled, trigger, conditions[], actions[], safetyRules
- Alert: id, severity, source, message, createdAt, status, actions[]

DELIVERABLES YOU MUST OUTPUT
1) Product PRD (problem, goals, scope, out-of-scope, success metrics)
2) UX flows for onboarding, dashboard, room control, automation creation, and alerts
3) Screen-by-screen wireframes description (layout, components, interactions)
4) API contract ideas for Home Assistant (what you'd call/subscribe to)
5) An MVP plan + Phase 2 + Phase 3 roadmap with clear milestones
6) Acceptance criteria for each MVP feature
Use crisp, implementation-ready language.
