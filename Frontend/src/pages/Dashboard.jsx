import { useState, useEffect } from 'react';
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { TrendingUp, TrendingDown, Users, Receipt, Calendar } from 'lucide-react';
import { dashboardService } from '../services/dashboardService';
import Layout from '../components/Layout';
import './Dashboard.css';

export default function Dashboard() {
    const [dashboardData, setDashboardData] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadDashboard();
    }, []);

    const loadDashboard = async () => {
        try {
            const response = await dashboardService.getDashboard();
            if (response.succeeded) {
                setDashboardData(response.data);
            }
        } catch (error) {
            console.error('Failed to load dashboard:', error);
        } finally {
            setLoading(false);
        }
    };

    const getChartData = () => {
        if (!dashboardData?.recentExpenses || dashboardData.recentExpenses.length === 0) {
            return [];
        }

        const last7Days = [];
        const today = new Date();

        for (let i = 6; i >= 0; i--) {
            const date = new Date(today);
            date.setDate(date.getDate() - i);
            date.setHours(0, 0, 0, 0);

            last7Days.push({
                date: date,
                name: date.toLocaleDateString('en-US', { weekday: 'short' }),
                expense: 0
            });
        }

        dashboardData.recentExpenses.forEach(expense => {
            const expenseDate = new Date(expense.createdAt);
            expenseDate.setHours(0, 0, 0, 0);

            const dayData = last7Days.find(day =>
                day.date.getTime() === expenseDate.getTime()
            );

            if (dayData) {
                dayData.expense += expense.amount || 0;
            }
        });

        return last7Days.map(day => ({
            name: day.name,
            expense: Math.round(day.expense * 100) / 100
        }));
    };

    const chartData = getChartData();

    if (loading) {
        return (
            <Layout>
                <div className="loading-container">
                    <div className="loading-spinner"></div>
                    <p>Loading dashboard...</p>
                </div>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="dashboard fade-in">
                <div className="dashboard-header">
                    <div>
                        <h1 className="page-title">Dashboard</h1>
                        <p className="page-subtitle">Welcome back! Here's your expense overview.</p>
                    </div>
                    <div className="date-badge">
                        <Calendar size={16} />
                        <span>{new Date().toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' })}</span>
                    </div>
                </div>

                <div className="stats-grid">
                    <div className="stat-card">
                        <div className="stat-header">
                            <span className="stat-label">You Owe</span>
                            <div className="stat-icon" style={{ background: 'var(--bg-danger)' }}>
                                <TrendingDown size={20} style={{ color: 'var(--danger)' }} />
                            </div>
                        </div>
                        <h2 className="stat-value">₹ {dashboardData?.totalOwed?.toFixed(2) || '0.00'}</h2>
                        <p className="stat-change negative">Total amount you owe</p>
                    </div>

                    <div className="stat-card">
                        <div className="stat-header">
                            <span className="stat-label">You Are Owed</span>
                            <div className="stat-icon" style={{ background: 'var(--bg-success)' }}>
                                <TrendingUp size={20} style={{ color: 'var(--success)' }} />
                            </div>
                        </div>
                        <h2 className="stat-value">₹ {dashboardData?.totalOwedTo?.toFixed(2) || '0.00'}</h2>
                        <p className="stat-change positive">Total amount owed to you</p>
                    </div>

                    <div className="stat-card">
                        <div className="stat-header">
                            <span className="stat-label">Total Groups</span>
                            <div className="stat-icon" style={{ background: 'var(--bg-info)' }}>
                                <Users size={20} style={{ color: 'var(--accent-blue)' }} />
                            </div>
                        </div>
                        <h2 className="stat-value">{dashboardData?.groups?.length || 0}</h2>
                        <p className="stat-change">Active groups</p>
                    </div>

                    <div className="stat-card">
                        <div className="stat-header">
                            <span className="stat-label">Recent Expenses</span>
                            <div className="stat-icon" style={{ background: 'var(--sidebar-active)' }}>
                                <Receipt size={20} style={{ color: 'var(--accent-pink)' }} />
                            </div>
                        </div>
                        <h2 className="stat-value">{dashboardData?.recentExpenses?.length || 0}</h2>
                        <p className="stat-change">Last 10 transactions</p>
                    </div>
                </div>

                <div className="chart-section card">
                    <div className="card-header">
                        <h3 className="card-title">Expense Trend (Last 7 Days)</h3>
                        <div className="chart-legend">
                            <div className="legend-item">
                                <div className="legend-dot" style={{ background: 'var(--accent-pink)' }}></div>
                                <span>Daily Expenses</span>
                            </div>
                        </div>
                    </div>
                    <div className="chart-container">
                        {chartData.length > 0 ? (
                            <ResponsiveContainer width="100%" height={300}>
                                <AreaChart data={chartData}>
                                    <defs>
                                        <linearGradient id="colorExpense" x1="0" y1="0" x2="0" y2="1">
                                            <stop offset="5%" stopColor="var(--accent-pink)" stopOpacity={0.3} />
                                            <stop offset="95%" stopColor="var(--accent-pink)" stopOpacity={0} />
                                        </linearGradient>
                                    </defs>
                                    <CartesianGrid strokeDasharray="3 3" stroke="rgba(255, 255, 255, 0.1)" />
                                    <XAxis dataKey="name" stroke="var(--text-secondary)" />
                                    <YAxis stroke="var(--text-secondary)" />
                                    <Tooltip
                                        contentStyle={{
                                            background: 'var(--bg-card)',
                                            border: '1px solid var(--border)',
                                            borderRadius: '12px',
                                            color: 'var(--text-primary)',
                                        }}
                                        formatter={(value) => `₹${value.toFixed(2)}`}
                                    />
                                    <Area
                                        type="monotone"
                                        dataKey="expense"
                                        stroke="var(--accent-pink)"
                                        fillOpacity={1}
                                        fill="url(#colorExpense)"
                                        strokeWidth={2}
                                    />
                                </AreaChart>
                            </ResponsiveContainer>
                        ) : (
                            <div className="empty-state">
                                <Receipt size={48} style={{ color: 'var(--text-secondary)' }} />
                                <p>No expense data available</p>
                            </div>
                        )}
                    </div>
                </div>

                <div className="content-grid">
                    <div className="card">
                        <div className="card-header">
                            <h3 className="card-title">Recent Transactions</h3>
                        </div>
                        <div className="transactions-list">
                            {dashboardData?.recentExpenses?.length > 0 ? (
                                dashboardData.recentExpenses.slice(0, 5).map((expense) => (
                                    <div key={expense.id} className="transaction-item">
                                        <div className="transaction-icon gradient-pink">
                                            <Receipt size={18} />
                                        </div>
                                        <div className="transaction-details">
                                            <p className="transaction-title">{expense.description}</p>
                                            <p className="transaction-date">
                                                {new Date(expense.createdAt).toLocaleDateString()}
                                            </p>
                                        </div>
                                        <div className="transaction-amount">₹ {(expense.amount || 0).toFixed(2)}</div>
                                    </div>
                                ))
                            ) : (
                                <div className="empty-state">
                                    <Receipt size={48} style={{ color: 'var(--text-secondary)' }} />
                                    <p>No recent transactions</p>
                                </div>
                            )}
                        </div>
                    </div>

                    <div className="card">
                        <div className="card-header">
                            <h3 className="card-title">Your Groups</h3>
                        </div>
                        <div className="groups-list">
                            {dashboardData?.groups?.length > 0 ? (
                                dashboardData.groups.map((group) => (
                                    <div key={group.id} className="group-item">
                                        <div className="group-icon gradient-blue">
                                            <Users size={18} />
                                        </div>
                                        <div className="group-details">
                                            <p className="group-name">{group.name}</p>
                                            <p className="group-members">{group.members?.length || 0} members</p>
                                        </div>
                                    </div>
                                ))
                            ) : (
                                <div className="empty-state">
                                    <Users size={48} style={{ color: 'var(--text-secondary)' }} />
                                    <p>No groups yet</p>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </Layout>
    );
}
