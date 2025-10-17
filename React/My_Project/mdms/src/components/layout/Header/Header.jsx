import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Bell, User } from 'lucide-react';
import darkModeIcon from '../../../assets/icons/darkmode.svg';
import lightModeIcon from '../../../assets/icons/lightmode.svg';
import logo from '../../../assets/images/logo.png';

export default function Header() {
  const { t, i18n } = useTranslation();
  const [darkMode, setDarkMode] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();

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

  const changeLanguage = (e) => {
    i18n.changeLanguage(e.target.value);
  };

  return (
    <header className="w-full flex justify-between items-center px-6 py-4 bg-gray-300 text-black dark:bg-gray-800 dark:text-white border-b border-gray-400">
      <h1 className="text-xl font-bold">MDMS</h1>

      <div className="flex items-center gap-4">
        {!isAuthPage && (
            <button
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
            <button
              onClick={() => navigate('/profile')}
              className="p-2 rounded-full bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 transition"
              aria-label="Profile"
            >
              <User size={18} />
            </button>
        )}
      </div>
    </header>
  );
}