import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { AppRouter } from './routes/AppRouter';
import { Toaster } from 'react-hot-toast';

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Toaster
          position="top-right"
          toastOptions={{
            duration: 4000,
            style: {
              background: '#0f172a',
              color: '#f8fafc',
              border: '1px solid #334155',
              fontSize: '12px',
              borderRadius: '12px',
            },
          }}
        />
        <AppRouter />
      </AuthProvider>
    </BrowserRouter>
  );
}
