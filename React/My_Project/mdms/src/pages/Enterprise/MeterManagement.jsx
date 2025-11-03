import React, { useEffect, useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EnterpriseSidebar from '../../components/layout/Sidebar/EnterpriseSidebar';
import { MoreVertical } from 'lucide-react';
import { enterpriseDataService } from '../../services/enterpriseDataService';
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';

export default function MeterManagement() {
  const [meters, setMeters] = useState([]);
  const [filterBy, setFilterBy] = useState('zone');
  const [query, setQuery] = useState('');
  const [activeMenu, setActiveMenu] = useState(null);
  const [editingId, setEditingId] = useState(null);
  const [editModel, setEditModel] = useState({});
  const [currentPage, setCurrentPage] = useState(1);
  const perPage = 10;

  useEffect(() => {
    loadMeters();

    const handleClickOutside = (e) => {
      if (!e.target.closest('.menu-container')) setActiveMenu(null);
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  useEffect(() => {
    window.dispatchEvent(
      new CustomEvent('resourceVisited', { detail: 'Meter management' })
    );
  }, []);

  function loadMeters() {
    const data = enterpriseDataService.getEnterpriseMeters();
    setMeters(data);
  }

  const filteredMeters = meters.filter((m) => {
    if (!query) return true;
    const value = (m[filterBy] || '').toString().toLowerCase();
    return value.includes(query.toLowerCase());
  });

  const totalPages = Math.max(1, Math.ceil(filteredMeters.length / perPage));
  const startIndex = (currentPage - 1) * perPage;
  const pageMeters = filteredMeters.slice(startIndex, startIndex + perPage);

  const toggleMenu = (id) => setActiveMenu(activeMenu === id ? null : id);

  const handleView = (m) => {
    alert(JSON.stringify(m, null, 2));
    setActiveMenu(null);
  };

  const onEdit = (m) => {
    setEditingId(m.id);
    setEditModel({ ...m });
    setActiveMenu(null);
  };

  const onSaveEdit = () => {
    enterpriseDataService.updateMeter(editingId, editModel);
    setEditingId(null);
    loadMeters();
  };

  const onCancelEdit = () => {
    setEditingId(null);
    setEditModel({});
  };

  const onDelete = (id) => {
    if (window.confirm('Are you sure you want to delete this meter?')) {
      enterpriseDataService.deleteMeter(id);
      loadMeters();
    }
  };

  const chartData = enterpriseDataService.getChartData();

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <EnterpriseSidebar />

        <main className="flex-1 p-6">
          <div className="max-w-6xl mx-auto">
            <h1 className="text-xl font-semibold text-gray-900 dark:text-white mb-6">
              Global Meter Management
            </h1>

            <div className="flex items-center gap-3 mb-4">
              <select
                value={filterBy}
                onChange={(e) => setFilterBy(e.target.value)}
                className="border rounded px-3 py-2 bg-white text-sm"
              >
                <option value="zone">Zone</option>
                <option value="owner">Owner</option>
                <option value="status">Status</option>
                <option value="id">Meter ID</option>
              </select>

              <div className="flex items-center border rounded px-2 py-1 w-96 bg-white">
                <input
                  type="text"
                  placeholder={`Search by ${filterBy}`}
                  value={query}
                  onChange={(e) => {
                    setQuery(e.target.value);
                    setCurrentPage(1);
                  }}
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
                  {pageMeters.length === 0 ? (
                    <tr>
                      <td
                        colSpan="6"
                        className="py-6 text-center text-gray-500 dark:text-gray-300"
                      >
                        No meters found.
                      </td>
                    </tr>
                  ) : (
                    pageMeters.map((m) => (
                      <tr
                        key={m.id}
                        className="border-t text-black dark:text-white border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
                      >
                        <td className="px-4 py-2 text-sm">{m.id}</td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === m.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-32"
                              value={editModel.zone}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  zone: e.target.value,
                                })
                              }
                            />
                          ) : (
                            m.zone
                          )}
                        </td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === m.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-32"
                              value={editModel.owner}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  owner: e.target.value,
                                })
                              }
                            />
                          ) : (
                            m.owner
                          )}
                        </td>
                        <td
                          className={`px-4 py-2 text-sm font-semibold ${
                            m.status === 'Active'
                              ? 'text-green-600'
                              : 'text-red-600'
                          }`}
                        >
                          {m.status}
                        </td>
                        <td className="px-4 py-2 text-sm">{m.lastReading}</td>

                        <td className="px-4 py-2 text-sm relative menu-container">
                          {editingId === m.id ? (
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
                                onClick={() => toggleMenu(m.id)}
                                className="p-1 bg-transparent"
                              >
                                <MoreVertical size={16} />
                              </button>

                              {activeMenu === m.id && (
                                <div className="absolute right-0 mt-2 w-32 bg-white dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded shadow z-20 text-sm">
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-black text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => handleView(m)}
                                  >
                                    View
                                  </button>
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-black text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onEdit(m)}
                                  >
                                    Edit
                                  </button>
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-red-600 text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onDelete(m.id)}
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
                  )}
                </tbody>
              </table>
            </div>

            <div className="flex justify-between items-center text-sm text-gray-700 dark:text-gray-300 mt-4">
              <div>
                Showing {Math.min(filteredMeters.length, startIndex + 1)}–
                {Math.min(
                  filteredMeters.length,
                  startIndex + pageMeters.length
                )}{' '}
                of {filteredMeters.length}
              </div>
              <div className="flex items-center gap-2">
                <button
                  disabled={currentPage === 1}
                  onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                  className={`${
                    currentPage === 1 ? 'opacity-50 cursor-not-allowed' : ''
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
                    currentPage === totalPages
                      ? 'opacity-50 cursor-not-allowed'
                      : ''
                  } bg-white text-black`}
                >
                  Next →
                </button>
              </div>
            </div>

            <div className="mt-8 bg-white dark:bg-gray-800 p-6 rounded-lg border border-gray-200 dark:border-gray-700">
              <h2 className="text-md font-semibold text-gray-800 dark:text-white mb-4">
                Each zone's Trend of energy usage over time
              </h2>

              <div className="w-full min-h-[320px] flex flex-col items-center">
                <ResponsiveContainer width="100%" height={300}>
                  <AreaChart
                    data={chartData}
                    margin={{ top: 10, right: 30, left: 20, bottom: 20 }}
                  >
                    <defs>
                      <linearGradient
                        id="colorUsage"
                        x1="0"
                        y1="0"
                        x2="0"
                        y2="1"
                      >
                        <stop
                          offset="0%"
                          stopColor="#6366F1"
                          stopOpacity={0.4}
                        />
                        <stop
                          offset="100%"
                          stopColor="#6366F1"
                          stopOpacity={0.05}
                        />
                      </linearGradient>
                    </defs>

                    <CartesianGrid
                      strokeDasharray="3 3"
                      vertical={false}
                      stroke="#E5E7EB"
                    />

                    <XAxis
                      dataKey="name"
                      axisLine={false}
                      tickLine={false}
                      interval={0}
                      padding={{ left: 30, right: 30 }}
                      tick={{
                        fill: '#6B7280',
                        fontSize: 12,
                        textAnchor: 'middle',
                        dy: 10,
                      }}
                    />

                    <YAxis
                      axisLine={false}
                      tickLine={false}
                      tick={{ fill: '#6B7280', fontSize: 12 }}
                      domain={[0, 'dataMax + 20']}
                    />

                    <Tooltip
                      contentStyle={{
                        backgroundColor: 'white',
                        border: '1px solid #E5E7EB',
                        borderRadius: '8px',
                        fontSize: '12px',
                        padding: '6px 10px',
                      }}
                    />

                    <Area
                      type="monotone"
                      dataKey="usage"
                      stroke="#6366F1"
                      strokeWidth={2.5}
                      fill="url(#colorUsage)"
                      dot={{
                        r: 4,
                        fill: '#fff',
                        stroke: '#6366F1',
                        strokeWidth: 1.5,
                      }}
                      activeDot={{ r: 6, fill: '#6366F1' }}
                    />
                  </AreaChart>
                </ResponsiveContainer>

                <div className="mt-2 text-sm text-gray-500 dark:text-gray-300 text-center">
                  <span className="inline-flex items-center gap-1 text-indigo-500">
                    <span className="w-2 h-2 rounded-full bg-indigo-500"></span>{' '}
                    2025
                  </span>
                </div>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
