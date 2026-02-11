import { describe, it, expect, vi, beforeEach } from 'vitest';
import { authService } from '../services/authService';
import api from '../services/api';

// Mock api
vi.mock('../services/api', () => ({
    default: {
        post: vi.fn(),
    }
}));

describe('authService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
        localStorage.clear();
    });

    it('login should store token and user on success', async () => {
        const mockUser = { id: 1, name: 'Test User' };
        const mockResponse = {
            data: {
                succeeded: true,
                data: {
                    token: 'test-token',
                    user: mockUser
                }
            }
        };

        api.post.mockResolvedValue(mockResponse);

        const result = await authService.login('test@example.com', 'password');

        expect(api.post).toHaveBeenCalledWith('/auth/login', {
            email: 'test@example.com',
            password: 'password'
        });
        expect(localStorage.getItem('token')).toBe('test-token');
        expect(localStorage.getItem('user')).toBe(JSON.stringify(mockUser));
        expect(result).toEqual(mockResponse.data);
    });

    it('logout should clear localStorage', () => {
        localStorage.setItem('token', 'some-token');
        localStorage.setItem('user', '{}');

        authService.logout();

        expect(localStorage.getItem('token')).toBeNull();
        expect(localStorage.getItem('user')).toBeNull();
    });

    it('getCurrentUser should return parsed user from localStorage', () => {
        const mockUser = { name: 'Test' };
        localStorage.setItem('user', JSON.stringify(mockUser));

        const user = authService.getCurrentUser();
        expect(user).toEqual(mockUser);
    });

    it('isAuthenticated should return true if token exists', () => {
        localStorage.setItem('token', 'some-token');
        expect(authService.isAuthenticated()).toBe(true);
    });

    it('isAuthenticated should return false if token does not exist', () => {
        expect(authService.isAuthenticated()).toBe(false);
    });
});
