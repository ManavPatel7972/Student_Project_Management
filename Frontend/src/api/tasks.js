import api from './axios';

export const getTasksApi = async (params) => {
  const response = await api.get('/tasks', { params });
  return response.data;
};

export const getTaskByIdApi = async (id) => {
  const response = await api.get(`/tasks/${id}`);
  return response.data;
};

export const createTaskApi = async (data) => {
  const response = await api.post('/tasks', data);
  return response.data;
};

export const updateTaskApi = async (id, data) => {
  const response = await api.put(`/tasks/${id}`, data);
  return response.data;
};

export const updateTaskStatusApi = async (id, taskStatusId) => {
  const response = await api.patch(`/tasks/${id}/status`, { taskStatusId });
  return response.data;
};

export const updateEarnedScoreApi = async (id, data) => {
  const response = await api.patch(`/tasks/${id}/earned-score`, data);
  return response.data;
};

export const updateStudentRemarksApi = async (id, studentRemarks) => {
  const response = await api.patch(`/tasks/${id}/student-remarks`, { studentRemarks });
  return response.data;
};

export const deleteTaskApi = async (id) => {
  const response = await api.delete(`/tasks/${id}`);
  return response.data;
};
