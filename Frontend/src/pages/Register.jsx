import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { authService } from '../services/authService';
import { UserPlus, Mail, Lock, User, AlertCircle } from 'lucide-react';
import './Auth.css';

export default function Register() {
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [fieldErrors, setFieldErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const validateName = (name) => {
        if (!name) return 'Name is required';
        if (name.length < 2) return 'Name must be at least 2 characters';
        if (name.length > 100) return 'Name cannot exceed 100 characters';
        if (!/^[a-zA-Z\s]+$/.test(name)) return 'Name can only contain letters and spaces';
        return '';
    };

    const validateEmail = (email) => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email) return 'Email is required';
        if (!emailRegex.test(email)) return 'Invalid email format';
        if (email.length > 255) return 'Email cannot exceed 255 characters';
        return '';
    };

    const validatePassword = (password) => {
        if (!password) return 'Password is required';
        if (password.length < 6) return 'Password must be at least 6 characters';
        if (password.length > 100) return 'Password cannot exceed 100 characters';
        if (!/(?=.*[a-z])/.test(password)) return 'Password must contain at least one lowercase letter';
        if (!/(?=.*[A-Z])/.test(password)) return 'Password must contain at least one uppercase letter';
        if (!/(?=.*\d)/.test(password)) return 'Password must contain at least one number';
        return '';
    };

    const handleNameBlur = () => {
        const error = validateName(name);
        setFieldErrors(prev => ({ ...prev, name: error }));
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
        const nameError = validateName(name);
        const emailError = validateEmail(email);
        const passwordError = validatePassword(password);

        setFieldErrors({
            name: nameError,
            email: emailError,
            password: passwordError
        });

        if (nameError || emailError || passwordError) {
            toast.error('Please fix the validation errors');
            return;
        }

        setLoading(true);

        try {
            const response = await authService.register(name, email, password);
            if (response.succeeded) {
                // Auto login after registration
                localStorage.setItem('token', response.data.token);
                localStorage.setItem('user', JSON.stringify(response.data.user));
                toast.success('Registration successful! Welcome to Settlr!');
                setTimeout(() => navigate('/dashboard'), 500);
            } else {
                const errorMsg = response.message || 'Registration failed';
                setError(errorMsg);
                toast.error(errorMsg);
            }
        } catch (err) {
            const errorMsg = err.response?.data?.message || 'An error occurred during registration';
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
                            <UserPlus size={32} />
                        </div>
                        <h1 className="logo-text">Settlr</h1>
                    </div>
                    <p className="auth-subtitle">Create your account to start managing expenses.</p>
                </div>

                {error && (
                    <div className="error-message">
                        <AlertCircle size={18} />
                        <span>{error}</span>
                    </div>
                )}

                <form onSubmit={handleSubmit} className="auth-form">
                    <div className="form-group">
                        <label htmlFor="name">Full Name</label>
                        <div className="input-wrapper">
                            <User size={20} className="input-icon" />
                            <input
                                id="name"
                                type="text"
                                placeholder="Enter your full name"
                                value={name}
                                onChange={(e) => {
                                    setName(e.target.value);
                                    if (fieldErrors.name) {
                                        setFieldErrors(prev => ({ ...prev, name: '' }));
                                    }
                                }}
                                onBlur={handleNameBlur}
                                className={fieldErrors.name ? 'input-error' : ''}
                                required
                            />
                        </div>
                        {fieldErrors.name && <span className="field-error">{fieldErrors.name}</span>}
                    </div>

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
                        <div className="password-requirements">
                            <p className="requirements-title">Password must contain:</p>
                            <ul className="requirements-list">
                                <li className={password.length >= 6 ? 'valid' : ''}>At least 6 characters</li>
                                <li className={/(?=.*[a-z])/.test(password) ? 'valid' : ''}>One lowercase letter</li>
                                <li className={/(?=.*[A-Z])/.test(password) ? 'valid' : ''}>One uppercase letter</li>
                                <li className={/(?=.*\d)/.test(password) ? 'valid' : ''}>One number</li>
                            </ul>
                        </div>
                    </div>

                    <button type="submit" className="btn btn-primary btn-full" disabled={loading}>
                        {loading ? 'Creating account...' : 'Register'}
                    </button>
                </form>

                <div className="auth-footer">
                    <p>
                        Already have an account?{' '}
                        <Link to="/login" className="auth-link">
                            Login here
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
}
