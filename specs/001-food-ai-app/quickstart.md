# Quickstart: Food-AI End-to-End

**Date**: 2026-02-09
**Feature**: C:\Users\verme\Desktop\Food-Ai\specs\001-food-ai-app\spec.md

## Prerequisites

- .NET 8 SDK
- Node.js not required (static frontend)

## Backend

1. Configure environment variables:

```bash
setx JWT_SECRET "<your-strong-secret>"
setx SPOONACULAR_API_KEY "<your-api-key>"
setx CONNECTION_STRING "Data Source=foodai.db"
setx CORS_ORIGIN "http://localhost:8080"
```

2. From the backend folder:

```bash
cd C:\Users\verme\Desktop\Food-Ai\Food_Ai\Backend\FoodAi.Api

dotnet restore

dotnet ef database update

dotnet run
```

3. API is available at `http://localhost:5000` (or as configured).

## Frontend

1. Serve the static frontend from `Front/`:

```bash
cd C:\Users\verme\Desktop\Food-Ai\Food_Ai\Front
python -m http.server 8080
```

2. Open `http://localhost:8080/LoginPage.html`.

## Tests

```bash
cd C:\Users\verme\Desktop\Food-Ai\Food_Ai\Backend\FoodAi.Api.Tests

dotnet test
```
