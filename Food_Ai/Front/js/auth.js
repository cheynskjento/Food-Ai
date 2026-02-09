import { apiRequest } from "./api.js";

const loginForm = document.getElementById("login-form");
const registerForm = document.getElementById("register-form");
const resetRequest = document.getElementById("reset-request");

function setFeedback(message, isError) {
    const errorEl = document.getElementById("form-error");
    const successEl = document.getElementById("form-success");

    if (errorEl) {
        errorEl.textContent = isError ? message : "";
        errorEl.classList.toggle("hidden", !isError);
    }

    if (successEl) {
        successEl.textContent = !isError ? message : "";
        successEl.classList.toggle("hidden", isError);
    }
}

function clearFeedback() {
    setFeedback("", true);
    setFeedback("", false);
}

async function handleLogin(event) {
    event.preventDefault();
    clearFeedback();

    const email = document.getElementById("email")?.value?.trim();
    const password = document.getElementById("password")?.value;

    if (!email || !password) {
        setFeedback("Email and password are required.", true);
        return;
    }

    try {
        const response = await apiRequest("/auth/login", {
            method: "POST",
            body: JSON.stringify({ email, password })
        });

        sessionStorage.setItem("auth_token", response.token);
        sessionStorage.setItem("auth_expires_in", response.expiresInSeconds.toString());
        setFeedback("Login successful. Redirecting...", false);
        window.location.href = "Preference.html";
    } catch (error) {
        setFeedback(error.message || "Login failed.", true);
    }
}

async function handleRegister(event) {
    event.preventDefault();
    clearFeedback();

    const name = document.getElementById("full-name")?.value?.trim();
    const email = document.getElementById("email")?.value?.trim();
    const password = document.getElementById("password")?.value;
    const confirmPassword = document.getElementById("confirm-password")?.value;

    if (!name || !email || !password || !confirmPassword) {
        setFeedback("All fields are required.", true);
        return;
    }

    try {
        await apiRequest("/auth/register", {
            method: "POST",
            body: JSON.stringify({ name, email, password, confirmPassword })
        });

        setFeedback("Account created. Redirecting to login...", false);
        window.location.href = "LoginPage.html";
    } catch (error) {
        setFeedback(error.message || "Registration failed.", true);
    }
}

async function handleReset(event) {
    event.preventDefault();
    clearFeedback();

    const email = document.getElementById("email")?.value?.trim();
    if (!email) {
        setFeedback("Enter your email to request a reset link.", true);
        return;
    }

    try {
        await apiRequest("/auth/password-reset/request", {
            method: "POST",
            body: JSON.stringify({ email })
        });
        setFeedback("Reset link sent if the email exists.", false);
    } catch (error) {
        setFeedback(error.message || "Reset request failed.", true);
    }
}

if (loginForm) {
    loginForm.addEventListener("submit", handleLogin);
}

if (registerForm) {
    registerForm.addEventListener("submit", handleRegister);
}

if (resetRequest) {
    resetRequest.addEventListener("click", handleReset);
}
