import React, { useEffect, useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';

export default function Logs() {
  const [logs, setLogs] = useState([]);
  const [filter, setFilter] = useState('all');
  const [currentPage, setCurrentPage] = useState(1);
  const perPage = 10;

  useEffect(() => {
    const currentUser =
      JSON.parse(localStorage.getItem('mdms_current_user')) ||
      JSON.parse(sessionStorage.getItem('mdms_current_user'));

    if (!currentUser || currentUser.role !== 'enduser') {
      setLogs([]);
      return;
    }

    const storedLogs = JSON.parse(localStorage.getItem('userLogs')) || [];

    const endUserLogs = storedLogs
      .filter((log) => log.email === currentUser.email)
      .sort(
        (a, b) =>
          new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
      );

    setLogs(endUserLogs);
  }, []);

  const clearLogs = () => {
    const currentUser =
      JSON.parse(localStorage.getItem('mdms_current_user')) ||
      JSON.parse(sessionStorage.getItem('mdms_current_user'));

    if (!currentUser) return;

    const storedLogs = JSON.parse(localStorage.getItem('userLogs')) || [];
    const updatedLogs = storedLogs.filter(
      (log) => log.email !== currentUser.email
    );
    localStorage.setItem('userLogs', JSON.stringify(updatedLogs));
    setLogs([]);
  };

  const filteredLogs =
    filter === 'all'
      ? logs
      : logs.filter((l) =>
          l.status.toLowerCase().includes(filter.toLowerCase())
        );

  const totalPages = Math.max(1, Math.ceil(filteredLogs.length / perPage));
  const startIndex = (currentPage - 1) * perPage;
  const pageLogs = filteredLogs.slice(startIndex, startIndex + perPage);

  const handlePrev = () => setCurrentPage((prev) => Math.max(1, prev - 1));
  const handleNext = () =>
    setCurrentPage((prev) => Math.min(totalPages, prev + 1));

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <div className="max-w-6xl mx-auto">
            <h1 className="text-xl font-semibold mb-6 dark:text-white">
              User Activity Logs
            </h1>

            <div className="flex justify-between mb-4">
              <select
                value={filter}
                onChange={(e) => {
                  setFilter(e.target.value);
                  setCurrentPage(1);
                }}
                className="border border-gray-300 rounded-md px-3 py-2 text-sm bg-white dark:bg-gray-800 dark:text-white"
              >
                <option value="all">All</option>
                <option value="login">Login</option>
                <option value="password">Password Reset</option>
                <option value="failed">Failed</option>
              </select>

              <button
                onClick={clearLogs}
                className="bg-red-500 text-white px-4 py-2 rounded-md text-sm hover:bg-red-600"
              >
                Clear Logs
              </button>
            </div>

            <div className="overflow-x-auto rounded-lg shadow border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
              <table className="min-w-full">
                <thead className="bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-200">
                  <tr>
                    {['Timestamp', 'User', 'Email', 'Zone', 'Status'].map(
                      (h) => (
                        <th
                          key={h}
                          className="py-3 px-4 text-left text-sm font-semibold"
                        >
                          {h}
                        </th>
                      )
                    )}
                  </tr>
                </thead>

                <tbody>
                  {pageLogs.length === 0 ? (
                    <tr>
                      <td
                        colSpan="5"
                        className="py-6 text-center text-gray-500 dark:text-gray-400"
                      >
                        No logs found.
                      </td>
                    </tr>
                  ) : (
                    pageLogs.map((log, index) => (
                      <tr
                        key={index}
                        className="border-t border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
                      >
                        <td className="px-4 py-2 text-sm text-gray-800 dark:text-gray-200">
                          {log.timestamp}
                        </td>
                        <td className="px-4 py-2 text-sm text-gray-800 dark:text-gray-200">
                          {log.user}
                        </td>
                        <td
                          className={`px-4 py-2 text-sm ${
                            log.status === 'Login Failed' ||
                            log.status === 'Password Reset'
                              ? 'text-red-500'
                              : 'text-green-500'
                          }`}
                        >
                          {log.email}
                        </td>
                        <td className="px-4 py-2 text-sm text-gray-800 dark:text-gray-200">
                          {log.zone || '-'}
                        </td>
                        <td
                          className={`px-4 py-2 text-sm font-medium ${
                            log.status === 'Login Failed' ||
                            log.status === 'Password Reset'
                              ? 'text-red-500'
                              : 'text-green-500'
                          }`}
                        >
                          {log.status}
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>

            <div className="flex justify-between items-center text-sm text-gray-700 dark:text-gray-300 mt-4">
              <div>
                Showing {Math.min(filteredLogs.length, startIndex + 1)}–
                {Math.min(filteredLogs.length, startIndex + pageLogs.length)} of{' '}
                {filteredLogs.length}
              </div>

              <div className="flex items-center gap-2">
                <button
                  disabled={currentPage === 1}
                  onClick={handlePrev}
                  className={`${
                    currentPage === 1
                      ? 'opacity-50 cursor-not-allowed'
                      : 'hover:bg-gray-200'
                  } bg-white text-black border rounded px-3 py-1`}
                >
                  ← Previous
                </button>
                <div className="px-3 py-1 border rounded bg-white text-black">
                  {currentPage}
                </div>
                <button
                  disabled={currentPage === totalPages}
                  onClick={handleNext}
                  className={`${
                    currentPage === totalPages
                      ? 'opacity-50 cursor-not-allowed'
                      : 'hover:bg-gray-200'
                  } bg-white text-black border rounded px-3 py-1`}
                >
                  Next →
                </button>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
