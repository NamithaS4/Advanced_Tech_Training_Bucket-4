import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Header from '../../components/layout/Header/Header';
import { authService } from '../../services/authService';

export default function ResetPassword() {
  const navigate = useNavigate();
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [resetData, setResetData] = useState(null);

  useEffect(() => {
    const data = sessionStorage.getItem('reset_user');
    if (!data) {
      navigate('/forgot-password', { replace: true });
    } else {
      setResetData(JSON.parse(data));
    }
  }, [navigate]);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!resetData) return;

    if (password !== confirmPassword) {
      alert('Passwords do not match!');
      return;
    }

    authService.resetPassword(resetData.email, password);
    alert('Password updated successfully! Please login again.');
    sessionStorage.removeItem('reset_user');
    navigate('/');
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
            Reset Password
          </h2>

          <input
            type="password"
            placeholder="New Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full mb-4 px-3 py-2 border border-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
            required
          />

          <input
            type="password"
            placeholder="Confirm New Password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            className="w-full mb-4 px-3 py-2 border border-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
            required
          />

          <button
            type="submit"
            className="border border-black px-6 text-black py-1 rounded-full bg-transparent dark:text-white dark:border-white"
          >
            Update Password
          </button>
        </form>
      </div>
    </div>
  );
}
