// ProtectedOutlet.js
import { Outlet, Navigate } from 'react-router-dom';


const ProtectedOutlet = () => {
  const token = localStorage.getItem('authToken');
  if (!token) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};

export default ProtectedOutlet;