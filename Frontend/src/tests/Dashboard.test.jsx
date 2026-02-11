import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import Dashboard from '../pages/Dashboard';
import { dashboardService } from '../services/dashboardService';

// Mock Services
vi.mock('../services/dashboardService', () => ({
    dashboardService: {
        getDashboard: vi.fn(),
    }
}));

// Mock Layout
vi.mock('../components/Layout', () => ({
    default: ({ children }) => <div data-testid="layout">{children}</div>
}));

// Mock Recharts
vi.mock('recharts', () => ({
    ResponsiveContainer: ({ children }) => <div>{children}</div>,
    AreaChart: () => <div data-testid="area-chart" />,
    Area: () => null,
    XAxis: () => null,
    YAxis: () => null,
    CartesianGrid: () => null,
    Tooltip: () => null,
}));

describe('Dashboard Component', () => {
    const mockDashboardData = {
        totalOwed: 500,
        totalOwedTo: 300,
        groups: [{ id: 1, name: 'Group 1', members: [] }],
        recentExpenses: [
            { id: 1, description: 'Service', amount: 100, createdAt: new Date() }
        ]
    };

    beforeEach(() => {
        vi.clearAllMocks();
        dashboardService.getDashboard.mockResolvedValue({ succeeded: true, data: mockDashboardData });
    });

    it('renders stats correctly', async () => {
        render(
            <BrowserRouter>
                <Dashboard />
            </BrowserRouter>
        );

        expect(await screen.findByText(/500\.00/)).toBeInTheDocument();
        expect(screen.getByText(/300\.00/)).toBeInTheDocument();
        // Just check that we have some "1" values in stat-value containers
        const ones = screen.getAllByText('1', { selector: '.stat-value' });
        expect(ones.length).toBeGreaterThanOrEqual(1);
    });

    it('renders recent transactions', async () => {
        render(
            <BrowserRouter>
                <Dashboard />
            </BrowserRouter>
        );

        expect(await screen.findByText('Service')).toBeInTheDocument();
    });
});
