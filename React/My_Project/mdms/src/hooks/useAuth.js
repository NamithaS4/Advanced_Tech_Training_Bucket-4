import { useState, useEffect } from 'react';
import { authService } from '../services/authService';

export default function useAuth() {
  const [user, setUser] = useState(() => authService.getCurrentUser() || null);
  const [isAuthenticated, setIsAuthenticated] = useState(() =>
    authService.isAuthenticated()
  );

  useEffect(() => {
    setUser(authService.getCurrentUser() || null);
    setIsAuthenticated(authService.isAuthenticated());
  }, []);

  const login = ({ email, password }) => {
    const res = authService.login({ email, password });
    if (res.success) {
      setUser(res.user);
      setIsAuthenticated(true);
    }
    return res;
  };

  const logout = () => {
    authService.logout();
    setUser(null);
    setIsAuthenticated(false);
  };

  const updateProfile = (data) => {
    const updated = authService.updateProfile(data);
    setUser(updated);
    return updated;
  };

  return { user, isAuthenticated, login, logout, updateProfile };
}
