import test from "node:test";
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { resolve } from "node:path";

const jsRoot = resolve("C:/Users/verme/Desktop/Food-Ai/Food_Ai/Front/js");

function readJs(name) {
    return readFileSync(resolve(jsRoot, name), "utf8");
}

test("Recipe search uses recipes endpoint", () => {
    const js = readJs("recipes.js");
    assert.ok(js.includes("/recipes/search"));
});

test("Preferences UI uses preferences endpoint", () => {
    const js = readJs("preferences.js");
    assert.ok(js.includes("/preferences"));
});

test("Shopping list UI uses shopping list endpoints", () => {
    const js = readJs("shoppinglist.js");
    assert.ok(js.includes("/shoppinglist"));
    assert.ok(js.includes("toggle"));
});
