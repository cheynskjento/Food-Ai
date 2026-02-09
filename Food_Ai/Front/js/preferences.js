import { apiRequest } from "./api.js";

const form = document.getElementById("preferences-form");
const feedback = document.getElementById("preferences-feedback");

function setFeedback(message, isError) {
    if (!feedback) {
        return;
    }

    feedback.textContent = message;
    feedback.classList.toggle("hidden", !message);
    feedback.classList.toggle("text-red-500", isError);
    feedback.classList.toggle("text-emerald-500", !isError);
}

function collectPreferences() {
    const selections = {
        dietary: [],
        allergies: [],
        cuisinePreferences: []
    };

    document.querySelectorAll(".preference-option").forEach((input) => {
        if (!(input instanceof HTMLInputElement) || !input.checked) {
            return;
        }

        const group = input.dataset.group;
        if (!group || !(group in selections)) {
            return;
        }

        selections[group].push(input.value);
    });

    return selections;
}

function syncCardStates() {
    document.querySelectorAll(".preference-card").forEach((card) => {
        const checkbox = card.querySelector("input[type='checkbox']");
        if (!(checkbox instanceof HTMLInputElement)) {
            return;
        }

        card.classList.toggle("is-selected", checkbox.checked);
    });
}

function applyPreferences(preferences) {
    if (!preferences) {
        return;
    }

    const values = new Set();
    ["dietary", "allergies", "cuisinePreferences"].forEach((key) => {
        const list = preferences[key];
        if (Array.isArray(list)) {
            list.forEach((value) => values.add(String(value).toLowerCase()));
        }
    });

    document.querySelectorAll(".preference-option").forEach((input) => {
        if (!(input instanceof HTMLInputElement)) {
            return;
        }

        input.checked = values.has(input.value.toLowerCase());
    });

    syncCardStates();
}

async function loadPreferences() {
    try {
        const response = await apiRequest("/preferences");
        sessionStorage.setItem("preferences", JSON.stringify(response));
        applyPreferences(response);
        setFeedback("", false);
    } catch (error) {
        setFeedback(error.message || "Unable to load preferences.", true);
    }
}

async function handleSubmit(event) {
    event.preventDefault();
    setFeedback("", false);

    const payload = collectPreferences();

    try {
        await apiRequest("/preferences", {
            method: "PUT",
            body: JSON.stringify(payload)
        });
        sessionStorage.setItem("preferences", JSON.stringify(payload));
        setFeedback("Preferences saved successfully.", false);
    } catch (error) {
        setFeedback(error.message || "Failed to save preferences.", true);
    }
}

if (form) {
    form.addEventListener("submit", handleSubmit);
}

document.querySelectorAll(".preference-option").forEach((input) => {
    input.addEventListener("change", syncCardStates);
});

syncCardStates();
loadPreferences();
