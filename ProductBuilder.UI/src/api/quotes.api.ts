import { apiClient } from './client';
import type { QuoteDto } from '../types';

export const quotesApi = {
  getAll: (params?: { status?: string }) => apiClient.get<QuoteDto[]>('/quotes', { params }).then(r => r.data),
  getById: (id: string) => apiClient.get<QuoteDto>(`/quotes/${id}`).then(r => r.data),
  create: (data: object) => apiClient.post<QuoteDto>('/quotes', data).then(r => r.data),
  calculate: (id: string) => apiClient.post<QuoteDto>(`/quotes/${id}/calculate`).then(r => r.data),
  submit: (id: string) => apiClient.put<QuoteDto>(`/quotes/${id}/submit`).then(r => r.data),
};
