import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { authService } from '../services/authService';
import { LogIn, Mail, Lock, AlertCircle } from 'lucide-react';
import './Auth.css';

export default function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [fieldErrors, setFieldErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const validateEmail = (email) => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email) return 'Email is required';
        if (!emailRegex.test(email)) return 'Invalid email format';
        return '';
    };

    const validatePassword = (password) => {
        if (!password) return 'Password is required';
        if (password.length < 6) return 'Password must be at least 6 characters';
        return '';
    };

    const handleEmailBlur = () => {
        const error = validateEmail(email);
        setFieldErrors(prev => ({ ...prev, email: error }));
    };

    const handlePasswordBlur = () => {
        const error = validatePassword(password);
        setFieldErrors(prev => ({ ...prev, password: error }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        // Validate all fields
        const emailError = validateEmail(email);
        const passwordError = validatePassword(password);

        setFieldErrors({
            email: emailError,
            password: passwordError
        });

        if (emailError || passwordError) {
            toast.error('Please fix the validation errors');
            return;
        }

        setLoading(true);

        try {
            const response = await authService.login(email, password);
            if (response.succeeded) {
                toast.success('Login successful! Welcome back!');
                setTimeout(() => navigate('/dashboard'), 500);
            } else {
                const errorMsg = response.message || 'Login failed';
                setError(errorMsg);
                toast.error(errorMsg);
            }
        } catch (err) {
            const errorMsg = err.response?.data?.message || 'An error occurred during login';
            setError(errorMsg);
            toast.error(errorMsg);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-card fade-in">
                <div className="auth-header">
                    <div className="logo-container">
                        <div className="logo-icon gradient-pink">
                            <LogIn size={32} />
                        </div>
                        <h1 className="logo-text">Settlr</h1>
                    </div>
                    <p className="auth-subtitle">Welcome back! Please login to your account.</p>
                </div>

                {error && (
                    <div className="error-message">
                        <AlertCircle size={18} />
                        <span>{error}</span>
                    </div>
                )}

                <form onSubmit={handleSubmit} className="auth-form">
                    <div className="form-group">
                        <label htmlFor="email">Email Address</label>
                        <div className="input-wrapper">
                            <Mail size={20} className="input-icon" />
                            <input
                                id="email"
                                type="email"
                                placeholder="Enter your email"
                                value={email}
                                onChange={(e) => {
                                    setEmail(e.target.value);
                                    if (fieldErrors.email) {
                                        setFieldErrors(prev => ({ ...prev, email: '' }));
                                    }
                                }}
                                onBlur={handleEmailBlur}
                                className={fieldErrors.email ? 'input-error' : ''}
                                required
                            />
                        </div>
                        {fieldErrors.email && <span className="field-error">{fieldErrors.email}</span>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <div className="input-wrapper">
                            <Lock size={20} className="input-icon" />
                            <input
                                id="password"
                                type="password"
                                placeholder="Enter your password"
                                value={password}
                                onChange={(e) => {
                                    setPassword(e.target.value);
                                    if (fieldErrors.password) {
                                        setFieldErrors(prev => ({ ...prev, password: '' }));
                                    }
                                }}
                                onBlur={handlePasswordBlur}
                                className={fieldErrors.password ? 'input-error' : ''}
                                required
                            />
                        </div>
                        {fieldErrors.password && <span className="field-error">{fieldErrors.password}</span>}
                    </div>

                    <button type="submit" className="btn btn-primary btn-full" disabled={loading}>
                        {loading ? 'Logging in...' : 'Login'}
                    </button>
                </form>

                <div className="auth-footer">
                    <p>
                        Don't have an account?{' '}
                        <Link to="/register" className="auth-link">
                            Register here
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
}
