import React from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import { ProtectedRoute } from "./ProtectedRoute";
import { Layout } from "../components/layout/Layout";
import { LoginPage } from "../pages/auth/LoginPage";
import { RegisterPage } from "../pages/auth/RegisterPage";
import { DashboardPage } from "../pages/dashboard/DashboardPage";
import { UsersPage } from "../pages/users/UsersPage";
import { StudentsPage } from "../pages/students/StudentsPage";
import { FacultyPage } from "../pages/faculty/FacultyPage";
import { ProjectsPage } from "../pages/projects/ProjectsPage";
import { AllocationsPage } from "../pages/allocations/AllocationsPage";
import { TasksPage } from "../pages/tasks/TasksPage";
import { ScoresPage } from "../pages/scores/ScoresPage";
import { DepartmentsPage } from "../pages/departments/DepartmentsPage";
import { RolesPage } from "../pages/roles/RolesPage";
import { NotFoundPage } from "../pages/NotFoundPage";
import { User } from "lucide-react";

export const AppRouter = () => {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* Protected Routes inside Layout */}
      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/projects" element={<ProjectsPage />} />
          <Route path="/allocations" element={<AllocationsPage />} />
          <Route path="/tasks" element={<TasksPage />} />
          <Route path="/scores" element={<ScoresPage />} />

          {/* Admin / Faculty Routes */}
          <Route element={<ProtectedRoute roles={["Admin", "Faculty"]} />}>
            <Route path="/students" element={<StudentsPage />} />
          </Route>

          {/* Admin Only Routes */}
          <Route element={<ProtectedRoute roles={["Admin"]} />}>
            <Route path="/faculty" element={<FacultyPage />} />
            <Route path="/users" element={<UsersPage />} />
            <Route path="/departments" element={<DepartmentsPage />} />
            <Route path="/roles" element={<RolesPage />} />
          </Route>

          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Route>
    </Routes>
  );
};
