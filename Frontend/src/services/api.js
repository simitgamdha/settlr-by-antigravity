import axios from 'axios';

/**
 * Base URL for the .NET Backend API.
 * Update this if your backend port changes.
 */
const API_BASE_URL = 'http://localhost:5028/api';

/**
 * Shared Axios instance with pre-configured defaults.
 * Uses JSON content type for all requests.
 */
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * Request Interceptor:
 * Automatically injects the JWT Bearer token from localStorage 
 * into every outgoing request if it exists.
 */
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

/**
 * Response Interceptor:
 * Centralized error handling. If the backend returns a 401 Unauthorized 
 * (e.g., expired token), we clear local state and force a redirect to login.
 */
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
