import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Header from '../../components/layout/Header/Header';
import { useTranslation } from 'react-i18next';
import { authService } from '../../services/authService';

export default function Login() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [rememberMe, setRememberMe] = useState(false);

  useEffect(() => {
    if (authService.isAuthenticated()) {
      const user = authService.getCurrentUser();
      if (user?.role === 'enterprise')
        navigate('/enterprise/dashboard', { replace: true });
      else if (user?.role === 'zone')
        navigate('/zone/dashboard', { replace: true });
      else navigate('/enduser/dashboard', { replace: true });
    }
  }, [navigate]);

  const handleSubmit = (e) => {
    e.preventDefault();
    const res = authService.login({ email, password, rememberMe });
    if (res.success) {
      const role = res.user.role;
      if (role === 'enterprise')
        navigate('/enterprise/dashboard', { replace: true });
      else if (role === 'zone') navigate('/zone/dashboard', { replace: true });
      else navigate('/enduser/dashboard', { replace: true });
    } else {
      alert('Invalid credentials');
    }
  };

  return (
    <div className="w-screen min-h-screen flex flex-col">
      <Header />
      <div className="flex flex-1 justify-center items-center bg-gray-100 dark:bg-gray-900 p-6">
        <form
          onSubmit={handleSubmit}
          className="w-full max-w-sm text-center bg-white dark:bg-gray-800 p-6 rounded-lg border border-gray-200 dark:border-gray-700"
        >
          <h2 className="text-xl mb-4 text-black dark:text-white">
            Login Form
          </h2>

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
              <input
                type="checkbox"
                checked={rememberMe}
                onChange={(e) => setRememberMe(e.target.checked)}
                className="mr-2 align-middle w-3 h-3"
              />
              <span className="text-black dark:text-white">
                {t('Remember Me')}
              </span>
            </label>

            <Link
              to="/forgot-password"
              className="font-bold text-blue-700 dark:text-blue-400"
            >
              {t('Forgot Password')}
            </Link>
          </div>

          <button
            type="submit"
            className="border border-black px-24 text-black py-1 rounded-full bg-transparent dark:text-white dark:border-white"
          >
            {t('Login')}
          </button>
        </form>
      </div>
    </div>
  );
}
