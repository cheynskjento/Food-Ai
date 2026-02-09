import { apiRequest } from "./api.js";

const form = document.getElementById("recipe-search-form");
const input = document.getElementById("ingredients-input");
const feedback = document.getElementById("recipe-feedback");
const button = document.getElementById("recipe-search-button");

const INPUT_STORAGE_KEY = "recipe_search_input";
const RESULTS_STORAGE_KEY = "recipe_search_results";
const CACHE_PREFIX = "recipe_search_cache:";
const CACHE_TTL_MS = 10 * 60 * 1000;

function setFeedback(message, isError) {
    if (!feedback) {
        return;
    }

    feedback.textContent = message;
    feedback.classList.toggle("text-red-500", isError);
    feedback.classList.toggle("text-emerald-500", !isError);
}

function setLoading(isLoading) {
    if (!button) {
        return;
    }

    button.disabled = isLoading;
    button.classList.toggle("opacity-70", isLoading);
    button.classList.toggle("cursor-not-allowed", isLoading);
}

function normalizeIngredients(rawInput) {
    if (!rawInput) {
        return [];
    }

    const seen = new Set();
    const output = [];
    rawInput
        .split(/[\n,]/g)
        .map((value) => value.trim().toLowerCase())
        .filter((value) => value.length > 0)
        .forEach((value) => {
            if (!seen.has(value)) {
                seen.add(value);
                output.push(value);
            }
        });

    return output;
}

function buildCacheKey(ingredients) {
    return `${CACHE_PREFIX}${ingredients.slice().sort().join(",")}`;
}

function debounce(fn, delay) {
    let timer;
    return (...args) => {
        clearTimeout(timer);
        timer = window.setTimeout(() => fn(...args), delay);
    };
}

async function loadPreferences() {
    try {
        const stored = sessionStorage.getItem("preferences");
        if (stored) {
            const parsed = JSON.parse(stored);
            return {
                dietary: parsed.dietary || [],
                allergies: parsed.allergies || [],
                cuisinePreferences: parsed.cuisinePreferences || []
            };
        }

        const response = await apiRequest("/preferences");
        sessionStorage.setItem("preferences", JSON.stringify(response));
        return {
            dietary: response.dietary || [],
            allergies: response.allergies || [],
            cuisinePreferences: response.cuisinePreferences || []
        };
    } catch {
        return { dietary: [], allergies: [], cuisinePreferences: [] };
    }
}

function loadCachedResults(cacheKey) {
    try {
        const payload = sessionStorage.getItem(cacheKey);
        if (!payload) {
            return null;
        }
        const parsed = JSON.parse(payload);
        if (!parsed || !parsed.timestamp || !parsed.recipes) {
            return null;
        }
        if (Date.now() - parsed.timestamp > CACHE_TTL_MS) {
            sessionStorage.removeItem(cacheKey);
            return null;
        }
        return parsed.recipes;
    } catch {
        return null;
    }
}

async function handleSearch(event) {
    event.preventDefault();
    setFeedback("", false);

    const rawInput = input?.value || "";
    const ingredients = normalizeIngredients(rawInput);
    if (ingredients.length === 0) {
        setFeedback("Add at least one ingredient to search.", true);
        return;
    }

    const cacheKey = buildCacheKey(ingredients);
    const cachedRecipes = loadCachedResults(cacheKey);
    if (cachedRecipes) {
        sessionStorage.setItem(RESULTS_STORAGE_KEY, JSON.stringify(cachedRecipes));
        window.location.href = "SuggestedRecipe.html";
        return;
    }

    setLoading(true);
    try {
        const preferences = await loadPreferences();
        const response = await apiRequest("/recipes/search", {
            method: "POST",
            body: JSON.stringify({ ingredients, preferences })
        });

        const recipes = response?.recipes || [];
        sessionStorage.setItem(RESULTS_STORAGE_KEY, JSON.stringify(recipes));
        sessionStorage.setItem(cacheKey, JSON.stringify({ timestamp: Date.now(), recipes }));
        window.location.href = "SuggestedRecipe.html";
    } catch (error) {
        setFeedback(error.message || "Failed to fetch recipes.", true);
    } finally {
        setLoading(false);
    }
}

if (input) {
    const restore = sessionStorage.getItem(INPUT_STORAGE_KEY);
    if (restore) {
        input.value = restore;
    }

    input.addEventListener(
        "input",
        debounce(() => {
            sessionStorage.setItem(INPUT_STORAGE_KEY, input.value);
        }, 300)
    );
}

if (form) {
    form.addEventListener("submit", handleSearch);
}
