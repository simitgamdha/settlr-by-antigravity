import { Navigate } from 'react-router-dom';
import { authService } from '../services/authService';

/**
 * Simple authentication guard.
 * If a user is not logged in (no token), it redirects them to the Login page.
 * If logged in, it renders the protected child components (e.g., Dashboard).
 */
export default function ProtectedRoute({ children }) {
    if (!authService.isAuthenticated()) {
        return <Navigate to="/login" replace />;
    }

    return children;
}
