import api from './axios';

export const getRolesApi = async () => {
  const response = await api.get('/roles');
  return response.data;
};

export const getRoleByIdApi = async (id) => {
  const response = await api.get(`/roles/${id}`);
  return response.data;
};

export const createRoleApi = async (data) => {
  const response = await api.post('/roles', data);
  return response.data;
};

export const updateRoleApi = async (id, data) => {
  const response = await api.put(`/roles/${id}`, data);
  return response.data;
};

export const deleteRoleApi = async (id) => {
  const response = await api.delete(`/roles/${id}`);
  return response.data;
};
