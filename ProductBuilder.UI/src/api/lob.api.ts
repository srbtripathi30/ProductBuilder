import { apiClient } from './client';
import type { LobDto } from '../types';

export const lobApi = {
  getAll: () => apiClient.get<LobDto[]>('/lob').then(r => r.data),
  getById: (id: string) => apiClient.get<LobDto>(`/lob/${id}`).then(r => r.data),
  create: (data: { name: string; code: string; description?: string }) =>
    apiClient.post<LobDto>('/lob', data).then(r => r.data),
  update: (id: string, data: { name: string; description?: string; isActive: boolean }) =>
    apiClient.put<LobDto>(`/lob/${id}`, data).then(r => r.data),
  delete: (id: string) => apiClient.delete(`/lob/${id}`),
};
