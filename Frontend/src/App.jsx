import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { ThemeProvider } from './context/ThemeContext';
import { SignalRProvider } from './context/SignalRContext';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Groups from './pages/Groups';
import Expenses from './pages/Expenses';
import Profile from './pages/Profile';
import ProtectedRoute from './components/ProtectedRoute';
import { authService } from './services/authService';

/**
 * The root Application component.
 * Sets up the Theme Provider, Routing, and global Toast notifications.
 */
function App() {
  return (
    <ThemeProvider>
      <SignalRProvider>
        <BrowserRouter>
          <Routes>
            {/* Default Route: Redirect to Dashboard if logged in, otherwise to Login */}
            <Route
              path="/"
              element={
                authService.isAuthenticated() ? (
                  <Navigate to="/dashboard" replace />
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />

            {/* Public Routes */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            {/* Protected Routes: These require a valid JWT token to access */}
            <Route
              path="/dashboard"
              element={
                <ProtectedRoute>
                  <Dashboard />
                </ProtectedRoute>
              }
            />
            <Route
              path="/groups"
              element={
                <ProtectedRoute>
                  <Groups />
                </ProtectedRoute>
              }
            />
            <Route
              path="/expenses"
              element={
                <ProtectedRoute>
                  <Expenses />
                </ProtectedRoute>
              }
            />
            <Route
              path="/profile"
              element={
                <ProtectedRoute>
                  <Profile />
                </ProtectedRoute>
              }
            />
          </Routes>

          {/* Global Toast configuration for app-wide notifications */}
          <ToastContainer
            position="top-right"
            autoClose={3000}
            theme="dark"
          />
          />
        </BrowserRouter>
      </SignalRProvider>
    </ThemeProvider>
  );
}

export default App;
