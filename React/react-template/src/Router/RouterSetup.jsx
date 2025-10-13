import { BrowserRouter, Routes, Route } from 'react-router-dom';
import PublicRoutes from './publicRoutes';
import LoginPage from '../pages/LoginPage';
import NotFoundPage from '../pages/NotFoundPage';
import ProtectedLayout from './ProtectedLayout';

export default function RouterSetup() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/*" element={<ProtectedLayout />} />
                <Route path="*" element={<NotFoundPage />} />
            </Routes>
        </BrowserRouter>
    );
}
