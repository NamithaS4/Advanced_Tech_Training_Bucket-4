import React, { useEffect, useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EnterpriseSidebar from '../../components/layout/Sidebar/EnterpriseSidebar';
import { enterpriseDataService } from '../../services/enterpriseDataService';
import EnterpriseMap from './EnterpriseMap';
import {
  Activity,
  TrendingUp,
  AlertTriangle,
  BarChart,
  MapPin,
  ThumbsUp,
  ThumbsDown,
  Maximize2,
} from 'lucide-react';

const StatCard = ({ icon: Icon, title, value, color }) => (
  <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-5 flex flex-col items-center text-center gap-3 shadow-sm">
    <Icon className={`h-8 w-8 ${color || 'text-black dark:text-white'}`} />
    <div className="text-xl font-semibold text-gray-900 dark:text-white">
      {value}
    </div>
    <div className="text-sm text-gray-500 dark:text-gray-300">{title}</div>
  </div>
);

export default function EnterpriseDashboard() {
  const [data, setData] = useState({});
  const [zones, setZones] = useState([]);
  const [alerts, setAlerts] = useState([]);
  const [selectedAlert, setSelectedAlert] = useState(null);
  const [hoveredZone, setHoveredZone] = useState(null);

  useEffect(() => {
    setData(enterpriseDataService.getDashboardData());
    setZones(enterpriseDataService.getZones());
    const alertData = enterpriseDataService.getAlerts();
    setAlerts(alertData);
    setSelectedAlert(alertData[0]);
  }, []);

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900">
      <Header />
      <div className="flex">
        <EnterpriseSidebar />

        <main className="flex-1 p-6">
          <div className="max-w-7xl mx-auto">
            <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
              Enterprise Dashboard
            </h1>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8 text-black dark:text-white">
              <StatCard
                icon={BarChart}
                title="Total zones"
                value={data.totalZones}
              />
              <StatCard
                icon={TrendingUp}
                title="Total meters"
                value={data.totalMeters}
              />
              <StatCard
                icon={AlertTriangle}
                title="Critical alerts"
                value={data.criticalAlerts}
                color="text-red-500"
              />
              <StatCard
                icon={Activity}
                title="Avg consumption per zone"
                value={`${data.avgConsumption}%`}
              />
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
              <EnterpriseMap zones={zones} />
              <div className="bg-white dark:bg-gray-800 px-4 py-4 rounded-2xl shadow-sm border border-gray-200 dark:border-gray-700 flex">
                <div className="w-2/5 pr-3 border-r border-gray-300 dark:border-gray-600">
                  <h2 className="font-semibold text-gray-800 dark:text-white mb-3">
                    Recent Alerts
                  </h2>
                  <div className="space-y-3 overflow-y-auto max-h-[400px] pr-1">
                    {alerts.map((alert) => (
                      <div
                        key={alert.id}
                        onClick={() => setSelectedAlert(alert)}
                        className={`cursor-pointer p-3 rounded-lg transition shadow-sm ${
                          selectedAlert?.id === alert.id
                            ? 'bg-gray-100 dark:bg-gray-700 border border-gray-400'
                            : 'bg-white dark:bg-gray-800 hover:bg-gray-100 dark:hover:bg-gray-700'
                        }`}
                      >
                        <p className="font-semibold text-black dark:text-white text-sm">
                          {alert.title}
                        </p>
                        <p className="text-xs text-gray-500 truncate">
                          {alert.description}
                        </p>
                      </div>
                    ))}
                  </div>
                </div>

                {selectedAlert && (
                  <div className="w-3/5 pl-4 flex flex-col items-center text-center">
                    <div className="self-end mb-2">
                      <Maximize2 size={16} className="text-gray-400" />
                    </div>
                    <AlertTriangle size={40} className="text-red-500 mb-3" />
                    <h3 className="font-semibold text-black dark:text-white text-lg mb-1">
                      {selectedAlert.title}
                    </h3>
                    <p className="text-sm text-gray-600 dark:text-gray-300 mb-3 px-2">
                      {selectedAlert.description}
                    </p>
                    <p className="text-xs text-gray-400 mb-4">
                      Zone: {selectedAlert.zone}
                    </p>
                    <div className="flex items-center gap-6">
                      <ThumbsUp
                        size={20}
                        className="cursor-pointer text-gray-500 hover:text-green-600 transition"
                      />
                      <ThumbsDown
                        size={20}
                        className="cursor-pointer text-gray-500 hover:text-red-600 transition"
                      />
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
