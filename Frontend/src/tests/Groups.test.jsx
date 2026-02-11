import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import Groups from '../pages/Groups';
import { groupService } from '../services/groupService';

// Mock groupService
vi.mock('../services/groupService', () => ({
    groupService: {
        getUserGroups: vi.fn(),
        createGroup: vi.fn(),
        addMember: vi.fn(),
    }
}));

// Mock Layout
vi.mock('../components/Layout', () => ({
    default: ({ children }) => <div data-testid="layout">{children}</div>
}));

describe('Groups Component', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('renders loading state initially', () => {
        groupService.getUserGroups.mockReturnValue(new Promise(() => { }));
        render(
            <BrowserRouter>
                <Groups />
            </BrowserRouter>
        );
        expect(screen.getByText(/Loading groups.../i)).toBeInTheDocument();
    });

    it('renders groups after loading', async () => {
        const mockGroups = [
            { id: 1, name: 'Group 1', members: [{ userId: 1, userName: 'User 1' }] },
            { id: 2, name: 'Group 2', members: [] }
        ];
        groupService.getUserGroups.mockResolvedValue({ succeeded: true, data: mockGroups });

        render(
            <BrowserRouter>
                <Groups />
            </BrowserRouter>
        );

        expect(await screen.findByText('Group 1')).toBeInTheDocument();
        expect(screen.getByText('Group 2')).toBeInTheDocument();
    });

    it('opens and submits create group modal', async () => {
        groupService.getUserGroups.mockResolvedValue({ succeeded: true, data: [] });
        groupService.createGroup.mockResolvedValue({ succeeded: true, data: { id: 3, name: 'New Group', members: [] } });

        render(
            <BrowserRouter>
                <Groups />
            </BrowserRouter>
        );

        // Wait for loading to finish
        const triggerBtn = await screen.findByTestId('header-create-group-btn');
        fireEvent.click(triggerBtn);

        // Fill form
        const input = screen.getByPlaceholderText(/Enter group name/i);
        fireEvent.change(input, { target: { value: 'New Group' } });

        // Submit
        const submitBtn = await screen.findByTestId('modal-submit-btn');
        fireEvent.click(submitBtn);

        await waitFor(() => {
            expect(groupService.createGroup).toHaveBeenCalledWith('New Group');
        });
    });

    it('shows empty state when no groups exist', async () => {
        groupService.getUserGroups.mockResolvedValue({ succeeded: true, data: [] });

        render(
            <BrowserRouter>
                <Groups />
            </BrowserRouter>
        );

        expect(await screen.findByText(/No groups yet/i)).toBeInTheDocument();
    });
});
