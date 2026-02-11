import { describe, it, expect, vi, beforeEach } from 'vitest';
import { groupService } from '../services/groupService';
import api from '../services/api';

vi.mock('../services/api', () => ({
    default: {
        post: vi.fn(),
        get: vi.fn(),
    }
}));

describe('groupService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('getUserGroups should call correct endpoint', async () => {
        const mockData = { data: [{ id: 1, name: 'Group 1' }] };
        api.get.mockResolvedValue({ data: mockData });

        const result = await groupService.getUserGroups();

        expect(api.get).toHaveBeenCalledWith('/group');
        expect(result).toEqual(mockData);
    });

    it('createGroup should send group name', async () => {
        const mockResponse = { data: { succeeded: true } };
        api.post.mockResolvedValue(mockResponse);

        await groupService.createGroup('New Group');

        expect(api.post).toHaveBeenCalledWith('/group', { name: 'New Group' });
    });

    it('addMember should send email and groupId', async () => {
        const mockResponse = { data: { succeeded: true } };
        api.post.mockResolvedValue(mockResponse);

        await groupService.addMember(1, 'test@example.com');

        expect(api.post).toHaveBeenCalledWith('/group/members', {
            groupId: 1,
            userEmail: 'test@example.com'
        });
    });
});
