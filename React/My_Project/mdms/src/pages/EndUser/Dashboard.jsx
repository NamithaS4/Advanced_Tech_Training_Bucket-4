// src/pages/EndUser/Dashboard.jsx
import React from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';
import useAuth from '../../hooks/useAuth';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';

const sampleData = [
  { name: '1 Oct', kwh: 420 },
  { name: '2 Oct', kwh: 350 },
  { name: '3 Oct', kwh: 280 },
  { name: '4 Oct', kwh: 300 },
  { name: '5 Oct', kwh: 260 },
  { name: '6 Oct', kwh: 384 },
  { name: '7 Oct', kwh: 220 },
];

function StatCard({ title, value, subtitle }) {
  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-4 shadow-sm">
      <div className="text-sm text-gray-500 dark:text-gray-300">{title}</div>
      <div className="mt-2 text-xl font-semibold text-gray-900 dark:text-white">{value}</div>
      {subtitle && <div className="text-xs text-gray-500 mt-1">{subtitle}</div>}
    </div>
  );
}

export default function Dashboard() {
  const { user } = useAuth();
  const currentDate = new Date().toLocaleDateString();

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900">
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <div className="max-w-6xl mx-auto">
            <div className="flex justify-between items-start mb-4">
              <div>
                <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                  Welcome, {user?.name ?? 'User'}
                </h1>
                <p className="text-sm text-gray-600 dark:text-gray-300">
                  As of {currentDate}
                </p>
                <p className="text-sm text-gray-600 dark:text-gray-300">Zone: {user?.zone ?? '—'}</p>
              </div>
              <div className="text-sm text-gray-600 dark:text-gray-300 text-right">
                <div>Last synced at 10:45 AM</div>
                <div className="mt-1">Data Source: Smart Meter #1023</div>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
              <StatCard title="Current Consumption" value="256 kWh" subtitle="" />
              <StatCard title="This Month's Bill" value="₹1,230" subtitle="Due on 12 Oct" />
              <StatCard title="Outstanding Balance" value="₹120" subtitle="" />
              <StatCard title="Last Payment" value="Paid ₹1,200" subtitle="10 Sep" />
            </div>

            <h2 className="text-lg font-semibold text-gray-800 dark:text-white mb-3">Electricity Consumption Overview</h2>

            <div className="bg-white dark:bg-gray-800 p-6 rounded-lg border border-gray-200 dark:border-gray-700 mb-6">
              <div className="mb-4 flex justify-end gap-2">
                <button className="px-3 py-1 rounded-full border">Day</button>
                <button className="px-3 py-1 rounded-full border bg-gray-100 dark:bg-gray-700">Week</button>
                <button className="px-3 py-1 rounded-full border">Month</button>
              </div>

              <div style={{ height: 280 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={sampleData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip />
                    <Line type="monotone" dataKey="kwh" stroke="#6b46c1" strokeWidth={2} dot={{ r: 4 }} />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="mb-6">
              <h3 className="text-md font-semibold text-gray-800 dark:text-white mb-2">Quick Actions</h3>
              <div className="flex gap-3 flex-wrap">
                <button className="px-4 py-2 rounded-lg border">Pay Bill</button>
                <button className="px-4 py-2 rounded-lg border">View Bill History</button>
                <button className="px-4 py-2 rounded-lg border">View Detailed Usage</button>
                <button className="px-4 py-2 rounded-lg border">Manage Notifications</button>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}