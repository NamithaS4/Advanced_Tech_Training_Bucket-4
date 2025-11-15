const STORAGE_KEY = 'mdms_auth_user';
const CURRENT_USER_KEY = 'mdms_current_user';
const TOKEN_KEY = 'mdms_token';

const DEFAULT_USER = {
  name: 'Namii',
  email: 'enduser@mdms.com',
  password: 'mdms123',
  role: 'enduser',
  zone: 'Bangalore South',
};

export const authService = {
  // Initialize default user if not present
  init() {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (!stored) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(DEFAULT_USER));
    }
  },

  /**
   * Login with optional rememberMe.
   * If rememberMe is true → store in localStorage (persistent)
   * Else → store in sessionStorage (clears on tab/browser close)
   */
  login({ email, password, rememberMe }) {
    this.init();

    const users = JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
    const user = Array.isArray(users)
      ? users.find((u) => u.email === email && u.password === password)
      : null;

    if (user) {
      const storage = rememberMe ? localStorage : sessionStorage;
      storage.setItem(TOKEN_KEY, 'mock-token-12345');
      storage.setItem(CURRENT_USER_KEY, JSON.stringify(user));

      const logs = JSON.parse(localStorage.getItem('userLogs')) || [];
      logs.unshift({
        timestamp: new Date().toLocaleString(),
        user: user.name,
        email: user.email,
        zone: user.zone || '-',
        status: 'Login Successful',
      });
      localStorage.setItem('userLogs', JSON.stringify(logs));

      return { success: true, user };
    }

    const logs = JSON.parse(localStorage.getItem('userLogs')) || [];
    logs.unshift({
      timestamp: new Date().toLocaleString(),
      user: 'Unknown',
      email,
      zone: '-',
      status: 'Login Failed',
    });
    localStorage.setItem('userLogs', JSON.stringify(logs));

    return { success: false, message: 'Invalid credentials' };
  },

  logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(CURRENT_USER_KEY);
    sessionStorage.removeItem(TOKEN_KEY);
    sessionStorage.removeItem(CURRENT_USER_KEY);
  },

  isAuthenticated() {
    return (
      !!localStorage.getItem(TOKEN_KEY) || !!sessionStorage.getItem(TOKEN_KEY)
    );
  },

  getCurrentUser() {
    const localUser = localStorage.getItem(CURRENT_USER_KEY);
    const sessionUser = sessionStorage.getItem(CURRENT_USER_KEY);
    return JSON.parse(localUser || sessionUser || 'null');
  },

  updateProfile({ name, email, password }) {
    const users = JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
    const current = this.getCurrentUser();
    if (!current) return null;

    const updatedUsers = users.map((u) =>
      u.email === current.email
        ? {
            ...u,
            name: name ?? u.name,
            email: email ?? u.email,
            password: password ?? u.password,
          }
        : u
    );

    localStorage.setItem(STORAGE_KEY, JSON.stringify(updatedUsers));

    const updatedUser = updatedUsers.find((u) => u.email === current.email);

    localStorage.setItem(CURRENT_USER_KEY, JSON.stringify(updatedUser));
    sessionStorage.setItem(CURRENT_USER_KEY, JSON.stringify(updatedUser));

    return updatedUser;
  },

  resetPassword(email, newPassword) {
    const users = JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
    const updatedUsers = users.map((u) =>
      u.email === email ? { ...u, password: newPassword } : u
    );
    localStorage.setItem(STORAGE_KEY, JSON.stringify(updatedUsers));

    const logs = JSON.parse(localStorage.getItem('userLogs')) || [];
    logs.unshift({
      timestamp: new Date().toLocaleString(),
      user: email,
      email,
      zone: '-',
      status: 'Password Reset',
    });
    localStorage.setItem('userLogs', JSON.stringify(logs));

    return true;
  },
};

authService.init();
