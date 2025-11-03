import React, { useState, useRef, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Bell, User, LogOut } from 'lucide-react';
import darkModeIcon from '../../../assets/icons/darkmode.svg';
import lightModeIcon from '../../../assets/icons/lightmode.svg';
import useAuth from '../../../hooks/useAuth';
import { authService } from '../../../services/authService';

export default function Header() {
  const { t, i18n } = useTranslation();
  const [darkMode, setDarkMode] = useState(false);
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const dropdownRef = useRef(null);
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const currentUser = authService.getCurrentUser();

  const isAuthPage =
    location.pathname === '/login' ||
    location.pathname === '/forgot-password' ||
    location.pathname === '/reset-password';

  const toggleTheme = () => {
    setDarkMode((prev) => !prev);
    if (document.documentElement.classList.contains('dark')) {
      document.documentElement.classList.remove('dark');
    } else {
      document.documentElement.classList.add('dark');
    }
  };

  const handleNotificationClick = () => {
    if (!currentUser) return navigate('/');

    switch (currentUser.role) {
      case 'enduser':
        navigate('/enduser/alerts');
        break;
      case 'zone':
        navigate('/zone/settings');
        break;
      case 'enterprise':
        navigate('/enterprise/dashboard');
        break;
      default:
        navigate('/');
    }
  };

  const handleProfileClick = () => {
    if (!currentUser) return navigate('/');

    switch (currentUser.role) {
      case 'enduser':
        navigate('/enduser/profile');
        break;
      case 'zone':
        navigate('/zone/settings');
        break;
      case 'enterprise':
        navigate('/enterprise/settings');
        break;
      default:
        navigate('/');
    }
  };

  const changeLanguage = (e) => {
    i18n.changeLanguage(e.target.value);
  };

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleLogout = () => {
    window.dispatchEvent(new Event('logout'));
    logout();
    navigate('/', { replace: true });
  };

  return (
    <header className="w-full flex justify-between items-center px-6 py-4 bg-gray-300 text-black dark:bg-gray-800 dark:text-white border-b border-gray-400">
      <div className="flex items-center gap-3">
        <h1 className="text-xl font-bold">MDMS</h1>
      </div>

      <div className="flex items-center gap-4 relative">
        {!isAuthPage && (
          <button
            onClick={handleNotificationClick}
            className="p-2 rounded-full bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 transition"
            aria-label="Notifications"
          >
            <Bell size={18} />
          </button>
        )}

        <button
          onClick={toggleTheme}
          aria-label="Toggle Theme"
          className={`relative w-12 h-8 flex items-center border-4 border-gray-500 rounded-full p-1 cursor-pointer transition-colors duration-300 ${
            darkMode ? 'bg-gray-700 justify-end' : 'bg-gray-300 justify-start'
          }`}
        >
          <div
            className={`absolute w-5 h-5 rounded-full shadow-md transition-transform duration-300 transform bg-white flex items-center justify-center`}
          >
            <img
              src={darkMode ? darkModeIcon : lightModeIcon}
              alt="Theme Icon"
              className="w-5 h-5"
            />
          </div>
        </button>

        <select
          onChange={changeLanguage}
          defaultValue={i18n.language}
          className="bg-transparent text-black dark:text-white px-2 py-1 rounded focus:outline-none"
        >
          <option value="en">en</option>
          <option value="fr">fr</option>
          <option value="hi">hi</option>
        </select>

        {!isAuthPage && (
          <div className="relative" ref={dropdownRef}>
            <button
              onClick={() => setDropdownOpen(!dropdownOpen)}
              className="p-2 rounded-full bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 transition relative"
              aria-label="Profile"
            >
              <User size={18} />
            </button>

            {dropdownOpen && (
              <div className="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-700 rounded-lg shadow-lg z-50">
                <div className="p-3 border-b border-gray-200 dark:border-gray-700">
                  <p className="text-sm font-semibold text-gray-900 dark:text-white">
                    {user?.name || 'User'}
                  </p>
                  <p className="text-xs text-gray-600 dark:text-gray-400">
                    {user?.email || ''}
                  </p>
                </div>

                <button
                  onClick={handleProfileClick}
                  className="w-full bg-white flex items-center gap-2 px-4 py-2 text-sm text-black hover:bg-gray-100 dark:hover:bg-gray-700 transition"
                >
                  <User size={16} /> Profile
                </button>

                <button
                  onClick={handleLogout}
                  className="w-full flex bg-white items-center gap-2 px-4 py-2 text-sm text-red-600 hover:bg-gray-100 dark:hover:bg-gray-700 transition"
                >
                  <LogOut size={16} /> Logout
                </button>
              </div>
            )}
          </div>
        )}
      </div>
    </header>
  );
}
