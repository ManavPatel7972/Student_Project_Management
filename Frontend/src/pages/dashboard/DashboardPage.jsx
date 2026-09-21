import React, { useEffect, useState } from "react";
import {
  getAdminDashboardApi,
  getStudentDashboardApi,
  getFacultyDashboardApi,
} from "../../api/dashboard";
import { StatCard } from "../../components/common/StatCard";
import { Badge } from "../../components/common/Badge";
import { LoadingSpinner } from "../../components/common/LoadingSpinner";
import { useAuth } from "../../context/AuthContext";
import {
  Users,
  GraduationCap,
  Briefcase,
  FolderGit2,
  ListTodo,
  CheckCircle2,
  Clock,
  AlertCircle,
} from "lucide-react";

export const DashboardPage = () => {
  const { user } = useAuth();
  const [dashboardData, setDashboardData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStats();
  }, [user?.role]);

  const fetchStats = async () => {
    try {
      setLoading(true);
      let data = null;
      const role = user?.role?.toLowerCase();

      if (role === "admin") {
        data = await getAdminDashboardApi();
      } else if (role === "student") {
        data = await getStudentDashboardApi();
      } else if (role === "faculty") {
        data = await getFacultyDashboardApi();
      }

      setDashboardData(data);
    } catch (err) {
      console.error("Failed to load dashboard stats:", err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <LoadingSpinner text="Loading Dashboard Overview..." />;
  if (!dashboardData)
    return <div className="text-slate-400">Failed to load statistics.</div>;

  // The backend wraps data in: { overview: {...}, recentAllocations: [...], recentTasks: [...] }
  const overview = dashboardData.overview || {};
  const recentAllocations = dashboardData.recentAllocations || [];
  const recentTasks = dashboardData.recentTasks || [];
  const isAdmin = user?.role?.toLowerCase() === "admin";

  return (
    <div className="space-y-8 animate-fadeIn">
      {/* Welcome Banner */}
      <div className="bg-gradient-to-r from-indigo-900/40 via-purple-900/40 to-slate-900 border border-indigo-500/20 rounded-3xl p-6 md:p-8 backdrop-blur-xl relative overflow-hidden">
        <div className="relative z-10">
          <span className="inline-block px-3 py-1 bg-indigo-500/20 text-indigo-300 font-semibold text-xs rounded-full border border-indigo-500/30 mb-2 uppercase tracking-wider">
            {user?.role} Portal
          </span>
          <h1 className="text-2xl md:text-3xl font-extrabold text-white">
            Welcome back, {user?.fullName}!
          </h1>
          <p className="text-slate-400 text-sm mt-1 max-w-2xl">
            Here is your live overview of projects, allocations, tasks, and
            system performance.
          </p>
        </div>
      </div>

      {/* Main Metric Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
        {isAdmin && (
          <>
            <StatCard
              title="Total Users"
              value={overview.totalUsers}
              icon={Users}
              color="indigo"
            />
            <StatCard
              title="Students"
              value={overview.totalStudents}
              icon={GraduationCap}
              color="emerald"
            />
            <StatCard
              title="Faculty"
              value={overview.totalFaculty}
              icon={Users}
              color="purple"
            />
          </>
        )}
        <StatCard
          title="Projects"
          value={overview.totalProjects}
          icon={FolderGit2}
          color="cyan"
        />
        <StatCard
          title="Allocations"
          value={overview.totalAllocations}
          icon={Briefcase}
          color="amber"
        />
        <StatCard
          title="Tasks"
          value={overview.totalTasks}
          icon={ListTodo}
          color="rose"
        />
      </div>

      {/* Status Breakdown Grid */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-slate-800/60 border border-slate-700/60 rounded-2xl p-6 backdrop-blur-xl">
          <div className="flex items-center justify-between mb-4">
            <h3 className="font-bold text-white text-base">
              Projects Breakdown
            </h3>
            <FolderGit2 className="w-5 h-5 text-indigo-400" />
          </div>
          <div className="space-y-3">
            <div className="flex items-center justify-between p-2.5 rounded-xl bg-slate-900/60">
              <span className="text-xs text-slate-400 flex items-center gap-2">
                <Clock className="w-3.5 h-3.5 text-amber-400" /> Not Started
              </span>
              <span className="font-bold text-white text-sm">
                {overview.projectsNotStarted ?? 0}
              </span>
            </div>
            <div className="flex items-center justify-between p-2.5 rounded-xl bg-slate-900/60">
              <span className="text-xs text-slate-400 flex items-center gap-2">
                <AlertCircle className="w-3.5 h-3.5 text-indigo-400" /> In
                Progress
              </span>
              <span className="font-bold text-white text-sm">
                {overview.projectsInProgress ?? 0}
              </span>
            </div>
            <div className="flex items-center justify-between p-2.5 rounded-xl bg-slate-900/60">
              <span className="text-xs text-slate-400 flex items-center gap-2">
                <CheckCircle2 className="w-3.5 h-3.5 text-emerald-400" />{" "}
                Completed
              </span>
              <span className="font-bold text-white text-sm">
                {overview.projectsCompleted ?? 0}
              </span>
            </div>
          </div>
        </div>

        <div className="bg-slate-800/60 border border-slate-700/60 rounded-2xl p-6 backdrop-blur-xl">
          <div className="flex items-center justify-between mb-4">
            <h3 className="font-bold text-white text-base">Task Statuses</h3>
            <ListTodo className="w-5 h-5 text-emerald-400" />
          </div>
          <div className="space-y-3">
            <div className="flex items-center justify-between p-2.5 rounded-xl bg-slate-900/60">
              <span className="text-xs text-slate-400">Pending</span>
              <Badge variant="warning">{overview.tasksPending ?? 0}</Badge>
            </div>
            <div className="flex items-center justify-between p-2.5 rounded-xl bg-slate-900/60">
              <span className="text-xs text-slate-400">In Progress</span>
              <Badge variant="info">{overview.tasksInProgress ?? 0}</Badge>
            </div>
            <div className="flex items-center justify-between p-2.5 rounded-xl bg-slate-900/60">
              <span className="text-xs text-slate-400">Completed</span>
              <Badge variant="success">{overview.tasksCompleted ?? 0}</Badge>
            </div>
          </div>
        </div>

        <div className="bg-slate-800/60 border border-slate-700/60 rounded-2xl p-6 backdrop-blur-xl">
          <div className="flex items-center justify-between mb-4">
            <h3 className="font-bold text-white text-base">Task Priorities</h3>
            <AlertCircle className="w-5 h-5 text-rose-400" />
          </div>
          <div className="grid grid-cols-2 gap-2">
            <div className="p-3 bg-slate-900/60 rounded-xl text-center">
              <p className="text-[10px] text-slate-400 font-bold uppercase">
                Low
              </p>
              <p className="text-lg font-extrabold text-slate-300">
                {overview.tasksLow ?? 0}
              </p>
            </div>
            <div className="p-3 bg-slate-900/60 rounded-xl text-center">
              <p className="text-[10px] text-indigo-400 font-bold uppercase">
                Medium
              </p>
              <p className="text-lg font-extrabold text-indigo-300">
                {overview.tasksMedium ?? 0}
              </p>
            </div>
            <div className="p-3 bg-slate-900/60 rounded-xl text-center">
              <p className="text-[10px] text-amber-400 font-bold uppercase">
                High
              </p>
              <p className="text-lg font-extrabold text-amber-300">
                {overview.tasksHigh ?? 0}
              </p>
            </div>
            <div className="p-3 bg-slate-900/60 rounded-xl text-center">
              <p className="text-[10px] text-rose-400 font-bold uppercase">
                Critical
              </p>
              <p className="text-lg font-extrabold text-rose-400">
                {overview.tasksCritical ?? 0}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Allocations Table */}
      <div className="rounded-2xl border border-slate-700/60 bg-slate-800/60 p-4 backdrop-blur-xl md:p-6">
        <h3 className="font-bold text-white text-lg mb-4">
          Recent Project Allocations
        </h3>
        <div className="space-y-3 md:hidden">
          {recentAllocations.map((item) => (
            <div
              key={item.id}
              className="rounded-xl border border-slate-700/60 bg-slate-950/35 p-4"
            >
              <div className="flex items-start justify-between gap-3">
                <div className="min-w-0">
                  <p className="break-words font-semibold text-white">
                    {item.projectTitle}
                  </p>
                  <p className="mt-1 break-words text-xs text-slate-400">
                    {item.studentName} ({item.studentCode || "N/A"})
                  </p>
                </div>
                <Badge
                  variant={item.overAllGrade === "A" ? "success" : "neutral"}
                >
                  {item.overAllGrade || "Pending"}
                </Badge>
              </div>
              <p className="mt-3 text-xs text-slate-400">
                Faculty:{" "}
                <span className="text-slate-200">{item.facultyName}</span>
              </p>
              <div className="mt-3 flex items-center gap-2">
                <div className="h-2 min-w-0 flex-1 overflow-hidden rounded-full bg-slate-900">
                  <div
                    className="h-full rounded-full bg-indigo-500"
                    style={{ width: `${item.progressPercentage}%` }}
                  />
                </div>
                <span className="shrink-0 text-[10px] font-bold text-slate-300">
                  {Math.round(item.progressPercentage)}%
                </span>
              </div>
            </div>
          ))}
          {recentAllocations.length === 0 && (
            <p className="py-4 text-center text-slate-500">
              No recent allocations found.
            </p>
          )}
        </div>
        <div className="hidden overflow-x-auto md:block">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="text-[11px] font-bold uppercase text-slate-400 border-b border-slate-700/60">
                <th className="pb-3">Project</th>
                <th className="pb-3">Student</th>
                <th className="pb-3">Faculty</th>
                <th className="pb-3">Progress</th>
                <th className="pb-3">Grade</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-800 text-xs text-slate-300">
              {recentAllocations.map((item) => (
                <tr key={item.id} className="hover:bg-slate-800/40">
                  <td className="py-3 font-semibold text-white">
                    {item.projectTitle}
                  </td>
                  <td className="py-3">
                    {item.studentName} ({item.studentCode || "N/A"})
                  </td>
                  <td className="py-3">{item.facultyName}</td>
                  <td className="py-3">
                    <div className="flex items-center gap-2">
                      <div className="w-24 bg-slate-900 h-2 rounded-full overflow-hidden">
                        <div
                          className="bg-indigo-500 h-full rounded-full"
                          style={{ width: `${item.progressPercentage}%` }}
                        ></div>
                      </div>
                      <span className="text-[10px] font-bold">
                        {Math.round(item.progressPercentage)}%
                      </span>
                    </div>
                  </td>
                  <td className="py-3">
                    <Badge
                      variant={
                        item.overAllGrade === "A" ? "success" : "neutral"
                      }
                    >
                      {item.overAllGrade || "Pending"}
                    </Badge>
                  </td>
                </tr>
              ))}
              {recentAllocations.length === 0 && (
                <tr>
                  <td colSpan="5" className="py-4 text-center text-slate-500">
                    No recent allocations found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Recent Tasks Table */}
      {recentTasks.length > 0 && (
        <div className="rounded-2xl border border-slate-700/60 bg-slate-800/60 p-4 backdrop-blur-xl md:p-6">
          <h3 className="font-bold text-white text-lg mb-4">Recent Tasks</h3>
          <div className="space-y-3 md:hidden">
            {recentTasks.map((task) => (
              <div
                key={task.id}
                className="rounded-xl border border-slate-700/60 bg-slate-950/35 p-4"
              >
                <p className="break-words font-semibold text-white">
                  {task.taskTitle}
                </p>
                <p className="mt-1 break-words text-xs text-slate-400">
                  {task.projectTitle}
                </p>
                <div className="mt-3 grid grid-cols-2 gap-3 text-xs">
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Student
                    </p>
                    <p className="mt-1 break-words text-slate-200">
                      {task.studentName}
                    </p>
                  </div>
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Status
                    </p>
                    <div className="mt-1">
                      <Badge variant="info">{task.taskStatusName}</Badge>
                    </div>
                  </div>
                  <div>
                    <p className="text-[10px] uppercase text-slate-500">
                      Priority
                    </p>
                    <div className="mt-1">
                      <Badge
                        variant={
                          task.taskPriorityName === "Critical"
                            ? "danger"
                            : task.taskPriorityName === "High"
                              ? "warning"
                              : "neutral"
                        }
                      >
                        {task.taskPriorityName}
                      </Badge>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
          <div className="hidden overflow-x-auto md:block">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="text-[11px] font-bold uppercase text-slate-400 border-b border-slate-700/60">
                  <th className="pb-3">Task</th>
                  <th className="pb-3">Project</th>
                  <th className="pb-3">Student</th>
                  <th className="pb-3">Status</th>
                  <th className="pb-3">Priority</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800 text-xs text-slate-300">
                {recentTasks.map((task) => (
                  <tr key={task.id} className="hover:bg-slate-800/40">
                    <td className="py-3 font-semibold text-white">
                      {task.taskTitle}
                    </td>
                    <td className="py-3">{task.projectTitle}</td>
                    <td className="py-3">{task.studentName}</td>
                    <td className="py-3">
                      <Badge variant="info">{task.taskStatusName}</Badge>
                    </td>
                    <td className="py-3">
                      <Badge
                        variant={
                          task.taskPriorityName === "Critical"
                            ? "danger"
                            : task.taskPriorityName === "High"
                              ? "warning"
                              : "neutral"
                        }
                      >
                        {task.taskPriorityName}
                      </Badge>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};
