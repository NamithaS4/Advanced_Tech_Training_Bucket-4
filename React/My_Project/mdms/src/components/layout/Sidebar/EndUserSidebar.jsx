// src/components/layout/Sidebar/EndUserSidebar.jsx
import React from 'react';
import { NavLink } from 'react-router-dom';

const items = [
  { to: '/enduser/dashboard', label: 'Dashboard' },
  { to: '/enduser/bills-payments', label: 'Bills & Payments' },
  { to: '/enduser/meter-data', label: 'Meter Data' },
  { to: '/enduser/alerts', label: 'Alerts & Notifications' },
  { to: '/enduser/profile', label: 'Profile & Settings' },
  { to: '/enduser/logs', label: 'Logs' },
];

export default function EndUserSidebar() {
  return (
    <aside className="w-64 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 min-h-screen p-4 hidden md:block">
      <div className="mb-6">
        <h2 className="text-lg font-semibold text-gray-800 dark:text-white">
          MDMS
        </h2>
      </div>
      <nav className="flex flex-col gap-2">
        {items.map((it) => (
          <NavLink
            key={it.to}
            to={it.to}
            className={({ isActive }) =>
              `px-4 py-3 rounded-md text-left text-sm hover:bg-gray-100 dark:hover:bg-gray-700 transition ${
                isActive
                  ? 'bg-gray-100 dark:bg-gray-700 font-medium'
                  : 'text-gray-700 dark:text-gray-300'
              }`
            }
          >
            {it.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}