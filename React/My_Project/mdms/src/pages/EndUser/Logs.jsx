import React from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';

export default function Logs() {
  return (
    <div>
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <h1 className="text-xl font-semibold">Logs</h1>
          <p className="text-sm text-gray-600">Local logs, uploads and audit trail UI.</p>
        </main>
      </div>
    </div>
  );
}