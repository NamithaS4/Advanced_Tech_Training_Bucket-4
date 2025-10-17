import React from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';

export default function MeterData() {
  return (
    <div>
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <h1 className="text-xl font-semibold">Meter Data</h1>
          <p className="text-sm text-gray-600">Historical readings and comparison UI will be here.</p>
        </main>
      </div>
    </div>
  );
}