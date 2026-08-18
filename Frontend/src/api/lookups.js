import api from './axios';

export const getTaskStatusesApi = async () => {
  const response = await api.get('/task-statuses');
  return response.data;
};

export const getTaskPrioritiesApi = async () => {
  const response = await api.get('/task-priorities');
  return response.data;
};

export const getUserTypesApi = async () => {
  const response = await api.get('/user-types');
  return response.data;
};
