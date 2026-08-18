import api from './axios';

export const getUsersApi = async (params) => {
  const response = await api.get('/users', { params });
  return response.data;
};

export const getUserByIdApi = async (id) => {
  const response = await api.get(`/users/${id}`);
  return response.data;
};

export const createUserApi = async (userData) => {
  const response = await api.post('/users', userData);
  return response.data;
};

export const updateUserApi = async (id, userData) => {
  const response = await api.put(`/users/${id}`, userData);
  return response.data;
};

export const updateUserPartialApi = async (id, patchData) => {
  const response = await api.patch(`/users/${id}`, patchData);
  return response.data;
};

export const deleteUserApi = async (id) => {
  const response = await api.delete(`/users/${id}`);
  return response.data;
};

export const uploadUserPhotoApi = async (id, formData) => {
  const response = await api.post(`/users/${id}/upload-photo`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return response.data;
};

export const deleteUserPhotoApi = async (id) => {
  const response = await api.delete(`/users/${id}/profile-photo`);
  return response.data;
};
