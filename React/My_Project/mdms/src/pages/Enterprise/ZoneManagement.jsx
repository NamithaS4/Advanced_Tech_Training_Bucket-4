import React, { useEffect, useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EnterpriseSidebar from '../../components/layout/Sidebar/EnterpriseSidebar';
import { MoreVertical } from 'lucide-react';
import { enterpriseDataService } from '../../services/enterpriseDataService';

export default function ZoneManagement() {
  const [zones, setZones] = useState([]);
  const [filterBy, setFilterBy] = useState('zoneName');
  const [query, setQuery] = useState('');
  const [activeMenu, setActiveMenu] = useState(null);
  const [editingId, setEditingId] = useState(null);
  const [editModel, setEditModel] = useState({});
  const [addOpen, setAddOpen] = useState(false);
  const [addData, setAddData] = useState({
    zoneName: '',
    admin: '',
    location: '',
    description: '',
  });
  const [currentPage, setCurrentPage] = useState(1);
  const perPage = 10;

  useEffect(() => {
    loadZones();

    const handleClickOutside = (e) => {
      if (!e.target.closest('.menu-container')) {
        setActiveMenu(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  useEffect(() => {
    window.dispatchEvent(
      new CustomEvent('resourceVisited', { detail: 'Zone management' })
    );
  }, []);

  function loadZones() {
    const data = enterpriseDataService.getEnterpriseZones();
    setZones(data);
  }

  const filtered = zones.filter((z) => {
    if (!query) return true;
    const field = (z[filterBy] || '').toString().toLowerCase();
    return field.includes(query.toLowerCase());
  });

  const totalPages = Math.max(1, Math.ceil(filtered.length / perPage));
  const startIndex = (currentPage - 1) * perPage;
  const pageZones = filtered.slice(startIndex, startIndex + perPage);

  const toggleMenu = (id) => {
    setActiveMenu(activeMenu === id ? null : id);
  };

  const onEdit = (z) => {
    setEditingId(z.id);
    setEditModel({ ...z });
    setActiveMenu(null);
  };

  const onSaveEdit = () => {
    enterpriseDataService.updateZone(editingId, editModel);
    setEditingId(null);
    loadZones();
  };

  const onCancelEdit = () => {
    setEditingId(null);
    setEditModel({});
  };

  const onDelete = (id) => {
    if (window.confirm('Are you sure you want to delete this zone?')) {
      enterpriseDataService.deleteZone(id);
      loadZones();
    }
  };

  const handleView = (z) => {
    alert(JSON.stringify(z, null, 2));
    setActiveMenu(null);
  };

  const openAdd = () => setAddOpen(true);
  const closeAdd = () => setAddOpen(false);

  const submitAdd = (e) => {
    e.preventDefault();
    if (!addData.zoneName || !addData.admin) {
      alert('Please fill in zone name and admin');
      return;
    }

    enterpriseDataService.addZone({
      zoneName: addData.zoneName,
      admin: addData.admin,
      totalMeters: Math.floor(Math.random() * 50) + 1,
      status: 'De-Activated',
      location: addData.location,
      description: addData.description,
    });

    loadZones();
    setAddOpen(false);
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
                Zone Management
              </h1>
              <button
                onClick={openAdd}
                className="bg-black text-white dark:bg-white dark:text-black px-4 py-2 rounded-md text-sm hover:opacity-90"
              >
                + Add zone
              </button>
            </div>

            <div className="overflow-x-auto rounded-lg shadow border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
              <table className="min-w-full">
                <thead className="bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-200">
                  <tr>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Zone ID
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Zone name
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Admin assigned
                    </th>
                    <th className="py-3 px-4 text-left text-sm font-semibold">
                      Total Meters
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
                  {pageZones.length === 0 ? (
                    <tr>
                      <td
                        colSpan="6"
                        className="py-6 text-center text-gray-500"
                      >
                        No zones found.
                      </td>
                    </tr>
                  ) : (
                    pageZones.map((z) => (
                      <tr
                        key={z.id}
                        className="border-t text-black dark:text-white border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
                      >
                        <td className="px-4 py-2 text-sm">{z.id}</td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === z.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-40"
                              value={editModel.zoneName}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  zoneName: e.target.value,
                                })
                              }
                            />
                          ) : (
                            z.zoneName
                          )}
                        </td>
                        <td className="px-4 py-2 text-sm">
                          {editingId === z.id ? (
                            <input
                              className="border rounded px-2 py-1 text-sm w-36"
                              value={editModel.admin}
                              onChange={(e) =>
                                setEditModel({
                                  ...editModel,
                                  admin: e.target.value,
                                })
                              }
                            />
                          ) : (
                            z.admin
                          )}
                        </td>
                        <td className="px-4 py-2 text-sm">{z.totalMeters}</td>
                        <td
                          className={`px-4 py-2 text-sm font-semibold ${
                            z.status === 'Active'
                              ? 'text-green-600'
                              : 'text-red-600'
                          }`}
                        >
                          {z.status}
                        </td>

                        <td className="px-4 py-2 text-sm relative menu-container">
                          {editingId === z.id ? (
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
                                onClick={() => toggleMenu(z.id)}
                                className="p-1 bg-transparent"
                              >
                                <MoreVertical size={16} />
                              </button>

                              {activeMenu === z.id && (
                                <div className="absolute right-0 mt-2 w-32 bg-white dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded shadow z-20 text-sm">
                                  <button
                                    className="block w-full px-4 py-2 text-left bg-white text-black hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => handleView(z)}
                                  >
                                    View
                                  </button>
                                  <button
                                    className="block w-full px-4 py-2 text-left bg-white text-black hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onEdit(z)}
                                  >
                                    Edit
                                  </button>
                                  <button
                                    className="block w-full px-4 py-2 text-left text-red-600 bg-white hover:bg-gray-100 dark:hover:bg-gray-600"
                                    onClick={() => onDelete(z.id)}
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
                Showing {Math.min(zones.length, startIndex + 1)}–
                {Math.min(zones.length, startIndex + pageZones.length)} of{' '}
                {zones.length}
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
          </div>
        </main>
      </div>

      {addOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
          <div
            className="absolute inset-0 bg-black opacity-30"
            onClick={closeAdd}
          ></div>
          <div className="relative bg-white dark:bg-gray-800 rounded-lg shadow-lg w-full max-w-lg p-6 z-10">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-lg dark:text-gray-300 font-semibold">
                Add zone
              </h3>
              <button
                onClick={closeAdd}
                className="bg-white border-black text-gray-600"
              >
                ✕
              </button>
            </div>

            <form onSubmit={submitAdd} className="space-y-4">
              <div>
                <label className="text-sm dark:text-gray-300 block mb-1">
                  Zone name
                </label>
                <input
                  className="w-full bg-gray-300 border rounded px-3 py-2"
                  value={addData.zoneName}
                  onChange={(e) =>
                    setAddData({ ...addData, zoneName: e.target.value })
                  }
                  required
                />
              </div>

              <div>
                <label className="text-sm dark:text-gray-300 block mb-1">
                  Admin
                </label>
                <input
                  className="w-full border bg-gray-300 rounded px-3 py-2"
                  value={addData.admin}
                  onChange={(e) =>
                    setAddData({ ...addData, admin: e.target.value })
                  }
                  required
                />
              </div>

              <div>
                <label className="text-sm dark:text-gray-300 block mb-1">
                  Location
                </label>
                <input
                  className="w-full border bg-gray-300 rounded px-3 py-2"
                  value={addData.location}
                  onChange={(e) =>
                    setAddData({ ...addData, location: e.target.value })
                  }
                />
              </div>

              <div>
                <label className="text-sm dark:text-gray-300 block mb-1">
                  Description
                </label>
                <input
                  className="w-full border bg-gray-300 rounded px-3 py-2"
                  value={addData.description}
                  onChange={(e) =>
                    setAddData({ ...addData, description: e.target.value })
                  }
                />
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={closeAdd}
                  className="px-4 py-2 bg-black text-white border rounded"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-black text-white rounded"
                >
                  Add zone
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
