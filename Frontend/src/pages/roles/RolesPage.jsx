import React, { useState, useEffect } from 'react';
import { getRolesApi, createRoleApi, updateRoleApi, deleteRoleApi } from '../../api/roles';
import { getPermissionMatrixApi, togglePermissionApi } from '../../api/permissions';
import { Modal } from '../../components/common/Modal';
import { Badge } from '../../components/common/Badge';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import { ShieldAlert, Plus, Edit2, Trash2, Check, X } from 'lucide-react';
import toast from 'react-hot-toast';

export const RolesPage = () => {
  const [roles, setRoles] = useState([]);
  const [matrix, setMatrix] = useState(null);
  const [loading, setLoading] = useState(true);

  // Modal State
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRole, setEditingRole] = useState(null);
  const [roleName, setRoleName] = useState('');
  const [description, setDescription] = useState('');

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [rolesData, matrixData] = await Promise.all([
        getRolesApi(),
        getPermissionMatrixApi(),
      ]);
      setRoles(rolesData);
      setMatrix(matrixData);
    } catch (err) {
      toast.error('Failed to load roles and permission matrix.');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreate = () => {
    setEditingRole(null);
    setRoleName('');
    setDescription('');
    setIsModalOpen(true);
  };

  const handleOpenEdit = (role) => {
    setEditingRole(role);
    setRoleName(role.roleName);
    setDescription(role.description || '');
    setIsModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingRole) {
        await updateRoleApi(editingRole.id, { roleName, description });
        toast.success('Role updated!');
      } else {
        await createRoleApi({ roleName, description });
        toast.success('Role created!');
      }
      setIsModalOpen(false);
      fetchData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Error saving role.');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this role?')) return;
    try {
      await deleteRoleApi(id);
      toast.success('Role deleted.');
      fetchData();
    } catch (err) {
      toast.error('Failed to delete role.');
    }
  };

  const handleTogglePermission = async (roleId, permissionId, currentGranted) => {
    try {
      await togglePermissionApi({
        roleId,
        permissionId,
        isGranted: !currentGranted,
      });
      toast.success('Permission toggled.');
      fetchData();
    } catch (err) {
      toast.error('Failed to update permission.');
    }
  };

  const isGranted = (roleId, permissionId) => {
    if (!matrix) return false;
    const item = matrix.rolePermissions.find(
      (rp) => rp.roleId === roleId && rp.permissionId === permissionId
    );
    return item ? item.isGranted : false;
  };

  return (
    <div className="space-y-8 animate-fadeIn">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-extrabold text-white">Roles & Permission Matrix</h1>
          <p className="text-xs text-slate-400">Manage role definitions and authorization grants</p>
        </div>
        <button
          onClick={handleOpenCreate}
          className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-indigo-600 to-purple-600 hover:from-indigo-500 hover:to-purple-500 text-white text-xs font-bold rounded-xl shadow-lg shadow-indigo-600/30 transition-all"
        >
          <Plus className="w-4 h-4" /> Add Role
        </button>
      </div>

      {loading ? (
        <LoadingSpinner text="Fetching role matrix..." />
      ) : (
        <>
          {/* Roles Cards */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {roles.map((r) => (
              <div
                key={r.id}
                className="bg-slate-800/60 border border-slate-700/60 rounded-2xl p-5 backdrop-blur-xl flex items-center justify-between"
              >
                <div>
                  <h3 className="font-bold text-white text-base">{r.roleName}</h3>
                  <p className="text-xs text-slate-400">{r.description || 'System Role'}</p>
                  <p className="text-[10px] text-indigo-400 font-semibold mt-1">
                    {r.userCount} Assigned Users
                  </p>
                </div>
                <div className="flex items-center gap-1">
                  <button
                    onClick={() => handleOpenEdit(r)}
                    className="p-1.5 text-slate-400 hover:text-indigo-400 hover:bg-slate-900 rounded-lg"
                  >
                    <Edit2 className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleDelete(r.id)}
                    className="p-1.5 text-slate-400 hover:text-rose-400 hover:bg-slate-900 rounded-lg"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </div>
            ))}
          </div>

          {/* Matrix Table */}
          {matrix && (
            <div className="bg-slate-800/60 border border-slate-700/60 rounded-2xl overflow-hidden backdrop-blur-xl p-6">
              <h3 className="font-bold text-white text-lg mb-4 flex items-center gap-2">
                <ShieldAlert className="w-5 h-5 text-purple-400" /> Authorization Matrix
              </h3>
              <div className="overflow-x-auto">
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="text-[11px] font-bold uppercase text-slate-400 border-b border-slate-700/60 bg-slate-950/40">
                      <th className="p-4">Permission Name</th>
                      {matrix.roles.map((r) => (
                        <th key={r.id} className="p-4 text-center">
                          {r.roleName}
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-800 text-xs text-slate-300">
                    {matrix.permissions.map((p) => (
                      <tr key={p.id} className="hover:bg-slate-800/40">
                        <td className="p-4 font-semibold text-white">
                          {p.permissionName}
                          <p className="text-[10px] text-slate-400 font-normal">{p.description}</p>
                        </td>
                        {matrix.roles.map((r) => {
                          const granted = isGranted(r.id, p.id);
                          return (
                            <td key={r.id} className="p-4 text-center">
                              <button
                                onClick={() => handleTogglePermission(r.id, p.id, granted)}
                                className={`p-2 rounded-xl border transition-all ${granted
                                    ? 'bg-emerald-500/20 text-emerald-400 border-emerald-500/30 hover:bg-emerald-500/30'
                                    : 'bg-slate-900 text-slate-600 border-slate-800 hover:text-slate-400'
                                  }`}
                              >
                                {granted ? <Check className="w-4 h-4" /> : <X className="w-4 h-4" />}
                              </button>
                            </td>
                          );
                        })}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </>
      )}

      {/* Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={editingRole ? 'Edit Role' : 'Create Role'}
        maxWidth="max-w-md"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-semibold text-slate-400 mb-1">Role Name</label>
            <input
              type="text"
              value={roleName}
              onChange={(e) => setRoleName(e.target.value)}
              className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              required
            />
          </div>
          <div>
            <label className="block text-xs font-semibold text-slate-400 mb-1">Description</label>
            <textarea
              rows="3"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
            />
          </div>
          <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
            <button
              type="button"
              onClick={() => setIsModalOpen(false)}
              className="px-4 py-2 text-xs text-slate-400 hover:text-white"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-4 py-2 text-xs font-bold text-white bg-indigo-600 hover:bg-indigo-500 rounded-xl"
            >
              Save Role
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
