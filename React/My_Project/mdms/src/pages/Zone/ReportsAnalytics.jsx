import React, { useState } from 'react';
import Header from '../../components/layout/Header/Header';
import ZoneSidebar from '../../components/layout/Sidebar/ZoneSidebar';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  BarChart,
  Bar,
  Legend,
  ResponsiveContainer,
} from 'recharts';
import { zoneReportsData } from '../../services/zoneManagementDataService';
import Papa from 'papaparse';
import jsPDF from 'jspdf';
import 'jspdf-autotable';

export default function ReportsAnalytics() {
  const [selectedUser, setSelectedUser] = useState('abc');
  const [selectedYear, setSelectedYear] = useState(2025);

  const trendData = zoneReportsData.getTrendData();
  const users = zoneReportsData.getAvailableUsers();
  const years = zoneReportsData.getAvailableYears();

  const zoneData =
    zoneReportsData.getZoneConsumption(selectedUser, selectedYear).length > 0
      ? zoneReportsData.getZoneConsumption(selectedUser, selectedYear)
      : zoneReportsData.getZoneConsumption(users[0], years[0]);

  const tableData = zoneReportsData.getReportTable();

  const [currentPage, setCurrentPage] = useState(1);
  const perPage = 6;
  const totalPages = Math.ceil(tableData.length / perPage);
  const pageData = tableData.slice(
    (currentPage - 1) * perPage,
    currentPage * perPage
  );

  const exportCSV = () => {
    const csv = Papa.unparse(tableData);
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'reports.csv';
    a.click();
  };

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <ZoneSidebar />
        <main className="flex-1 p-6 w-screen">
          <div className="max-w-7xl mx-auto">
            <h1 className="text-xl font-semibold mb-6 dark:text-white">
              Reports and Analytics
            </h1>

            <div>
              <h2 className="text-sm mb-2 text-gray-700 dark:text-gray-300">
                Trend of energy usage over time.
              </h2>
              <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-4 mb-10 w-3/4">
                <ResponsiveContainer width="100%" height={260}>
                  <LineChart data={trendData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis
                      dataKey="date"
                      tickFormatter={(tick) => tick.slice(5)}
                    />
                    <YAxis />
                    <Tooltip />
                    <Line
                      type="linear"
                      dataKey="usage"
                      stroke="#a855f7"
                      strokeWidth={2}
                      dot
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="mb-10 w-1/2">
              <h2 className="text-sm mb-3 text-gray-700 dark:text-gray-300">
                Compare zone consumption
              </h2>

              <div className="flex justify-between items-center mb-4">
                <div className="flex items-center gap-3">
                  <div className="flex items-center border rounded px-4 py-1 bg-white">
                    <label className="text-sm mr-2">User:</label>
                    <select
                      value={selectedUser}
                      onChange={(e) => setSelectedUser(e.target.value)}
                      className="outline-none bg-transparent px-4 py-1 text-sm"
                    >
                      {users.map((u) => (
                        <option key={u}>{u}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="flex items-center gap-2">
                  <button
                    onClick={() =>
                      setSelectedYear((y) =>
                        y === Math.min(...years) ? y : y - 1
                      )
                    }
                    className="px-3 py-1 bg-gray-300 border rounded hover:bg-gray-100"
                  >
                    ←
                  </button>
                  <span className="px-3 text-sm text-black dark:text-white font-medium">
                    {selectedYear}
                  </span>
                  <button
                    onClick={() =>
                      setSelectedYear((y) =>
                        y === Math.max(...years) ? y : y + 1
                      )
                    }
                    className="px-3 py-1 bg-gray-300 border rounded hover:bg-gray-100"
                  >
                    →
                  </button>
                </div>
              </div>

              <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-4 w-full">
                {zoneData.length > 0 ? (
                  <ResponsiveContainer width="100%" height={260}>
                    <BarChart data={zoneData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="zone" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Bar
                        dataKey="consumption"
                        fill="#a855f7"
                        radius={[20, 20, 20, 20]}
                        barSize={50}
                      />
                    </BarChart>
                  </ResponsiveContainer>
                ) : (
                  <p className="text-center text-gray-500 py-10">
                    No data available for selected filters.
                  </p>
                )}
              </div>
            </div>

            <div className="w-3/4">
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-sm font-medium text-gray-700 dark:text-gray-200">
                  Reports
                </h2>
                <div className="flex gap-3">
                  <button
                    onClick={exportCSV}
                    className="px-3 py-1 bg-white border border-black rounded text-sm hover:bg-gray-100"
                  >
                    Export as CSV
                  </button>
                  <button className="px-3 py-1 bg-white border border-black rounded text-sm hover:bg-gray-100">
                    Export as PDF
                  </button>
                </div>
              </div>

              <table className="w-full text-sm border-collapse">
                <thead className="bg-gray-100 dark:bg-gray-700">
                  <tr>
                    <th className="px-4 py-2 text-left border">Meter ID</th>
                    <th className="px-4 py-2 text-left border">Date</th>
                    <th className="px-4 py-2 text-left border">User name</th>
                    <th className="px-4 py-2 text-left border">Consumption</th>
                    <th className="px-4 py-2 text-left border">Status</th>
                  </tr>
                </thead>
                <tbody>
                  {pageData.map((r, i) => (
                    <tr
                      key={i}
                      className="hover:bg-gray-50 dark:hover:bg-gray-700"
                    >
                      <td className="px-4 py-2 dark:text-white border">
                        {r.meterId}
                      </td>
                      <td className="px-4 py-2 dark:text-white border">
                        {r.date}
                      </td>
                      <td className="px-4 py-2 dark:text-white border">
                        {r.user}
                      </td>
                      <td className="px-4 py-2 dark:text-white border">
                        {r.consumption}
                      </td>
                      <td
                        className={`px-4 py-2 border ${
                          r.status === 'Active'
                            ? 'text-green-600'
                            : 'text-red-600'
                        }`}
                      >
                        {r.status}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>

              <div className="flex justify-between items-center text-sm text-gray-700 mt-4">
                <div>
                  Showing {(currentPage - 1) * perPage + 1}–
                  {Math.min(tableData.length, currentPage * perPage)} of{' '}
                  {tableData.length}
                </div>
                <div className="flex gap-2">
                  <button
                    disabled={currentPage === 1}
                    onClick={() => setCurrentPage((p) => p - 1)}
                    className={`px-3 py-1 bg-white text-black border rounded ${
                      currentPage === 1
                        ? 'opacity-50 cursor-not-allowed'
                        : 'hover:bg-gray-100'
                    }`}
                  >
                    ← Prev
                  </button>
                  <span className="px-3 py-1 border rounded bg-white">
                    {currentPage}
                  </span>
                  <button
                    disabled={currentPage === totalPages}
                    onClick={() => setCurrentPage((p) => p + 1)}
                    className={`px-3 py-1 bg-white text-black border rounded ${
                      currentPage === totalPages
                        ? 'opacity-50 cursor-not-allowed'
                        : 'hover:bg-gray-100'
                    }`}
                  >
                    Next →
                  </button>
                </div>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
