import React, { useEffect, useState } from 'react';
import Header from '../../components/layout/Header/Header';
import ZoneSidebar from '../../components/layout/Sidebar/ZoneSidebar';
import { MoreVertical } from 'lucide-react';
import { zoneManagementDataService } from '../../services/zoneManagementDataService';

export default function UserManagement() {
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
  });
  const [currentPage, setCurrentPage] = useState(1);
  const perPage = 4;

  useEffect(() => {
    loadUsers();
  }, []);

  function loadUsers() {
    const data = zoneManagementDataService.getUsers();
    setUsers(data);
  }

  const filtered = users.filter((u) => {
    if (!query) return true;
    const field = (u[filterBy] || '').toString().toLowerCase();
    return field.includes(query.toLowerCase());
  });

  const totalPages = Math.max(1, Math.ceil(filtered.length / perPage));
  const startIndex = (currentPage - 1) * perPage;
  const pageUsers = filtered.slice(startIndex, startIndex + perPage);

  const toggleMenu = (id) => setActiveMenu((prev) => (prev === id ? null : id));

  const onEdit = (user) => {
    setEditingId(user.id);
    setEditModel({ ...user });
    setActiveMenu(null);
  };

  const onSaveEdit = () => {
    if (!editModel.name || !editModel.email) {
      alert('Name and Email are required.');
      return;
    }
    zoneManagementDataService.updateUser(editingId, editModel);
    setEditingId(null);
    setEditModel({});
    loadUsers();
  };

  const onCancelEdit = () => {
    setEditingId(null);
    setEditModel({});
  };

  const onToggleActivate = (id) => {
    zoneManagementDataService.toggleStatus(id);
    setActiveMenu(null);
    loadUsers();
  };

  const onResetPassword = (id) => {
    const newPass = `reset_${Math.random().toString(36).slice(2, 9)}`;
    zoneManagementDataService.resetPassword(id, newPass);
    loadUsers();
    setActiveMenu(null);
    alert(`Password reset for user ID ${id}\nNew password: ${newPass}`);
  };

  const openInvite = () => {
    setInviteData({ name: '', email: '', role: 'Viewer', zone: '' });
    setInviteOpen(true);
  };
  const closeInvite = () => setInviteOpen(false);

  const submitInvite = (e) => {
    e.preventDefault();
    if (!inviteData.name || !inviteData.email) {
      alert('Please enter name and email');
      return;
    }
    zoneManagementDataService.addUser({
      name: inviteData.name,
      email: inviteData.email,
      role: inviteData.role || 'Viewer',
      zone: inviteData.zone || 'Unknown',
      status: 'De-Activated',
      password: 'changeme',
    });
    loadUsers();
    setInviteOpen(false);
  };

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <ZoneSidebar />
        <main className="flex-1 p-6">
          <div className="max-w-6xl mx-auto">
            <div className="flex justify-between items-center mb-6">
              <h1 className="text-xl font-semibold text-gray-900 dark:text-white">
                User Management
              </h1>
              <div className="flex items-center gap-3">
                <button
                  onClick={openInvite}
                  className="bg-transparent text-black dark:text-white dark:border-white border-black px-4 py-2 rounded-md text-sm"
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
                  className="w-full bg-white outline-none text-sm"
                />
                {query && (
                  <button onClick={() => setQuery('')} className="text-sm px-2">
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
                      ID
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
                      Zone
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
                        colSpan="7"
                        className="py-6 text-center text-gray-500"
                      >
                        No users found.
                      </td>
                    </tr>
                  ) : (
                    pageUsers.map((u) => (
                      <tr
                        key={u.id}
                        className="border-t border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
                      >
                        <td className="px-4 py-2 dark:text-white text-sm">
                          {u.id}
                        </td>

                        <td className="px-4 py-2 dark:text-white text-sm">
                          {editingId === u.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-40"
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

                        <td className="px-4 py-2 dark:text-white text-sm">
                          {editingId === u.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-56"
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

                        <td className="px-4 py-2 dark:text-white text-sm">
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

                        <td className="px-4 py-2 dark:text-white text-sm">
                          {editingId === u.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-36"
                              value={editModel.zone}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  zone: e.target.value,
                                })
                              }
                            />
                          ) : (
                            u.zone
                          )}
                        </td>

                        <td
                          className={`px-4 py-2 text-sm font-semibold ${u.status === 'Active' ? 'text-green-600' : 'text-red-600'}`}
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

                        <td className="px-4 py-2 text-sm relative">
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
                                className="p-1 bg-transparent dark:text-white"
                              >
                                <MoreVertical size={16} />
                              </button>
                              {activeMenu === u.id && (
                                <div className="absolute right-0 mt-2 w-44 bg-white dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded shadow z-20 text-sm">
                                  <button
                                    className="block w-full bg-white px-4 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onEdit(u)}
                                  >
                                    Edit
                                  </button>
                                  <button
                                    className="block w-full bg-white px-4 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onToggleActivate(u.id)}
                                  >
                                    {u.status === 'Active'
                                      ? 'Deactivate'
                                      : 'Activate'}
                                  </button>
                                  <button
                                    className="block w-full bg-white px-4 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onResetPassword(u.id)}
                                  >
                                    Reset password
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

            <div className="flex justify-between items-center mt-4 text-sm text-gray-700 dark:text-gray-300">
              <div>
                Showing {Math.min(users.length, startIndex + 1)}–
                {Math.min(users.length, startIndex + pageUsers.length)} of{' '}
                {users.length} users
              </div>

              <div className="flex items-center gap-2">
                <button
                  onClick={() =>
                    setCurrentPage((prev) => Math.max(1, prev - 1))
                  }
                  disabled={currentPage === 1}
                  className={`px-3 bg-white text-black py-1 border rounded ${
                    currentPage === 1
                      ? 'opacity-50 cursor-not-allowed'
                      : 'hover:bg-gray-100'
                  }`}
                >
                  ← Prev
                </button>
                <span className="px-3 py-1 text-black border rounded bg-white">
                  {currentPage}
                </span>
                <button
                  onClick={() =>
                    setCurrentPage((prev) => Math.min(totalPages, prev + 1))
                  }
                  disabled={currentPage === totalPages}
                  className={`px-3 bg-white text-black py-1 border rounded ${
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
              <h3 className="text-lg dark:text-white font-semibold">
                Invite user
              </h3>
              <button
                onClick={closeInvite}
                className="bg-white border border-black text-gray-600"
              >
                ✕
              </button>
            </div>

            <form onSubmit={submitInvite} className="space-y-4">
              <div>
                <label className="text-sm dark:text-white block mb-1">
                  Name
                </label>
                <input
                  className="w-full bg-white border border-black rounded px-3 py-2"
                  value={inviteData.name}
                  onChange={(e) =>
                    setInviteData({ ...inviteData, name: e.target.value })
                  }
                  required
                />
              </div>

              <div>
                <label className="text-sm dark:text-white block mb-1">
                  Email
                </label>
                <input
                  type="email"
                  className="w-full bg-white border border-black rounded px-3 py-2"
                  value={inviteData.email}
                  onChange={(e) =>
                    setInviteData({ ...inviteData, email: e.target.value })
                  }
                  required
                />
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="text-sm dark:text-white block mb-1">
                    Role
                  </label>
                  <select
                    className="w-full bg-white border border-black rounded px-3 py-2"
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
                  <label className="text-sm dark:text-white block mb-1">
                    Zone
                  </label>
                  <input
                    className="w-full bg-white border border-black rounded px-3 py-2"
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
                  className="px-4 py-2 bg-white border border-black rounded"
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
