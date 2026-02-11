import { describe, it, expect, vi, beforeEach } from 'vitest';
import { expenseService } from '../services/expenseService';
import api from '../services/api';

vi.mock('../services/api', () => ({
    default: {
        post: vi.fn(),
        get: vi.fn(),
    }
}));

describe('expenseService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('getGroupExpenses should call correct endpoint', async () => {
        const mockData = { data: [{ id: 1, amount: 100 }] };
        api.get.mockResolvedValue({ data: mockData });

        const result = await expenseService.getGroupExpenses(1);

        expect(api.get).toHaveBeenCalledWith('/expense/group/1');
        expect(result).toEqual(mockData);
    });

    it('createExpense should send correct payload', async () => {
        api.post.mockResolvedValue({ data: { succeeded: true } });

        await expenseService.createExpense(1, 50, 'Launch');

        expect(api.post).toHaveBeenCalledWith('/expense', {
            groupId: 1,
            amount: 50,
            description: 'Launch'
        });
    });

    it('getGroupBalances should call correct endpoint', async () => {
        api.get.mockResolvedValue({ data: [] });
        await expenseService.getGroupBalances(1);
        expect(api.get).toHaveBeenCalledWith('/expense/group/1/balances');
    });
});
