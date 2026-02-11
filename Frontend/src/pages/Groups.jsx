import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Users, Plus, X, UserPlus } from 'lucide-react';
import { groupService } from '../services/groupService';
import Layout from '../components/Layout';
import './Groups.css';

export default function Groups() {
    const [groups, setGroups] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [showAddMemberModal, setShowAddMemberModal] = useState(false);
    const [selectedGroup, setSelectedGroup] = useState(null);
    const [groupName, setGroupName] = useState('');
    const [memberEmail, setMemberEmail] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        loadGroups();
    }, []);

    const loadGroups = async () => {
        try {
            const response = await groupService.getUserGroups();
            if (response.succeeded) {
                setGroups(response.data);
            }
        } catch (error) {
            console.error('Failed to load groups:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleCreateGroup = async (e) => {
        e.preventDefault();
        try {
            const response = await groupService.createGroup(groupName);
            if (response.succeeded) {
                setGroups([...groups, response.data]);
                setGroupName('');
                setShowCreateModal(false);
                toast.success(`Group "${groupName}" created successfully!`);
            }
        } catch (error) {
            console.error('Failed to create group:', error);
            toast.error(error.response?.data?.message || 'Failed to create group');
        }
    };

    const handleAddMember = async (e) => {
        e.preventDefault();
        try {
            const response = await groupService.addMember(selectedGroup.id, memberEmail);
            if (response.succeeded) {
                loadGroups();
                setMemberEmail('');
                setShowAddMemberModal(false);
                toast.success(`Member added to "${selectedGroup.name}" successfully!`);
            }
        } catch (error) {
            console.error('Failed to add member:', error);
            toast.error(error.response?.data?.message || 'Failed to add member');
        }
    };

    const openAddMemberModal = (group) => {
        setSelectedGroup(group);
        setShowAddMemberModal(true);
    };

    if (loading) {
        return (
            <Layout>
                <div className="loading-container">
                    <div className="loading-spinner"></div>
                    <p>Loading groups...</p>
                </div>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="groups-page fade-in">
                <div className="page-header">
                    <div>
                        <h1 className="page-title">Groups</h1>
                        <p className="page-subtitle">Manage your expense groups and members</p>
                    </div>
                    <button className="btn btn-primary" onClick={() => setShowCreateModal(true)} data-testid="header-create-group-btn">
                        <Plus size={20} />
                        Create Group
                    </button>
                </div>

                <div className="groups-grid">
                    {groups.length > 0 ? (
                        groups.map((group) => (
                            <div key={group.id} className="group-card card">
                                <div className="group-card-header">
                                    <div className="group-icon-large gradient-blue">
                                        <Users size={24} />
                                    </div>
                                    <h3 className="group-card-title">{group.name}</h3>
                                </div>

                                <div className="group-members">
                                    <p className="members-label">{group.members?.length || 0} Members</p>
                                    <div className="members-list">
                                        {group.members?.map((member) => (
                                            <div key={member.userId} className="member-item">
                                                <div className="member-avatar gradient-pink">
                                                    {member.userName?.charAt(0).toUpperCase()}
                                                </div>
                                                <div className="member-info">
                                                    <p className="member-name">{member.userName}</p>
                                                    <p className="member-email">{member.userEmail}</p>
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                </div>

                                <div className="group-actions">
                                    <button
                                        className="btn btn-secondary btn-sm"
                                        onClick={() => openAddMemberModal(group)}
                                    >
                                        <UserPlus size={16} />
                                        Add Member
                                    </button>
                                    <button
                                        className="btn btn-secondary btn-sm"
                                        onClick={() => navigate(`/expenses?groupId=${group.id}`)}
                                    >
                                        View Expenses
                                    </button>
                                </div>
                            </div>
                        ))
                    ) : (
                        <div className="empty-state-large">
                            <Users size={64} style={{ color: 'var(--text-secondary)' }} />
                            <h3>No groups yet</h3>
                            <p>Create your first group to start tracking expenses</p>
                            <button className="btn btn-primary" onClick={() => setShowCreateModal(true)} data-testid="empty-create-group-btn">
                                <Plus size={20} />
                                Create Group
                            </button>
                        </div>
                    )}
                </div>

                {/* Create Group Modal */}
                {showCreateModal && (
                    <div className="modal-overlay" onClick={() => setShowCreateModal(false)}>
                        <div className="modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Create New Group</h2>
                                <button className="modal-close" onClick={() => setShowCreateModal(false)}>
                                    <X size={20} />
                                </button>
                            </div>
                            <form onSubmit={handleCreateGroup}>
                                <div className="form-group">
                                    <label>Group Name</label>
                                    <input
                                        type="text"
                                        placeholder="Enter group name"
                                        value={groupName}
                                        onChange={(e) => setGroupName(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="modal-actions">
                                    <button type="button" className="btn btn-secondary" onClick={() => setShowCreateModal(false)}>
                                        Cancel
                                    </button>
                                    <button type="submit" className="btn btn-primary" data-testid="modal-submit-btn">
                                        Create Group
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                )}

                {/* Add Member Modal */}
                {showAddMemberModal && (
                    <div className="modal-overlay" onClick={() => setShowAddMemberModal(false)}>
                        <div className="modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Add Member to {selectedGroup?.name}</h2>
                                <button className="modal-close" onClick={() => setShowAddMemberModal(false)}>
                                    <X size={20} />
                                </button>
                            </div>
                            <form onSubmit={handleAddMember}>
                                <div className="form-group">
                                    <label>Member Email</label>
                                    <input
                                        type="email"
                                        placeholder="test@example.com"
                                        value={memberEmail}
                                        onChange={(e) => setMemberEmail(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="modal-actions">
                                    <button type="button" className="btn btn-secondary" onClick={() => setShowAddMemberModal(false)}>
                                        Cancel
                                    </button>
                                    <button type="submit" className="btn btn-primary">
                                        Add Member
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
