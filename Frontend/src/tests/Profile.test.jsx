import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import Profile from '../pages/Profile';
import { authService } from '../services/authService';

// Mock authService
vi.mock('../services/authService', () => ({
    authService: {
        getCurrentUser: vi.fn(),
    }
}));

// Mock Layout
vi.mock('../components/Layout', () => ({
    default: ({ children }) => <div data-testid="layout">{children}</div>
}));

// Mock toast
vi.mock('react-toastify', () => ({
    toast: {
        success: vi.fn(),
    }
}));

describe('Profile Component', () => {
    const mockUser = { name: 'John Doe', email: 'john@example.com' };

    beforeEach(() => {
        vi.clearAllMocks();
        localStorage.clear();
        authService.getCurrentUser.mockReturnValue(mockUser);
    });

    it('renders user information correctly', () => {
        render(
            <BrowserRouter>
                <Profile />
            </BrowserRouter>
        );

        expect(screen.getByDisplayValue('John Doe')).toBeInTheDocument();
        expect(screen.getByDisplayValue('john@example.com')).toBeInTheDocument();
        expect(screen.getByDisplayValue('john@example.com')).toBeDisabled();
    });

    it('updates profile name in localStorage', () => {
        render(
            <BrowserRouter>
                <Profile />
            </BrowserRouter>
        );

        const nameInput = screen.getByLabelText(/Full Name/i);
        fireEvent.change(nameInput, { target: { value: 'Jane Doe' } });

        fireEvent.click(screen.getByRole('button', { name: /Save Changes/i }));

        const savedUser = JSON.parse(localStorage.getItem('user'));
        expect(savedUser.name).toBe('Jane Doe');
    });
});
