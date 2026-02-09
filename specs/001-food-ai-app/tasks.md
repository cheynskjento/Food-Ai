---

description: "Task list for Food-AI End-to-End implementation"
---

# Tasks: Food-AI End-to-End

**Input**: Design documents from `/specs/001-food-ai-app/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: Tests are REQUIRED for every feature (unit + relevant integration).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Create backend solution and projects in `Food_Ai/Backend/FoodAi.sln`
- [x] T002 Add required package references in `Food_Ai/Backend/FoodAi.Api/FoodAi.Api.csproj`
- [x] T003 [P] Create base API folders in `Food_Ai/Backend/FoodAi.Api/Controllers/`
- [x] T004 [P] Create frontend JS modules in `Food_Ai/Front/js/api.js`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**?? CRITICAL**: No user story work can begin until this phase is complete

- [x] T005 Configure appsettings and env binding in `Food_Ai/Backend/FoodAi.Api/appsettings.json`
- [x] T006 Implement EF Core DbContext in `Food_Ai/Backend/FoodAi.Api/Data/FoodAiDbContext.cs`
- [x] T007 [P] Create domain models in `Food_Ai/Backend/FoodAi.Domain/Models/User.cs`
- [x] T008 [P] Create API contracts/DTOs in `Food_Ai/Backend/FoodAi.Api/Contracts/AuthContracts.cs`
- [x] T009 Implement password hashing service in `Food_Ai/Backend/FoodAi.Domain/Services/PasswordHasher.cs`
- [x] T010 Implement JWT token service in `Food_Ai/Backend/FoodAi.Api/Services/JwtTokenService.cs`
- [x] T011 Configure JWT auth + authorization in `Food_Ai/Backend/FoodAi.Api/Program.cs`
- [x] T012 Implement global exception handling middleware in `Food_Ai/Backend/FoodAi.Api/Middleware/ExceptionHandlingMiddleware.cs`
- [x] T013 Implement input validation helpers in `Food_Ai/Backend/FoodAi.Api/Validation/Validators.cs`
- [x] T014 Configure rate limiting policies in `Food_Ai/Backend/FoodAi.Api/Program.cs`
- [x] T015 Implement recipe cache service in `Food_Ai/Backend/FoodAi.Api/Services/RecipeCacheService.cs`
- [x] T016 Implement external AI client interface in `Food_Ai/Backend/FoodAi.Api/Services/IRecipeClient.cs`
- [x] T017 Implement Spoonacular client in `Food_Ai/Backend/FoodAi.Api/Services/SpoonacularClient.cs`
- [x] T018 Implement email sender abstraction in `Food_Ai/Backend/FoodAi.Api/Services/EmailSender.cs`
- [x] T019 Configure CORS + response compression in `Food_Ai/Backend/FoodAi.Api/Program.cs`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Account aanmaken en inloggen (Priority: P1) ?? MVP

**Goal**: Registratie, login en wachtwoord reset met veilige sessie (JWT)

**Independent Test**: Gebruiker kan registeren, inloggen en een wachtwoord reset aanvragen/voltooien

### Tests for User Story 1 (REQUIRED) ??

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T020 [P] [US1] Unit test email/password validation in `Food_Ai/Backend/FoodAi.Api.Tests/Unit/ValidationTests.cs`
- [x] T021 [P] [US1] Unit test JWT generation/verification in `Food_Ai/Backend/FoodAi.Api.Tests/Unit/JwtTokenTests.cs`
- [x] T022 [P] [US1] Unit test reset token hashing in `Food_Ai/Backend/FoodAi.Api.Tests/Unit/PasswordResetTokenTests.cs`
- [x] T023 [P] [US1] Integration test register/login flow in `Food_Ai/Backend/FoodAi.Api.Tests/Integration/AuthFlowTests.cs`
- [x] T024 [P] [US1] Integration test password reset flow in `Food_Ai/Backend/FoodAi.Api.Tests/Integration/PasswordResetTests.cs`

### Implementation for User Story 1

- [x] T025 [US1] Implement AuthService (register/login) in `Food_Ai/Backend/FoodAi.Api/Services/AuthService.cs`
- [x] T026 [US1] Implement PasswordResetService in `Food_Ai/Backend/FoodAi.Api/Services/PasswordResetService.cs`
- [x] T027 [US1] Implement /auth/register and /auth/login in `Food_Ai/Backend/FoodAi.Api/Controllers/AuthController.cs`
- [x] T028 [US1] Implement password reset endpoints in `Food_Ai/Backend/FoodAi.Api/Controllers/PasswordResetController.cs`
- [x] T029 [US1] Add auth migrations in `Food_Ai/Backend/FoodAi.Api/Data/FoodAiDbContext.cs`
- [x] T030 [US1] Update login UI + reset link in `Food_Ai/Front/LoginPage.html`
- [x] T031 [US1] Update register UI in `Food_Ai/Front/RegisterPage.html`
- [x] T032 [US1] Implement auth logic + token storage in `Food_Ai/Front/js/auth.js`
- [x] T033 [US1] Implement fetch wrapper with auth headers in `Food_Ai/Front/js/api.js`

**Checkpoint**: User Story 1 fully functional and testable independently

---

## Phase 4: User Story 2 - Voorkeuren instellen en recepten zoeken (Priority: P1)

**Goal**: Voorkeuren opslaan en recepten zoeken met AI + caching

**Independent Test**: Gebruiker kan voorkeuren opslaan en recipes ontvangen op basis van ingrediënten

### Tests for User Story 2 (REQUIRED) ??

- [ ] T034 [P] [US2] Unit test ingredient parsing/normalization in `Food_Ai/Backend/FoodAi.Api.Tests/Unit/IngredientParserTests.cs`
- [ ] T035 [P] [US2] Unit test preference mapping in `Food_Ai/Backend/FoodAi.Api.Tests/Unit/PreferencesMappingTests.cs`
- [ ] T036 [P] [US2] Integration test preferences save in `Food_Ai/Backend/FoodAi.Api.Tests/Integration/PreferencesTests.cs`
- [ ] T037 [P] [US2] Integration test recipe search + cache in `Food_Ai/Backend/FoodAi.Api.Tests/Integration/RecipeSearchTests.cs`

### Implementation for User Story 2

- [ ] T038 [US2] Implement PreferencesService in `Food_Ai/Backend/FoodAi.Api/Services/PreferencesService.cs`
- [ ] T039 [US2] Implement /preferences endpoint in `Food_Ai/Backend/FoodAi.Api/Controllers/PreferencesController.cs`
- [ ] T040 [US2] Implement ingredient normalizer in `Food_Ai/Backend/FoodAi.Api/Services/IngredientsNormalizer.cs`
- [ ] T041 [US2] Implement recipe search orchestration in `Food_Ai/Backend/FoodAi.Api/Services/RecipeSearchService.cs`
- [ ] T042 [US2] Implement /recipes/search endpoint in `Food_Ai/Backend/FoodAi.Api/Controllers/RecipesController.cs`
- [ ] T043 [US2] Update preferences UI in `Food_Ai/Front/Preference.html`
- [ ] T044 [US2] Implement preferences form handling in `Food_Ai/Front/js/preferences.js`
- [ ] T045 [US2] Update recipe input UI in `Food_Ai/Front/Recipe.html`
- [ ] T046 [US2] Implement recipe search flow + loading state in `Food_Ai/Front/js/recipes.js`
- [ ] T047 [US2] Add debounce + sessionStorage cache in `Food_Ai/Front/js/recipes.js`
- [ ] T048 [US2] Pass search results to SuggestedRecipe in `Food_Ai/Front/js/recipes.js`

**Checkpoint**: User Stories 1 and 2 functional and independently testable

---

## Phase 5: User Story 3 - Recepten beheren en boodschappenlijst gebruiken (Priority: P2)

**Goal**: Recepten sorteren en beheren van een per-user boodschappenlijst

**Independent Test**: Gebruiker kan recepten sorteren, toevoegen, afvinken en verwijderen

### Tests for User Story 3 (REQUIRED) ??

- [ ] T049 [P] [US3] Integration test shopping list CRUD in `Food_Ai/Backend/FoodAi.Api.Tests/Integration/ShoppingListTests.cs`
- [ ] T050 [P] [US3] Unit test shopping list mapping in `Food_Ai/Backend/FoodAi.Api.Tests/Unit/ShoppingListMappingTests.cs`

### Implementation for User Story 3

- [ ] T051 [US3] Update contracts for shopping list delete/toggle in `specs/001-food-ai-app/contracts/openapi.yaml`
- [ ] T052 [US3] Implement ShoppingListService in `Food_Ai/Backend/FoodAi.Api/Services/ShoppingListService.cs`
- [ ] T053 [US3] Implement shopping list endpoints in `Food_Ai/Backend/FoodAi.Api/Controllers/ShoppingListController.cs`
- [ ] T054 [US3] Render recipe list + filters in `Food_Ai/Front/SuggestedRecipe.html`
- [ ] T055 [US3] Implement add-to-list + sorting in `Food_Ai/Front/js/shoppinglist.js`
- [ ] T056 [US3] Update shopping list UI in `Food_Ai/Front/ShoppingList.html`
- [ ] T057 [US3] Implement checkbox/remove actions + cache sync in `Food_Ai/Front/js/shoppinglist.js`

**Checkpoint**: All user stories functional and independently testable

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T058 [P] Update quickstart instructions in `specs/001-food-ai-app/quickstart.md`
- [ ] T059 [P] Add frontend error messaging patterns in `Food_Ai/Front/js/api.js`
- [ ] T060 [P] Verify CORS origin matches frontend in `Food_Ai/Backend/FoodAi.Api/Program.cs`
- [ ] T061 [P] Add gzip response compression in `Food_Ai/Backend/FoodAi.Api/Program.cs`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 ? P1 ? P2)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) - Uses auth from US1 but can be tested with a mocked token
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) - Depends on recipe data flow from US2

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Models before services
- Services before endpoints
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, User Stories 1 and 2 can start in parallel
- All tests for a user story marked [P] can run in parallel
- Models within a story marked [P] can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
Task: "Unit test email/password validation in Food_Ai/Backend/FoodAi.Api.Tests/Unit/ValidationTests.cs"
Task: "Unit test JWT generation/verification in Food_Ai/Backend/FoodAi.Api.Tests/Unit/JwtTokenTests.cs"
Task: "Unit test reset token hashing in Food_Ai/Backend/FoodAi.Api.Tests/Unit/PasswordResetTokenTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational ? Foundation ready
2. Add User Story 1 ? Test independently ? Deploy/Demo (MVP!)
3. Add User Story 2 ? Test independently ? Deploy/Demo
4. Add User Story 3 ? Test independently ? Deploy/Demo
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence


