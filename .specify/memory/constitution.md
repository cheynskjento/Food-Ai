<!--
Sync Impact Report
- Version change: N/A -> 1.0.0
- Modified principles: N/A (initial adoption)
- Added sections: Core Principles, Technical and Functional Requirements, Best Practices and Standards, Governance
- Removed sections: N/A
- Templates requiring updates: updated C:\Users\verme\Desktop\Food-Ai\Food_Ai\.specify\templates\plan-template.md, updated C:\Users\verme\Desktop\Food-Ai\Food_Ai\.specify\templates\spec-template.md, updated C:\Users\verme\Desktop\Food-Ai\Food_Ai\.specify\templates\tasks-template.md
- Follow-up TODOs: TODO(RATIFICATION_DATE): original adoption date not provided
-->
# Food-AI Constitution

## Core Principles

### Test-First (NON-NEGOTIABLE)
- All new features MUST be developed with test-driven development.
- Unit tests are REQUIRED for core logic.
- Integration tests are REQUIRED for API endpoints and external service calls.

### Integration-First
- Integrate early with external free AI recipe services.
- Build and validate a minimal integration path before feature expansion.
- Contract expectations with external services MUST be documented.

### Security-First
- Minimize storage of user data; store only what is required to deliver features.
- Validate all user inputs at the boundary.
- Do not log sensitive user data.
- Apply rate limiting for external calls.

## Technical and Functional Requirements

- **Frontend**: HTML, CSS, JavaScript.
- **Backend**: C# Web API.
- **Tests**: Unit tests and integration tests for core logic and API endpoints.
- **Functional**:
  - Users can enter leftovers/ingredients.
  - Backend sends ingredients to a free external AI service that returns recipes.
  - Results MUST only use the provided leftovers or clearly propose alternatives.
- **Constraints**:
  - Use only free external AI services for recipes.
  - No paid AI APIs.

## Best Practices and Standards

- **Code style**: Consistent naming and clear frontend/backend separation.
- **Security**: Input validation, rate limiting for external calls, no sensitive data logging.
- **Tests**: Every new feature MUST include unit tests and relevant integration tests.
- **Documentation**: Keep API endpoints documented and up to date.

## Governance

- This constitution supersedes all other practices and templates.
- Amendments require updating this file with a version bump and rationale.
- All specs, plans, and tasks MUST explicitly check compliance with the principles.
- Reviews MUST verify TDD coverage, integration with a free AI service, and security controls.
- Versioning follows semantic versioning:
  - MAJOR: backward-incompatible governance or principle changes.
  - MINOR: new principle/section or materially expanded guidance.
  - PATCH: clarifications and non-semantic refinements.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): original adoption date not provided | **Last Amended**: 2026-02-09
