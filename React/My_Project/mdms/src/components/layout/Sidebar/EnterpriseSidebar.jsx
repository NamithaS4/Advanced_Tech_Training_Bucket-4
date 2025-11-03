import React from 'react';
import { NavLink } from 'react-router-dom';

const items = [
  { to: '/enterprise/dashboard', label: 'Dashboard' },
  { to: '/enterprise/zone-management', label: 'Zone management' },
  { to: '/enterprise/meter-management', label: 'Meter management' },
  { to: '/enterprise/user-management', label: 'User & Role management' },
  { to: '/enterprise/audit-logs', label: 'Audit logs' },
  { to: '/enterprise/settings', label: 'Setting & Configuration' },
];

export default function EnterpriseSidebar() {
  return (
    <aside className="w-64 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 min-h-screen p-4 hidden md:block">
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
