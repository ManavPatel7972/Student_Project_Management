import api from './axios';

export const loginApi = async (credentials) => {
  const response = await api.post('/auth/login', credentials);
  return response.data;
};

export const getCurrentUserApi = async () => {
  const response = await api.get('/auth/me');
  return response.data;
};

export const changePasswordApi = async (data) => {
  const response = await api.put('/auth/change-password', data);
  return response.data;
};

export const registerApi = async (data) => {
  const response = await api.post('/auth/register', data);
  return response.data;
};

export const logoutApi = async () => {
  const response = await api.post('/auth/logout');
  return response.data;
};
