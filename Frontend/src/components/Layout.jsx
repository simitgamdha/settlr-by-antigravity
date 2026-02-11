import { NavLink, useNavigate } from 'react-router-dom';
import { Home, Users, Receipt, LogOut, TrendingUp, UserCircle } from 'lucide-react';
import { authService } from '../services/authService';
import { useTheme } from '../context/ThemeContext';
import './Layout.css';

/**
 * The primary structural component of the app.
 * Provides a sticky Sidebar navigation and wraps the main page content.
 */
export default function Layout({ children }) {
    const navigate = useNavigate();
    const user = authService.getCurrentUser();
    const { theme, changeTheme } = useTheme();

    /**
     * Wipes session data and kicks user back to the login screen.
     */
    const handleLogout = () => {
        authService.logout();
        navigate('/login');
    };

    return (
        <div className="layout">
            <aside className="sidebar">
                {/* Brand Identity Section */}
                <div className="sidebar-header">
                    <div className="logo-icon gradient-pink">
                        <TrendingUp size={24} />
                    </div>
                    <h2 className="sidebar-title">Settlr</h2>
                </div>

                {/* Main Navigation Links */}
                <nav className="sidebar-nav">
                    <NavLink to="/dashboard" className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}>
                        <Home size={20} />
                        <span>Dashboard</span>
                    </NavLink>
                    <NavLink to="/groups" className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}>
                        <Users size={20} />
                        <span>Groups</span>
                    </NavLink>
                    <NavLink to="/expenses" className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}>
                        <Receipt size={20} />
                        <span>Expenses</span>
                    </NavLink>
                </nav>

                <div className="sidebar-footer">
                    {/* Logged in User Profile Snippet */}
                    <div className="user-info">
                        <div className="user-avatar gradient-pink">
                            {user?.name?.charAt(0).toUpperCase()}
                        </div>
                        <div className="user-details">
                            <p className="user-name">{user?.name}</p>
                            <p className="user-email">{user?.email}</p>
                        </div>
                    </div>

                    {/* Quick Action Buttons (Profile & Logout) */}
                    <div className="sidebar-actions">
                        <button
                            onClick={() => navigate('/profile')}
                            className="btn btn-secondary btn-icon"
                            title="View Profile"
                        >
                            <UserCircle size={18} />
                        </button>
                        <button onClick={handleLogout} className="btn btn-secondary btn-logout" title="Logout">
                            <LogOut size={18} />
                        </button>
                    </div>

                    {/* Global Theme Switcher Presets */}
                    <div className="theme-presets">
                        <span className="preset-label">Theme:</span>
                        <div className="preset-buttons">
                            <button
                                onClick={() => changeTheme('dark')}
                                className={`btn-preset ${theme === 'dark' ? 'active' : ''}`}
                                title="Theme 1 (Classic Dark)"
                            >
                                1
                            </button>
                            <button
                                onClick={() => changeTheme('theme2')}
                                className={`btn-preset ${theme === 'theme2' ? 'active' : ''}`}
                                title="Theme 2 (Neon Cyberpunk)"
                            >
                                2
                            </button>
                            <button
                                onClick={() => changeTheme('light')}
                                className={`btn-preset ${theme === 'light' ? 'active' : ''}`}
                                title="Theme 3 (Clean Light Mode)"
                            >
                                3
                            </button>
                            <button
                                onClick={() => changeTheme('theme4')}
                                className={`btn-preset ${theme === 'theme4' ? 'active' : ''}`}
                                title="Theme 4 (Dreamy Lavender)"
                            >
                                4
                            </button>
                        </div>
                    </div>
                </div>
            </aside >

            {/* Main Application Content Area */}
            <main className="main-content">
                {children}
            </main>
        </div >
    );
}
