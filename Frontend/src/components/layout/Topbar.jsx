// import React, { useState } from 'react';
// import { useAuth } from '../../context/AuthContext';
// import { Menu, LogOut, KeyRound } from 'lucide-react';
// import { Modal } from '../common/Modal';
// import { changePasswordApi } from '../../api/auth';
// import toast from 'react-hot-toast';

// export const Topbar = ({ onOpenSidebar }) => {
//   const { user, logout } = useAuth();
//   const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
//   const [currentPassword, setCurrentPassword] = useState('');
//   const [newPassword, setNewPassword] = useState('');
//   const [loading, setLoading] = useState(false);

//   const handlePasswordSubmit = async (e) => {
//     e.preventDefault();
//     if (!currentPassword || !newPassword) {
//       toast.error('All password fields are required.');
//       return;
//     }
//     try {
//       setLoading(true);
//       await changePasswordApi({ currentPassword, newPassword });
//       toast.success('Password updated successfully!');
//       setIsPasswordModalOpen(false);
//       setCurrentPassword('');
//       setNewPassword('');
//     } catch (err) {
//       toast.error(err.response?.data?.message || 'Failed to update password.');
//     } finally {
//       setLoading(false);
//     }
//   };

//   return (
//     <header className="sticky top-0 z-30 h-16 bg-slate-950/80 backdrop-blur-xl border-b border-slate-800/80 px-4 md:px-8 flex items-center justify-between">
//       <div className="flex items-center gap-4">
//         <button
//           onClick={onOpenSidebar}
//           className="p-2 text-slate-400 hover:text-white hover:bg-slate-900 rounded-lg md:hidden"
//         >
//           <Menu className="w-5 h-5" />
//         </button>
//         <span className="text-xs font-semibold px-2.5 py-1 rounded-full bg-indigo-500/10 text-indigo-400 border border-indigo-500/20">
//           SPMS Web App
//         </span>
//       </div>

//       <div className="flex items-center gap-3">
//         <button
//           onClick={() => setIsPasswordModalOpen(true)}
//           className="flex items-center gap-2 px-3 py-1.5 text-xs font-medium text-slate-300 hover:text-white bg-slate-900 hover:bg-slate-800 border border-slate-800 rounded-lg transition-colors"
//         >
//           <KeyRound className="w-3.5 h-3.5" />
//           <span>Change Password</span>
//         </button>
//         <button
//           onClick={logout}
//           className="flex items-center gap-2 px-3 py-1.5 text-xs font-medium text-rose-400 hover:text-rose-300 bg-rose-500/10 hover:bg-rose-500/20 border border-rose-500/20 rounded-lg transition-colors"
//         >
//           <LogOut className="w-3.5 h-3.5" />
//           <span>Logout</span>
//         </button>
//       </div>

//       {/* Change Password Modal */}
//       <Modal
//         isOpen={isPasswordModalOpen}
//         onClose={() => setIsPasswordModalOpen(false)}
//         title="Change Account Password"
//         maxWidth="max-w-md"
//       >
//         <form onSubmit={handlePasswordSubmit} className="space-y-4">
//           <div>
//             <label className="block text-xs font-semibold text-slate-400 mb-1">
//               Current Password
//             </label>
//             <input
//               type="password"
//               value={currentPassword}
//               onChange={(e) => setCurrentPassword(e.target.value)}
//               className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm focus:outline-none focus:border-indigo-500"
//               required
//             />
//           </div>
//           <div>
//             <label className="block text-xs font-semibold text-slate-400 mb-1">
//               New Password
//             </label>
//             <input
//               type="password"
//               value={newPassword}
//               onChange={(e) => setNewPassword(e.target.value)}
//               className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm focus:outline-none focus:border-indigo-500"
//               required
//             />
//           </div>
//           <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
//             <button
//               type="button"
//               onClick={() => setIsPasswordModalOpen(false)}
//               className="px-4 py-2 text-xs font-medium text-slate-400 hover:text-white"
//             >
//               Cancel
//             </button>
//             <button
//               type="submit"
//               disabled={loading}
//               className="px-4 py-2 text-xs font-semibold text-white bg-indigo-600 hover:bg-indigo-500 rounded-xl transition-colors disabled:opacity-50"
//             >
//               {loading ? 'Updating...' : 'Update Password'}
//             </button>
//           </div>
//         </form>
//       </Modal>
//     </header>
//   );
// };

import React, { useEffect, useRef, useState } from "react";
import {
  Building2,
  Camera,
  ChevronDown,
  IdCard,
  KeyRound,
  LogOut,
  Mail,
  Menu,
  Phone,
  ShieldCheck,
  Trash2,
  X,
} from "lucide-react";
import { useAuth } from "../../context/AuthContext";
import { changePasswordApi } from "../../api/auth";
import { deleteUserPhotoApi, uploadUserPhotoApi } from "../../api/users";
import toast from "react-hot-toast";

export const Topbar = ({ onOpenSidebar }) => {
  const { user, logout, setUser } = useAuth();
  const fileInputRef = useRef(null);

  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [isPasswordFormOpen, setIsPasswordFormOpen] = useState(false);
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [photoLoading, setPhotoLoading] = useState(false);

  // Keep the page fixed while the profile drawer is open.
  useEffect(() => {
    if (isProfileOpen) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "";
    }

    return () => {
      document.body.style.overflow = "";
    };
  }, [isProfileOpen]);

  const closeProfile = () => {
    if (loading) return;

    setIsProfileOpen(false);
    setIsPasswordFormOpen(false);
    setCurrentPassword("");
    setNewPassword("");
  };

  const handlePasswordSubmit = async (e) => {
    e.preventDefault();

    if (!currentPassword || !newPassword) {
      toast.error("All password fields are required.");
      return;
    }

    try {
      setLoading(true);

      await changePasswordApi({
        currentPassword,
        newPassword,
      });

      toast.success("Password updated successfully!");

      setIsPasswordFormOpen(false);
      setCurrentPassword("");
      setNewPassword("");
    } catch (err) {
      toast.error(err.response?.data?.message || "Failed to update password.");
    } finally {
      setLoading(false);
    }
  };

  const updateProfilePhoto = (profilePicturePath) => {
    setUser((currentUser) => {
      if (!currentUser) return currentUser;

      const updatedUser = { ...currentUser, profilePicturePath };
      localStorage.setItem("user", JSON.stringify(updatedUser));
      return updatedUser;
    });
  };

  const handlePhotoSelected = async (e) => {
    const file = e.target.files?.[0];
    e.target.value = "";

    if (!file || !user?.id) return;
    if (!file.type.startsWith("image/")) {
      toast.error("Please select an image file.");
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      toast.error("Profile image must be smaller than 5 MB.");
      return;
    }

    try {
      setPhotoLoading(true);
      const formData = new FormData();
      formData.append("file", file);
      const result = await uploadUserPhotoApi(user.id, formData);
      updateProfilePhoto(result.photoUrl);
      toast.success("Profile image updated successfully!");
    } catch (err) {
      toast.error(
        err.response?.data?.message || "Failed to upload profile image.",
      );
    } finally {
      setPhotoLoading(false);
    }
  };

  const handleRemovePhoto = async () => {
    if (!user?.id || !profileImage) return;

    try {
      setPhotoLoading(true);
      await deleteUserPhotoApi(user.id);
      updateProfilePhoto(null);
      toast.success("Profile image removed.");
    } catch (err) {
      toast.error(
        err.response?.data?.message || "Failed to remove profile image.",
      );
    } finally {
      setPhotoLoading(false);
    }
  };

  const initials =
    user?.fullName
      ?.split(" ")
      .map((part) => part[0])
      .join("")
      .slice(0, 2)
      .toUpperCase() || "U";

  const profileImage = user?.profilePicturePath;

  return (
    <>
      {/* ==================== TOPBAR ==================== */}
      <header className="sticky top-0 z-30 h-16 bg-slate-950/80 backdrop-blur-xl border-b border-slate-800/80 px-4 md:px-8 flex items-center justify-between">
        {/* Left Side */}
        <div className="flex items-center gap-4">
          {/* Mobile Menu */}
          <button
            type="button"
            onClick={onOpenSidebar}
            className="p-2 text-slate-400 hover:text-white hover:bg-slate-900 rounded-lg transition-colors md:hidden"
            aria-label="Open sidebar"
          >
            <Menu className="w-5 h-5" />
          </button>

          {/* App Badge */}
          <span className="text-xs font-semibold px-2.5 py-1 rounded-full bg-indigo-500/10 text-indigo-400 border border-indigo-500/20">
            SPMS Web App
          </span>
        </div>

        {/* Profile trigger */}
        <div className="relative">
          <button
            type="button"
            onClick={() => setIsProfileOpen(true)}
            className="flex items-center gap-2 rounded-xl border border-slate-800 bg-slate-900/80 px-2 py-1.5 text-left transition-colors hover:border-indigo-500/50 hover:bg-slate-800"
            aria-label="Open user profile"
            aria-expanded={isProfileOpen}
          >
            {profileImage ? (
              <img
                src={profileImage}
                alt=""
                className="h-8 w-8 rounded-lg object-cover"
              />
            ) : (
              <span className="flex h-8 w-8 items-center justify-center rounded-lg bg-gradient-to-br from-indigo-500 to-purple-600 text-xs font-bold text-white">
                {initials}
              </span>
            )}
            <span className="hidden max-w-32 sm:block">
              <span className="block truncate text-xs font-bold text-white">
                {user?.fullName || "User"}
              </span>
              <span className="block truncate text-[10px] font-semibold text-indigo-400">
                {user?.role || "Guest"}
              </span>
            </span>
            <ChevronDown className="hidden h-4 w-4 text-slate-500 sm:block" />
          </button>
        </div>
      </header>

      {/* ==================== PROFILE DRAWER ==================== */}
      {isProfileOpen && (
        <div className="fixed inset-0 z-[100]">
          <div
            className="absolute inset-0 bg-slate-950/75 backdrop-blur-sm"
            onMouseDown={closeProfile}
            aria-hidden="true"
          />
          <div
            className="absolute right-0 top-0 flex h-full w-full max-w-md flex-col overflow-y-auto border-l border-slate-700/70 bg-slate-900 shadow-2xl shadow-black/40"
            onMouseDown={(e) => e.stopPropagation()}
          >
            <div className="flex items-center justify-between border-b border-slate-800 px-6 py-5">
              <div>
                <p className="text-[10px] font-bold uppercase tracking-widest text-indigo-400">
                  Account
                </p>
                <h2 className="mt-1 text-lg font-bold text-white">
                  My Profile
                </h2>
              </div>
              <button
                type="button"
                onClick={closeProfile}
                disabled={loading}
                className="flex h-8 w-8 items-center justify-center rounded-lg text-slate-400 hover:text-white hover:bg-slate-800 transition-colors disabled:opacity-50"
                aria-label="Close modal"
              >
                <X className="h-4 w-4" />
              </button>
            </div>

            <div className="px-6 py-6">
              <div className="flex items-center gap-4 border-b border-slate-800 pb-6">
                <div className="relative shrink-0">
                  {profileImage ? (
                    <img
                      src={profileImage}
                      alt={user?.fullName || "User"}
                      className="h-16 w-16 rounded-2xl object-cover"
                    />
                  ) : (
                    <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-gradient-to-br from-indigo-500 to-purple-600 text-xl font-bold text-white">
                      {initials}
                    </div>
                  )}
                  {photoLoading && (
                    <div className="absolute inset-0 flex items-center justify-center rounded-2xl bg-slate-950/70 text-[10px] text-white">
                      Saving...
                    </div>
                  )}
                </div>
                <div className="min-w-0 flex-1">
                  <h3 className="truncate text-lg font-bold text-white">
                    {user?.fullName || "User"}
                  </h3>
                  <p className="truncate text-xs font-semibold text-indigo-400">
                    {user?.role || "Guest"}
                  </p>
                  <div className="mt-3 flex flex-wrap gap-2">
                    <input
                      ref={fileInputRef}
                      type="file"
                      accept="image/*"
                      onChange={handlePhotoSelected}
                      className="hidden"
                    />
                    <button
                      type="button"
                      onClick={() => fileInputRef.current?.click()}
                      disabled={photoLoading}
                      className="inline-flex items-center gap-1.5 rounded-lg border border-indigo-500/30 bg-indigo-500/10 px-2.5 py-1.5 text-[10px] font-semibold text-indigo-300 transition hover:bg-indigo-500/20 disabled:cursor-not-allowed disabled:opacity-50"
                    >
                      <Camera className="h-3.5 w-3.5" />{" "}
                      {profileImage ? "Change photo" : "Upload photo"}
                    </button>
                    {profileImage && (
                      <button
                        type="button"
                        onClick={handleRemovePhoto}
                        disabled={photoLoading}
                        className="inline-flex items-center gap-1.5 rounded-lg border border-rose-500/20 bg-rose-500/10 px-2.5 py-1.5 text-[10px] font-semibold text-rose-400 transition hover:bg-rose-500/20 disabled:cursor-not-allowed disabled:opacity-50"
                      >
                        <Trash2 className="h-3.5 w-3.5" /> Remove
                      </button>
                    )}
                  </div>
                </div>
              </div>

              <div className="space-y-4 py-6">
                <ProfileDetail icon={Mail} label="Email" value={user?.email} />
                <ProfileDetail
                  icon={Phone}
                  label="Mobile number"
                  value={user?.mobileNumber}
                />
                <ProfileDetail
                  icon={IdCard}
                  label="User code"
                  value={user?.userCode}
                />
                <ProfileDetail
                  icon={ShieldCheck}
                  label="User type"
                  value={user?.userTypeName || user?.role}
                />
                <ProfileDetail
                  icon={Building2}
                  label="Department"
                  value={user?.departmentName}
                />
              </div>

              <div className="border-t border-slate-800 pt-5">
                <button
                  type="button"
                  onClick={() => setIsPasswordFormOpen((open) => !open)}
                  className="flex w-full items-center justify-between rounded-xl border border-slate-800 bg-slate-950/60 px-4 py-3 text-left text-sm font-semibold text-slate-200 transition hover:border-indigo-500/50 hover:text-white"
                >
                  <span className="flex items-center gap-2">
                    <KeyRound className="h-4 w-4 text-indigo-400" /> Change
                    password
                  </span>
                  <ChevronDown
                    className={`h-4 w-4 transition-transform ${isPasswordFormOpen ? "rotate-180" : ""}`}
                  />
                </button>

                {isPasswordFormOpen && (
                  <form
                    onSubmit={handlePasswordSubmit}
                    className="mt-4 space-y-4"
                  >
                    <PasswordField
                      id="currentPassword"
                      label="Current password"
                      value={currentPassword}
                      onChange={setCurrentPassword}
                      autoComplete="current-password"
                      disabled={loading}
                    />
                    <PasswordField
                      id="newPassword"
                      label="New password"
                      value={newPassword}
                      onChange={setNewPassword}
                      autoComplete="new-password"
                      disabled={loading}
                    />
                    <button
                      type="submit"
                      disabled={loading}
                      className="w-full rounded-xl bg-indigo-600 px-5 py-2.5 text-xs font-semibold text-white transition hover:bg-indigo-500 disabled:cursor-not-allowed disabled:opacity-50"
                    >
                      {loading ? "Updating..." : "Update password"}
                    </button>
                  </form>
                )}
              </div>

              <button
                type="button"
                onClick={logout}
                className="mt-6 flex w-full items-center justify-center gap-2 rounded-xl border border-rose-500/20 bg-rose-500/10 px-4 py-3 text-xs font-semibold text-rose-400 transition hover:bg-rose-500/20"
              >
                <LogOut className="h-4 w-4" /> Logout
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

const ProfileDetail = ({ icon: Icon, label, value }) => (
  <div className="flex items-center gap-3">
    <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-slate-800 text-indigo-400">
      <Icon className="h-4 w-4" />
    </div>
    <div className="min-w-0">
      <p className="text-[10px] font-bold uppercase tracking-wider text-slate-500">
        {label}
      </p>
      <p className="truncate text-sm text-slate-200">
        {value || "Not provided"}
      </p>
    </div>
  </div>
);

const PasswordField = ({
  id,
  label,
  value,
  onChange,
  autoComplete,
  disabled,
}) => (
  <div>
    <label
      htmlFor={id}
      className="mb-2 block text-xs font-semibold text-slate-400"
    >
      {label}
    </label>
    <input
      id={id}
      type="password"
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={`Enter ${label.toLowerCase()}`}
      autoComplete={autoComplete}
      disabled={disabled}
      className="w-full rounded-xl border border-slate-800 bg-slate-950 px-3.5 py-2.5 text-sm text-white placeholder:text-slate-600 transition focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500/30 disabled:opacity-50"
      required
    />
  </div>
);
