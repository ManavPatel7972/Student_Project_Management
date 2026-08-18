import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import {
  LayoutDashboard,
  Users,
  GraduationCap,
  Briefcase,
  FolderGit2,
  ListTodo,
  Award,
  ShieldAlert,
  Building2,
  LogOut,
  Sparkles,
} from 'lucide-react';

export const Sidebar = ({ isOpen, onClose }) => {
  const { user, logout, hasRole } = useAuth();

  const navItems = [
    { title: 'Dashboard', path: '/dashboard', icon: LayoutDashboard, roles: ['Admin', 'Faculty', 'Student'] },
    { title: 'Projects', path: '/projects', icon: FolderGit2, roles: ['Admin', 'Faculty', 'Student'] },
    { title: 'Allocations', path: '/allocations', icon: Briefcase, roles: ['Admin', 'Faculty', 'Student'] },
    { title: 'Tasks', path: '/tasks', icon: ListTodo, roles: ['Admin', 'Faculty', 'Student'] },
    { title: 'Scores', path: '/scores', icon: Award, roles: ['Admin', 'Faculty', 'Student'] },
    { title: 'Students', path: '/students', icon: GraduationCap, roles: ['Admin', 'Faculty'] },
    { title: 'Faculty', path: '/faculty', icon: Users, roles: ['Admin'] },
    { title: 'All Users', path: '/users', icon: Users, roles: ['Admin'] },
    { title: 'Departments', path: '/departments', icon: Building2, roles: ['Admin'] },
    { title: 'Roles & Matrix', path: '/roles', icon: ShieldAlert, roles: ['Admin'] },
  ];

  const filteredNav = navItems.filter((item) => hasRole(item.roles));

  return (
    <>
      {/* Mobile overlay */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-slate-950/80 backdrop-blur-sm md:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={`fixed top-0 left-0 z-40 h-screen w-64 bg-slate-950 border-r border-slate-800/80 flex flex-col transition-transform duration-300 md:translate-x-0 ${isOpen ? 'translate-x-0' : '-translate-x-full'
          }`}
      >
        {/* Brand logo */}
        <div className="flex items-center gap-3 px-6 h-16 border-b border-slate-800/80 bg-slate-950/50">
          <div className="p-2 bg-gradient-to-tr from-indigo-600 to-purple-600 rounded-xl shadow-lg shadow-indigo-500/30">
            <Sparkles className="w-5 h-5 text-white" />
          </div>
          <div>
            <h1 className="font-extrabold text-lg text-white tracking-wide bg-clip-text text-transparent bg-gradient-to-r from-indigo-400 to-purple-400">
              SPMS
            </h1>
            <p className="text-[10px] uppercase font-bold text-slate-500 tracking-wider">
              Project Manager
            </p>
          </div>
        </div>

        {/* Navigation list */}
        <div className="flex-1 overflow-y-auto px-4 py-6 space-y-1.5 custom-scrollbar">
          <p className="px-3 text-[10px] font-bold text-slate-500 uppercase tracking-widest mb-2">
            Main Navigation
          </p>
          {filteredNav.map((item) => {
            const Icon = item.icon;
            return (
              <NavLink
                key={item.path}
                to={item.path}
                onClick={onClose}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-3.5 py-2.5 rounded-xl font-medium text-sm transition-all duration-200 ${isActive
                    ? 'bg-gradient-to-r from-indigo-600 to-indigo-700 text-white shadow-lg shadow-indigo-600/30 font-semibold'
                    : 'text-slate-400 hover:text-white hover:bg-slate-900'
                  }`
                }
              >
                <Icon className="w-4 h-4" />
                <span>{item.title}</span>
              </NavLink>
            );
          })}
        </div>

        {/* User profile footer */}
        <div className="p-4 border-t border-slate-800/80 bg-slate-950/80">
          <div className="flex items-center gap-3 p-2.5 rounded-xl bg-slate-900/60 border border-slate-800/60">
            <div className="w-9 h-9 rounded-lg bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center font-bold text-white shadow-inner">
              {user?.fullName?.charAt(0) || 'U'}
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-bold text-white truncate">{user?.fullName || 'User'}</p>
              <p className="text-[10px] text-indigo-400 font-semibold truncate capitalize">
                {user?.role || 'Guest'}
              </p>
            </div>
            <button
              onClick={logout}
              title="Logout"
              className="p-1.5 text-slate-400 hover:text-rose-400 hover:bg-rose-500/10 rounded-lg transition-colors"
            >
              <LogOut className="w-4 h-4" />
            </button>
          </div>
        </div>
      </aside>
    </>
  );
};
