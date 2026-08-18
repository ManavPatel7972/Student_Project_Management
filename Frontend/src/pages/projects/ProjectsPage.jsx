import React, { useState, useEffect } from 'react';
import {
  getProjectsApi,
  createProjectApi,
  updateProjectApi,
  updateProjectStatusApi,
  deleteProjectApi,
} from '../../api/projects';
import { Modal } from '../../components/common/Modal';
import { Badge } from '../../components/common/Badge';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import { Search, Plus, Edit2, Trash2, FolderGit2, CheckCircle2, Clock, PlayCircle } from 'lucide-react';
import toast from 'react-hot-toast';
import { useAuth } from '../../context/AuthContext';

export const ProjectsPage = () => {
  const { hasRole } = useAuth();
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  // Modal state
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingProject, setEditingProject] = useState(null);
  const [formData, setFormData] = useState({
    projectTitle: '',
    description: '',
    status: 0, // 0 = NotStarted, 1 = InProgress, 2 = Completed
  });

  useEffect(() => {
    fetchProjects();
  }, [search, statusFilter]);

  const fetchProjects = async () => {
    try {
      setLoading(true);
      const params = {};
      if (search) params.search = search;
      if (statusFilter !== '') params.status = parseInt(statusFilter);

      const data = await getProjectsApi(params);
      setProjects(data);
    } catch (err) {
      toast.error('Failed to load projects.');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreate = () => {
    setEditingProject(null);
    setFormData({ projectTitle: '', description: '', status: 0 });
    setIsModalOpen(true);
  };

  const handleOpenEdit = (project) => {
    setEditingProject(project);
    setFormData({
      projectTitle: project.projectTitle,
      description: project.description || '',
      status: project.status,
    });
    setIsModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        ...formData,
        status: parseInt(formData.status),
      };

      if (editingProject) {
        await updateProjectApi(editingProject.id, payload);
        toast.success('Project updated successfully!');
      } else {
        await createProjectApi(payload);
        toast.success('Project created successfully!');
      }
      setIsModalOpen(false);
      fetchProjects();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Error saving project.');
    }
  };

  const handleStatusChange = async (projectId, newStatus) => {
    try {
      await updateProjectStatusApi(projectId, parseInt(newStatus));
      toast.success('Project status updated.');
      fetchProjects();
    } catch (err) {
      toast.error('Failed to update project status.');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this project?')) return;
    try {
      await deleteProjectApi(id);
      toast.success('Project deleted.');
      fetchProjects();
    } catch (err) {
      toast.error('Failed to delete project.');
    }
  };

  const getStatusBadge = (status) => {
    switch (status) {
      case 0:
        return <Badge variant="warning"><Clock className="w-3 h-3 mr-1 inline" /> Not Started</Badge>;
      case 1:
        return <Badge variant="info"><PlayCircle className="w-3 h-3 mr-1 inline" /> In Progress</Badge>;
      case 2:
        return <Badge variant="success"><CheckCircle2 className="w-3 h-3 mr-1 inline" /> Completed</Badge>;
      default:
        return <Badge variant="neutral">Unknown</Badge>;
    }
  };

  return (
    <div className="space-y-6 animate-fadeIn">
      {/* Header */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-extrabold text-white">Project Master</h1>
          <p className="text-xs text-slate-400">Manage all student projects and development statuses</p>
        </div>
        {hasRole(['Admin', 'Faculty']) && (
          <button
            onClick={handleOpenCreate}
            className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-indigo-600 to-purple-600 hover:from-indigo-500 hover:to-purple-500 text-white text-xs font-bold rounded-xl shadow-lg shadow-indigo-600/30 transition-all"
          >
            <Plus className="w-4 h-4" /> Create New Project
          </button>
        )}
      </div>

      {/* Filter Bar */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 bg-slate-800/60 p-4 rounded-2xl border border-slate-700/60 backdrop-blur-xl">
        <div className="relative">
          <Search className="w-4 h-4 text-slate-400 absolute left-3 top-3" />
          <input
            type="text"
            placeholder="Search project title or description..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-9 pr-4 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
          />
        </div>
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
        >
          <option value="">All Statuses</option>
          <option value="0">Not Started</option>
          <option value="1">In Progress</option>
          <option value="2">Completed</option>
        </select>
      </div>

      {/* Projects Grid */}
      {loading ? (
        <LoadingSpinner text="Fetching projects..." />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
          {projects.map((p) => (
            <div
              key={p.id}
              className="bg-slate-800/60 border border-slate-700/60 rounded-2xl p-6 backdrop-blur-xl flex flex-col justify-between hover:border-slate-600 transition-all duration-300 shadow-lg"
            >
              <div>
                <div className="flex items-center justify-between mb-3">
                  {getStatusBadge(p.status)}
                  <span className="text-[11px] font-bold text-slate-400">
                    {p.totalAllocations} Allocation{p.totalAllocations !== 1 ? 's' : ''}
                  </span>
                </div>
                <h3 className="font-bold text-white text-lg mb-2 flex items-center gap-2">
                  <FolderGit2 className="w-5 h-5 text-indigo-400 shrink-0" />
                  <span className="line-clamp-1">{p.projectTitle}</span>
                </h3>
                <p className="text-xs text-slate-400 line-clamp-3 mb-4">
                  {p.description || 'No detailed project description provided.'}
                </p>
              </div>

              <div className="pt-4 border-t border-slate-700/60 flex items-center justify-between">
                {hasRole(['Admin', 'Faculty']) ? (
                  <select
                    value={p.status}
                    onChange={(e) => handleStatusChange(p.id, e.target.value)}
                    className="px-2.5 py-1 bg-slate-950 border border-slate-800 rounded-lg text-xs text-slate-300 focus:outline-none"
                  >
                    <option value={0}>Not Started</option>
                    <option value={1}>In Progress</option>
                    <option value={2}>Completed</option>
                  </select>
                ) : (
                  <span className="text-[11px] text-slate-500 font-medium">Status Locked</span>
                )}

                <div className="flex items-center gap-2">
                  {hasRole(['Admin', 'Faculty']) && (
                    <button
                      onClick={() => handleOpenEdit(p)}
                      className="p-1.5 text-slate-400 hover:text-indigo-400 hover:bg-slate-900 rounded-lg transition-colors"
                      title="Edit Project"
                    >
                      <Edit2 className="w-4 h-4" />
                    </button>
                  )}
                  {hasRole('Admin') && (
                    <button
                      onClick={() => handleDelete(p.id)}
                      className="p-1.5 text-slate-400 hover:text-rose-400 hover:bg-slate-900 rounded-lg transition-colors"
                      title="Delete Project"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))}
          {projects.length === 0 && (
            <div className="col-span-full py-12 text-center text-slate-500">
              No projects found.
            </div>
          )}
        </div>
      )}

      {/* Project Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={editingProject ? 'Edit Project Master' : 'Create New Project'}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-semibold text-slate-400 mb-1">Project Title</label>
            <input
              type="text"
              value={formData.projectTitle}
              onChange={(e) => setFormData({ ...formData, projectTitle: e.target.value })}
              className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              required
            />
          </div>
          <div>
            <label className="block text-xs font-semibold text-slate-400 mb-1">Description</label>
            <textarea
              rows="4"
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
            />
          </div>
          <div>
            <label className="block text-xs font-semibold text-slate-400 mb-1">Project Status</label>
            <select
              value={formData.status}
              onChange={(e) => setFormData({ ...formData, status: e.target.value })}
              className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
            >
              <option value={0}>Not Started</option>
              <option value={1}>In Progress</option>
              <option value={2}>Completed</option>
            </select>
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
              className="px-4 py-2 text-xs font-bold text-white bg-indigo-600 hover:bg-indigo-500 rounded-xl shadow-md shadow-indigo-600/30"
            >
              Save Project
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
