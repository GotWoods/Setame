// ProtectedOutlet.js
import { Outlet, Navigate } from 'react-router-dom';
import SettingsClient from '../clients/settingsClient';


const ProtectedOutlet = () => {
  const settingsClient = new SettingsClient();
  let token = localStorage.getItem('authToken');

  if (settingsClient.isJwtTokenExpired())
  {
    token = null;
  }

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};

export default ProtectedOutlet;