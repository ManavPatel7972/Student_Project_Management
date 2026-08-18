import api from './axios';

export const getPermissionMatrixApi = async () => {
  const response = await api.get('/permissions');
  return response.data;
};

export const togglePermissionApi = async (data) => {
  const response = await api.put('/permissions/toggle', data);
  return response.data;
};
