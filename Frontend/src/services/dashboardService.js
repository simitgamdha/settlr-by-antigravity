import api from './api';

/**
 * Service for the main Dashboard statistics and recent activity.
 */
export const dashboardService = {
    /**
     * Fetches consolidated data for the dashboard (total balances, recent activity, group lists).
     */
    async getDashboard() {
        const response = await api.get('/dashboard');
        return response.data;
    },
};
