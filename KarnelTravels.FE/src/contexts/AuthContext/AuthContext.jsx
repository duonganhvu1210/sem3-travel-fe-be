import { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { authService } from '@/services/authService';

const AuthContext = createContext(null);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(() => localStorage.getItem('token'));
  const [refreshToken, setRefreshToken] = useState(() => localStorage.getItem('refreshToken'));
  const [isAuthenticated, setIsAuthenticated] = useState(!!token);
  const [isLoading, setIsLoading] = useState(true);

  // Khởi tạo auth state từ token trong localStorage
  useEffect(() => {
    const initAuth = async () => {
      if (token) {
        try {
          const response = await authService.getCurrentUser();
          if (response.success) {
            setUser(response.data);
            setIsAuthenticated(true);
          } else {
            logout();
          }
        } catch (error) {
          console.error('Auth initialization error:', error);
          logout();
        }
      }
      setIsLoading(false);
    };

    initAuth();
  }, [token]);

  const login = useCallback(async (credentials) => {
    const response = await authService.login(credentials);
    if (response.success) {
      const { token: newToken, refreshToken: newRefreshToken, ...userData } = response.data;
      localStorage.setItem('token', newToken);
      localStorage.setItem('refreshToken', newRefreshToken);
      setToken(newToken);
      setRefreshToken(newRefreshToken);
      setUser(userData);
      setIsAuthenticated(true);
      return { success: true, data: userData };
    }
    return { success: false, message: response.message };
  }, []);

  const register = useCallback(async (userData) => {
    const response = await authService.register(userData);
    if (response.success) {
      const { token: newToken, refreshToken: newRefreshToken, ...userInfo } = response.data;
      localStorage.setItem('token', newToken);
      localStorage.setItem('refreshToken', newRefreshToken);
      setToken(newToken);
      setRefreshToken(newRefreshToken);
      setUser(userInfo);
      setIsAuthenticated(true);
      return { success: true, data: userInfo };
    }
    return { success: false, message: response.message };
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    setToken(null);
    setRefreshToken(null);
    setUser(null);
    setIsAuthenticated(false);
  }, []);

  const changePassword = useCallback(async (passwordData) => {
    const response = await authService.changePassword(passwordData);
    return response;
  }, []);

  const value = {
    user,
    token,
    refreshToken,
    isAuthenticated,
    isLoading,
    isAdmin: user?.role === 'Admin' || user?.role === 'admin',
    isManager: user?.role === 'Manager' || user?.role === 'manager',
    isUser: user?.role === 'User' || user?.role === 'user',
    login,
    register,
    logout,
    changePassword,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;
