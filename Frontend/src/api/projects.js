import api from './axios';

export const getProjectsApi = async (params) => {
  const response = await api.get('/projects', { params });
  return response.data;
};

export const getProjectByIdApi = async (id) => {
  const response = await api.get(`/projects/${id}`);
  return response.data;
};

export const createProjectApi = async (data) => {
  const response = await api.post('/projects', data);
  return response.data;
};

export const updateProjectApi = async (id, data) => {
  const response = await api.put(`/projects/${id}`, data);
  return response.data;
};

export const updateProjectStatusApi = async (id, status) => {
  const response = await api.patch(`/projects/${id}/status`, { status });
  return response.data;
};

export const deleteProjectApi = async (id) => {
  const response = await api.delete(`/projects/${id}`);
  return response.data;
};
