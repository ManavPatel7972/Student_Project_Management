import React, { useState, useEffect } from "react";
import {
  getAllocationsApi,
  createAllocationApi,
  updateAllocationApi,
  deleteAllocationApi,
} from "../../api/allocations";
import { getProjectsApi } from "../../api/projects";
import { getUsersApi } from "../../api/users";
import { Modal } from "../../components/common/Modal";
import { Badge } from "../../components/common/Badge";
import { LoadingSpinner } from "../../components/common/LoadingSpinner";
import {
  Plus,
  Edit2,
  Trash2,
  Briefcase,
  GraduationCap,
  Calendar,
  Award,
} from "lucide-react";
import toast from "react-hot-toast";
import { useAuth } from "../../context/AuthContext";

export const AllocationsPage = () => {
  const { hasRole } = useAuth();
  const isStudent = hasRole("Student");
  const canManage = hasRole(["Admin", "Faculty"]);
  const [allocations, setAllocations] = useState([]);
  const [projects, setProjects] = useState([]);
  const [students, setStudents] = useState([]);
  const [facultyList, setFacultyList] = useState([]);
  const [loading, setLoading] = useState(true);

  // Modal State
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingAllocation, setEditingAllocation] = useState(null);
  const [formData, setFormData] = useState({
    projectId: "",
    studentId: "",
    facultyId: "",
    projectStartDate: new Date().toISOString().split("T")[0],
    projectEndDate: new Date(Date.now() + 90 * 86400000)
      .toISOString()
      .split("T")[0],
    overAllGrade: "A",
  });

  useEffect(() => {
    fetchLookups();
    fetchAllocations();
  }, []);

  const fetchLookups = async () => {
    if (isStudent) return;
    try {
      const [projsData, studsData, facsData] = await Promise.all([
        getProjectsApi(),
        getUsersApi({ userTypeId: 3 }), // Students
        getUsersApi({ userTypeId: 2 }), // Faculty
      ]);
      setProjects(projsData);
      setStudents(studsData);
      setFacultyList(facsData);
    } catch (err) {
      console.error("Failed to load lookup data:", err);
    }
  };

  const fetchAllocations = async () => {
    try {
      setLoading(true);
      const data = await getAllocationsApi();
      setAllocations(data);
    } catch (err) {
      toast.error("Failed to load allocations.");
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreate = () => {
    setEditingAllocation(null);
    setFormData({
      projectId: projects[0]?.id || "",
      studentId: students[0]?.id || "",
      facultyId: facultyList[0]?.id || "",
      projectStartDate: new Date().toISOString().split("T")[0],
      projectEndDate: new Date(Date.now() + 90 * 86400000)
        .toISOString()
        .split("T")[0],
      overAllGrade: "",
    });
    setIsModalOpen(true);
  };

  const handleOpenEdit = (allocation) => {
    setEditingAllocation(allocation);
    setFormData({
      projectId: allocation.projectId,
      studentId: allocation.studentId,
      facultyId: allocation.facultyId,
      projectStartDate: allocation.projectStartDate
        ? allocation.projectStartDate.split("T")[0]
        : "",
      projectEndDate: allocation.projectEndDate
        ? allocation.projectEndDate.split("T")[0]
        : "",
      overAllGrade: allocation.overAllGrade || "",
    });
    setIsModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingAllocation) {
        await updateAllocationApi(editingAllocation.id, {
          projectStartDate: formData.projectStartDate,
          projectEndDate: formData.projectEndDate,
          overAllGrade: formData.overAllGrade || null,
        });
        toast.success("Allocation updated successfully!");
      } else {
        await createAllocationApi({
          projectId: parseInt(formData.projectId),
          studentId: parseInt(formData.studentId),
          facultyId: parseInt(formData.facultyId),
          projectStartDate: formData.projectStartDate,
          projectEndDate: formData.projectEndDate,
        });
        toast.success("Student allocated to project!");
      }
      setIsModalOpen(false);
      fetchAllocations();
    } catch (err) {
      toast.error(err.response?.data?.message || "Error saving allocation.");
    }
  };

  const handleDelete = async (id) => {
    if (
      !window.confirm(
        "Are you sure you want to remove this project allocation?",
      )
    )
      return;
    try {
      await deleteAllocationApi(id);
      toast.success("Allocation deleted.");
      fetchAllocations();
    } catch (err) {
      toast.error("Failed to delete allocation.");
    }
  };

  return (
    <div className="space-y-6 animate-fadeIn">
      {/* Header */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-extrabold text-white">
            {isStudent ? "My Project Allocations" : "Project Allocations"}
          </h1>
          <p className="text-xs text-slate-400">
            {isStudent
              ? "View your assigned projects, faculty guide, and progress"
              : "Map students to projects and assign faculty guides"}
          </p>
        </div>
        {canManage && (
          <button
            onClick={handleOpenCreate}
            className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-indigo-600 to-purple-600 hover:from-indigo-500 hover:to-purple-500 text-white text-xs font-bold rounded-xl shadow-lg shadow-indigo-600/30 transition-all"
          >
            <Plus className="w-4 h-4" /> Allocate Student
          </button>
        )}
      </div>

      {/* Allocations Table */}
      {loading ? (
        <LoadingSpinner text="Fetching allocations..." />
      ) : (
        <div className="rounded-2xl border border-slate-700/60 bg-slate-800/60 backdrop-blur-xl">
          <div className="space-y-3 p-3 md:hidden">
            {allocations.map((a) => (
              <div
                key={a.id}
                className="rounded-xl border border-slate-700/60 bg-slate-950/35 p-4"
              >
                <div className="flex items-start justify-between gap-3">
                  <div className="min-w-0">
                    <p className="flex items-start gap-2 break-words font-bold text-white">
                      <Briefcase className="mt-0.5 h-4 w-4 shrink-0 text-indigo-400" />
                      {a.projectTitle}
                    </p>
                    <p className="mt-1 text-[10px] font-semibold uppercase text-slate-500">
                      {a.projectStatus}
                    </p>
                  </div>
                  <Badge variant={a.overAllGrade ? "success" : "neutral"}>
                    {a.overAllGrade || "Not Graded"}
                  </Badge>
                </div>
                <div className="mt-4 grid grid-cols-2 gap-3 text-xs">
                  {!isStudent && (
                    <div>
                      <p className="text-[10px] uppercase text-slate-500">
                        Student
                      </p>
                      <p className="mt-1 break-words font-semibold text-slate-200">
                        {a.studentName}
                      </p>
                      <p className="text-[11px] text-slate-400">
                        {a.studentCode || "No Code"}
                      </p>
                    </div>
                  )}
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Faculty guide
                    </p>
                    <p className="mt-1 break-words font-semibold text-slate-200">
                      {a.facultyName}
                    </p>
                  </div>
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Duration
                    </p>
                    <p className="mt-1 text-slate-300">
                      {a.projectStartDate
                        ? new Date(a.projectStartDate).toLocaleDateString()
                        : "-"}{" "}
                      -{" "}
                      {a.projectEndDate
                        ? new Date(a.projectEndDate).toLocaleDateString()
                        : "-"}
                    </p>
                  </div>
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Progress
                    </p>
                    <div className="mt-2 flex items-center gap-2">
                      <div className="h-2 min-w-0 flex-1 overflow-hidden rounded-full border border-slate-800 bg-slate-950">
                        <div
                          className="h-full rounded-full bg-indigo-500"
                          style={{ width: `${a.progressPercentage}%` }}
                        />
                      </div>
                      <span className="shrink-0 text-[10px] font-bold">
                        {Math.round(a.progressPercentage)}%
                      </span>
                    </div>
                  </div>
                </div>
                {canManage && (
                  <div className="mt-4 flex justify-end gap-1 border-t border-slate-800 pt-3">
                    <button
                      onClick={() => handleOpenEdit(a)}
                      className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-900 hover:text-indigo-400"
                      title="Edit Allocation / Grade"
                    >
                      <Edit2 className="h-4 w-4" />
                    </button>
                    {hasRole("Admin") && (
                      <button
                        onClick={() => handleDelete(a.id)}
                        className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-900 hover:text-rose-400"
                        title="Delete Allocation"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    )}
                  </div>
                )}
              </div>
            ))}
            {allocations.length === 0 && (
              <p className="p-5 text-center text-slate-500">
                {isStudent
                  ? "You have no project allocations yet. Contact your faculty guide."
                  : "No project allocations found."}
              </p>
            )}
          </div>
          <div className="hidden overflow-x-auto md:block">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="text-[11px] font-bold uppercase text-slate-400 border-b border-slate-700/60 bg-slate-950/40">
                  <th className="p-4">Project Title</th>
                  {!isStudent && <th className="p-4">Assigned Student</th>}
                  <th className="p-4">Faculty Guide</th>
                  <th className="p-4">Duration</th>
                  <th className="p-4">Task Progress</th>
                  <th className="p-4">Grade</th>
                  {canManage && <th className="p-4 text-right">Actions</th>}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800 text-xs text-slate-300">
                {allocations.map((a) => (
                  <tr
                    key={a.id}
                    className="hover:bg-slate-800/40 transition-colors"
                  >
                    <td className="p-4">
                      <p className="font-bold text-white flex items-center gap-2">
                        <Briefcase className="w-4 h-4 text-indigo-400 shrink-0" />
                        {a.projectTitle}
                      </p>
                      <span className="text-[10px] text-slate-500 uppercase font-semibold">
                        {a.projectStatus}
                      </span>
                    </td>
                    {!isStudent && (
                      <td className="p-4">
                        <p className="font-semibold text-white flex items-center gap-1.5">
                          <GraduationCap className="w-3.5 h-3.5 text-emerald-400" />
                          {a.studentName}
                        </p>
                        <p className="text-[11px] text-slate-400">
                          {a.studentCode || "No Code"}
                        </p>
                      </td>
                    )}
                    <td className="p-4 font-medium text-slate-300">
                      {a.facultyName}
                    </td>
                    <td className="p-4">
                      <p className="text-[11px] text-slate-300 flex items-center gap-1">
                        <Calendar className="w-3 h-3 text-slate-500" />
                        {a.projectStartDate
                          ? new Date(a.projectStartDate).toLocaleDateString()
                          : "-"}
                      </p>
                      <p className="text-[11px] text-slate-500">
                        to{" "}
                        {a.projectEndDate
                          ? new Date(a.projectEndDate).toLocaleDateString()
                          : "-"}
                      </p>
                    </td>
                    <td className="p-4">
                      <div className="flex items-center gap-2">
                        <div className="w-20 bg-slate-950 h-2 rounded-full overflow-hidden border border-slate-800">
                          <div
                            className="bg-indigo-500 h-full rounded-full"
                            style={{ width: `${a.progressPercentage}%` }}
                          ></div>
                        </div>
                        <span className="text-[10px] font-bold">
                          {a.totalCompletedTasks}/{a.totalTasksGiven} (
                          {Math.round(a.progressPercentage)}%)
                        </span>
                      </div>
                    </td>
                    <td className="p-4">
                      <Badge variant={a.overAllGrade ? "success" : "neutral"}>
                        {a.overAllGrade || "Not Graded"}
                      </Badge>
                    </td>
                    {canManage && (
                      <td className="p-4 text-right">
                        <div className="flex items-center justify-end gap-2">
                          <button
                            onClick={() => handleOpenEdit(a)}
                            className="p-1.5 text-slate-400 hover:text-indigo-400 hover:bg-slate-900 rounded-lg transition-colors"
                            title="Edit Allocation / Grade"
                          >
                            <Edit2 className="w-4 h-4" />
                          </button>
                          {hasRole("Admin") && (
                            <button
                              onClick={() => handleDelete(a.id)}
                              className="p-1.5 text-slate-400 hover:text-rose-400 hover:bg-slate-900 rounded-lg transition-colors"
                              title="Delete Allocation"
                            >
                              <Trash2 className="w-4 h-4" />
                            </button>
                          )}
                        </div>
                      </td>
                    )}
                  </tr>
                ))}
                {allocations.length === 0 && (
                  <tr>
                    <td
                      colSpan={isStudent ? 5 : 7}
                      className="p-8 text-center text-slate-500"
                    >
                      {isStudent
                        ? "You have no project allocations yet. Contact your faculty guide."
                        : "No project allocations found."}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Allocation Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={
          editingAllocation
            ? "Edit Allocation & Grade"
            : "New Project Allocation"
        }
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          {!editingAllocation ? (
            <>
              <div>
                <label className="block text-xs font-semibold text-slate-400 mb-1">
                  Select Project
                </label>
                <select
                  value={formData.projectId}
                  onChange={(e) =>
                    setFormData({ ...formData, projectId: e.target.value })
                  }
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                  required
                >
                  <option value="">-- Choose Project --</option>
                  {projects.map((p) => (
                    <option key={p.id} value={p.id}>
                      {p.projectTitle}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-xs font-semibold text-slate-400 mb-1">
                  Select Student
                </label>
                <select
                  value={formData.studentId}
                  onChange={(e) =>
                    setFormData({ ...formData, studentId: e.target.value })
                  }
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                  required
                >
                  <option value="">-- Choose Student --</option>
                  {students.map((s) => (
                    <option key={s.id} value={s.id}>
                      {s.fullName} ({s.userCode || "No Code"})
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-xs font-semibold text-slate-400 mb-1">
                  Select Faculty Guide
                </label>
                <select
                  value={formData.facultyId}
                  onChange={(e) =>
                    setFormData({ ...formData, facultyId: e.target.value })
                  }
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                  required
                >
                  <option value="">-- Choose Faculty Guide --</option>
                  {facultyList.map((f) => (
                    <option key={f.id} value={f.id}>
                      {f.fullName}
                    </option>
                  ))}
                </select>
              </div>
            </>
          ) : (
            <div className="p-3 bg-slate-950/60 rounded-xl border border-slate-800 mb-2">
              <p className="text-xs text-slate-400 font-semibold">
                Editing Project Allocation for:
              </p>
              <p className="text-sm font-bold text-white mt-0.5">
                {editingAllocation.projectTitle}
              </p>
              <p className="text-xs text-indigo-400 font-medium">
                Student: {editingAllocation.studentName}
              </p>
            </div>
          )}

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Start Date
              </label>
              <input
                type="date"
                value={formData.projectStartDate}
                onChange={(e) =>
                  setFormData({ ...formData, projectStartDate: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                required
              />
            </div>
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                End Date
              </label>
              <input
                type="date"
                value={formData.projectEndDate}
                onChange={(e) =>
                  setFormData({ ...formData, projectEndDate: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
                required
              />
            </div>
          </div>

          {editingAllocation && (
            <div>
              <label className="block text-xs font-semibold text-slate-400 mb-1">
                Overall Grade
              </label>
              <select
                value={formData.overAllGrade}
                onChange={(e) =>
                  setFormData({ ...formData, overAllGrade: e.target.value })
                }
                className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-indigo-500"
              >
                <option value="">Not Graded</option>
                <option value="A+">A+ (Outstanding)</option>
                <option value="A">A (Excellent)</option>
                <option value="B">B (Good)</option>
                <option value="C">C (Satisfactory)</option>
                <option value="D">D (Pass)</option>
                <option value="F">F (Fail)</option>
              </select>
            </div>
          )}

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
              Save Allocation
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
