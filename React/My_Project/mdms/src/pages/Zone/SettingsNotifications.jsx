import React, { useState, useEffect } from 'react';
import Header from '../../components/layout/Header/Header';
import ZoneSidebar from '../../components/layout/Sidebar/ZoneSidebar';

export default function SettingsNotifications() {
  const [activeTab, setActiveTab] = useState('settings');

  const [settings, setSettings] = useState({
    highThreshold: 600,
    lowThreshold: 300,
    abnormalFreq: 4,
    inactiveDay: 'Sunday',
  });

  const [notifications, setNotifications] = useState({
    email: true,
    sms: false,
    push: true,
  });

  useEffect(() => {
    const savedSettings = JSON.parse(localStorage.getItem('zone_settings'));
    const savedNotif = JSON.parse(localStorage.getItem('zone_notifications'));
    if (savedSettings) setSettings(savedSettings);
    if (savedNotif) setNotifications(savedNotif);
  }, []);

  const saveSettings = () => {
    localStorage.setItem('zone_settings', JSON.stringify(settings));
    localStorage.setItem('zone_notifications', JSON.stringify(notifications));
    alert('Preferences saved successfully!');
  };

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <ZoneSidebar />
        <main className="flex-1 p-8">
          <div className="max-w-5xl mx-auto">
            <h1 className="text-xl font-semibold mb-1 dark:text-white">
              User Management
            </h1>
            <p className="text-gray-500 mb-6">
              Manage your alert rules and communication preferences.
            </p>

            <div className="flex border-b border-gray-300 mb-8">
              {['settings', 'notifications'].map((tab) => (
                <button
                  key={tab}
                  onClick={() => setActiveTab(tab)}
                  className={`w-full text-black dark:text-white bg-transparent pb-2 text-sm font-medium relative focus:outline-none hover:border-transparent ${
                    activeTab === tab
                      ? 'after:absolute after:bottom-0 after:left-0 after:w-full after:h-[2px] after:bg-purple-500'
                      : ''
                  }`}
                >
                  {tab.charAt(0).toUpperCase() + tab.slice(1)}
                </button>
              ))}
            </div>

            {activeTab === 'settings' && (
              <div>
                <h2 className="font-medium mb-2 dark:text-white">
                  Alert Thresholds
                </h2>
                <p className="text-gray-500 text-sm mb-6">
                  Set consumption limits that trigger automatic alerts for
                  meters in your zone.
                </p>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                  <div className="bg-white dark:bg-gray-800 shadow-md rounded-xl p-6">
                    <p className="text-sm font-medium mb-3 dark:text-white">
                      High Consumption Threshold (kWh)
                    </p>
                    <input
                      type="range"
                      min="0"
                      max="1000"
                      step="10"
                      value={settings.highThreshold}
                      onChange={(e) =>
                        setSettings({
                          ...settings,
                          highThreshold: Number(e.target.value),
                        })
                      }
                      className="w-full accent-purple-600"
                    />
                    <div className="flex justify-between text-xs mt-1 text-gray-500">
                      <span>0</span>
                      <span>{settings.highThreshold}</span>
                      <span>1000</span>
                    </div>
                  </div>

                  <div className="bg-white dark:bg-gray-800 shadow-md rounded-xl p-6">
                    <p className="text-sm font-medium mb-3 dark:text-white">
                      Low Consumption Threshold (kWh)
                    </p>
                    <input
                      type="range"
                      min="0"
                      max="1000"
                      step="10"
                      value={settings.lowThreshold}
                      onChange={(e) =>
                        setSettings({
                          ...settings,
                          lowThreshold: Number(e.target.value),
                        })
                      }
                      className="w-full accent-purple-600"
                    />
                    <div className="flex justify-between text-xs mt-1 text-gray-500">
                      <span>0</span>
                      <span>{settings.lowThreshold}</span>
                      <span>1000</span>
                    </div>
                  </div>

                  <div className="bg-white dark:bg-gray-800 shadow-md rounded-xl p-6">
                    <p className="text-sm font-medium mb-3 dark:text-white">
                      Abnormal Reading Frequency (hours)
                    </p>
                    <input
                      type="range"
                      min="0"
                      max="10"
                      step="1"
                      value={settings.abnormalFreq}
                      onChange={(e) =>
                        setSettings({
                          ...settings,
                          abnormalFreq: Number(e.target.value),
                        })
                      }
                      className="w-full accent-purple-600"
                    />
                    <div className="flex justify-between text-xs mt-1 text-gray-500">
                      <span>0</span>
                      <span>{settings.abnormalFreq}</span>
                      <span>10</span>
                    </div>
                  </div>

                  <div className="bg-white dark:bg-gray-800 shadow-md rounded-xl p-6">
                    <p className="text-sm font-medium mb-3 dark:text-white">
                      Inactive Meters Duration (days)
                    </p>
                    <select
                      className="border rounded-md px-3 bg-white py-2 w-full dark:bg-gray-700 dark:text-white"
                      value={settings.inactiveDay}
                      onChange={(e) =>
                        setSettings({
                          ...settings,
                          inactiveDay: e.target.value,
                        })
                      }
                    >
                      {[
                        'Sunday',
                        'Monday',
                        'Tuesday',
                        'Wednesday',
                        'Thursday',
                        'Friday',
                        'Saturday',
                      ].map((day) => (
                        <option key={day}>{day}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="mt-10 flex justify-center">
                  <button
                    onClick={saveSettings}
                    className="bg-black text-white px-8 py-3 rounded-full shadow-md hover:opacity-90"
                  >
                    Save and continue
                  </button>
                </div>
              </div>
            )}

            {activeTab === 'notifications' && (
              <div className="flex flex-col items-center">
                <div className="space-y-6 mt-6">
                  {Object.keys(notifications).map((key) => (
                    <div
                      key={key}
                      className="flex justify-between items-center w-64"
                    >
                      <span className="capitalize dark:text-white">{key}</span>
                      <label className="relative inline-flex items-center cursor-pointer">
                        <input
                          type="checkbox"
                          checked={notifications[key]}
                          onChange={() =>
                            setNotifications({
                              ...notifications,
                              [key]: !notifications[key],
                            })
                          }
                          className="sr-only peer"
                        />
                        <div className="w-11 h-6 bg-gray-200 rounded-full peer peer-checked:bg-purple-600 transition-all"></div>
                        <div className="absolute left-1 top-1 bg-white w-4 h-4 rounded-full peer-checked:translate-x-full transition-transform"></div>
                      </label>
                    </div>
                  ))}
                </div>

                <button
                  onClick={saveSettings}
                  className="mt-10 bg-black text-white px-8 py-3 rounded-full shadow-md hover:opacity-90"
                >
                  Save and continue
                </button>
              </div>
            )}
          </div>
        </main>
      </div>
    </div>
  );
}
