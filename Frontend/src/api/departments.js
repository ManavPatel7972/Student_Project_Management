import api from './axios';

export const getDepartmentsApi = async () => {
  const response = await api.get('/departments');
  return response.data;
};

export const getDepartmentByIdApi = async (id) => {
  const response = await api.get(`/departments/${id}`);
  return response.data;
};

export const createDepartmentApi = async (data) => {
  const response = await api.post('/departments', data);
  return response.data;
};

export const updateDepartmentApi = async (id, data) => {
  const response = await api.put(`/departments/${id}`, data);
  return response.data;
};

export const deleteDepartmentApi = async (id) => {
  const response = await api.delete(`/departments/${id}`);
  return response.data;
};
