import React, { useState, useEffect } from 'react';
import Header from '../../components/layout/Header/Header';
import ZoneSidebar from '../../components/layout/Sidebar/ZoneSidebar';
import Papa from 'papaparse';
import { MoreVertical } from 'lucide-react';

export default function MeterManagement() {
  const [meters, setMeters] = useState([]);
  const [activeDropdown, setActiveDropdown] = useState(null);
  const [editingMeter, setEditingMeter] = useState(null);
  const [editedData, setEditedData] = useState({});
  const [currentPage, setCurrentPage] = useState(1);
  const metersPerPage = 10;

  useEffect(() => {
    const saved = localStorage.getItem('zoneMeters');
    if (saved) {
      setMeters(JSON.parse(saved));
    } else {
      const defaultMeters = [
        {
          id: 123,
          zone: 'Mangalore',
          owner: 'abc',
          status: 'Active',
          lastReading: '2025-10-07T07:15:13Z',
        },
        {
          id: 124,
          zone: 'Bajpe',
          owner: 'xyz',
          status: 'De-Activated',
          lastReading: '2025-10-07T07:15:13Z',
        },
        {
          id: 125,
          zone: 'Udupi',
          owner: 'pqr',
          status: 'Active',
          lastReading: '2025-10-07T07:15:13Z',
        },
        {
          id: 126,
          zone: 'Surathkal',
          owner: 'lmn',
          status: 'De-Activated',
          lastReading: '2025-10-07T07:15:13Z',
        },
      ];
      setMeters(defaultMeters);
      localStorage.setItem('zoneMeters', JSON.stringify(defaultMeters));
    }
  }, []);

  const saveToLocal = (updated) => {
    setMeters(updated);
    localStorage.setItem('zoneMeters', JSON.stringify(updated));
  };

  const handleImportCSV = (e) => {
    const file = e.target.files[0];
    if (!file) return;
    Papa.parse(file, {
      header: true,
      complete: (results) => {
        const importedMeters = results.data.map((r) => ({
          id: r['Meter ID'],
          zone: r['Zone'],
          owner: r['Owner'],
          status: r['Status'],
          lastReading: r['Last Reading'],
        }));
        saveToLocal(importedMeters);
      },
    });
  };

  const handleExportCSV = () => {
    const csvData = meters.map((m) => ({
      'Meter ID': m.id,
      Zone: m.zone,
      Owner: m.owner,
      Status: m.status,
      'Last Reading': m.lastReading,
    }));
    const csv = Papa.unparse(csvData);
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'meter_management_data.csv';
    link.click();
  };

  const toggleDropdown = (id) => {
    setActiveDropdown(activeDropdown === id ? null : id);
  };

  const handleEdit = (meter) => {
    setEditingMeter(meter.id);
    setEditedData(meter);
    setActiveDropdown(null);
  };

  const handleSaveEdit = () => {
    const updated = meters.map((m) => (m.id === editingMeter ? editedData : m));
    saveToLocal(updated);
    setEditingMeter(null);
  };

  const handleActivate = (id) => {
    const updated = meters.map((m) =>
      m.id === id ? { ...m, status: 'Active' } : m
    );
    saveToLocal(updated);
    setActiveDropdown(null);
  };

  const handleDeactivateAll = () => {
    const updated = meters.map((m) => ({ ...m, status: 'De-Activated' }));
    saveToLocal(updated);
  };

  const startIndex = (currentPage - 1) * metersPerPage;
  const displayedMeters = meters.slice(startIndex, startIndex + metersPerPage);
  const totalPages = Math.ceil(meters.length / metersPerPage);

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900">
      <Header />
      <div className="flex">
        <ZoneSidebar />
        <main className="flex-1 p-6">
          <div className="max-w-6xl mx-auto">
            <h1 className="text-xl font-semibold text-gray-900 dark:text-white mb-6">
              Meter Management
            </h1>

            <div className="overflow-x-auto rounded-lg shadow border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 mb-6">
              <table className="min-w-full">
                <thead className="bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-200">
                  <tr>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Meter ID
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Zone
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Owner
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Status
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Last Reading
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      More Actions
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {displayedMeters.map((meter) => (
                    <tr
                      key={meter.id}
                      className="border-t text-black dark:text-white border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
                    >
                      {editingMeter === meter.id ? (
                        <>
                          <td className="px-4 py-2">
                            <input
                              value={editedData.id}
                              onChange={(e) =>
                                setEditedData({
                                  ...editedData,
                                  id: e.target.value,
                                })
                              }
                              className="border px-2 py-1 rounded w-20"
                            />
                          </td>
                          <td className="px-4 py-2">
                            <input
                              value={editedData.zone}
                              onChange={(e) =>
                                setEditedData({
                                  ...editedData,
                                  zone: e.target.value,
                                })
                              }
                              className="border px-2 py-1 rounded"
                            />
                          </td>
                          <td className="px-4 py-2">
                            <input
                              value={editedData.owner}
                              onChange={(e) =>
                                setEditedData({
                                  ...editedData,
                                  owner: e.target.value,
                                })
                              }
                              className="border px-2 py-1 rounded"
                            />
                          </td>
                          <td className="px-4 py-2">
                            <select
                              value={editedData.status}
                              onChange={(e) =>
                                setEditedData({
                                  ...editedData,
                                  status: e.target.value,
                                })
                              }
                              className="border px-2 py-1 rounded"
                            >
                              <option>Active</option>
                              <option>De-Activated</option>
                            </select>
                          </td>
                          <td className="px-4 py-2">
                            <input
                              value={editedData.lastReading}
                              onChange={(e) =>
                                setEditedData({
                                  ...editedData,
                                  lastReading: e.target.value,
                                })
                              }
                              className="border px-2 py-1 rounded"
                            />
                          </td>
                          <td className="px-4 py-2">
                            <button
                              onClick={handleSaveEdit}
                              className="text-green-600 hover:underline"
                            >
                              Save
                            </button>
                          </td>
                        </>
                      ) : (
                        <>
                          <td className="px-4 py-2">{meter.id}</td>
                          <td className="px-4 py-2">{meter.zone}</td>
                          <td className="px-4 py-2">{meter.owner}</td>
                          <td
                            className={`px-4 py-2 font-semibold ${
                              meter.status === 'Active'
                                ? 'text-green-600'
                                : 'text-red-600'
                            }`}
                          >
                            {meter.status}
                          </td>
                          <td className="px-4 py-2">{meter.lastReading}</td>
                          <td className="px-4 py-2 relative">
                            <button
                              onClick={() => toggleDropdown(meter.id)}
                              className="p-1 bg-transparent "
                            >
                              <MoreVertical size={16} />
                            </button>
                            {activeDropdown === meter.id && (
                              <div className="absolute right-0 bg-white dark:bg-gray-700 shadow rounded text-sm border border-gray-200 dark:border-gray-600 z-10">
                                <button
                                  className="block w-full px-4 py-2 bg-white text-black text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                  onClick={() =>
                                    alert(JSON.stringify(meter, null, 2))
                                  }
                                >
                                  View
                                </button>
                                <button
                                  className="block w-full px-4 py-2 bg-white text-black text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                  onClick={() => handleEdit(meter)}
                                >
                                  Edit
                                </button>
                                <button
                                  className="block w-full px-4 py-2 text-left text-black bg-white hover:bg-gray-100 dark:hover:bg-gray-600"
                                  onClick={() => handleActivate(meter.id)}
                                >
                                  Activate
                                </button>
                              </div>
                            )}
                          </td>
                        </>
                      )}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="flex justify-between items-center text-sm text-gray-700 dark:text-gray-300 mb-4">
              <button
                disabled={currentPage === 1}
                onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                className="bg-white text-black"
              >
                ← Previous
              </button>
              <div>
                Page {currentPage} of {totalPages}
              </div>
              <button
                disabled={currentPage === totalPages}
                onClick={() =>
                  setCurrentPage((p) => Math.min(totalPages, p + 1))
                }
                className="bg-white text-black"
              >
                Next →
              </button>
            </div>

            <div className="mt-6 text-sm text-gray-800 dark:text-gray-300">
              <strong>Bulk operations</strong>
              <div className="flex gap-4 mt-3">
                <label className="border px-4 py-2 rounded-md bg-gray-100 hover:bg-gray-200 text-black cursor-pointer">
                  Import CSV
                  <input
                    type="file"
                    accept=".csv"
                    className="hidden"
                    onChange={handleImportCSV}
                  />
                </label>
                <button
                  onClick={handleExportCSV}
                  className="border px-4 py-2 rounded-md bg-gray-100 hover:bg-gray-200 text-black"
                >
                  Export CSV
                </button>
                <button
                  onClick={handleDeactivateAll}
                  className="border px-4 py-2 rounded-md bg-gray-100 hover:bg-gray-200 text-black"
                >
                  De-Activate meters
                </button>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
