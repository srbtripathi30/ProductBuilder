import { apiClient } from './client';
import type { InsurerDto, UnderwriterDto, BrokerDto, UserDetailDto } from '../types';

export const insurersApi = {
  getAll: () => apiClient.get<InsurerDto[]>('/insurers').then(r => r.data),
  getById: (id: string) => apiClient.get<InsurerDto>(`/insurers/${id}`).then(r => r.data),
  create: (data: object) => apiClient.post<InsurerDto>('/insurers', data).then(r => r.data),
  update: (id: string, data: object) => apiClient.put<InsurerDto>(`/insurers/${id}`, data).then(r => r.data),
  delete: (id: string) => apiClient.delete(`/insurers/${id}`),
};

export const underwritersApi = {
  getAll: () => apiClient.get<UnderwriterDto[]>('/underwriters').then(r => r.data),
  create: (data: object) => apiClient.post<UnderwriterDto>('/underwriters', data).then(r => r.data),
  update: (id: string, data: object) => apiClient.put<UnderwriterDto>(`/underwriters/${id}`, data).then(r => r.data),
};

export const brokersApi = {
  getAll: () => apiClient.get<BrokerDto[]>('/brokers').then(r => r.data),
  create: (data: object) => apiClient.post<BrokerDto>('/brokers', data).then(r => r.data),
  update: (id: string, data: object) => apiClient.put<BrokerDto>(`/brokers/${id}`, data).then(r => r.data),
};

export const usersApi = {
  getAll: () => apiClient.get<UserDetailDto[]>('/users').then(r => r.data),
  create: (data: object) => apiClient.post<UserDetailDto>('/users', data).then(r => r.data),
  update: (id: string, data: object) => apiClient.put<UserDetailDto>(`/users/${id}`, data).then(r => r.data),
  delete: (id: string) => apiClient.delete(`/users/${id}`),
};
