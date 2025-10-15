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

function App() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    setTimeout(() => {
      setLoading(false);
    }, 1500);
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
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}

export default App;
