import React, { useState, useEffect } from "react";
import {
  getUsersApi,
  createUserApi,
  updateUserApi,
  deleteUserApi,
  uploadUserPhotoApi,
} from "../../api/users";
import { getDepartmentsApi } from "../../api/departments";
import { getRolesApi } from "../../api/roles";
import { getUserTypesApi } from "../../api/lookups";
import { Modal } from "../../components/common/Modal";
import { Badge } from "../../components/common/Badge";
import { LoadingSpinner } from "../../components/common/LoadingSpinner";
import {
  Search,
  Plus,
  Edit2,
  Trash2,
  Camera,
  Shield,
  Mail,
  Phone,
} from "lucide-react";
import toast from "react-hot-toast";
import { useAuth } from "../../context/AuthContext";

export const UsersPage = () => {
  const { hasRole } = useAuth();
  const [users, setUsers] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [roles, setRoles] = useState([]);
  const [userTypes, setUserTypes] = useState([]);
  const [loading, setLoading] = useState(true);

  // Filters
  const [search, setSearch] = useState("");
  const [selectedType, setSelectedType] = useState("");
  const [selectedDept, setSelectedDept] = useState("");

  // Modal State
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [formData, setFormData] = useState({
    userTypeId: 3,
    departmentId: "",
    roleId: 3,
    fullName: "",
    userCode: "",
    email: "",
    password: "",
    mobileNumber: "",
    isActive: true,
  });

  // Photo Upload State
  const [photoUser, setPhotoUser] = useState(null);
  const [selectedFile, setSelectedFile] = useState(null);

  useEffect(() => {
    fetchLookups();
    fetchUsers();
  }, []);

  useEffect(() => {
    fetchUsers();
  }, [search, selectedType, selectedDept]);

  const fetchLookups = async () => {
    try {
      const [deptsData, rolesData, typesData] = await Promise.all([
        getDepartmentsApi(),
        getRolesApi(),
        getUserTypesApi(),
      ]);

      setDepartments(deptsData);
      setRoles(rolesData);
      setUserTypes(typesData);
    } catch (err) {
      console.error("Failed to load lookups:", err);
    }
  };

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const params = {};
      if (search) params.search = search;
      if (selectedType) params.userTypeId = selectedType;
      if (selectedDept) params.departmentId = selectedDept;

      const data = await getUsersApi(params);
      setUsers(data);
    } catch (err) {
      toast.error("Failed to load users.");
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreateModal = () => {
    setEditingUser(null);
    setFormData({
      userTypeId: userTypes[0]?.id || 3,
      departmentId: departments[0]?.id || "",
      roleId: roles[0]?.id || 3,
      fullName: "",
      userCode: "",
      email: "",
      password: "",
      mobileNumber: "",
      isActive: true,
    });
    setIsModalOpen(true);
  };

  const handleOpenEditModal = (user) => {
    setEditingUser(user);
    setFormData({
      userTypeId: user.userTypeId,
      departmentId: user.departmentId || "",
      roleId: user.roleId || roles[0]?.id || 3,
      fullName: user.fullName,
      userCode: user.userCode || "",
      email: user.email,
      password: "",
      mobileNumber: user.mobileNumber,
      isActive: user.isActive,
    });
    setIsModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        ...formData,
        userTypeId: parseInt(formData.userTypeId),
        departmentId: formData.departmentId
          ? parseInt(formData.departmentId)
          : null,
        roleId: parseInt(formData.roleId),
      };

      if (editingUser) {
        await updateUserApi(editingUser.id, payload);
        toast.success("User updated successfully!");
      } else {
        await createUserApi(payload);
        toast.success("User created successfully!");
      }
      setIsModalOpen(false);
      fetchUsers();
    } catch (err) {
      toast.error(err.response?.data?.message || "Error saving user.");
    }
  };

  const handleDelete = async (id) => {
    if (
      !window.confirm("Are you sure you want to deactivate/delete this user?")
    )
      return;
    try {
      await deleteUserApi(id);
      toast.success("User deleted successfully.");
      fetchUsers();
    } catch (err) {
      toast.error("Failed to delete user.");
    }
  };

  const handleUploadPhoto = async (e) => {
    e.preventDefault();
    if (!selectedFile || !photoUser) return;
    const form = new FormData();
    form.append("file", selectedFile);
    try {
      await uploadUserPhotoApi(photoUser.id, form);
      toast.success("Photo uploaded!");
      setPhotoUser(null);
      setSelectedFile(null);
      fetchUsers();
    } catch (err) {
      toast.error("Photo upload failed.");
    }
  };

  return (
    <div className="space-y-6 animate-fadeIn">
      {/* Header */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-extrabold text-white">
            Users Directory
          </h1>
          <p className="text-xs text-slate-400">
            Manage students, faculty & system administrators
          </p>
        </div>
        {hasRole("Admin") && (
          <button
            onClick={handleOpenCreateModal}
            className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-indigo-600 to-purple-600 hover:from-indigo-500 hover:to-purple-500 text-white text-xs font-bold rounded-xl shadow-lg shadow-indigo-600/30 transition-all"
          >
            <Plus className="w-4 h-4" /> Add New User
          </button>
        )}
      </div>

      {/* Filter Bar */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 bg-slate-800/60 p-4 rounded-2xl border border-slate-700/60 backdrop-blur-xl">
        <div className="relative">
          <Search className="w-4 h-4 text-slate-400 absolute left-3 top-3" />
          <input
            type="text"
            placeholder="Search name, email, code..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-9 pr-4 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
          />
        </div>
        <select
          value={selectedType}
          onChange={(e) => setSelectedType(e.target.value)}
          className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
        >
          <option value="">All User Types</option>
          {userTypes.map((t) => (
            <option key={t.id} value={t.id}>
              {t.userTypeName}
            </option>
          ))}
        </select>
        <select
          value={selectedDept}
          onChange={(e) => setSelectedDept(e.target.value)}
          className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
        >
          <option value="">All Departments</option>
          {departments.map((d) => (
            <option key={d.id} value={d.id}>
              {d.name}
            </option>
          ))}
        </select>
      </div>

      {/* Users Grid/Table */}
      {loading ? (
        <LoadingSpinner text="Fetching users..." />
      ) : (
        <div className="bg-slate-800/60 border border-slate-700/60 rounded-2xl overflow-hidden backdrop-blur-xl">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="text-[11px] font-bold uppercase text-slate-400 border-b border-slate-700/60 bg-slate-950/40">
                  <th className="p-4">User</th>
                  <th className="p-4">Code / Reg No</th>
                  <th className="p-4">Type & Dept</th>
                  <th className="p-4">Role</th>
                  <th className="p-4">Status</th>
                  {hasRole("Admin") && (
                    <th className="p-4 text-right">Actions</th>
                  )}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800 text-xs text-slate-300">
                {users.map((u) => (
                  <tr
                    key={u.id}
                    className="hover:bg-slate-800/40 transition-colors"
                  >
                    <td className="p-4">
                      <div className="flex items-center gap-3">
                        <div className="relative group">
                          {u.profilePicturePath ? (
                            <img
                              src={u.profilePicturePath}
                              alt={u.fullName}
                              className="w-9 h-9 rounded-xl object-cover border border-slate-700"
                            />
                          ) : (
                            <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center font-bold text-white shadow-inner">
                              {u.fullName.charAt(0)}
                            </div>
                          )}
                          <button
                            onClick={() => setPhotoUser(u)}
                            className="absolute -bottom-1 -right-1 p-1 bg-slate-900 text-slate-300 hover:text-white rounded-full border border-slate-700 opacity-0 group-hover:opacity-100 transition-opacity"
                            title="Upload Photo"
                          >
                            <Camera className="w-2.5 h-2.5" />
                          </button>
                        </div>
                        <div>
                          <p className="font-bold text-white">{u.fullName}</p>
                          <p className="text-[11px] text-slate-400 flex items-center gap-1">
                            <Mail className="w-3 h-3 text-slate-500" />{" "}
                            {u.email}
                          </p>
                        </div>
                      </div>
                    </td>
                    <td className="p-4 font-semibold text-slate-300">
                      {u.userCode || "-"}
                    </td>
                    <td className="p-4">
                      <p className="font-medium text-white">{u.userTypeName}</p>
                      <p className="text-[11px] text-slate-400">
                        {u.departmentName || "General"}
                      </p>
                    </td>
                    <td className="p-4">
                      <Badge variant="purple">{u.roleName || "User"}</Badge>
                    </td>
                    <td className="p-4">
                      <Badge variant={u.isActive ? "success" : "danger"}>
                        {u.isActive ? "Active" : "Inactive"}
                      </Badge>
                    </td>
                    {hasRole("Admin") && (
                      <td className="p-4 text-right">
                        <div className="flex items-center justify-end gap-2">
                          <button
                            onClick={() => handleOpenEditModal(u)}
                            className="p-1.5 text-slate-400 hover:text-indigo-400 hover:bg-slate-900 rounded-lg transition-colors"
                          >
                            <Edit2 className="w-4 h-4" />
                          </button>
                          <button
                            onClick={() => handleDelete(u.id)}
                            className="p-1.5 text-slate-400 hover:text-rose-400 hover:bg-slate-900 rounded-lg transition-colors"
                          >
                            <Trash2 className="w-4 h-4" />
                          </button>
                        </div>
                      </td>
                    )}
                  </tr>
                ))}
                {users.length === 0 && (
                  <tr>
                    <td colSpan="6" className="p-8 text-center text-slate-500">
                      No users found matching your filters.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* User Create/Edit Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={editingUser ? "Edit User Profile" : "Add New User"}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Full Name
              </label>
              <input
                type="text"
                value={formData.fullName}
                onChange={(e) =>
                  setFormData({ ...formData, fullName: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                required
              />
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                User Code / Enrollment No
              </label>
              <input
                type="text"
                value={formData.userCode}
                onChange={(e) =>
                  setFormData({ ...formData, userCode: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              />
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Email
              </label>
              <input
                type="email"
                value={formData.email}
                onChange={(e) =>
                  setFormData({ ...formData, email: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                required
              />
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Mobile Number
              </label>
              <input
                type="text"
                value={formData.mobileNumber}
                onChange={(e) =>
                  setFormData({ ...formData, mobileNumber: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              />
            </div>
            {!editingUser && (
              <div>
                <label className="block text-xs font-semibold text-slate-400 mb-1">
                  Password
                </label>
                <input
                  type="password"
                  value={formData.password}
                  onChange={(e) =>
                    setFormData({ ...formData, password: e.target.value })
                  }
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                  required
                />
              </div>
            )}
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                User Type
              </label>
              <select
                value={formData.userTypeId}
                onChange={(e) =>
                  setFormData({ ...formData, userTypeId: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              >
                {userTypes.map((t) => (
                  <option key={t.id} value={t.id}>
                    {t.userTypeName}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Department
              </label>
              <select
                value={formData.departmentId}
                onChange={(e) =>
                  setFormData({ ...formData, departmentId: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              >
                <option value="">None / System</option>
                {departments.map((d) => (
                  <option key={d.id} value={d.id}>
                    {d.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                System Role
              </label>
              <select
                value={formData.roleId}
                onChange={(e) =>
                  setFormData({ ...formData, roleId: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              >
                {roles.map((r) => (
                  <option key={r.id} value={r.id}>
                    {r.roleName}
                  </option>
                ))}
              </select>
            </div>
          </div>
          <div className="pt-4 border-t border-slate-800 flex items-center justify-between">
            <label className="flex items-center gap-2 text-xs text-slate-300">
              <input
                type="checkbox"
                checked={formData.isActive}
                onChange={(e) =>
                  setFormData({ ...formData, isActive: e.target.checked })
                }
                className="rounded bg-slate-950 border-slate-800 text-indigo-600 focus:ring-0"
              />
              Account Active
            </label>
            <div className="flex gap-3">
              <button
                type="button"
                onClick={() => setIsModalOpen(false)}
                className="px-4 py-2 text-xs text-slate-400 hover:text-white"
              >
                Cancel
              </button>
              <button
                type="submit"
                className="px-4 py-2 text-xs font-bold text-white bg-indigo-600 hover:bg-indigo-500 rounded-xl shadow-md shadow-indigo-600/30"
              >
                Save User
              </button>
            </div>
          </div>
        </form>
      </Modal>

      {/* Photo Upload Modal */}
      <Modal
        isOpen={!!photoUser}
        onClose={() => setPhotoUser(null)}
        title={`Upload Photo for ${photoUser?.fullName}`}
        maxWidth="max-w-md"
      >
        <form onSubmit={handleUploadPhoto} className="space-y-4">
          <input
            type="file"
            accept="image/*"
            onChange={(e) => setSelectedFile(e.target.files[0])}
            className="w-full text-xs text-slate-400 file:mr-4 file:py-2 file:px-4 file:rounded-xl file:border-0 file:text-xs file:font-semibold file:bg-indigo-600 file:text-white hover:file:bg-indigo-500"
            required
          />
          <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
            <button
              type="button"
              onClick={() => setPhotoUser(null)}
              className="px-4 py-2 text-xs text-slate-400 hover:text-white"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-4 py-2 text-xs font-bold text-white bg-indigo-600 hover:bg-indigo-500 rounded-xl"
            >
              Upload Photo
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
