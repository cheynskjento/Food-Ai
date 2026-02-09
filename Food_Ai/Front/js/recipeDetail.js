import { apiRequest } from "./api.js";

const stored = sessionStorage.getItem("selected_recipe");
const titleEl = document.getElementById("recipe-title");
const prepEl = document.getElementById("recipe-prep-time");
const servingsEl = document.getElementById("recipe-servings");
const difficultyEl = document.getElementById("recipe-difficulty");
const ingredientsEl = document.getElementById("ingredients-list");
const stepsEl = document.getElementById("steps-list");
const addButton = document.getElementById("add-to-list");
const feedback = document.getElementById("recipe-feedback");

function setFeedback(message, isError) {
    if (!feedback) {
        return;
    }
    feedback.textContent = message;
    feedback.classList.toggle("text-red-500", isError);
    feedback.classList.toggle("text-emerald-500", !isError);
}

function renderIngredients(ingredients) {
    if (!ingredientsEl) {
        return;
    }
    ingredientsEl.innerHTML = "";
    if (!ingredients || ingredients.length === 0) {
        const empty = document.createElement("p");
        empty.className = "text-slate-500 dark:text-slate-400";
        empty.textContent = "No ingredients provided.";
        ingredientsEl.appendChild(empty);
        return;
    }

    ingredients.forEach((ingredient) => {
        const label = document.createElement("label");
        label.className = "flex items-start gap-x-3 py-3 group cursor-pointer border-b border-slate-50 dark:border-slate-800/50";

        const input = document.createElement("input");
        input.type = "checkbox";
        input.className = "mt-1 h-5 w-5 rounded border-slate-300 dark:border-slate-600 border-2 bg-transparent text-primary focus:ring-[#116DED]/20 checked:bg-[#116DED] checked:border-[#116DED] focus:ring-2 focus:ring-offset-0 transition-colors";

        const text = document.createElement("p");
        text.className = "text-slate-700 dark:text-slate-300 text-base leading-normal group-hover:text-slate-900 dark:group-hover:text-white transition-colors";
        text.textContent = ingredient;

        label.appendChild(input);
        label.appendChild(text);
        ingredientsEl.appendChild(label);
    });
}

function renderSteps(steps) {
    if (!stepsEl) {
        return;
    }
    stepsEl.innerHTML = "";
    if (!steps || steps.length === 0) {
        const empty = document.createElement("p");
        empty.className = "text-slate-500 dark:text-slate-400";
        empty.textContent = "No preparation steps provided.";
        stepsEl.appendChild(empty);
        return;
    }

    steps.forEach((step, index) => {
        const wrapper = document.createElement("div");
        wrapper.className = "flex gap-4";

        const badge = document.createElement("div");
        badge.className = "flex-shrink-0 flex items-center justify-center w-8 h-8 rounded-full bg-primary/10 text-primary font-bold text-sm";
        badge.textContent = String(index + 1);

        const content = document.createElement("div");
        const title = document.createElement("h4");
        title.className = "font-bold text-slate-900 dark:text-white mb-1";
        title.textContent = `Step ${index + 1}`;

        const paragraph = document.createElement("p");
        paragraph.className = "text-slate-600 dark:text-slate-400 leading-relaxed text-base";
        paragraph.textContent = step;

        content.appendChild(title);
        content.appendChild(paragraph);
        wrapper.appendChild(badge);
        wrapper.appendChild(content);
        stepsEl.appendChild(wrapper);
    });
}

let currentRecipe = null;

if (stored) {
    try {
        currentRecipe = JSON.parse(stored);
    } catch {
        currentRecipe = null;
    }
}

if (currentRecipe) {
    if (titleEl) {
        titleEl.textContent = currentRecipe.name || "Recipe";
    }
    if (prepEl) {
        prepEl.textContent = currentRecipe.prepTime ? `${currentRecipe.prepTime} mins` : "Quick";
    }
    if (difficultyEl) {
        difficultyEl.textContent = currentRecipe.difficulty || "Easy";
    }
    if (servingsEl) {
        servingsEl.textContent = "1 Serving";
    }

    renderIngredients(currentRecipe.ingredients || []);
    renderSteps(currentRecipe.steps || []);
}

if (addButton) {
    addButton.addEventListener("click", async () => {
        if (!currentRecipe) {
            setFeedback("Search for a recipe first.", true);
            return;
        }
        setFeedback("", false);
        try {
            await apiRequest("/shoppinglist/add", {
                method: "POST",
                body: JSON.stringify({ recipe: currentRecipe, missingIngredients: [] })
            });
            setFeedback("Added to shopping list.", false);
        } catch (error) {
            setFeedback(error.message || "Unable to add to shopping list.", true);
        }
    });
}
