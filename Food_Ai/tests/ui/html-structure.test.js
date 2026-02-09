import test from "node:test";
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { resolve } from "node:path";

const root = resolve("C:/Users/verme/Desktop/Food-Ai/Food_Ai/Front");

function readHtml(name) {
    return readFileSync(resolve(root, name), "utf8");
}

test("Preference page wiring", () => {
    const html = readHtml("Preference.html");
    assert.match(html, /id=\"preferences-form\"/);
    assert.ok(html.includes("js/preferences.js"));
});

test("Recipe page wiring", () => {
    const html = readHtml("Recipe.html");
    assert.match(html, /id=\"recipe-search-form\"/);
    assert.match(html, /id=\"ingredients-input\"/);
    assert.match(html, /id=\"add-to-list\"/);
    assert.ok(html.includes("js/recipes.js"));
    assert.ok(html.includes("js/recipeDetail.js"));
});

test("Suggested recipes wiring", () => {
    const html = readHtml("SuggestedRecipe.html");
    assert.match(html, /id=\"recipes-grid\"/);
    assert.ok(html.includes("js/suggestedRecipes.js"));
});

test("Shopping list wiring", () => {
    const html = readHtml("ShoppingList.html");
    assert.match(html, /id=\"shopping-list-items\"/);
    assert.ok(html.includes("js/shoppinglist.js"));
});
