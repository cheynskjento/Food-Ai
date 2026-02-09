const API_BASE_URL = "http://localhost:5000/api";

async function apiRequest(path, options = {}) {
    const token = sessionStorage.getItem("auth_token");
    const headers = Object.assign(
        { "Content-Type": "application/json" },
        options.headers || {}
    );

    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    const response = await fetch(`${API_BASE_URL}${path}`, {
        ...options,
        headers
    });

    if (!response.ok) {
        let message = `API error (${response.status})`;
        try {
            const data = await response.json();
            if (data && typeof data.error === "string") {
                message = data.error;
            } else {
                message = JSON.stringify(data);
            }
        } catch {
            const text = await response.text();
            if (text) {
                message = text;
            }
        }
        throw new Error(message);
    }

    return response.status === 204 ? null : response.json();
}

export { apiRequest, API_BASE_URL };
