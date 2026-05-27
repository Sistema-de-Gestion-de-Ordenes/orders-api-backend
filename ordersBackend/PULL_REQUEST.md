# Pull Request

## Description

This PR implements the delivery detail feature and aligns the exposed API surface with the currently prioritized issues. It adds an authenticated endpoint to retrieve full delivery details by ID (including customer and driver data), plus the required service/repository/DTO flow, validations, and tests.

It also removes unrelated endpoints from Swagger so only the currently needed routes remain visible.

---

## Related Issue

Closes #IssueNumber

---

## Changes Included

### Backend Changes

* [x] Added new endpoint(s)
* [x] Added/updated DTOs
* [x] Added/updated services
* [x] Added/updated repositories
* [ ] Added/updated database migrations
* [x] Added validations
* [x] Added exception handling
* [x] Other: Removed non-priority endpoints from exposed controllers/routes



---

## Architecture

This implementation follows the layered backend architecture:

Controller -> Service -> Repository -> Database

- Controller handles route, auth, request validation, and HTTP responses.
- Service contains business logic and not-found rules.
- Repository handles SQL data access and joins for delivery/customer/driver details.
- DTOs define the API contract and response shape.

---

## API / Database Changes

### Added/Updated API

- Added delivery detail endpoint:
  - `GET /deliveries/{id}`
  - Auth required
  - Returns delivery status, origin, destination, full customer data, and full driver data
  - Returns `400` for invalid ID format and `404` when not found

- Kept delivery list endpoint:
  - `GET /deliveries`

### Endpoint cleanup (to match current issue scope)

- Removed or stopped exposing non-priority routes from controllers, including:
  - `POST /auth/logout`
  - `orders` routes
  - `notifications` routes
  - delivery evidence upload route
  - extra customer/driver CRUD routes not currently required

### Database

- No schema changes were required for this PR.

---

## Testing Performed

* [x] Feature tested manually
* [ ] Navigation tested
* [x] Error handling tested
* [x] Validation tested
* [x] Backend integration tested
* [x] No crashes detected

### Test Details

- Added unit tests for delivery detail service mapping and not-found behavior.
- Added controller tests for invalid ID (`400`) and valid ID (`200`) flows.
- Added integration auth test to verify unauthorized access returns `401` when token is missing.
- Executed test suite successfully (`5 passed, 0 failed`).

---

## Evidence

Add screenshots, videos, or GIFs here.

### Screenshots / Videos

<!-- Drag and drop files here -->

---

## Notes

- JSON contract for delivery detail was adjusted to full English naming.
- Swagger now shows only the currently active/prioritized endpoints.
- `Closes #IssueNumber` should be updated with the real issue ID before merge.
