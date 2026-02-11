import { createContext, useContext, useState, useEffect } from 'react';

/**
 * Context to manage the global visual theme of the application.
 */
const ThemeContext = createContext();

/**
 * Custom hook to easily access theme state and switching logic.
 */
export const useTheme = () => {
    const context = useContext(ThemeContext);
    if (!context) {
        throw new Error('useTheme must be used within ThemeProvider');
    }
    return context;
};

/**
 * High-level wrapper that manages theme persistence and DOM updates.
 * It applies the 'data-theme' attribute to the <html> element for CSS variables.
 */
export const ThemeProvider = ({ children }) => {
    // Initialize theme from localStorage so user preference persists on refresh
    const [theme, setTheme] = useState(() => {
        const savedTheme = localStorage.getItem('theme');
        return savedTheme || 'dark'; // 'dark' is our default Theme 1 (Classic)
    });

    useEffect(() => {
        // Apply the theme to the root element so Layout.css/index.css can react to it
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    }, [theme]);

    const changeTheme = (themeName) => {
        setTheme(themeName);
    };

    return (
        <ThemeContext.Provider value={{ theme, changeTheme }}>
            {children}
        </ThemeContext.Provider>
    );
};
