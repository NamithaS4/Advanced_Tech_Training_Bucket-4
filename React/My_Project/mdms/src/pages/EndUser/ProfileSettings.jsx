import React, { useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';
import useAuth from '../../hooks/useAuth';

export default function ProfileSettings() {
  const { user, updateProfile } = useAuth();
  const [name, setName] = useState(user?.name || '');
  const [email, setEmail] = useState(user?.email || '');
  const [password, setPassword] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    const updated = updateProfile({ name, email, password: password || undefined });
    alert('Profile updated');
    // you might want to refresh or do a full state update - our useAuth updates current user.
  };

  return (
    <div>
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <h1 className="text-xl font-semibold mb-4">Profile & Settings</h1>
          <form onSubmit={handleSubmit} className="max-w-md bg-white dark:bg-gray-800 p-6 rounded-md border border-gray-200 dark:border-gray-700">
            <label className="block mb-2 text-sm">Name</label>
            <input value={name} onChange={(e) => setName(e.target.value)} className="w-full mb-3 p-2 rounded border" />
            <label className="block mb-2 text-sm">Email</label>
            <input value={email} onChange={(e) => setEmail(e.target.value)} className="w-full mb-3 p-2 rounded border" />
            <label className="block mb-2 text-sm">Change Password</label>
            <input value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Leave blank to keep" className="w-full mb-3 p-2 rounded border" />
            <button type="submit" className="px-4 py-2 rounded bg-indigo-600 text-white">Save</button>
          </form>
        </main>
      </div>
    </div>
  );
}