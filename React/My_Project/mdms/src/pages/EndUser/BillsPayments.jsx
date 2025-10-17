import React from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';

export default function BillsPayments() {
  return (
    <div>
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <h1 className="text-xl font-semibold">Bills & Payments</h1>
          <p className="text-sm text-gray-600">Placeholder for bills & payments list and actions.</p>
        </main>
      </div>
    </div>
  );
}