import api from './api';

/**
 * Service to handle user authentication state and API calls.
 */
export const authService = {
    /**
     * Registers a new user account.
     */
    async register(name, email, password) {
        const response = await api.post('/auth/register', { name, email, password });
        return response.data;
    },

    /**
     * Logs in an existing user and persists the JWT + User data to localStorage.
     */
    async login(email, password) {
        const response = await api.post('/auth/login', { email, password });
        if (response.data.succeeded) {
            // Store credentials for the api.js interceptor to pick up
            localStorage.setItem('token', response.data.data.token);
            localStorage.setItem('user', JSON.stringify(response.data.data.user));
        }
        return response.data;
    },

    /**
     * Clears local authentication state.
     */
    logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    },

    /**
     * Retrieves the stored user info from localStorage.
     */
    getCurrentUser() {
        const userStr = localStorage.getItem('user');
        return userStr ? JSON.parse(userStr) : null;
    },

    /**
     * Quick check if a user is currently logged in.
     */
    isAuthenticated() {
        return !!localStorage.getItem('token');
    },
};
