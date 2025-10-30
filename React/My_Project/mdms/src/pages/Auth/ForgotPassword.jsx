import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Header from '../../components/layout/Header/Header';
import { useTranslation } from 'react-i18next';
import { authService } from '../../services/authService';

export default function ForgotPassword() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!email) {
      alert('Please enter your email address.');
      return;
    }
    sessionStorage.setItem('reset_email', email);
    navigate('/reset-password');
  };

  return (
    <div className="w-screen min-h-screen flex flex-col">
      <Header />
      <div className="flex flex-1 justify-center items-center bg-gray-100 dark:bg-gray-900">
        <form
          onSubmit={handleSubmit}
          className="w-full max-w-sm text-center bg-white dark:bg-gray-800 p-6 rounded-lg border border-gray-200 dark:border-gray-700"
        >
          <h2 className="text-lg mb-4 text-gray-900 dark:text-white">
            Forgot password
          </h2>
          <input
            type="email"
            placeholder="Enter your registered email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full mb-4 px-3 py-2 border border-gray-300 bg-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
            required
          />
          <Link to="/" className="text-blue-600 dark:text-blue-400 flex mb-4">
            {t('Login')}
          </Link>
          <button
            type="submit"
            className="border border-black text-black py-1 px-16 rounded-full bg-transparent dark:text-white dark:border-white"
          >
            {t('Reset Password')}
          </button>
        </form>
      </div>
    </div>
  );
}