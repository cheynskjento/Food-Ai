# Repository Guidelines

## Project Structure & Module Organization
- `Front/` contains all static UI pages (HTML only). Main entry is `Front/index.html`; other pages include `LoginPage.html`, `RegisterPage.html`, `Recipe.html`, `SuggestedRecipe.html`, `ShoppingList.html`, and `Preference.html`.
- `.specify/` and `.codex/` store specification and automation assets; treat these as tooling and do not mix with product code.
- `constitution` is a root-level reference file.

## Build, Test, and Development Commands
- No build system is configured. Pages use Tailwind via CDN and run as static HTML.
- Local preview (direct): open `Front/index.html` in a browser.
- Local preview (static server): from `Front/`, run `python -m http.server 8080` and visit `http://localhost:8080/`.

## Coding Style & Naming Conventions
- Keep markup consistent with existing pages: 4-space indentation and one tag per line for major blocks.
- HTML files use PascalCase names for pages (e.g., `LoginPage.html`). Maintain this pattern.
- Tailwind configuration is defined inline in a `<script id="tailwind-config">` and utilities are authored in `<style type="text/tailwindcss">`. Prefer reusing the existing color tokens (e.g., `primary`, `background-light`).
- Fonts are loaded via Google Fonts (Inter). Keep typography changes centralized in the `tailwind.config` or base styles.

## Testing Guidelines
- No automated tests are present. If you add tests later, document the framework and add a `tests/` or similar directory with clear naming (e.g., `*.spec.js`).

## Commit & Pull Request Guidelines
- Git history is not present in this workspace, so no commit convention can be inferred. Use a clear, imperative subject line (e.g., "Add shopping list filters") and keep commits focused.
- PRs should include a short summary, list of pages touched, and screenshots or a short screen recording for UI changes.

## Security & Configuration Tips
- Avoid adding secrets or API keys directly in HTML. Use environment-based injection only when a backend is introduced.
- External CDN links (Tailwind, Google Fonts) should be pinned to known versions if stability becomes a concern.
