import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LoadingSpinner } from '../components/common/LoadingSpinner';

export const ProtectedRoute = ({ roles }) => {
  const { user, token, loading, hasRole } = useAuth();

  if (loading) {
    return <LoadingSpinner fullScreen text="Authenticating..." />;
  }

  if (!token || !user) {
    return <Navigate to="/login" replace />;
  }

  if (roles && !hasRole(roles)) {
    return <Navigate to="/dashboard" replace />;
  }

  return <Outlet />;
};
