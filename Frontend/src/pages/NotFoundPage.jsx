import React from 'react';
import { Link } from 'react-router-dom';

export const NotFoundPage = () => {
  return (
    <div className="min-h-[70vh] flex flex-col items-center justify-center text-center p-4">
      <h1 className="text-6xl font-extrabold text-indigo-500">404</h1>
      <h2 className="text-2xl font-bold text-white mt-2">Page Not Found</h2>
      <p className="text-slate-400 text-xs mt-1 max-w-sm">
        The page you are looking for does not exist or has been moved.
      </p>
      <Link
        to="/dashboard"
        className="mt-6 px-5 py-2.5 bg-indigo-600 hover:bg-indigo-500 text-white font-bold text-xs rounded-xl shadow-lg shadow-indigo-600/30 transition-all"
      >
        Back to Dashboard
      </Link>
    </div>
  );
};
