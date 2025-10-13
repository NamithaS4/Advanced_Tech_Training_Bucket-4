import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import I18n from './i18n.js';
import App from './App.jsx';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>
);
