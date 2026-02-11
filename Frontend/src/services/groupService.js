import api from './api';

/**
 * Service for Group management: Creating groups and managing memberships.
 */
export const groupService = {
    /**
     * Creates a new expense group.
     */
    async createGroup(name) {
        const response = await api.post('/group', { name });
        return response.data;
    },

    /**
     * Fetches all groups the current user belongs to.
     */
    async getUserGroups() {
        const response = await api.get('/group');
        return response.data;
    },

    /**
     * Fetches full details for a specific group by ID.
     */
    async getGroupById(groupId) {
        const response = await api.get(`/group/${groupId}`);
        return response.data;
    },

    /**
     * Invites a new user to a group using their email.
     */
    async addMember(groupId, userEmail) {
        const response = await api.post('/group/members', { groupId, userEmail });
        return response.data;
    },
};
