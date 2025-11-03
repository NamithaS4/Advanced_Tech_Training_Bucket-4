const USER_STORAGE_KEY = 'zone_management_users_v3';

const zoneBaseData = {
  zoneName: 'Bangalore South',
  totalMeters: 256,
  avgUsage: 55,
  pendingAlerts: 26,
  weeklyData: [
    { name: 'Day 1', usage: 320 },
    { name: 'Day 2', usage: 180 },
    { name: 'Day 3', usage: 390 },
    { name: 'Day 4', usage: 310 },
    { name: 'Day 5', usage: 330 },
    { name: 'Day 6', usage: 315 },
    { name: 'Day 7', usage: 200 },
  ],
  monthlyData: [
    { name: 'Jan', usage: 350 },
    { name: 'Feb', usage: 280 },
    { name: 'Mar', usage: 420 },
    { name: 'Apr', usage: 400 },
    { name: 'May', usage: 460 },
    { name: 'Jun', usage: 380 },
    { name: 'Jul', usage: 440 },
    { name: 'Aug', usage: 410 },
    { name: 'Sep', usage: 370 },
    { name: 'Oct', usage: 430 },
  ],
};

const DEFAULT_USERS = [
  {
    id: 101,
    name: 'abc',
    email: 'abc@gmail.com',
    role: 'Admin',
    zone: 'Mangalore',
    status: 'Active',
    password: 'pass101',
  },
  {
    id: 102,
    name: 'def',
    email: 'def@gmail.com',
    role: 'Support',
    zone: 'Mangalore',
    status: 'Active',
    password: 'pass102',
  },
  {
    id: 103,
    name: 'xyz',
    email: 'xyz@gmail.com',
    role: 'Viewer',
    zone: 'Bajpe',
    status: 'De-Activated',
    password: 'pass103',
  },
  {
    id: 104,
    name: 'pqr',
    email: 'pqr@gmail.com',
    role: 'Admin',
    zone: 'Bajpe',
    status: 'Active',
    password: 'pass104',
  },
  {
    id: 105,
    name: 'stu',
    email: 'stu@gmail.com',
    role: 'Support',
    zone: 'Kankanady',
    status: 'De-Activated',
    password: 'pass105',
  },
  {
    id: 106,
    name: 'vij',
    email: 'vij@gmail.com',
    role: 'Viewer',
    zone: 'Padubidri',
    status: 'Active',
    password: 'pass106',
  },
  {
    id: 107,
    name: 'raj',
    email: 'raj@gmail.com',
    role: 'Support',
    zone: 'Bajpe',
    status: 'Active',
    password: 'pass107',
  },
  {
    id: 108,
    name: 'sun',
    email: 'sun@gmail.com',
    role: 'Viewer',
    zone: 'Mangalore',
    status: 'De-Activated',
    password: 'pass108',
  },
];

const reportBaseData = {
  trendData: [
    { date: '2025-01-01', usage: 320 },
    { date: '2025-02-01', usage: 410 },
    { date: '2025-03-01', usage: 380 },
    { date: '2025-04-01', usage: 460 },
    { date: '2025-05-01', usage: 440 },
    { date: '2025-06-01', usage: 390 },
    { date: '2025-07-01', usage: 420 },
    { date: '2025-08-01', usage: 450 },
  ],

  zoneConsumption: [
    { user: 'abc', zone: 'Mangalore', year: 2023, consumption: 65 },
    { user: 'abc', zone: 'Bajpe', year: 2023, consumption: 35 },
    { user: 'abc', zone: 'Pumpwell', year: 2023, consumption: 85 },
    { user: 'abc', zone: 'PVS', year: 2023, consumption: 50 },
    { user: 'abc', zone: 'Kottara', year: 2023, consumption: 70 },
    { user: 'abc', zone: 'Mangalore', year: 2024, consumption: 60 },
    { user: 'abc', zone: 'Bajpe', year: 2024, consumption: 25 },
    { user: 'abc', zone: 'Pumpwell', year: 2024, consumption: 80 },
    { user: 'abc', zone: 'PVS', year: 2024, consumption: 45 },
    { user: 'abc', zone: 'Kottara', year: 2024, consumption: 65 },
    { user: 'abc', zone: 'Mangalore', year: 2025, consumption: 70 },
    { user: 'abc', zone: 'Bajpe', year: 2025, consumption: 40 },
    { user: 'abc', zone: 'Pumpwell', year: 2025, consumption: 92 },
    { user: 'abc', zone: 'PVS', year: 2025, consumption: 55 },
    { user: 'abc', zone: 'Kottara', year: 2025, consumption: 72 },

    { user: 'xyz', zone: 'Mangalore', year: 2023, consumption: 50 },
    { user: 'xyz', zone: 'Bajpe', year: 2023, consumption: 30 },
    { user: 'xyz', zone: 'Pumpwell', year: 2023, consumption: 70 },
    { user: 'xyz', zone: 'PVS', year: 2023, consumption: 35 },
    { user: 'xyz', zone: 'Kottara', year: 2023, consumption: 60 },
    { user: 'xyz', zone: 'Mangalore', year: 2024, consumption: 75 },
    { user: 'xyz', zone: 'Bajpe', year: 2024, consumption: 40 },
    { user: 'xyz', zone: 'Pumpwell', year: 2024, consumption: 95 },
    { user: 'xyz', zone: 'PVS', year: 2024, consumption: 55 },
    { user: 'xyz', zone: 'Kottara', year: 2024, consumption: 62 },
    { user: 'xyz', zone: 'Mangalore', year: 2025, consumption: 80 },
    { user: 'xyz', zone: 'Bajpe', year: 2025, consumption: 45 },
    { user: 'xyz', zone: 'Pumpwell', year: 2025, consumption: 100 },
    { user: 'xyz', zone: 'PVS', year: 2025, consumption: 60 },
    { user: 'xyz', zone: 'Kottara', year: 2025, consumption: 70 },

    { user: 'def', zone: 'Mangalore', year: 2023, consumption: 68 },
    { user: 'def', zone: 'Bajpe', year: 2023, consumption: 32 },
    { user: 'def', zone: 'Pumpwell', year: 2023, consumption: 82 },
    { user: 'def', zone: 'PVS', year: 2023, consumption: 50 },
    { user: 'def', zone: 'Kottara', year: 2023, consumption: 65 },
    { user: 'def', zone: 'Mangalore', year: 2024, consumption: 70 },
    { user: 'def', zone: 'Bajpe', year: 2024, consumption: 36 },
    { user: 'def', zone: 'Pumpwell', year: 2024, consumption: 88 },
    { user: 'def', zone: 'PVS', year: 2024, consumption: 52 },
    { user: 'def', zone: 'Kottara', year: 2024, consumption: 68 },
    { user: 'def', zone: 'Mangalore', year: 2025, consumption: 73 },
    { user: 'def', zone: 'Bajpe', year: 2025, consumption: 40 },
    { user: 'def', zone: 'Pumpwell', year: 2025, consumption: 91 },
    { user: 'def', zone: 'PVS', year: 2025, consumption: 58 },
    { user: 'def', zone: 'Kottara', year: 2025, consumption: 72 },
  ],

  reportTable: [
    {
      meterId: 123,
      date: '2025-01-01',
      user: 'abc',
      consumption: '28 kWh',
      status: 'Active',
    },
    {
      meterId: 123,
      date: '2025-02-05',
      user: 'abc',
      consumption: '34 kWh',
      status: 'Active',
    },
    {
      meterId: 124,
      date: '2025-03-07',
      user: 'xyz',
      consumption: '18 kWh',
      status: 'De-Activated',
    },
    {
      meterId: 124,
      date: '2025-04-07',
      user: 'xyz',
      consumption: '20 kWh',
      status: 'Active',
    },
    {
      meterId: 125,
      date: '2025-05-08',
      user: 'pqr',
      consumption: '42 kWh',
      status: 'Active',
    },
    {
      meterId: 126,
      date: '2025-06-09',
      user: 'stu',
      consumption: '37 kWh',
      status: 'De-Activated',
    },
    {
      meterId: 127,
      date: '2025-07-11',
      user: 'vij',
      consumption: '50 kWh',
      status: 'Active',
    },
    {
      meterId: 128,
      date: '2025-08-13',
      user: 'raj',
      consumption: '48 kWh',
      status: 'Active',
    },
  ],
};

function readUsersFromStorage() {
  try {
    const raw = localStorage.getItem(USER_STORAGE_KEY);
    if (!raw) return null;
    const parsed = JSON.parse(raw);
    if (!Array.isArray(parsed)) return null;
    return parsed;
  } catch (e) {
    console.error('Failed to read users from storage', e);
    return null;
  }
}
function writeUsersToStorage(arr) {
  try {
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(arr));
  } catch (e) {
    console.error('Failed to write users to storage', e);
  }
}

export const zoneManagementDataService = {
  getZoneData() {
    return { ...zoneBaseData };
  },

  getChartData(filter = 'week') {
    const d = this.getZoneData();
    return filter === 'month' ? d.monthlyData : d.weeklyData;
  },

  init() {
    const existing = readUsersFromStorage();
    if (!existing) {
      writeUsersToStorage(DEFAULT_USERS);
    }
  },

  getUsers() {
    this.init();
    return readUsersFromStorage() || [];
  },

  getUserById(id) {
    const users = this.getUsers();
    return users.find((u) => Number(u.id) === Number(id)) || null;
  },

  addUser({
    name,
    email,
    role = 'Viewer',
    zone = 'Unknown',
    status = 'De-Activated',
    password = 'changeme',
  }) {
    const users = this.getUsers();
    const maxId = users.reduce((m, u) => Math.max(m, Number(u.id || 0)), 100);
    const newUser = {
      id: maxId + 1,
      name,
      email,
      role,
      zone,
      status,
      password,
    };
    const updated = [newUser, ...users];
    writeUsersToStorage(updated);
    return newUser;
  },

  updateUser(id, changes = {}) {
    const users = this.getUsers();
    const updated = users.map((u) =>
      Number(u.id) === Number(id) ? { ...u, ...changes } : u
    );
    writeUsersToStorage(updated);
    return this.getUserById(id);
  },

  toggleStatus(id) {
    const user = this.getUserById(id);
    if (!user) return null;
    const newStatus = user.status === 'Active' ? 'De-Activated' : 'Active';
    return this.updateUser(id, { status: newStatus });
  },

  resetPassword(id, newPassword = null) {
    const user = this.getUserById(id);
    if (!user) return null;
    const password =
      newPassword || `reset_${Math.random().toString(36).slice(2, 9)}`;
    return this.updateUser(id, { password });
  },

  replaceAllUsers(usersArray) {
    writeUsersToStorage(usersArray);
  },
};

export const zoneReportsData = {
  getTrendData() {
    return reportBaseData.trendData;
  },

  getZoneConsumption(user, year) {
    return reportBaseData.zoneConsumption.filter(
      (d) => d.user === user && d.year === year
    );
  },

  getReportTable() {
    return reportBaseData.reportTable;
  },

  getAvailableUsers() {
    return [...new Set(reportBaseData.zoneConsumption.map((d) => d.user))];
  },

  getAvailableYears() {
    return [...new Set(reportBaseData.zoneConsumption.map((d) => d.year))];
  },
};

zoneManagementDataService.init();
