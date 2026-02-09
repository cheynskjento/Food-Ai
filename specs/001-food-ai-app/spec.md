# Feature Specification: Food-AI End-to-End

**Feature Branch**: `001-food-ai-app`  
**Created**: 2026-02-09  
**Status**: Draft  
**Input**: User description: "Bouw Food-AI volgens onderstaande specificaties. Volg de constitution en zorg voor een werkende end-to-end implementatie."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Account aanmaken en inloggen (Priority: P1)

Als nieuwe gebruiker wil ik een account aanmaken en daarna inloggen, zodat ik de app kan gebruiken.

**Why this priority**: Zonder account en login kan geen enkele andere functionaliteit gebruikt worden.

**Independent Test**: Een gebruiker kan registreren met geldig email/wachtwoord, daarna inloggen, en wordt doorgestuurd naar de voorkeurenpagina.

**Acceptance Scenarios**:

1. **Given** een nieuwe gebruiker zonder account, **When** die zich registreert met geldige gegevens, **Then** wordt het account aangemaakt en ziet de gebruiker een succesmelding en kan inloggen.
2. **Given** een bestaande gebruiker, **When** die inlogt met correcte gegevens, **Then** wordt de gebruiker ingelogd en doorgestuurd naar de voorkeurenpagina.

---

### User Story 2 - Voorkeuren instellen en recepten zoeken (Priority: P1)

Als ingelogde gebruiker wil ik mijn dieetwensen en allergieën instellen en mijn restjes invoeren, zodat ik passende receptsuggesties krijg.

**Why this priority**: Dit is de kernwaarde van Food-AI: persoonlijke recepten op basis van restjes en voorkeuren.

**Independent Test**: Een ingelogde gebruiker slaat voorkeuren op, voert ingrediënten in, start een zoekactie, en ontvangt een lijst met recepten.

**Acceptance Scenarios**:

1. **Given** een ingelogde gebruiker, **When** die voorkeuren opslaat, **Then** worden de voorkeuren bewaard en kan de gebruiker doorgaan naar recepten zoeken.
2. **Given** ingevulde ingrediënten en opgeslagen voorkeuren, **When** de gebruiker recepten zoekt, **Then** krijgt de gebruiker een lijst met recepten die passen bij de ingevoerde restjes en voorkeuren.

---

### User Story 3 - Recepten beheren en boodschappenlijst gebruiken (Priority: P2)

Als gebruiker wil ik recepten kunnen sorteren en items kunnen toevoegen aan mijn boodschappenlijst, zodat ik eenvoudig kan plannen wat ik moet kopen.

**Why this priority**: Het verhoogt bruikbaarheid na het ontvangen van recepten.

**Independent Test**: Een gebruiker kan recepten sorteren, een recept toevoegen aan de boodschappenlijst en items afvinken of verwijderen.

**Acceptance Scenarios**:

1. **Given** een lijst met recepten, **When** de gebruiker sorteert op bereidingstijd of moeilijkheid, **Then** verandert de volgorde van de lijst overeenkomstig.
2. **Given** een recept, **When** de gebruiker dit toevoegt aan de boodschappenlijst, **Then** verschijnt het recept met ontbrekende ingrediënten in de lijst en kunnen items worden afgevinkt of verwijderd.

---

### Edge Cases

- Wat gebeurt er wanneer een email ongeldig is of al bestaat?
- Wat gebeurt er wanneer het wachtwoord minder dan 6 tekens is of bevestiging niet matcht?
- Wat gebeurt er wanneer ingrediënten leeg zijn of alleen spaties bevatten?
- Wat gebeurt er wanneer de externe AI-service niet bereikbaar is of faalt?
- Wat gebeurt er wanneer de gebruiker de rate limit overschrijdt?

## Clarifications

### Session 2026-02-09
- Q: Include password reset and email verification in v1? ? A: Include password reset only (no email verification)
- Q: Password reset delivery method for v1? ? A: Email reset link
- Q: Shopping list scope for v1? ? A: Per-user only
- Q: How should shopping list items sync between client and server in v1? ? A: Server source of truth with client cache
- Q: Where should password reset tokens be stored? ? A: Store hashed tokens in database

## Constitution Constraints *(mandatory)*

- Features MUST include unit tests for core logic and integration tests for API endpoints.
- External AI integrations MUST use free services only (no paid APIs).
- User data storage MUST be minimized; do not log sensitive data.
- Input validation and rate limiting MUST be planned for external calls.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST allow users to register with name, email, password, and password confirmation.
- **FR-002**: The system MUST validate email format and enforce a minimum password length of 6 characters.
- **FR-003**: The system MUST prevent duplicate email registrations and return a clear error message.
- **FR-004**: The system MUST allow users to log in with email and password and create an authenticated session.
- **FR-005**: The system MUST expire authenticated sessions after 1 hour of inactivity.
- **FR-006**: The system MUST allow users to set dietary preferences, allergies, and cuisine preferences.
- **FR-007**: The system MUST allow users to submit ingredients and preferences to request recipes.
- **FR-008**: The system MUST display a loading state during recipe search.
- **FR-009**: The system MUST present recipes with name, ingredients, steps, preparation time, and difficulty.
- **FR-010**: The system MUST allow users to sort/filter recipes by preparation time and difficulty.
- **FR-011**: The system MUST allow users to add recipes to a shopping list and remove them later.
- **FR-012**: The system MUST allow shopping list items to be checked off.
- **FR-013**: The system MUST limit recipe search requests to 10 per minute per user.
- **FR-014**: The system MUST cache recipe results for common ingredient combinations for 24 hours.
- **FR-015**: When the external AI service fails, the system MUST show a clear user-facing error and offer cached results or a maintenance message.
- **FR-016**: The system MUST show inline validation errors on all forms.
- **FR-017**: The system MUST sanitize all user inputs before processing.
- **FR-018**: The system MUST restrict access to authenticated-only features and data.
- **FR-019**: The system MUST provide a password reset flow for users who forgot their password.
- **FR-020**: Password reset MUST be delivered via an email reset link.
- **FR-021**: Shopping list data MUST be scoped per-user only.
- **FR-022**: Shopping list changes MUST be persisted on the server with a client-side cache for fast UI and offline reads.
- **FR-023**: Password reset tokens MUST be stored as hashed values in the database.

### Key Entities *(include if feature involves data)*

- **User**: Account with name, email, credential, and creation timestamp.
- **Preferences**: Dietary, allergy, and cuisine selections linked to a user.
- **Recipe**: AI-generated suggestion with ingredients, steps, time, and difficulty.
- **ShoppingListItem**: Saved recipe with missing ingredients and added timestamp.

### Assumptions

- There is a single user role (no admin features).
- Email verification is not required for initial launch.
- Preferences are optional and can be edited at any time.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: At least 90% of users who start registration can complete registration and first login without assistance.
- **SC-002**: 95% of recipe searches return results or a clear fallback message within 5 seconds.
- **SC-003**: 95% of validation errors are shown inline at the point of entry.
- **SC-004**: Rate limiting is enforced with a user-visible message for 100% of over-limit requests.
- **SC-005**: At least 80% of users who receive recipes can add at least one recipe to the shopping list.
