import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import Header from '../../components/layout/Header/Header';
import { useTranslation } from 'react-i18next';

export default function ResetPassword() {
  const { t } = useTranslation();
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (password !== confirmPassword) {
      alert('Passwords do not match');
      return;
    }
    alert('Password reset successfully!');
  };

  return (
    <div className="w-screen min-h-screen flex flex-col">
      <Header />
      <div className="flex flex-1 justify-center items-center bg-gray-100 dark:bg-gray-900">
        <form onSubmit={handleSubmit} className="w-full max-w-sm text-center">
          <h2 className="text-lg mb-4 text-gray-900 dark:text-white">
            {t('Reset password')}
          </h2>

          <input
            type="password"
            placeholder={t('Enter new password')}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            className="w-full mb-4 px-3 py-2 border border-gray-300 bg-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
          />

          <input
            type="password"
            placeholder={t('Re-enter password')}
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
            className="w-full mb-4 px-3 py-2 border border-gray-300 bg-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
          />

          <button
            type="submit"
            className="border border-black text-black py-1 rounded-full bg-transparent dark:text-white dark:border-white"
          >
            {t('Update password')}
          </button>
          <Link
            to="/"
            className="text-blue-600 dark:text-blue-400 flex justify-center mt-4"
          >
            {t('Login')}
          </Link>
        </form>
      </div>
    </div>
  );
}