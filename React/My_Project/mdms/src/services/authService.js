const STORAGE_KEY = 'mdms_auth_user';
const CURRENT_USER_KEY = 'mdms_current_user';
const TOKEN_KEY = 'mdms_token';

const DEFAULT_USER = {
  name: 'Namii',
  email: 'enduser@mdms.com',
  password: 'mdms123',
  role: 'enduser',
  zone: 'Mumbai South',
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
    const user = JSON.parse(localStorage.getItem(STORAGE_KEY)) || DEFAULT_USER;

    if (email === user.email && password === user.password) {
      const storage = rememberMe ? localStorage : sessionStorage;
      storage.setItem(TOKEN_KEY, 'mock-token-12345');
      storage.setItem(CURRENT_USER_KEY, JSON.stringify(user));
      return { success: true, user };
    }

    return { success: false, message: 'Invalid credentials' };
  },

  logout() {
    // Clear from both storages to ensure complete logout
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
    const user = JSON.parse(localStorage.getItem(STORAGE_KEY)) || DEFAULT_USER;

    const updated = {
      ...user,
      name: name ?? user.name,
      email: email ?? user.email,
      password: password ?? user.password,
    };

    // Always keep master user info in localStorage
    localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));

    // If logged in, update current user in both storages
    if (localStorage.getItem(CURRENT_USER_KEY)) {
      localStorage.setItem(CURRENT_USER_KEY, JSON.stringify(updated));
    }
    if (sessionStorage.getItem(CURRENT_USER_KEY)) {
      sessionStorage.setItem(CURRENT_USER_KEY, JSON.stringify(updated));
    }

    return updated;
  },
};

// Initialize default user on module load
authService.init();