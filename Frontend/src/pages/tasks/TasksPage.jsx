import React, { useState, useEffect } from "react";
import {
  getTasksApi,
  createTaskApi,
  updateTaskApi,
  updateTaskStatusApi,
  updateEarnedScoreApi,
  updateStudentRemarksApi,
  deleteTaskApi,
} from "../../api/tasks";
import { getAllocationsApi } from "../../api/allocations";
import { getTaskStatusesApi, getTaskPrioritiesApi } from "../../api/lookups";
import { Modal } from "../../components/common/Modal";
import { Badge } from "../../components/common/Badge";
import { LoadingSpinner } from "../../components/common/LoadingSpinner";
import {
  Search,
  Plus,
  Edit2,
  Trash2,
  ListTodo,
  Award,
  CheckCircle2,
  MessageSquare,
} from "lucide-react";
import toast from "react-hot-toast";
import { useAuth } from "../../context/AuthContext";

export const TasksPage = () => {
  const { user, hasRole } = useAuth();
  const isStudent = hasRole("Student");
  const canManage = hasRole(["Admin", "Faculty"]);
  const [tasks, setTasks] = useState([]);
  const [allocations, setAllocations] = useState([]);
  const [statuses, setStatuses] = useState([]);
  const [priorities, setPriorities] = useState([]);
  const [loading, setLoading] = useState(true);

  // Filters
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [priorityFilter, setPriorityFilter] = useState("");

  // Modals state
  const [isTaskModalOpen, setIsTaskModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState(null);

  // Remarks / Score Modal
  const [selectedTaskForRemark, setSelectedTaskForRemark] = useState(null);
  const [facultyRemark, setFacultyRemark] = useState("");
  const [studentRemark, setStudentRemark] = useState("");
  const [earnedScore, setEarnedScore] = useState(0);

  // Task Form State
  const [formData, setFormData] = useState({
    projectAllocationId: "",
    taskTitle: "",
    taskDescription: "",
    taskStatusId: 1,
    taskPriorityId: 2,
    assignedScore: 10,
    taskStartDate: new Date().toISOString().split("T")[0],
    taskDueDate: new Date(Date.now() + 7 * 86400000)
      .toISOString()
      .split("T")[0],
    facultyRemarks: "",
  });

  useEffect(() => {
    fetchLookups();
    fetchTasks();
  }, [search, statusFilter, priorityFilter]);

  const fetchLookups = async () => {
    try {
      const [allocsData, statsData, priosData] = await Promise.all([
        getAllocationsApi(),
        getTaskStatusesApi(),
        getTaskPrioritiesApi(),
      ]);
      setAllocations(allocsData);
      setStatuses(statsData);
      setPriorities(priosData);
    } catch (err) {
      console.error("Failed to load task lookups:", err);
    }
  };

  const fetchTasks = async () => {
    try {
      setLoading(true);
      const params = {};
      if (search) params.search = search;
      if (statusFilter) params.statusId = statusFilter;
      if (priorityFilter) params.priorityId = priorityFilter;

      const data = await getTasksApi(params);
      setTasks(data);
    } catch (err) {
      toast.error("Failed to load task items.");
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreate = () => {
    setEditingTask(null);
    setFormData({
      projectAllocationId: allocations[0]?.id || "",
      taskTitle: "",
      taskDescription: "",
      taskStatusId: statuses[0]?.id || 1,
      taskPriorityId: priorities[1]?.id || 2,
      assignedScore: 10,
      taskStartDate: new Date().toISOString().split("T")[0],
      taskDueDate: new Date(Date.now() + 7 * 86400000)
        .toISOString()
        .split("T")[0],
      facultyRemarks: "",
    });
    setIsTaskModalOpen(true);
  };

  const handleOpenEdit = (task) => {
    setEditingTask(task);
    setFormData({
      projectAllocationId: task.projectAllocationId,
      taskTitle: task.taskTitle,
      taskDescription: task.taskDescription || "",
      taskStatusId: task.taskStatusId,
      taskPriorityId: task.taskPriorityId,
      assignedScore: task.assignedScore,
      taskStartDate: task.taskStartDate ? task.taskStartDate.split("T")[0] : "",
      taskDueDate: task.taskDueDate ? task.taskDueDate.split("T")[0] : "",
      facultyRemarks: task.facultyRemarks || "",
    });
    setIsTaskModalOpen(true);
  };

  const handleTaskSubmit = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        ...formData,
        projectAllocationId: parseInt(formData.projectAllocationId),
        taskStatusId: parseInt(formData.taskStatusId),
        taskPriorityId: parseInt(formData.taskPriorityId),
        assignedScore: parseFloat(formData.assignedScore),
      };

      if (editingTask) {
        await updateTaskApi(editingTask.id, {
          ...payload,
          earnedScore: editingTask.earnedScore,
          taskCompletedDate: editingTask.taskCompletedDate,
          nextFollowUpDate: editingTask.nextFollowUpDate,
          studentRemarks: editingTask.studentRemarks,
        });
        toast.success("Task updated successfully!");
      } else {
        await createTaskApi(payload);
        toast.success("Task item created and assigned!");
      }
      setIsTaskModalOpen(false);
      fetchTasks();
    } catch (err) {
      toast.error(err.response?.data?.message || "Error saving task.");
    }
  };

  const handleStatusChange = async (taskId, newStatusId) => {
    try {
      await updateTaskStatusApi(taskId, parseInt(newStatusId));
      toast.success("Task status updated.");
      fetchTasks();
    } catch (err) {
      toast.error("Failed to update status.");
    }
  };

  const handleOpenRemarkModal = (task) => {
    setSelectedTaskForRemark(task);
    setFacultyRemark(task.facultyRemarks || "");
    setStudentRemark(task.studentRemarks || "");
    setEarnedScore(task.earnedScore ?? 0);
  };

  const handleSaveRemarks = async (e) => {
    e.preventDefault();
    if (!selectedTaskForRemark) return;

    try {
      if (hasRole(["Admin", "Faculty"])) {
        await updateEarnedScoreApi(selectedTaskForRemark.id, {
          earnedScore: parseFloat(earnedScore),
          facultyRemarks: facultyRemark,
        });
      }
      if (hasRole("Student")) {
        await updateStudentRemarksApi(selectedTaskForRemark.id, studentRemark);
      }

      toast.success("Remarks / Score updated!");
      setSelectedTaskForRemark(null);
      fetchTasks();
    } catch (err) {
      toast.error("Failed to update remarks.");
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this task item?")) return;
    try {
      await deleteTaskApi(id);
      toast.success("Task deleted.");
      fetchTasks();
    } catch (err) {
      toast.error("Failed to delete task.");
    }
  };

  return (
    <div className="space-y-6 animate-fadeIn">
      {/* Header */}
      <div className="flex flex-col items-start justify-between gap-4 sm:flex-row sm:items-center">
        <div>
          <h1 className="text-2xl font-extrabold text-white sm:text-3xl">
            {isStudent ? "My Tasks & Milestones" : "Project Tasks & Milestones"}
          </h1>
          <p className="text-xs text-slate-400">
            {isStudent
              ? "Track your assigned tasks, update status, and submit remarks"
              : "Track tasks, priorities, score assignments & feedback"}
          </p>
        </div>
        {canManage && (
          <button
            onClick={handleOpenCreate}
            className="flex w-full items-center justify-center gap-2 rounded-xl bg-gradient-to-r from-indigo-600 to-purple-600 px-4 py-2 text-xs font-bold text-white shadow-lg shadow-indigo-600/30 transition-all hover:from-indigo-500 hover:to-purple-500 sm:w-auto"
          >
            <Plus className="w-4 h-4" /> Create Task Item
          </button>
        )}
      </div>

      {/* Filter Bar */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 bg-slate-800/60 p-4 rounded-2xl border border-slate-700/60 backdrop-blur-xl">
        <div className="relative">
          <Search className="w-4 h-4 text-slate-400 absolute left-3 top-3" />
          <input
            type="text"
            placeholder="Search task title or description..."
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
          {statuses.map((st) => (
            <option key={st.id} value={st.id}>
              {st.taskStatusName}
            </option>
          ))}
        </select>
        <select
          value={priorityFilter}
          onChange={(e) => setPriorityFilter(e.target.value)}
          className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
        >
          <option value="">All Priorities</option>
          {priorities.map((pr) => (
            <option key={pr.id} value={pr.id}>
              {pr.taskPriorityName}
            </option>
          ))}
        </select>
      </div>

      {/* Tasks Table */}
      {loading ? (
        <LoadingSpinner text="Fetching tasks..." />
      ) : (
        <div className="rounded-2xl border border-slate-700/60 bg-slate-800/60 backdrop-blur-xl">
          <div className="space-y-3 p-3 md:hidden">
            {tasks.map((t) => (
              <div
                key={t.id}
                className="rounded-xl border border-slate-700/60 bg-slate-950/35 p-4"
              >
                <div className="flex items-start justify-between gap-3">
                  <div className="min-w-0">
                    <p className="flex items-start gap-2 font-bold text-white">
                      <ListTodo className="mt-0.5 h-4 w-4 shrink-0 text-indigo-400" />
                      <span className="break-words">{t.taskTitle}</span>
                    </p>
                    <p className="mt-1 break-words text-[11px] text-slate-400">
                      {t.projectTitle}
                    </p>
                  </div>
                  <Badge
                    variant={
                      t.taskPriorityName === "Critical"
                        ? "danger"
                        : t.taskPriorityName === "High"
                          ? "warning"
                          : t.taskPriorityName === "Medium"
                            ? "info"
                            : "neutral"
                    }
                  >
                    {t.taskPriorityName}
                  </Badge>
                </div>
                <div className="mt-4 grid grid-cols-2 gap-3 text-xs">
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Student
                    </p>
                    <p className="mt-1 break-words font-semibold text-slate-200">
                      {t.studentName}
                    </p>
                  </div>
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Faculty
                    </p>
                    <p className="mt-1 break-words text-slate-300">
                      {t.facultyName}
                    </p>
                  </div>
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Score
                    </p>
                    <p className="mt-1 font-bold text-white">
                      <span className="text-emerald-400">
                        {t.earnedScore ?? 0}
                      </span>{" "}
                      / {t.assignedScore}
                    </p>
                  </div>
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Status
                    </p>
                    <select
                      value={t.taskStatusId}
                      onChange={(e) => handleStatusChange(t.id, e.target.value)}
                      className="mt-1 max-w-full rounded-lg border border-slate-800 bg-slate-950 px-2 py-1 text-xs font-semibold text-indigo-300 focus:outline-none"
                    >
                      {statuses.map((st) => (
                        <option key={st.id} value={st.id}>
                          {st.taskStatusName}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
                <div className="mt-4 flex items-center justify-between gap-2 border-t border-slate-800 pt-3">
                  <button
                    onClick={() => handleOpenRemarkModal(t)}
                    className="flex min-w-0 items-center gap-1 rounded-lg border border-slate-800 bg-slate-900 px-2 py-1.5 text-[11px] font-medium text-slate-300"
                  >
                    <MessageSquare className="h-3 w-3 shrink-0 text-indigo-400" />
                    <span className="truncate">Remarks & Score</span>
                  </button>
                  <div className="flex shrink-0 items-center gap-1">
                    {hasRole(["Admin", "Faculty"]) && (
                      <button
                        onClick={() => handleOpenEdit(t)}
                        className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-900 hover:text-indigo-400"
                      >
                        <Edit2 className="h-4 w-4" />
                      </button>
                    )}
                    {hasRole("Admin") && (
                      <button
                        onClick={() => handleDelete(t.id)}
                        className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-900 hover:text-rose-400"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    )}
                  </div>
                </div>
              </div>
            ))}
            {tasks.length === 0 && (
              <p className="p-5 text-center text-slate-500">
                No task items found.
              </p>
            )}
          </div>
          <div className="hidden overflow-x-auto md:block">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="text-[11px] font-bold uppercase text-slate-400 border-b border-slate-700/60 bg-slate-950/40">
                  <th className="p-4">Task & Project</th>
                  <th className="p-4">
                    {isStudent ? "Faculty Guide" : "Student & Faculty"}
                  </th>
                  <th className="p-4">Priority</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Score</th>
                  <th className="p-4">Remarks</th>
                  <th className="p-4 text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800 text-xs text-slate-300">
                {tasks.map((t) => (
                  <tr
                    key={t.id}
                    className="hover:bg-slate-800/40 transition-colors"
                  >
                    <td className="p-4 max-w-xs">
                      <p className="font-bold text-white flex items-center gap-2">
                        <ListTodo className="w-4 h-4 text-indigo-400 shrink-0" />
                        <span className="truncate">{t.taskTitle}</span>
                      </p>
                      <p className="text-[11px] text-slate-400 truncate">
                        {t.projectTitle}
                      </p>
                    </td>
                    <td className="p-4">
                      <p className="font-semibold text-white">
                        {t.studentName}
                      </p>
                      <p className="text-[11px] text-slate-400">
                        Guide: {t.facultyName}
                      </p>
                    </td>
                    <td className="p-4">
                      <Badge
                        variant={
                          t.taskPriorityName === "Critical"
                            ? "danger"
                            : t.taskPriorityName === "High"
                              ? "warning"
                              : t.taskPriorityName === "Medium"
                                ? "info"
                                : "neutral"
                        }
                      >
                        {t.taskPriorityName}
                      </Badge>
                    </td>
                    <td className="p-4">
                      <select
                        value={t.taskStatusId}
                        onChange={(e) =>
                          handleStatusChange(t.id, e.target.value)
                        }
                        className="px-2 py-1 bg-slate-950 border border-slate-800 rounded-lg text-xs font-semibold text-indigo-300 focus:outline-none"
                      >
                        {statuses.map((st) => (
                          <option key={st.id} value={st.id}>
                            {st.taskStatusName}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td className="p-4 font-bold text-white">
                      <span className="text-emerald-400">
                        {t.earnedScore ?? 0}
                      </span>{" "}
                      / {t.assignedScore}
                    </td>
                    <td className="p-4">
                      <button
                        onClick={() => handleOpenRemarkModal(t)}
                        className="flex items-center gap-1 text-[11px] font-medium text-slate-400 hover:text-white bg-slate-900 px-2 py-1 rounded-lg border border-slate-800"
                      >
                        <MessageSquare className="w-3 h-3 text-indigo-400" />
                        <span>Remarks & Score</span>
                      </button>
                    </td>
                    <td className="p-4 text-right">
                      <div className="flex items-center justify-end gap-2">
                        {hasRole(["Admin", "Faculty"]) && (
                          <button
                            onClick={() => handleOpenEdit(t)}
                            className="p-1.5 text-slate-400 hover:text-indigo-400 hover:bg-slate-900 rounded-lg transition-colors"
                          >
                            <Edit2 className="w-4 h-4" />
                          </button>
                        )}
                        {hasRole("Admin") && (
                          <button
                            onClick={() => handleDelete(t.id)}
                            className="p-1.5 text-slate-400 hover:text-rose-400 hover:bg-slate-900 rounded-lg transition-colors"
                          >
                            <Trash2 className="w-4 h-4" />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
                {tasks.length === 0 && (
                  <tr>
                    <td colSpan="7" className="p-8 text-center text-slate-500">
                      No task items found.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Task Form Modal */}
      <Modal
        isOpen={isTaskModalOpen}
        onClose={() => setIsTaskModalOpen(false)}
        title={editingTask ? "Edit Task Item" : "Create Task Item"}
      >
        <form onSubmit={handleTaskSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-semibold text-slate-400 mb-1">
              Project Allocation
            </label>
            <select
              value={formData.projectAllocationId}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  projectAllocationId: e.target.value,
                })
              }
              className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              required
            >
              <option value="">-- Select Project Allocation --</option>
              {allocations.map((a) => (
                <option key={a.id} value={a.id}>
                  {a.projectTitle} (Student: {a.studentName})
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-xs font-semibold text-slate-400 mb-1">
              Task Title
            </label>
            <input
              type="text"
              value={formData.taskTitle}
              onChange={(e) =>
                setFormData({ ...formData, taskTitle: e.target.value })
              }
              className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              required
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-slate-400 mb-1">
              Task Description
            </label>
            <textarea
              rows="3"
              value={formData.taskDescription}
              onChange={(e) =>
                setFormData({ ...formData, taskDescription: e.target.value })
              }
              className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Status
              </label>
              <select
                value={formData.taskStatusId}
                onChange={(e) =>
                  setFormData({ ...formData, taskStatusId: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              >
                {statuses.map((st) => (
                  <option key={st.id} value={st.id}>
                    {st.taskStatusName}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Priority
              </label>
              <select
                value={formData.taskPriorityId}
                onChange={(e) =>
                  setFormData({ ...formData, taskPriorityId: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              >
                {priorities.map((pr) => (
                  <option key={pr.id} value={pr.id}>
                    {pr.taskPriorityName}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Assigned Score
              </label>
              <input
                type="number"
                value={formData.assignedScore}
                onChange={(e) =>
                  setFormData({ ...formData, assignedScore: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                required
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Start Date
              </label>
              <input
                type="date"
                value={formData.taskStartDate}
                onChange={(e) =>
                  setFormData({ ...formData, taskStartDate: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              />
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Due Date
              </label>
              <input
                type="date"
                value={formData.taskDueDate}
                onChange={(e) =>
                  setFormData({ ...formData, taskDueDate: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              />
            </div>
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
            <button
              type="button"
              onClick={() => setIsTaskModalOpen(false)}
              className="px-4 py-2 text-xs text-slate-400 hover:text-white"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-4 py-2 text-xs font-bold text-white bg-indigo-600 hover:bg-indigo-500 rounded-xl shadow-md shadow-indigo-600/30"
            >
              Save Task Item
            </button>
          </div>
        </form>
      </Modal>

      {/* Remarks & Score Modal */}
      <Modal
        isOpen={!!selectedTaskForRemark}
        onClose={() => setSelectedTaskForRemark(null)}
        title={`Task Feedback & Scoring (${selectedTaskForRemark?.taskTitle})`}
        maxWidth="max-w-md"
      >
        <form onSubmit={handleSaveRemarks} className="space-y-4">
          {hasRole(["Admin", "Faculty"]) && (
            <>
              <div>
                <label className="block text-xs font-semibold text-slate-400 mb-1">
                  Earned Score (Max: {selectedTaskForRemark?.assignedScore})
                </label>
                <input
                  type="number"
                  step="0.5"
                  max={selectedTaskForRemark?.assignedScore}
                  value={earnedScore}
                  onChange={(e) => setEarnedScore(e.target.value)}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-xs font-semibold text-slate-400 mb-1">
                  Faculty Remarks
                </label>
                <textarea
                  rows="3"
                  value={facultyRemark}
                  onChange={(e) => setFacultyRemark(e.target.value)}
                  placeholder="Enter guide feedback..."
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                />
              </div>
            </>
          )}

          {hasRole("Student") && (
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Student Remarks
              </label>
              <textarea
                rows="3"
                value={studentRemark}
                onChange={(e) => setStudentRemark(e.target.value)}
                placeholder="Enter progress notes or questions..."
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              />
            </div>
          )}

          <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
            <button
              type="button"
              onClick={() => setSelectedTaskForRemark(null)}
              className="px-4 py-2 text-xs text-slate-400 hover:text-white"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-4 py-2 text-xs font-bold text-white bg-indigo-600 hover:bg-indigo-500 rounded-xl"
            >
              Save Feedback
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
