// src/pages/Auth/Login.jsx
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Header from '../../components/layout/Header/Header';
import { useTranslation } from 'react-i18next';
import useAuth from '../../hooks/useAuth';

export default function Login() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { login, isAuthenticated } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  // if already authenticated, redirect
  React.useEffect(() => {
    if (localStorage.getItem('mdms_token')) {
      navigate('/enduser/dashboard', { replace: true });
    }
  }, [navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    const res = login({ email, password });
    if (res.success) {
      navigate('/enduser/dashboard', { replace: true });
    } else {
      alert('Invalid credentials — use enduser@mdms.com / mdms123');
    }
  };

  return (
    <div className="w-screen min-h-screen flex flex-col">
      <Header />
      <div className="flex flex-1 justify-center items-center bg-gray-100 dark:bg-gray-900 p-6">
        <form onSubmit={handleSubmit} className="w-full max-w-sm text-center bg-white dark:bg-gray-800 p-6 rounded-lg border border-gray-200 dark:border-gray-700">
          <h2 className="text-xl mb-4 text-black dark:text-white">Login Form</h2>

          <input
            type="email"
            placeholder={t('email') || 'Email'}
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full mb-4 px-3 py-2 border border-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
            required
          />

          <input
            type="password"
            placeholder={t('password') || 'Password'}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full mb-4 px-3 py-2 border border-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
            required
          />

          <div className="flex justify-between items-center text-sm mb-4">
            <label className="inline-flex items-center">
              <input type="checkbox" className="mr-2 align-middle w-3 h-3" />
              <span className="text-black dark:text-white">{t('Remember Me')}</span>
            </label>
            <Link to="/forgot-password" className="font-bold text-blue-700 dark:text-blue-400">
              {t('Forgot Password')}
            </Link>
          </div>

          <button type="submit" className="border border-black px-24 text-black py-1 rounded-full bg-transparent dark:text-white dark:border-white">
            {t('Login')}
          </button>

          <div className="mt-4 text-xs text-gray-500 dark:text-gray-400">
            Use <strong>enduser@mdms.com</strong> / <strong>mdms123</strong>
          </div>
        </form>
      </div>
    </div>
  );
}