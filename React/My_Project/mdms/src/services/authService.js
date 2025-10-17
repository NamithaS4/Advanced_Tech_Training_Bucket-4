
const STORAGE_KEY = 'mdms_auth_user';

const DEFAULT_USER = {
  name: 'John Doe',
  email: 'enduser@mdms.com',
  password: 'mdms123',
  role: 'enduser', 
  zone: 'Bangalore North',
};

export const authService = {
  
  init() {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (!stored) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(DEFAULT_USER));
    }
  },

  login({ email, password }) {
    this.init();
    const user = JSON.parse(localStorage.getItem(STORAGE_KEY));
    if (!user) return { success: false, message: 'User not found' };

    if (email === user.email && password === user.password) {
      // save auth token (simple)
      localStorage.setItem('mdms_token', 'mock-token-12345');
      localStorage.setItem('mdms_current_user', JSON.stringify(user));
      return { success: true, user };
    }
    return { success: false, message: 'Invalid credentials' };
  },

  logout() {
    localStorage.removeItem('mdms_token');
    localStorage.removeItem('mdms_current_user');
  },

  isAuthenticated() {
    return !!localStorage.getItem('mdms_token');
  },

  getCurrentUser() {
    return JSON.parse(localStorage.getItem('mdms_current_user'));
  },

  // Update profile (name/email/password). Overwrites stored user and current user.
  updateProfile({ name, email, password }) {
    const user = JSON.parse(localStorage.getItem(STORAGE_KEY)) || DEFAULT_USER;
    const updated = {
      ...user,
      name: name ?? user.name,
      email: email ?? user.email,
      password: password ?? user.password,
    };
    localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
    localStorage.setItem('mdms_current_user', JSON.stringify(updated));
    return updated;
  },
};


authService.init();