# Implementation Plan: Food-AI End-to-End

**Branch**: `001-food-ai-app` | **Date**: 2026-02-09 | **Spec**: C:\Users\verme\Desktop\Food-Ai\specs\001-food-ai-app\spec.md
**Input**: Feature specification from `/specs/001-food-ai-app/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Deliver a web app that lets users register/login, set dietary preferences, search for recipes via a free AI provider, and manage a per-user shopping list. Backend is a C# Web API with JWT auth, rate limiting, caching, and password reset; frontend is static HTML/CSS/JS pages calling the API.

## Technical Context

**Language/Version**: C# (.NET 8) for backend; JavaScript (ES2022) for frontend  
**Primary Dependencies**: ASP.NET Core Web API, Entity Framework Core, BCrypt.Net-Next, System.IdentityModel.Tokens.Jwt, MemoryCache  
**Storage**: SQLite (single-file DB) with JSON fields for preferences and recipe data  
**Testing**: xUnit, Microsoft.AspNetCore.Mvc.Testing, FluentAssertions  
**Target Platform**: Windows dev, cross-platform server (Linux)  
**Project Type**: web  
**Performance Goals**: 95% of recipe searches return results or fallback within 5 seconds  
**Constraints**: Free AI services only, 10 requests/min per user, 24h cache for ingredient combos, no sensitive data logging  
**Scale/Scope**: MVP for ~1k users, single backend service

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- TDD required: unit tests for core logic and integration tests for API endpoints/external calls
- Integration-first: identify a free external AI recipe service and plan an early integration/contract check
- Security-first: input validation, rate limiting for external calls, no sensitive data logging
- Constraints: only free AI services; no paid AI APIs
- Post-Phase 1 re-check: All gates satisfied (tests required, free AI integration selected, security controls planned, no paid APIs).

## Project Structure

### Documentation (this feature)

```text
specs/001-food-ai-app/
|-- plan.md              # This file (/speckit.plan command output)
|-- research.md          # Phase 0 output (/speckit.plan command)
|-- data-model.md        # Phase 1 output (/speckit.plan command)
|-- quickstart.md        # Phase 1 output (/speckit.plan command)
|-- contracts/           # Phase 1 output (/speckit.plan command)
`-- tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
Food_Ai/
|-- Front/               # Static HTML/CSS/JS pages
`-- Backend/             # C# Web API solution (to be added)
    |-- FoodAi.Api/
    |-- FoodAi.Api.Tests/
    `-- FoodAi.Domain/   # Core models and services
```

**Structure Decision**: Web application with static frontend in `Food_Ai/Front/` and backend in `Food_Ai/Backend/`.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| N/A | N/A | N/A |
