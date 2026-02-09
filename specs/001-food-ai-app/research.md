# Research: Food-AI End-to-End

**Date**: 2026-02-09
**Feature**: C:\Users\verme\Desktop\Food-Ai\specs\001-food-ai-app\spec.md

## Decisions

### External AI Provider
- **Decision**: Use Spoonacular API (free tier) as primary recipe provider, with a mock provider for tests and a cached fallback when the API fails.
- **Rationale**: Fits the requirement for a free recipe API, has a simple REST interface, and is commonly used for recipe data.
- **Alternatives considered**: Edamam (free tier but stricter limits), Hugging Face inference (requires prompt engineering and may return inconsistent structure).

### Database
- **Decision**: SQLite via Entity Framework Core.
- **Rationale**: Simple to set up, file-based storage suits MVP scope, and EF Core provides migrations and JSON column support.
- **Alternatives considered**: PostgreSQL (more operational overhead), in-memory only (not durable).

### Caching
- **Decision**: Use server-side in-memory cache with 24-hour TTL keyed by normalized ingredient list + preference signature.
- **Rationale**: Meets 24h cache requirement with minimal complexity; appropriate for MVP scale.
- **Alternatives considered**: Redis (extra infra), DB-backed cache (more complex).

### Rate Limiting
- **Decision**: Enforce 10 requests/min per user on recipe search using ASP.NET rate limiting middleware.
- **Rationale**: Fits requirement and can be keyed by user id from JWT.
- **Alternatives considered**: API gateway throttling (not available in MVP).

### Password Reset Tokens
- **Decision**: Store hashed reset tokens in database with expiry, deliver reset link via email.
- **Rationale**: Limits impact if DB leaks and aligns with clarification decisions.
- **Alternatives considered**: Stateless JWT reset tokens (harder revocation).
