import React, { useState, useEffect } from 'react';
import { Clock, Banknote, ReceiptText, BarChart, Settings } from 'lucide-react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';
import useAuth from '../../hooks/useAuth';
import { endUserDataService } from '../../services/endUserDataService';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';
import { Link } from 'react-router-dom';

function StatCard({ title, value }) {
  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-4 shadow-sm flex flex-col items-center text-center gap-3">
      <Clock className="h-10 w-10 text-purple-600 dark:text-white" />
      <div>
        <div className="mt-1 text-xl font-semibold text-gray-900 dark:text-white">
          {value}
        </div>
        <div className="text-sm text-gray-500 dark:text-gray-300">{title}</div>
      </div>
    </div>
  );
}

export default function Dashboard() {
  const { user } = useAuth();
  const [filter, setFilter] = useState('month');
  const stats = endUserDataService.getDashboardStats();
  const [chartData, setChartData] = useState(
    endUserDataService.getChartData(filter)
  );

  const handleFilter = (type) => {
    setFilter(type);
    setChartData(endUserDataService.getChartData(type));
  };

  const currentDate = new Date().toLocaleDateString();
  const [currentTime, setCurrentTime] = useState(new Date());
  useEffect(() => {
    const timer = setInterval(() => setCurrentTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);
  const formattedTime = currentTime.toLocaleTimeString();

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
                <p className="text-sm text-gray-600 dark:text-gray-300">
                  Zone: {user?.zone ?? '—'}
                </p>
              </div>
              <div className="text-sm text-gray-600 dark:text-gray-300 text-right">
                <div>Last synced at {formattedTime}</div>
                <div className="mt-1">Data Source: Smart Meter #1023</div>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
              <StatCard
                title="Current Consumption"
                value={stats.currentConsumption}
              />
              <StatCard title="This Month's Bill" value={stats.currentBill} />
              <StatCard title="Outstanding Balance" value={stats.outstanding} />
              <StatCard title="Last Payment" value={stats.lastPayment} />
            </div>

            <h2 className="text-lg font-semibold text-gray-800 dark:text-white mb-3">
              Electricity Consumption Overview
            </h2>
            <div className="bg-white dark:bg-gray-800 p-6 rounded-lg border border-gray-200 dark:border-gray-700 mb-6">
              <div className="flex justify-end mb-4">
                <div className="bg-gray-200 dark:bg-white rounded-full flex p-1">
                  {['day', 'week', 'month'].map((type) => (
                    <button
                      key={type}
                      onClick={() => handleFilter(type)}
                      className={`px-4 py-1 rounded-full font-medium transition ${
                        filter === type
                          ? 'bg-purple-600 text-white'
                          : 'text-black bg-gray-200 dark:text-black hover:bg-gray-300 dark:bg-white'
                      }`}
                    >
                      {type.charAt(0).toUpperCase() + type.slice(1)}
                    </button>
                  ))}
                </div>
              </div>

              <div style={{ height: 280 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={chartData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip />
                    <Line
                      type="monotone"
                      dataKey="kwh"
                      stroke="#6b46c1"
                      strokeWidth={2}
                      dot={{ r: 4 }}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="mb-6">
              <h3 className="text-md font-semibold text-gray-800 dark:text-white mb-2">
                Quick Actions
              </h3>
              <div className="flex gap-3 flex-wrap">
                <Link to="/enduser/bills-payments">
                  <button className="bg-gray-200 text-black flex items-center px-4 py-2 rounded-lg border">
                    <Banknote size={16} />
                    Pay Bill
                  </button>
                </Link>
                <Link to="/enduser/bills-payments">
                  <button className="bg-gray-200 text-black flex items-center px-4 py-2 rounded-lg border">
                    <ReceiptText size={16} />
                    View Bill History
                  </button>
                </Link>
                <Link to="/enduser/meter-data">
                  <button className="bg-gray-200 text-black flex items-center px-4 py-2 rounded-lg border">
                    <BarChart size={16} />
                    View Detailed Usage
                  </button>
                </Link>
                <Link to="/enduser/alerts">
                  <button className="bg-gray-200 text-black flex items-center px-4 py-2 rounded-lg border">
                    <Settings size={16} />
                    Manage Notifications
                  </button>
                </Link>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
