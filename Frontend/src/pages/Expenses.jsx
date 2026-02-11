import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Receipt, Plus, X, TrendingUp, TrendingDown } from 'lucide-react';
import { expenseService } from '../services/expenseService';
import { groupService } from '../services/groupService';
import { useSignalR } from '../context/SignalRContext';
import Layout from '../components/Layout';
import './Expenses.css';

export default function Expenses() {
    const [searchParams] = useSearchParams();
    const groupId = searchParams.get('groupId');

    const [groups, setGroups] = useState([]);
    const [selectedGroupId, setSelectedGroupId] = useState(groupId || '');
    const [expenses, setExpenses] = useState([]);
    const [balances, setBalances] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [amount, setAmount] = useState('');
    const [description, setDescription] = useState('');
    const { connection, joinGroup } = useSignalR();

    useEffect(() => {
        loadGroups();
    }, []);

    useEffect(() => {
        if (selectedGroupId) {
            loadExpenses();
            loadBalances();

            // Join the SignalR group for real-time updates
            joinGroup(selectedGroupId);
        }

        if (connection && selectedGroupId) {
            connection.on("ReceiveExpenseUpdate", () => {
                console.log("Real-time update: Refreshing expenses...");
                loadExpenses();
                loadBalances();
            });
        }

        return () => {
            if (connection) {
                connection.off("ReceiveExpenseUpdate");
            }
        };
    }, [selectedGroupId, connection]);

    const loadGroups = async () => {
        try {
            const response = await groupService.getUserGroups();
            if (response.succeeded) {
                setGroups(response.data);
                if (!selectedGroupId && response.data.length > 0) {
                    setSelectedGroupId(response.data[0].id.toString());
                }
            }
        } catch (error) {
            console.error('Failed to load groups:', error);
        } finally {
            setLoading(false);
        }
    };

    const loadExpenses = async () => {
        try {
            const response = await expenseService.getGroupExpenses(selectedGroupId);
            if (response.succeeded) {
                setExpenses(response.data);
            }
        } catch (error) {
            console.error('Failed to load expenses:', error);
        }
    };

    const loadBalances = async () => {
        try {
            const response = await expenseService.getGroupBalances(selectedGroupId);
            if (response.succeeded) {
                setBalances(response.data);
            }
        } catch (error) {
            console.error('Failed to load balances:', error);
        }
    };

    const handleCreateExpense = async (e) => {
        e.preventDefault();
        try {
            const response = await expenseService.createExpense(
                parseInt(selectedGroupId),
                parseFloat(amount),
                description
            );
            if (response.succeeded) {
                loadExpenses();
                loadBalances();
                setAmount('');
                setDescription('');
                setShowCreateModal(false);
                toast.success(`Expense "${description}" added successfully!`);
            }
        } catch (error) {
            console.error('Failed to create expense:', error);
            toast.error(error.response?.data?.message || 'Failed to create expense');
        }
    };

    if (loading) {
        return (
            <Layout>
                <div className="loading-container">
                    <div className="loading-spinner"></div>
                    <p>Loading expenses...</p>
                </div>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="expenses-page fade-in">
                <div className="page-header">
                    <div>
                        <h1 className="page-title">Expenses</h1>
                        <p className="page-subtitle">Track and manage group expenses</p>
                    </div>
                    <div className="header-actions">
                        <select
                            value={selectedGroupId}
                            onChange={(e) => setSelectedGroupId(e.target.value)}
                            className="group-select"
                        >
                            <option value="">Select a group</option>
                            {groups.map((group) => (
                                <option key={group.id} value={group.id}>
                                    {group.name}
                                </option>
                            ))}
                        </select>
                        <button
                            className="btn btn-primary"
                            onClick={() => setShowCreateModal(true)}
                            disabled={!selectedGroupId}
                        >
                            <Plus size={20} />
                            Add Expense
                        </button>
                    </div>
                </div>

                {selectedGroupId ? (
                    <div className="expenses-content">
                        <div className="balances-section">
                            <div className="card">
                                <h3 className="card-title">Balances</h3>
                                <div className="balances-list">
                                    {balances.length > 0 ? (
                                        balances.map((balance, index) => (
                                            <div key={index} className="balance-item">
                                                <div className="balance-user">
                                                    <div className="user-avatar gradient-pink">
                                                        {balance.userName?.charAt(0).toUpperCase()}
                                                    </div>
                                                    <span className="user-name">{balance.userName}</span>
                                                </div>
                                                <div className={`balance-amount ${balance.balance >= 0 ? 'positive' : 'negative'}`}>
                                                    {balance.balance >= 0 ? (
                                                        <TrendingUp size={18} />
                                                    ) : (
                                                        <TrendingDown size={18} />
                                                    )}
                                                    <span>₹ {Math.abs(balance.balance || 0).toFixed(2)}</span>
                                                </div>
                                            </div>
                                        ))
                                    ) : (
                                        <div className="empty-state">
                                            <p>No balances yet</p>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>

                        <div className="expenses-section">
                            <div className="card">
                                <h3 className="card-title">All Expenses</h3>
                                <div className="expenses-list">
                                    {expenses.length > 0 ? (
                                        expenses.map((expense) => (
                                            <div key={expense.id} className="expense-item">
                                                <div className="expense-header">
                                                    <div className="expense-icon gradient-pink">
                                                        <Receipt size={20} />
                                                    </div>
                                                    <div className="expense-info">
                                                        <h4 className="expense-description">{expense.description}</h4>
                                                        <p className="expense-meta">
                                                            Paid by {expense.paidByName} on{' '}
                                                            {new Date(expense.createdAt).toLocaleDateString()}
                                                        </p>
                                                    </div>
                                                    <div className="expense-amount">₹ {(expense.amount || 0).toFixed(2)}</div>
                                                </div>
                                                <div className="expense-splits">
                                                    <p className="splits-label">Split among:</p>
                                                    <div className="splits-grid">
                                                        {expense.splits?.map((split) => (
                                                            <div key={split.userId} className="split-item">
                                                                <span className="split-name">{split.userName || 'Unknown'}</span>
                                                                <span className="split-amount">₹ {(split.amount || 0).toFixed(2)}</span>
                                                            </div>
                                                        ))}
                                                    </div>
                                                </div>
                                            </div>
                                        ))
                                    ) : (
                                        <div className="empty-state">
                                            <Receipt size={48} style={{ color: 'var(--text-secondary)' }} />
                                            <p>No expenses yet</p>
                                            <button className="btn btn-primary" onClick={() => setShowCreateModal(true)}>
                                                <Plus size={20} />
                                                Add First Expense
                                            </button>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>
                ) : (
                    <div className="empty-state-large">
                        <Receipt size={64} style={{ color: 'var(--text-secondary)' }} />
                        <h3>Select a group</h3>
                        <p>Choose a group from the dropdown to view and manage expenses</p>
                    </div>
                )}

                {/* Create Expense Modal */}
                {showCreateModal && (
                    <div className="modal-overlay" onClick={() => setShowCreateModal(false)}>
                        <div className="modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Add New Expense</h2>
                                <button className="modal-close" onClick={() => setShowCreateModal(false)}>
                                    <X size={20} />
                                </button>
                            </div>
                            <form onSubmit={handleCreateExpense}>
                                <div className="form-group">
                                    <label>Description</label>
                                    <input
                                        type="text"
                                        placeholder="What was this expense for?"
                                        value={description}
                                        onChange={(e) => setDescription(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="form-group">
                                    <label>Amount (₹)</label>
                                    <input
                                        type="number"
                                        step="0.01"
                                        placeholder="0.00"
                                        value={amount}
                                        onChange={(e) => setAmount(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="modal-actions">
                                    <button type="button" className="btn btn-secondary" onClick={() => setShowCreateModal(false)}>
                                        Cancel
                                    </button>
                                    <button type="submit" className="btn btn-primary">
                                        Add Expense
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                )}
            </div>
        </Layout>
    );
}
