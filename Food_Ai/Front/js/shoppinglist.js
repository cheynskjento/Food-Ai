import { apiRequest } from "./api.js";

const listContainer = document.getElementById("shopping-list-items");
const sidebar = document.getElementById("selected-recipes");
const feedback = document.getElementById("shopping-feedback");
const sortSelect = document.getElementById("shopping-sort");

let items = [];

function setFeedback(message, isError) {
    if (!feedback) {
        return;
    }
    feedback.textContent = message;
    feedback.classList.toggle("text-red-500", isError);
    feedback.classList.toggle("text-emerald-500", !isError);
}

function sortItems(list, mode) {
    const copy = list.slice();
    if (mode === "name") {
        return copy.sort((a, b) => (a.recipe?.name || "").localeCompare(b.recipe?.name || ""));
    }
    if (mode === "checked") {
        return copy.sort((a, b) => Number(a.isChecked) - Number(b.isChecked));
    }
    return copy.sort((a, b) => new Date(b.addedAt).getTime() - new Date(a.addedAt).getTime());
}

function renderSidebar(list) {
    if (!sidebar) {
        return;
    }
    sidebar.innerHTML = "";
    if (list.length === 0) {
        const empty = document.createElement("p");
        empty.className = "text-slate-500 text-sm";
        empty.textContent = "No recipes added yet.";
        sidebar.appendChild(empty);
        return;
    }

    list.forEach((item) => {
        const card = document.createElement("div");
        card.className = "group relative flex items-center gap-3 p-3 rounded-xl bg-white dark:bg-primary/5 border border-slate-100 dark:border-slate-800 hover:border-primary/30 transition-all";

        const avatar = document.createElement("div");
        avatar.className = "size-12 rounded-lg bg-primary/10 flex items-center justify-center text-primary";
        avatar.innerHTML = "<span class='material-symbols-outlined'>restaurant</span>";

        const info = document.createElement("div");
        info.className = "flex-1 overflow-hidden";
        const title = document.createElement("p");
        title.className = "text-slate-900 dark:text-white text-sm font-semibold truncate leading-tight";
        title.textContent = item.recipe?.name || "Recipe";
        const meta = document.createElement("p");
        meta.className = "text-slate-500 text-xs";
        meta.textContent = item.recipe?.ingredients?.length
            ? `${item.recipe.ingredients.length} items`
            : "1 recipe";

        info.appendChild(title);
        info.appendChild(meta);

        const remove = document.createElement("button");
        remove.className = "opacity-0 group-hover:opacity-100 text-slate-400 hover:text-red-500 transition-opacity";
        remove.type = "button";
        remove.innerHTML = "<span class='material-symbols-outlined text-sm'>close</span>";
        remove.addEventListener("click", async () => {
            await removeItem(item.id);
        });

        card.appendChild(avatar);
        card.appendChild(info);
        card.appendChild(remove);
        sidebar.appendChild(card);
    });
}

function renderList(list) {
    if (!listContainer) {
        return;
    }
    listContainer.innerHTML = "";
    if (list.length === 0) {
        const empty = document.createElement("p");
        empty.className = "text-slate-500 dark:text-slate-400 text-sm";
        empty.textContent = "Your shopping list is empty.";
        listContainer.appendChild(empty);
        return;
    }

    list.forEach((item) => {
        const section = document.createElement("div");
        section.className = "bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-5 shadow-sm";

        const header = document.createElement("div");
        header.className = "flex flex-wrap items-center justify-between gap-3";
        const title = document.createElement("h3");
        title.className = "text-slate-900 dark:text-white text-lg font-bold";
        title.textContent = item.recipe?.name || "Recipe";

        const actions = document.createElement("div");
        actions.className = "flex items-center gap-3";

        const toggle = document.createElement("button");
        toggle.type = "button";
        toggle.className = item.isChecked
            ? "px-3 py-1 rounded-full text-xs font-bold bg-emerald-100 text-emerald-700"
            : "px-3 py-1 rounded-full text-xs font-bold bg-slate-100 text-slate-600";
        toggle.textContent = item.isChecked ? "Checked" : "Active";

        const remove = document.createElement("button");
        remove.type = "button";
        remove.className = "text-sm font-semibold text-red-500 hover:text-red-600";
        remove.textContent = "Remove";
        remove.addEventListener("click", async () => {
            await removeItem(item.id);
        });

        actions.appendChild(toggle);
        actions.appendChild(remove);

        header.appendChild(title);
        header.appendChild(actions);

        const listWrapper = document.createElement("div");
        listWrapper.className = "mt-4 space-y-2";

        const ingredients = item.recipe?.ingredients || [];
        ingredients.forEach((ingredient) => {
            const row = document.createElement("label");
            row.className = "group flex items-center justify-between p-2 rounded-lg hover:bg-secondary-blue/50 dark:hover:bg-primary/5 transition-colors cursor-pointer";

            const left = document.createElement("div");
            left.className = "flex items-center gap-3";

            const checkbox = document.createElement("input");
            checkbox.type = "checkbox";
            checkbox.className = "appearance-none size-5 rounded border-2 border-slate-300 dark:border-slate-600 checked:bg-primary checked:border-primary transition-all cursor-pointer";
            checkbox.checked = item.isChecked;
            checkbox.addEventListener("change", async () => {
                await toggleItem(item.id);
            });

            const text = document.createElement("span");
            text.className = "text-sm text-slate-700 dark:text-slate-200";
            text.textContent = ingredient;

            left.appendChild(checkbox);
            left.appendChild(text);

            row.appendChild(left);
            listWrapper.appendChild(row);
        });

        section.appendChild(header);
        section.appendChild(listWrapper);
        listContainer.appendChild(section);
    });
}

function renderAll() {
    const mode = sortSelect?.value || "recent";
    const sorted = sortItems(items, mode);
    renderList(sorted);
    renderSidebar(sorted);
}

async function loadList() {
    setFeedback("", false);
    try {
        items = await apiRequest("/shoppinglist");
        renderAll();
    } catch (error) {
        setFeedback(error.message || "Unable to load shopping list.", true);
    }
}

async function toggleItem(id) {
    try {
        const updated = await apiRequest(`/shoppinglist/${id}/toggle`, { method: "POST" });
        items = items.map((item) => (item.id === updated.id ? updated : item));
        renderAll();
    } catch (error) {
        setFeedback(error.message || "Unable to update item.", true);
    }
}

async function removeItem(id) {
    try {
        await apiRequest(`/shoppinglist/${id}`, { method: "DELETE" });
        items = items.filter((item) => item.id !== id);
        renderAll();
    } catch (error) {
        setFeedback(error.message || "Unable to remove item.", true);
    }
}

if (sortSelect) {
    sortSelect.addEventListener("change", () => renderAll());
}

loadList();
