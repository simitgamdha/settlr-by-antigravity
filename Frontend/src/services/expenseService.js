import api from './api';

/**
 * Service for Expense management: Creating expenses and viewing balances.
 */
export const expenseService = {
    /**
     * Creates a new expense within a group.
     * The backend automatically handles the split among members.
     */
    async createExpense(groupId, amount, description) {
        const response = await api.post('/expense', { groupId, amount, description });
        return response.data;
    },

    /**
     * Fetches all expenses for a specific group.
     */
    async getGroupExpenses(groupId) {
        const response = await api.get(`/expense/group/${groupId}`);
        return response.data;
    },

    /**
     * Fetches the net balances for all members in a group.
     */
    async getGroupBalances(groupId) {
        const response = await api.get(`/expense/group/${groupId}/balances`);
        return response.data;
    },
};
