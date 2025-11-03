import React, { useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';
import { Bell } from 'lucide-react';
import { endUserDataService } from '../../services/endUserDataService';

export default function AlertsNotifications() {
  const userData = endUserDataService.getUserData();
  const notifications = userData.notifications || [];

  const [selectedNotification, setSelectedNotification] = useState(
    notifications[0] || null
  );

  const grouped = notifications.reduce((acc, notif) => {
    const date = notif.date;
    if (!acc[date]) acc[date] = [];
    acc[date].push(notif);
    return acc;
  }, {});

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <EndUserSidebar />

        <main className="flex-1 p-6">
          <div className="max-w-7xl mx-auto flex gap-6">
            <div className="w-2/5 bg-white dark:bg-gray-800 rounded-xl p-4 overflow-y-auto shadow-sm">
              {Object.entries(grouped).map(([date, list]) => (
                <div key={date} className="mb-6">
                  <p className="text-sm text-gray-500 mb-3">{date}</p>
                  {list.map((notif) => (
                    <div
                      key={notif.id}
                      onClick={() => setSelectedNotification(notif)}
                      className={`flex items-center p-4 mb-3 border rounded-xl cursor-pointer transition shadow-sm ${
                        selectedNotification?.id === notif.id
                          ? 'border-purple-600 shadow-md'
                          : 'border-gray-300 hover:border-gray-400'
                      }`}
                    >
                      <div className="flex items-center justify-center w-10 h-10 bg-gray-100 rounded-full mr-4">
                        <Bell className="w-5 h-5 text-gray-500" />
                      </div>
                      <div>
                        <h3 className="text-base font-semibold text-gray-900 dark:text-white">
                          {notif.title}
                        </h3>
                        <p className="text-sm text-gray-600 dark:text-gray-300">
                          {notif.description}
                        </p>
                      </div>
                    </div>
                  ))}
                </div>
              ))}
            </div>

            <div className="w-3/5 bg-white dark:bg-gray-800 rounded-xl p-6 shadow-sm">
              {selectedNotification ? (
                <>
                  <div className="flex justify-between items-start mb-4">
                    <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                      {selectedNotification.title}
                    </h2>
                    <div className="text-right text-sm text-gray-500">
                      <p>{selectedNotification.date}</p>
                      <p>{selectedNotification.time}</p>
                    </div>
                  </div>
                  <div className="text-gray-700 dark:text-gray-300 space-y-4 text-sm leading-relaxed">
                    {selectedNotification.content}
                  </div>
                </>
              ) : (
                <p className="text-gray-500 dark:text-gray-400 text-sm">
                  Select a notification to view details.
                </p>
              )}
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
