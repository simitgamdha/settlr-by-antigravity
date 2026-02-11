import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import Expenses from '../pages/Expenses';
import { expenseService } from '../services/expenseService';
import { groupService } from '../services/groupService';

// Mock Services
vi.mock('../services/expenseService', () => ({
    expenseService: {
        getGroupExpenses: vi.fn(),
        getGroupBalances: vi.fn(),
        createExpense: vi.fn(),
    }
}));

vi.mock('../services/groupService', () => ({
    groupService: {
        getUserGroups: vi.fn(),
    }
}));

// Mock Layout
vi.mock('../components/Layout', () => ({
    default: ({ children }) => <div data-testid="layout">{children}</div>
}));

describe('Expenses Component', () => {
    const mockGroups = [{ id: 1, name: 'Group 1' }];
    const mockExpenses = [
        { id: 101, description: 'Dinner', amount: 300, paidByName: 'User 1', createdAt: new Date() }
    ];
    const mockBalances = [
        { userName: 'User 1', balance: 100 }
    ];

    beforeEach(() => {
        vi.clearAllMocks();
        groupService.getUserGroups.mockResolvedValue({ succeeded: true, data: mockGroups });
        expenseService.getGroupExpenses.mockResolvedValue({ succeeded: true, data: mockExpenses });
        expenseService.getGroupBalances.mockResolvedValue({ succeeded: true, data: mockBalances });
    });

    it('renders loading state initially', () => {
        groupService.getUserGroups.mockReturnValue(new Promise(() => { }));
        render(
            <BrowserRouter>
                <Expenses />
            </BrowserRouter>
        );
        expect(screen.getByText(/Loading expenses.../i)).toBeInTheDocument();
    });

    it('renders expenses and balances for selected group', async () => {
        render(
            <BrowserRouter>
                <Expenses />
            </BrowserRouter>
        );

        expect(await screen.findByText('Dinner')).toBeInTheDocument();
        expect(screen.getByText(/₹ 100/i)).toBeInTheDocument();
    });

    it('opens and submits add expense modal', async () => {
        expenseService.createExpense.mockResolvedValue({ succeeded: true });

        render(
            <BrowserRouter>
                <Expenses />
            </BrowserRouter>
        );

        // Wait for data to load
        await screen.findByText('Dinner');

        // Open modal
        fireEvent.click(screen.getByRole('button', { name: /Add Expense/i }));

        // Fill form
        fireEvent.change(screen.getByPlaceholderText(/What was this expense for?/i), {
            target: { value: 'Movie' }
        });
        fireEvent.change(screen.getByPlaceholderText(/0.00/i), {
            target: { value: '500' }
        });

        // Submit
        fireEvent.click(screen.getAllByRole('button', { name: /Add Expense/i })[1]);

        await waitFor(() => {
            expect(expenseService.createExpense).toHaveBeenCalledWith(1, 500, 'Movie');
        });
    });
});
