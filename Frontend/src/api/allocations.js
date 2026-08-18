import api from './axios';

export const getAllocationsApi = async (params) => {
  const response = await api.get('/projectallocations', { params });
  return response.data;
};

export const getAllocationByIdApi = async (id) => {
  const response = await api.get(`/projectallocations/${id}`);
  return response.data;
};

export const createAllocationApi = async (data) => {
  const response = await api.post('/projectallocations', data);
  return response.data;
};

export const updateAllocationApi = async (id, data) => {
  const response = await api.put(`/projectallocations/${id}`, data);
  return response.data;
};

export const deleteAllocationApi = async (id) => {
  const response = await api.delete(`/projectallocations/${id}`);
  return response.data;
};
