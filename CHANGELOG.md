# Sprint 30E–30F Release Notes

## Sprint 30E: Technician Mobile Tracking & ETA
- Live technician GPS tracking UI for mobile (Razor Pages)
- /api/tech/update-location endpoint with JWT/role security, anti-spam debounce, and audit logging
- Dynamic ETA prediction and update on GPS change
- SignalR integration: live ETA broadcast to dispatcher/admin UI
- Technician LiveTracking page: HTML5 geolocation, job list, and ETA polling

## Sprint 31.1: Technician Schedule Acceptance
- Technician mobile UI to accept/reject scheduled jobs
- /api/tech/respond-to-schedule endpoint: Accept/Reject with live SignalR update to dispatcher
- Dispatcher UI updates in real time on technician response

## Sprint 30F: QA & Test Data
- Two test technicians and three ScheduleQueue jobs injected for QA (DEBUG only)
- Automated ETA broadcast for dispatched job
- QA plan for GPS, ETA, schedule, and security flows

---
- All features implemented as Razor Pages (.NET 8)
- All new endpoints and UI tested for session claim handling and security
- See code for // Sprint 30E, 31.1, and 30F annotations
