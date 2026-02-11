import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import Login from '../pages/Login';
import { authService } from '../services/authService';

// Mock authService
vi.mock('../services/authService', () => ({
    authService: {
        login: vi.fn(),
    }
}));

// Mock useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

// Mock toast
vi.mock('react-toastify', () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    }
}));

describe('Login Component', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('renders login form correctly', () => {
        render(
            <BrowserRouter>
                <Login />
            </BrowserRouter>
        );

        expect(screen.getByPlaceholderText(/Enter your email/i)).toBeInTheDocument();
        expect(screen.getByPlaceholderText(/Enter your password/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /Login/i })).toBeInTheDocument();
    });

    it('shows validation errors for invalid input', async () => {
        render(
            <BrowserRouter>
                <Login />
            </BrowserRouter>
        );

        const emailInput = screen.getByPlaceholderText(/Enter your email/i);
        fireEvent.change(emailInput, { target: { value: 'invalid-email' } });
        fireEvent.blur(emailInput);

        expect(await screen.findByText(/Invalid email format/i)).toBeInTheDocument();
    });

    it('submits form with valid data', async () => {
        authService.login.mockResolvedValue({ succeeded: true });

        render(
            <BrowserRouter>
                <Login />
            </BrowserRouter>
        );

        fireEvent.change(screen.getByPlaceholderText(/Enter your email/i), {
            target: { value: 'test@example.com' }
        });
        fireEvent.change(screen.getByPlaceholderText(/Enter your password/i), {
            target: { value: 'password123' }
        });

        fireEvent.click(screen.getByRole('button', { name: /Login/i }));

        await waitFor(() => {
            expect(authService.login).toHaveBeenCalledWith('test@example.com', 'password123');
        });
    });

    it('shows error message on failed login', async () => {
        authService.login.mockResolvedValue({ succeeded: false, message: 'Invalid credentials' });

        render(
            <BrowserRouter>
                <Login />
            </BrowserRouter>
        );

        fireEvent.change(screen.getByPlaceholderText(/Enter your email/i), {
            target: { value: 'test@example.com' }
        });
        fireEvent.change(screen.getByPlaceholderText(/Enter your password/i), {
            target: { value: 'password123' }
        });

        fireEvent.click(screen.getByRole('button', { name: /Login/i }));

        expect(await screen.findByText(/Invalid credentials/i)).toBeInTheDocument();
    });
});
