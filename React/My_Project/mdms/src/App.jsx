// src/App.jsx
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
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

import ProtectedRoute from './components/layout/ProtectedRoute';
import { authService } from './services/authService';

function App() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    // small loader for splash effect
    setTimeout(() => {
      setLoading(false);
    }, 400);
  }, []);

  if (loading) {
    return <Loading />;
  }

  if (error === '500') return <InternalServerError />;
  if (error === 'maintenance') return <Maintenance />;
  if (error === 'access-denied') return <AccessDenied />;

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/reset-password" element={<ResetPassword />} />

        {/* EndUser protected routes */}
        <Route
          path="/enduser/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/bills-payments"
          element={
            <ProtectedRoute>
              <BillsPayments />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/meter-data"
          element={
            <ProtectedRoute>
              <MeterData />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/alerts"
          element={
            <ProtectedRoute>
              <AlertsNotifications />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/profile"
          element={
            <ProtectedRoute>
              <ProfileSettings />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enduser/logs"
          element={
            <ProtectedRoute>
              <Logs />
            </ProtectedRoute>
          }
        />

        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}

export default App;