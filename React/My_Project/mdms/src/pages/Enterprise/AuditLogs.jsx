import React, { useEffect, useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EnterpriseSidebar from '../../components/layout/Sidebar/EnterpriseSidebar';
import jsPDF from 'jspdf';
import 'jspdf-autotable';

export default function AuditLogs() {
  const [logs, setLogs] = useState([]);
  const [filterBy, setFilterBy] = useState('status');
  const [query, setQuery] = useState('');
  const [activeMenu, setActiveMenu] = useState(null);
  const [editingId, setEditingId] = useState(null);
  const [editModel, setEditModel] = useState({});
  const [currentPage, setCurrentPage] = useState(1);
  const perPage = 10;

  useEffect(() => {
    sessionStorage.setItem('isOnAuditLogs', 'true');
    return () => sessionStorage.removeItem('isOnAuditLogs');
  }, []);

  useEffect(() => {
    const stored = JSON.parse(localStorage.getItem('auditLogs')) || [];
    setLogs(stored);
  }, []);

  useEffect(() => {
    const handleResourceVisited = (event) => {
      const resource = event.detail;
      if (!resource || sessionStorage.getItem('isOnAuditLogs')) return;

      const storedLogs = JSON.parse(localStorage.getItem('auditLogs')) || [];
      const visited =
        JSON.parse(sessionStorage.getItem('sessionResources')) || [];

      if (visited.includes(resource)) return;

      const user = JSON.parse(localStorage.getItem('currentUser')) || {
        name: 'Enterprise User',
      };

      let sessionId = sessionStorage.getItem('sessionId');
      if (!sessionId) {
        sessionId = Date.now().toString();
        sessionStorage.setItem('sessionId', sessionId);
      }

      const newLog = {
        id: Date.now(),
        timestamp: new Date().toISOString(),
        sessionId,
        user: user.name,
        resource,
        status: 'Active',
      };

      const updatedLogs = [newLog, ...storedLogs];
      localStorage.setItem('auditLogs', JSON.stringify(updatedLogs));
      sessionStorage.setItem(
        'sessionResources',
        JSON.stringify([...visited, resource])
      );
      setLogs(updatedLogs);
    };

    const handleLogout = () => {
      const stored = JSON.parse(localStorage.getItem('auditLogs')) || [];
      const updated = stored.map((l) => ({ ...l, status: 'De-Activated' }));
      localStorage.setItem('auditLogs', JSON.stringify(updated));
      setLogs(updated);
      sessionStorage.clear();
    };

    window.addEventListener('resourceVisited', handleResourceVisited);
    window.addEventListener('logout', handleLogout);

    return () => {
      window.removeEventListener('resourceVisited', handleResourceVisited);
      window.removeEventListener('logout', handleLogout);
    };
  }, []);

  const filtered = logs.filter((l) => {
    if (!query) return true;
    const val = (l[filterBy] || '').toString().toLowerCase();
    return val.includes(query.toLowerCase());
  });

  const totalPages = Math.max(1, Math.ceil(filtered.length / perPage));
  const pageLogs = filtered.slice(
    (currentPage - 1) * perPage,
    currentPage * perPage
  );

  const toggleMenu = (id) => setActiveMenu(activeMenu === id ? null : id);

  const handleView = (log) => {
    alert(JSON.stringify(log, null, 2));
    setActiveMenu(null);
  };

  const onEdit = (log) => {
    setEditingId(log.id);
    setEditModel({ ...log });
    setActiveMenu(null);
  };

  const onSaveEdit = () => {
    const updated = logs.map((l) =>
      l.id === editingId ? { ...l, ...editModel } : l
    );
    localStorage.setItem('auditLogs', JSON.stringify(updated));
    setLogs(updated);
    setEditingId(null);
    setEditModel({});
  };

  const onCancelEdit = () => {
    setEditingId(null);
    setEditModel({});
  };

  const onDelete = (id) => {
    if (window.confirm('Are you sure you want to delete this log?')) {
      const updated = logs.filter((l) => l.id !== id);
      localStorage.setItem('auditLogs', JSON.stringify(updated));
      setLogs(updated);
    }
  };

  const exportToCSV = () => {
    if (!logs.length) return alert('No logs to export.');
    const headers = ['ID', 'Timestamp', 'User', 'Resource', 'Status'];
    const rows = logs.map((l) => [
      l.id,
      l.timestamp,
      l.user,
      l.resource,
      l.status,
    ]);
    const csv =
      'data:text/csv;charset=utf-8,' +
      [headers.join(','), ...rows.map((r) => r.join(','))].join('\n');
    const link = document.createElement('a');
    link.href = encodeURI(csv);
    link.download = 'audit_logs.csv';
    link.click();
  };

  const exportToPDF = () => {
    if (!logs.length) return alert('No logs to export.');
    const doc = new jsPDF();
    doc.text('Audit Logs', 14, 15);
    const data = logs.map((l) => [
      l.id,
      l.timestamp,
      l.user,
      l.resource,
      l.status,
    ]);
    doc.autoTable({
      head: [['ID', 'Timestamp', 'User', 'Resource', 'Status']],
      body: data,
      startY: 25,
    });
    doc.save('audit_logs.pdf');
  };

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <EnterpriseSidebar />

        <main className="flex-1 p-6">
          <div className="max-w-6xl mx-auto">
            <div className="flex justify-between items-center mb-6">
              <h1 className="text-xl font-semibold text-gray-900 dark:text-white">
                Audit Logs
              </h1>
              <div className="flex gap-3">
                <button
                  onClick={exportToCSV}
                  className="border border-black dark:borger-white text-black dark:text-white bg-transparent px-4 py-2 rounded text-sm"
                >
                  Export as CSV
                </button>
                <button
                  onClick={exportToPDF}
                  className="border border-black dark:borger-white text-black dark:text-white bg-transparent px-4 py-2 rounded text-sm"
                >
                  Export as PDF
                </button>
              </div>
            </div>

            <div className="flex items-center gap-3 mb-4">
              <select
                value={filterBy}
                onChange={(e) => setFilterBy(e.target.value)}
                className="border rounded px-3 py-2 bg-white text-sm"
              >
                <option value="status">Status</option>
                <option value="resource">Resource</option>
                <option value="user">User</option>
              </select>

              <div className="flex items-center border rounded px-2 py-1 w-96 bg-white">
                <input
                  type="text"
                  placeholder={`Search by ${filterBy}`}
                  value={query}
                  onChange={(e) => setQuery(e.target.value)}
                  className="w-full py-1 bg-white outline-none text-sm"
                />
                {query && (
                  <button
                    onClick={() => setQuery('')}
                    className="bg-gray-300 text-sm px-2"
                  >
                    ✕
                  </button>
                )}
              </div>
            </div>

            <div className="overflow-x-auto rounded-lg shadow border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
              <table className="min-w-full">
                <thead className="bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-200">
                  <tr>
                    {[
                      'ID',
                      'Timestamp',
                      'User',
                      'Resource',
                      'Status',
                      'More Actions',
                    ].map((h) => (
                      <th
                        key={h}
                        className="py-3 px-4 text-left text-sm font-semibold"
                      >
                        {h}
                      </th>
                    ))}
                  </tr>
                </thead>

                <tbody>
                  {pageLogs.length ? (
                    pageLogs.map((l) => (
                      <tr
                        key={l.id}
                        className="border-t border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
                      >
                        <td className="px-4 py-2 text-sm">{l.id}</td>
                        <td className="px-4 py-2 text-sm">{l.timestamp}</td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === l.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-32"
                              value={editModel.user}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  user: e.target.value,
                                })
                              }
                            />
                          ) : (
                            l.user
                          )}
                        </td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === l.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-40"
                              value={editModel.resource}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  resource: e.target.value,
                                })
                              }
                            />
                          ) : (
                            l.resource
                          )}
                        </td>
                        <td
                          className={`px-4 py-2 text-sm font-semibold ${
                            l.status === 'Active'
                              ? 'text-green-600'
                              : 'text-red-600'
                          }`}
                        >
                          {editingId === l.id ? (
                            <select
                              className="border rounded px-2 py-1 text-sm"
                              value={editModel.status}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  status: e.target.value,
                                })
                              }
                            >
                              <option>Active</option>
                              <option>De-Activated</option>
                            </select>
                          ) : (
                            l.status
                          )}
                        </td>

                        <td className="px-4 py-2 text-sm relative menu-container">
                          {editingId === l.id ? (
                            <>
                              <button
                                onClick={onSaveEdit}
                                className="text-green-600 mr-3"
                              >
                                Save
                              </button>
                              <button
                                onClick={onCancelEdit}
                                className="text-gray-700"
                              >
                                Cancel
                              </button>
                            </>
                          ) : (
                            <>
                              <button
                                onClick={() => toggleMenu(l.id)}
                                className="p-1 bg-transparent"
                              >
                                ⋮
                              </button>

                              {activeMenu === l.id && (
                                <div className="absolute right-0 mt-2 w-32 bg-white dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded shadow z-20 text-sm">
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-black text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => handleView(l)}
                                  >
                                    View
                                  </button>
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-black text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onEdit(l)}
                                  >
                                    Edit
                                  </button>
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-red-600 text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onDelete(l.id)}
                                  >
                                    Delete
                                  </button>
                                </div>
                              )}
                            </>
                          )}
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td
                        colSpan="6"
                        className="py-6 text-center text-gray-500"
                      >
                        No logs found.
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            <div className="flex justify-between items-center text-sm text-gray-700 dark:text-gray-300 mt-4">
              <div>
                Showing{' '}
                {Math.min(filtered.length, (currentPage - 1) * perPage + 1)}–
                {Math.min(filtered.length, currentPage * perPage)} of{' '}
                {filtered.length}
              </div>
              <div className="flex items-center gap-2">
                <button
                  disabled={currentPage === 1}
                  onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                  className={`${
                    currentPage === 1 ? 'opacity-50' : ''
                  } bg-white text-black`}
                >
                  ← Previous
                </button>
                <div className="px-3 py-1 border rounded bg-white text-black">
                  {currentPage}
                </div>
                <button
                  disabled={currentPage === totalPages}
                  onClick={() =>
                    setCurrentPage((p) => Math.min(totalPages, p + 1))
                  }
                  className={`${
                    currentPage === totalPages ? 'opacity-50' : ''
                  } bg-white text-black`}
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
