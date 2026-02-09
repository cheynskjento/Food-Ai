# Data Model: Food-AI End-to-End

**Date**: 2026-02-09
**Feature**: C:\Users\verme\Desktop\Food-Ai\specs\001-food-ai-app\spec.md

## Entities

### User
- **Id**: GUID (PK)
- **Name**: string (required, max 100)
- **Email**: string (required, unique, normalized)
- **PasswordHash**: string (bcrypt)
- **CreatedAt**: datetime (UTC)

**Rules**:
- Email must be unique (case-insensitive).
- Password stored only as hash.

### Preferences
- **UserId**: GUID (PK, FK -> User.Id)
- **Dietary**: JSON array of strings
- **Allergies**: JSON array of strings
- **CuisinePreferences**: JSON array of strings
- **UpdatedAt**: datetime (UTC)

**Rules**:
- One preferences row per user.
- All arrays may be empty.

### ShoppingListItem
- **Id**: GUID (PK)
- **UserId**: GUID (FK -> User.Id)
- **RecipeData**: JSON (name, ingredients, steps, prepTime, difficulty, missingIngredients)
- **AddedAt**: datetime (UTC)
- **IsChecked**: boolean (per item check-off)

**Rules**:
- Scoped per user.

### PasswordResetToken
- **Id**: GUID (PK)
- **UserId**: GUID (FK -> User.Id)
- **TokenHash**: string (hashed token)
- **ExpiresAt**: datetime (UTC)
- **UsedAt**: datetime (UTC, nullable)
- **CreatedAt**: datetime (UTC)

**Rules**:
- Tokens expire after a short window (e.g., 1 hour).
- Only latest unused token should be valid per user.

### RecipeCache
- **Id**: GUID (PK)
- **CacheKey**: string (normalized ingredients + preferences signature)
- **ResponseJson**: JSON (recipes array)
- **CreatedAt**: datetime (UTC)
- **ExpiresAt**: datetime (UTC)

**Rules**:
- TTL 24 hours.

## Relationships

- User 1:1 Preferences
- User 1:N ShoppingListItem
- User 1:N PasswordResetToken
- RecipeCache is standalone

## State Transitions

- PasswordResetToken: Created -> (Used) -> Expired (time-based)
- ShoppingListItem: Active -> Checked (toggle)
