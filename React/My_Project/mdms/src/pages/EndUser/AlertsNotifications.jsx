import React from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';

export default function AlertsNotifications() {
  return (
    <div>
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <h1 className="text-xl font-semibold">Alerts & Notifications</h1>
        </main>
      </div>
    </div>
  );
}