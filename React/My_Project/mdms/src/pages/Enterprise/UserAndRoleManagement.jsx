import React, { useEffect, useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EnterpriseSidebar from '../../components/layout/Sidebar/EnterpriseSidebar';
import { enterpriseDataService } from '../../services/enterpriseDataService';
import {
  ComposedChart,
  Bar,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Cell,
} from 'recharts';

export default function UserAndRoleManagement() {
  const [users, setUsers] = useState([]);
  const [filterBy, setFilterBy] = useState('name');
  const [query, setQuery] = useState('');
  const [activeMenu, setActiveMenu] = useState(null);
  const [editingId, setEditingId] = useState(null);
  const [editModel, setEditModel] = useState({});
  const [inviteOpen, setInviteOpen] = useState(false);
  const [inviteData, setInviteData] = useState({
    name: '',
    email: '',
    role: 'Viewer',
    zone: '',
    status: 'De-Activated',
    year: new Date().getFullYear(),
  });
  const [currentPage, setCurrentPage] = useState(1);
  const [chartYear, setChartYear] = useState(new Date().getFullYear());
  const perPage = 10;

  useEffect(() => {
    loadUsers();
    const handleClickOutside = (e) => {
      if (!e.target.closest('.menu-container')) setActiveMenu(null);
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  useEffect(() => {
    window.dispatchEvent(
      new CustomEvent('resourceVisited', { detail: 'User & Role management' })
    );
  }, []);

  function loadUsers() {
    const data = enterpriseDataService.getUsers();
    setUsers(data);
  }

  const filtered = users.filter((u) => {
    if (!query) return true;
    const value = (u[filterBy] || '').toString().toLowerCase();
    return value.includes(query.toLowerCase());
  });

  const totalPages = Math.max(1, Math.ceil(filtered.length / perPage));
  const startIndex = (currentPage - 1) * perPage;
  const pageUsers = filtered.slice(startIndex, startIndex + perPage);

  const toggleMenu = (id) => setActiveMenu(activeMenu === id ? null : id);

  const handleView = (u) => {
    alert(JSON.stringify(u, null, 2));
    setActiveMenu(null);
  };

  const onEdit = (u) => {
    setEditingId(u.id);
    setEditModel({ ...u });
    setActiveMenu(null);
  };

  const onSaveEdit = () => {
    enterpriseDataService.updateUser(editingId, editModel);
    setEditingId(null);
    setEditModel({});
    loadUsers();
  };

  const onCancelEdit = () => {
    setEditingId(null);
    setEditModel({});
  };

  const onDelete = (id) => {
    if (window.confirm('Are you sure you want to delete this user?')) {
      enterpriseDataService.deleteUser(id);
      loadUsers();
    }
  };

  const exportToCSV = () => {
    if (users.length === 0) {
      alert('No users to export.');
      return;
    }

    const csvHeaders = [
      'User ID',
      'Name',
      'Email',
      'Role',
      'Zone',
      'Status',
      'Year',
    ];
    const csvRows = users.map((u) =>
      [u.id, u.name, u.email, u.role, u.zone, u.status, u.year].join(',')
    );

    const csvString = [csvHeaders.join(','), ...csvRows].join('\n');
    const blob = new Blob([csvString], { type: 'text/csv;charset=utf-8;' });

    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `user_role_management_${chartYear}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const openInvite = () => setInviteOpen(true);
  const closeInvite = () => setInviteOpen(false);

  const submitInvite = (e) => {
    e.preventDefault();
    enterpriseDataService.addUser(inviteData);
    loadUsers();
    closeInvite();
  };

  const usersChart = enterpriseDataService.getUsersCountsByYear(chartYear);
  const availableYears = enterpriseDataService.getUsersAvailableYears();

  const prevYear = () => {
    const idx = availableYears.indexOf(Number(chartYear));
    if (idx > 0) setChartYear(availableYears[idx - 1]);
  };

  const nextYear = () => {
    const idx = availableYears.indexOf(Number(chartYear));
    if (idx < availableYears.length - 1) setChartYear(availableYears[idx + 1]);
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
                User and Role Management
              </h1>
              <div className="flex items-center gap-3">
                <button
                  onClick={exportToCSV}
                  className="bg-transparent text-black dark:text-white border-black dark:border-white px-4 py-2 rounded-md text-sm"
                >
                  Export as CSV
                </button>
                <button
                  onClick={openInvite}
                  className="bg-transparent text-black dark:text-white border-black dark:border-white px-4 py-2 rounded-md text-sm"
                >
                  Invite user
                </button>
              </div>
            </div>

            <div className="flex items-center gap-3 mb-4">
              <select
                value={filterBy}
                onChange={(e) => setFilterBy(e.target.value)}
                className="border rounded px-3 py-2 bg-white text-sm"
              >
                <option value="name">Name</option>
                <option value="email">Email</option>
                <option value="role">Role</option>
                <option value="zone">Zone</option>
                <option value="status">Status</option>
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
                      User ID
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Name
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Email
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Role
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Status
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      More Actions
                    </th>
                  </tr>
                </thead>

                <tbody>
                  {pageUsers.length === 0 ? (
                    <tr>
                      <td
                        colSpan="6"
                        className="py-6 text-center text-gray-500 dark:text-gray-300"
                      >
                        No users found.
                      </td>
                    </tr>
                  ) : (
                    pageUsers.map((u) => (
                      <tr
                        key={u.id}
                        className="border-t text-black dark:text-white border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
                      >
                        <td className="px-4 py-2 text-sm">{u.id}</td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === u.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-32"
                              value={editModel.name}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  name: e.target.value,
                                })
                              }
                            />
                          ) : (
                            u.name
                          )}
                        </td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === u.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-48"
                              value={editModel.email}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  email: e.target.value,
                                })
                              }
                            />
                          ) : (
                            u.email
                          )}
                        </td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === u.id ? (
                            <select
                              className="border rounded px-2 py-1 text-sm"
                              value={editModel.role}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  role: e.target.value,
                                })
                              }
                            >
                              <option>Admin</option>
                              <option>Support</option>
                              <option>Viewer</option>
                            </select>
                          ) : (
                            u.role
                          )}
                        </td>
                        <td
                          className={`px-4 py-2 text-sm font-semibold ${
                            u.status === 'Active'
                              ? 'text-green-600'
                              : 'text-red-600'
                          }`}
                        >
                          {editingId === u.id ? (
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
                            u.status
                          )}
                        </td>

                        <td className="px-4 py-2 text-sm relative menu-container">
                          {editingId === u.id ? (
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
                                onClick={() => toggleMenu(u.id)}
                                className="p-1 bg-transparent"
                              >
                                ⋮
                              </button>

                              {activeMenu === u.id && (
                                <div className="absolute right-0 mt-2 w-32 bg-white dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded shadow z-20 text-sm">
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-black text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => handleView(u)}
                                  >
                                    View
                                  </button>
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-black text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onEdit(u)}
                                  >
                                    Edit
                                  </button>
                                  <button
                                    className="block w-full px-4 py-2 bg-white text-red-600 text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onDelete(u.id)}
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
                Showing {Math.min(filtered.length, startIndex + 1)}–
                {Math.min(filtered.length, startIndex + pageUsers.length)} of{' '}
                {filtered.length}
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

            <div className="mt-8 bg-white dark:bg-gray-800 p-6 rounded-lg border border-gray-200 dark:border-gray-700 shadow-md">
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-sm font-semibold text-gray-700 dark:text-gray-200">
                  Comparison between Active and De-Active users on each year
                </h2>
                <div className="flex items-center gap-2">
                  <button
                    onClick={prevYear}
                    className="px-3 py-1 border rounded bg-gray-200 text-black hover:bg-gray-300"
                  >
                    ←
                  </button>
                  <div className="px-3 py-1 border rounded bg-white text-black font-medium">
                    {chartYear}
                  </div>
                  <button
                    onClick={nextYear}
                    className="px-3 py-1 border rounded bg-gray-200 text-black hover:bg-gray-300"
                  >
                    →
                  </button>
                </div>
              </div>

              <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-4 w-full">
                <ResponsiveContainer width="100%" height={300}>
                  <ComposedChart
                    data={usersChart}
                    margin={{ top: 20, right: 30, left: 10, bottom: 10 }}
                  >
                    <defs>
                      <linearGradient
                        id="curveShade"
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
                          stopOpacity={0.1}
                        />
                      </linearGradient>
                    </defs>

                    <CartesianGrid strokeDasharray="3 3" vertical={false} />
                    <XAxis
                      dataKey="category"
                      axisLine={false}
                      tickLine={false}
                      tick={{ fill: '#6B7280', fontSize: 12 }}
                    />
                    <YAxis
                      domain={[0, 100]}
                      tick={{ fill: '#6B7280', fontSize: 12 }}
                    />
                    <Tooltip
                      contentStyle={{
                        backgroundColor: 'white',
                        border: '1px solid #E5E7EB',
                        borderRadius: '8px',
                        fontSize: '12px',
                      }}
                    />

                    <Bar dataKey="value" barSize={80} radius={[40, 40, 40, 40]}>
                      {usersChart.map((entry, index) => (
                        <Cell
                          key={`cell-${index}`}
                          fill={
                            entry.category === 'Active' ? '#7C3AED' : '#EF4444'
                          }
                        />
                      ))}
                    </Bar>

                    <Area
                      type="basis"
                      dataKey="value"
                      stroke="#6366F1"
                      strokeWidth={2.5}
                      fill="url(#curveShade)"
                      fillOpacity={1}
                      dot={{
                        r: 5,
                        stroke: '#6366F1',
                        strokeWidth: 2,
                        fill: '#fff',
                      }}
                      activeDot={{ r: 7 }}
                      connectNulls={true}
                    />
                  </ComposedChart>
                </ResponsiveContainer>
              </div>

              <div className="text-center text-sm text-gray-600 mt-2">
                ⎯⎯ {chartYear}
              </div>
            </div>
          </div>
        </main>
      </div>

      {inviteOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
          <div
            className="absolute inset-0 bg-black opacity-30"
            onClick={closeInvite}
          ></div>
          <div className="relative bg-white dark:bg-gray-800 rounded-lg shadow-lg w-full max-w-lg p-6 z-10">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-lg dark:text-gray-300 font-semibold">
                Invite user
              </h3>
              <button
                onClick={closeInvite}
                className="bg-white border-black text-gray-600"
              >
                ✕
              </button>
            </div>

            <form onSubmit={submitInvite} className="space-y-4">
              <div>
                <label className="text-sm dark:text-gray-300 block mb-1">
                  Name
                </label>
                <input
                  className="w-full bg-gray-300 border rounded px-3 py-2"
                  value={inviteData.name}
                  onChange={(e) =>
                    setInviteData({ ...inviteData, name: e.target.value })
                  }
                  required
                />
              </div>
              <div>
                <label className="text-sm dark:text-gray-300 block mb-1">
                  Email
                </label>
                <input
                  type="email"
                  className="w-full bg-gray-300 border rounded px-3 py-2"
                  value={inviteData.email}
                  onChange={(e) =>
                    setInviteData({ ...inviteData, email: e.target.value })
                  }
                  required
                />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="text-sm dark:text-gray-300 block mb-1">
                    Role
                  </label>
                  <select
                    className="w-full bg-gray-300 border rounded px-3 py-2"
                    value={inviteData.role}
                    onChange={(e) =>
                      setInviteData({ ...inviteData, role: e.target.value })
                    }
                  >
                    <option>Admin</option>
                    <option>Support</option>
                    <option>Viewer</option>
                  </select>
                </div>
                <div>
                  <label className="text-sm dark:text-gray-300 block mb-1">
                    Zone
                  </label>
                  <input
                    className="w-full bg-gray-300 border rounded px-3 py-2"
                    value={inviteData.zone}
                    onChange={(e) =>
                      setInviteData({ ...inviteData, zone: e.target.value })
                    }
                  />
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={closeInvite}
                  className="px-4 py-2 bg-black text-white border rounded"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-black text-white rounded"
                >
                  Invite user
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
