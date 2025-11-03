import React, { useState } from 'react';
import Header from '../../components/layout/Header/Header';
import ZoneSidebar from '../../components/layout/Sidebar/ZoneSidebar';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';
import { zoneManagementDataService } from '../../services/zoneManagementDataService';
import { Activity, TrendingUp, AlertTriangle } from 'lucide-react';
import { Link } from 'react-router-dom';

const StatCard = ({ icon: Icon, title, value }) => (
  <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-5 flex flex-col items-center text-center gap-3 shadow-sm">
    <Icon className="h-8 w-8 text-purple-600 dark:text-white" />
    <div className="text-xl font-semibold text-gray-900 dark:text-white">
      {value}
    </div>
    <div className="text-sm text-gray-500 dark:text-gray-300">{title}</div>
  </div>
);

export default function ZoneDashboard() {
  const zoneData = zoneManagementDataService.getZoneData();
  const [filter, setFilter] = useState('week');
  const chartData = zoneManagementDataService.getChartData(filter);

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900">
      <Header />
      <div className="flex">
        <ZoneSidebar />

        <main className="flex-1 p-6">
          <div className="max-w-6xl mx-auto">
            <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
              Zone Dashboard
            </h1>

            <div className="grid grid-cols-1 text-black dark:text-white sm:grid-cols-3 gap-4 mb-8">
              <StatCard
                icon={Activity}
                title="Active meters"
                value={zoneData.totalMeters}
              />
              <StatCard
                icon={TrendingUp}
                title="Avg usage"
                value={`${zoneData.avgUsage}%`}
              />
              <StatCard
                icon={AlertTriangle}
                title="Pending alerts"
                value={zoneData.pendingAlerts}
              />
            </div>

            <div className="bg-white dark:bg-gray-800 p-6 rounded-lg border border-gray-200 dark:border-gray-700 mb-6">
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-lg font-semibold text-gray-800 dark:text-white">
                  Analytics Chart
                </h2>
                <div className="bg-gray-200 dark:bg-white rounded-full flex p-1">
                  {['week', 'month'].map((type) => (
                    <button
                      key={type}
                      onClick={() => setFilter(type)}
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

              <div style={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={chartData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip />
                    <Line
                      type="linear"
                      dataKey="usage"
                      stroke="#a855f7"
                      strokeWidth={2}
                      dot={{ r: 4 }}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="flex gap-4 justify-start">
              <Link to={'/zone/meter-management'}>
                <button className="flex items-center gap-2 border px-4 py-2 rounded-md bg-gray-100 hover:bg-gray-200 text-sm text-black dark:bg-gray-700 dark:text-white dark:hover:bg-gray-600">
                  + Add meter
                </button>
              </Link>
              <Link to={'/zone/reports'}>
                <button className="flex items-center gap-2 border px-4 py-2 rounded-md bg-gray-100 hover:bg-gray-200 text-sm text-black dark:bg-gray-700 dark:text-white dark:hover:bg-gray-600">
                  📊 Generate Report
                </button>
              </Link>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
