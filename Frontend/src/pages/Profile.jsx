import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { User, Mail, Save, ArrowLeft } from 'lucide-react';
import { authService } from '../services/authService';
import Layout from '../components/Layout';
import './Profile.css';

export default function Profile() {
    const navigate = useNavigate();
    const user = authService.getCurrentUser();

    const [name, setName] = useState(user?.name || '');
    const [email, setEmail] = useState(user?.email || '');

    const handleSave = () => {
        // Update user info in localStorage (only name is editable now)
        const updatedUser = { ...user, name };
        localStorage.setItem('user', JSON.stringify(updatedUser));
        toast.success('Profile updated successfully!');
    };

    return (
        <Layout>
            <div className="profile-page fade-in">
                <div className="page-header">
                    <button onClick={() => navigate(-1)} className="btn-back">
                        <ArrowLeft size={20} />
                        Back
                    </button>
                    <div>
                        <h1 className="page-title">My Profile</h1>
                        <p className="page-subtitle">Manage your account information</p>
                    </div>
                </div>

                <div className="profile-content">
                    <div className="profile-card card">
                        <div className="profile-header">
                            <div className="profile-avatar gradient-pink">
                                {user?.name?.charAt(0).toUpperCase()}
                            </div>
                            <div className="profile-info">
                                <h2>{user?.name}</h2>
                                <p>{user?.email}</p>
                            </div>
                        </div>

                        <div className="profile-form">
                            <div className="form-group">
                                <label htmlFor="name">
                                    <User size={18} />
                                    Full Name
                                </label>
                                <input
                                    id="name"
                                    type="text"
                                    value={name}
                                    onChange={(e) => setName(e.target.value)}
                                    placeholder="Enter your name"
                                />
                            </div>

                            <div className="form-group">
                                <label htmlFor="email">
                                    <Mail size={18} />
                                    Email Address (Read-only)
                                </label>
                                <input
                                    id="email"
                                    type="email"
                                    value={email}
                                    disabled
                                    className="disabled-input"
                                    placeholder="Enter your email"
                                />
                            </div>

                            <button onClick={handleSave} className="btn btn-primary">
                                <Save size={20} />
                                Save Changes
                            </button>
                        </div>
                    </div>

                    <div className="profile-stats card">
                        <h3 className="card-title">Account Statistics</h3>
                        <div className="stats-grid">
                            <div className="stat-item">
                                <div className="stat-icon gradient-pink">
                                    <User size={24} />
                                </div>
                                <div className="stat-details">
                                    <p className="stat-label">Member Since</p>
                                    <p className="stat-value">
                                        {new Date().toLocaleDateString('en-US', {
                                            month: 'long',
                                            year: 'numeric'
                                        })}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </Layout>
    );
}
