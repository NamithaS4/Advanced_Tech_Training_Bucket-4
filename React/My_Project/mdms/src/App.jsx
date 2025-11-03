import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { useState, useEffect } from 'react';

import NotFound from './pages/Utility/NotFound';
import InternalServerError from './pages/Utility/InternalServerError';
import Maintenance from './pages/Utility/Maintenance';
import AccessDenied from './pages/Utility/AccessDenied';
import Loading from './pages/Utility/Loading';

import Login from './pages/Auth/Login';
import ForgotPassword from './pages/Auth/ForgotPassword';
import ResetPassword from './pages/Auth/ResetPassword';

import Dashboard from './pages/EndUser/Dashboard';
import BillsPayments from './pages/EndUser/BillsPayments';
import MeterData from './pages/EndUser/MeterData';
import AlertsNotifications from './pages/EndUser/AlertsNotifications';
import ProfileSettings from './pages/EndUser/ProfileSettings';
import Logs from './pages/EndUser/Logs';

import ZoneDashboard from './pages/Zone/Dashboard';
import MeterManagement from './pages/Zone/MeterManagement';
import UserManagement from './pages/Zone/UserManagement';
import ReportsAnalytics from './pages/Zone/ReportsAnalytics';
import ZoneSettings from './pages/Zone/SettingsNotifications';

import EnterpriseDashboard from './pages/Enterprise/Dashboard';
import ZoneManagement from './pages/Enterprise/ZoneManagement';
import EMeterManagement from './pages/Enterprise/MeterManagement';
import UserAndRoleManagement from './pages/Enterprise/UserAndRoleManagement';
import AuditLogs from './pages/Enterprise/AuditLogs';
import EnterpriseSettings from './pages/Enterprise/SettingsNotifications';

import ProtectedRoute from './components/layout/ProtectedRoute';
import BillDetails from './pages/EndUser/BillDetails';

function App() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

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
    };

    const handleLogout = () => {
      const stored = JSON.parse(localStorage.getItem('auditLogs')) || [];
      const updated = stored.map((l) => ({ ...l, status: 'De-Activated' }));
      localStorage.setItem('auditLogs', JSON.stringify(updated));
      sessionStorage.clear();
    };

    window.addEventListener('resourceVisited', handleResourceVisited);
    window.addEventListener('logout', handleLogout);

    return () => {
      window.removeEventListener('resourceVisited', handleResourceVisited);
      window.removeEventListener('logout', handleLogout);
    };
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => setLoading(false), 400);
    return () => clearTimeout(timer);
  }, []);

  if (loading) return <Loading />;
  if (error === '500') return <InternalServerError />;
  if (error === 'maintenance') return <Maintenance />;
  if (error === 'access-denied') return <AccessDenied />;

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/reset-password" element={<ResetPassword />} />

        {/* EndUser */}
        <Route
          path="/enduser/dashboard"
          element={
            <ProtectedRoute role="enduser">
              <Dashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/bills-payments"
          element={
            <ProtectedRoute role="enduser">
              <BillsPayments />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/meter-data"
          element={
            <ProtectedRoute role="enduser">
              <MeterData />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/alerts"
          element={
            <ProtectedRoute role="enduser">
              <AlertsNotifications />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/profile"
          element={
            <ProtectedRoute role="enduser">
              <ProfileSettings />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/logs"
          element={
            <ProtectedRoute role="enduser">
              <Logs />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/bill/:receiptId"
          element={
            <ProtectedRoute role="enduser">
              <BillDetails />
            </ProtectedRoute>
          }
        />

        {/* Zone */}
        <Route
          path="/zone/dashboard"
          element={
            <ProtectedRoute role="zone">
              <ZoneDashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/zone/meter-management"
          element={
            <ProtectedRoute role="zone">
              <MeterManagement />
            </ProtectedRoute>
          }
        />
        <Route
          path="/zone/user-management"
          element={
            <ProtectedRoute role="zone">
              <UserManagement />
            </ProtectedRoute>
          }
        />
        <Route
          path="/zone/reports"
          element={
            <ProtectedRoute role="zone">
              <ReportsAnalytics />
            </ProtectedRoute>
          }
        />
        <Route
          path="/zone/settings"
          element={
            <ProtectedRoute role="zone">
              <ZoneSettings />
            </ProtectedRoute>
          }
        />

        {/* Enterprise */}
        <Route
          path="/enterprise/dashboard"
          element={
            <ProtectedRoute role="enterprise">
              <EnterpriseDashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enterprise/zone-management"
          element={
            <ProtectedRoute role="enterprise">
              <ZoneManagement />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enterprise/meter-management"
          element={
            <ProtectedRoute role="enterprise">
              <EMeterManagement />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enterprise/user-management"
          element={
            <ProtectedRoute role="enterprise">
              <UserAndRoleManagement />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enterprise/audit-logs"
          element={
            <ProtectedRoute role="enterprise">
              <AuditLogs />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enterprise/settings"
          element={
            <ProtectedRoute role="enterprise">
              <EnterpriseSettings />
            </ProtectedRoute>
          }
        />

        {/* Utility */}
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}

export default App;
