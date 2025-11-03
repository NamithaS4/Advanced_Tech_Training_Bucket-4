import React, { useState, useEffect } from 'react';
import Header from '../../components/layout/Header/Header';
import EnterpriseSidebar from '../../components/layout/Sidebar/EnterpriseSidebar';

export default function SettingsNotifications() {
  const [activeTab, setActiveTab] = useState('settings');

  const [settings, setSettings] = useState({
    dataRetentionDays: 30,
    autoLogoutMinutes: 30,
    auditLogRetentionDays: 30,
    timezone: 'UTC+0',
    language: 'English',
    currency: 'INR',
  });

  const [notifications, setNotifications] = useState({
    email: true,
    sms: false,
    push: true,
  });

  useEffect(() => {
    const savedSettings = JSON.parse(
      localStorage.getItem('enterprise_settings')
    );
    const savedNotif = JSON.parse(
      localStorage.getItem('enterprise_notifications')
    );
    if (savedSettings) setSettings(savedSettings);
    if (savedNotif) setNotifications(savedNotif);
  }, []);

  const saveSettings = () => {
    localStorage.setItem('enterprise_settings', JSON.stringify(settings));
    localStorage.setItem(
      'enterprise_notifications',
      JSON.stringify(notifications)
    );
    alert('Preferences saved successfully!');
  };

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <EnterpriseSidebar />

        <main className="flex-1 p-8">
          <div className="max-w-5xl mx-auto">
            <h1 className="text-xl font-semibold mb-1 dark:text-white">
              Setting and Configuration
            </h1>
            <p className="text-gray-500 mb-6">
              Manage organization-wide configurations and integrations
            </p>

            <div className="flex border-b border-gray-300 mb-8">
              {['settings', 'notification'].map((tab) => (
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
                  Policies & Rules :
                </h2>
                <p className="text-gray-500 text-sm mb-6">
                  Define enterprise-wide operational constraints and retention
                  policies.
                </p>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-10">
                  <div>
                    <label className="block text-sm font-medium mb-2 dark:text-white">
                      Data Retention Period (in days)
                    </label>
                    <input
                      type="number"
                      className="border rounded-md px-3 py-2 w-full dark:bg-gray-700 dark:text-white"
                      value={settings.dataRetentionDays}
                      onChange={(e) =>
                        setSettings({
                          ...settings,
                          dataRetentionDays: Number(e.target.value),
                        })
                      }
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-2 dark:text-white">
                      Auto Logout Timer (minutes)
                    </label>
                    <input
                      type="number"
                      className="border rounded-md px-3 py-2 w-full dark:bg-gray-700 dark:text-white"
                      value={settings.autoLogoutMinutes}
                      onChange={(e) =>
                        setSettings({
                          ...settings,
                          autoLogoutMinutes: Number(e.target.value),
                        })
                      }
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-2 dark:text-white">
                      Audit Log Retention (in days)
                    </label>
                    <input
                      type="number"
                      className="border rounded-md px-3 py-2 w-full dark:bg-gray-700 dark:text-white"
                      value={settings.auditLogRetentionDays}
                      onChange={(e) =>
                        setSettings({
                          ...settings,
                          auditLogRetentionDays: Number(e.target.value),
                        })
                      }
                    />
                  </div>
                </div>

                <h2 className="font-medium mb-2 dark:text-white">
                  Localization :
                </h2>
                <p className="text-gray-500 text-sm mb-6">
                  Set the enterprise’s regional preferences and user experience
                  defaults.
                </p>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                  <div>
                    <label className="block text-sm font-medium mb-2 dark:text-white">
                      Timezone
                    </label>
                    <select
                      className="border rounded-md px-3 py-2 w-full dark:bg-gray-700 dark:text-white"
                      value={settings.timezone}
                      onChange={(e) =>
                        setSettings({ ...settings, timezone: e.target.value })
                      }
                    >
                      {[
                        'UTC-5',
                        'UTC-2',
                        'UTC+0',
                        'UTC+3',
                        'UTC+5:30',
                        'UTC+8',
                      ].map((tz) => (
                        <option key={tz}>{tz}</option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-2 dark:text-white">
                      Default Language
                    </label>
                    <select
                      className="border rounded-md px-3 py-2 w-full dark:bg-gray-700 dark:text-white"
                      value={settings.language}
                      onChange={(e) =>
                        setSettings({ ...settings, language: e.target.value })
                      }
                    >
                      {['English', 'Spanish', 'French', 'Hindi', 'Chinese'].map(
                        (lang) => (
                          <option key={lang}>{lang}</option>
                        )
                      )}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-2 dark:text-white">
                      Currency Format
                    </label>
                    <select
                      className="border rounded-md px-3 py-2 w-full dark:bg-gray-700 dark:text-white"
                      value={settings.currency}
                      onChange={(e) =>
                        setSettings({ ...settings, currency: e.target.value })
                      }
                    >
                      {['INR', 'USD', 'EUR', 'GBP', 'JPY'].map((curr) => (
                        <option key={curr}>{curr}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="mt-10 flex justify-center">
                  <button
                    onClick={saveSettings}
                    className="bg-black text-white px-8 py-3 rounded-full shadow-md hover:opacity-90"
                  >
                    Save the changes
                  </button>
                </div>
              </div>
            )}

            {activeTab === 'notification' && (
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
