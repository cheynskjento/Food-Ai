import { apiRequest } from "./api.js";

const grid = document.getElementById("recipes-grid");
const stored = sessionStorage.getItem("recipe_search_results");
const feedback = document.getElementById("suggested-feedback");

function setFeedback(message, isError) {
    if (!feedback) {
        return;
    }
    feedback.textContent = message;
    feedback.classList.toggle("text-red-500", isError);
    feedback.classList.toggle("text-emerald-500", !isError);
}

if (grid && stored) {
    try {
        const recipes = JSON.parse(stored);
        if (Array.isArray(recipes)) {
            grid.innerHTML = "";

            if (recipes.length === 0) {
                const empty = document.createElement("div");
                empty.className = "col-span-full text-center text-slate-500 dark:text-slate-400";
                empty.textContent = "No recipes matched your ingredients. Try adding more items.";
                grid.appendChild(empty);
                return;
            }

            recipes.forEach((recipe) => {
                const card = document.createElement("div");
                card.className = "group bg-white dark:bg-slate-800 rounded-xl overflow-hidden shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 border border-slate-200 dark:border-slate-700";

                const media = document.createElement("div");
                media.className = "relative h-40 overflow-hidden bg-slate-100 dark:bg-slate-700 flex items-center justify-center";
                media.innerHTML = "<span class='material-symbols-outlined text-5xl text-slate-400'>restaurant</span>";

                const body = document.createElement("div");
                body.className = "p-5";

                const title = document.createElement("h3");
                title.className = "text-slate-900 dark:text-white text-lg font-bold leading-tight mb-3 group-hover:text-[#116DED] transition-colors";
                title.textContent = recipe.name || "Recipe";

                const meta = document.createElement("div");
                meta.className = "flex items-center gap-4 text-slate-500 dark:text-slate-400 text-sm mb-5";

                const time = document.createElement("div");
                time.className = "flex items-center gap-1";
                time.innerHTML = "<span class='material-symbols-outlined text-lg'>schedule</span>";
                const timeText = document.createElement("span");
                timeText.textContent = recipe.prepTime ? `${recipe.prepTime} mins` : "Quick";
                time.appendChild(timeText);

                const difficulty = document.createElement("div");
                difficulty.className = "flex items-center gap-1";
                difficulty.innerHTML = "<span class='material-symbols-outlined text-lg'>fitness_center</span>";
                const difficultyText = document.createElement("span");
                difficultyText.textContent = recipe.difficulty || "Easy";
                difficulty.appendChild(difficultyText);

                meta.appendChild(time);
                meta.appendChild(difficulty);

                const actions = document.createElement("div");
                actions.className = "flex flex-col gap-2";

                const viewButton = document.createElement("button");
                viewButton.className = "w-full bg-[#116DED] hover:bg-brand-blue-hover text-white font-bold py-2.5 px-4 rounded-lg transition-colors flex items-center justify-center gap-2 shadow-sm";
                viewButton.type = "button";
                viewButton.innerHTML = "View Recipe <span class='material-symbols-outlined text-lg'>arrow_forward</span>";
                viewButton.addEventListener("click", () => {
                    sessionStorage.setItem("selected_recipe", JSON.stringify(recipe));
                    window.location.href = "Recipe.html";
                });

                const addButton = document.createElement("button");
                addButton.className = "w-full border border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-200 font-semibold py-2.5 px-4 rounded-lg hover:border-[#116DED] hover:text-[#116DED] transition-colors flex items-center justify-center gap-2";
                addButton.type = "button";
                addButton.innerHTML = "Add to List <span class='material-symbols-outlined text-lg'>add_circle</span>";
                addButton.addEventListener("click", async () => {
                    setFeedback("", false);
                    try {
                        await apiRequest("/shoppinglist/add", {
                            method: "POST",
                            body: JSON.stringify({
                                recipe,
                                missingIngredients: []
                            })
                        });
                        setFeedback("Added to shopping list.", false);
                    } catch (error) {
                        setFeedback(error.message || "Unable to add to shopping list.", true);
                    }
                });

                body.appendChild(title);
                body.appendChild(meta);
                actions.appendChild(viewButton);
                actions.appendChild(addButton);
                body.appendChild(actions);

                card.appendChild(media);
                card.appendChild(body);
                grid.appendChild(card);
            });
        }
    } catch {
        // Ignore malformed storage data and keep the static markup.
    }
}
