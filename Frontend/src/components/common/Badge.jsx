import React from 'react';

export const Badge = ({ children, variant = 'info', size = 'md' }) => {
  const variantStyles = {
    info: 'bg-indigo-500/15 text-indigo-400 border-indigo-500/30',
    success: 'bg-emerald-500/15 text-emerald-400 border-emerald-500/30',
    warning: 'bg-amber-500/15 text-amber-400 border-amber-500/30',
    danger: 'bg-rose-500/15 text-rose-400 border-rose-500/30',
    neutral: 'bg-slate-700/50 text-slate-300 border-slate-600',
    purple: 'bg-purple-500/15 text-purple-400 border-purple-500/30',
  };

  const sizeStyles = {
    sm: 'px-2 py-0.5 text-xs',
    md: 'px-2.5 py-1 text-xs',
    lg: 'px-3 py-1 text-sm',
  };

  return (
    <span className={`inline-flex items-center font-medium rounded-full border ${variantStyles[variant] || variantStyles.neutral} ${sizeStyles[size]}`}>
      {children}
    </span>
  );
};
