import { apiClient } from './client';
import type { ProductDto, CoverageDto, CoverDto, LimitDto, DeductibleDto, PremiumDto, ModifierDto } from '../types';

export const productsApi = {
  getAll: (params?: { status?: string; lobId?: string }) =>
    apiClient.get<ProductDto[]>('/products', { params }).then(r => r.data),
  getById: (id: string) => apiClient.get<ProductDto>(`/products/${id}`).then(r => r.data),
  create: (data: object) => apiClient.post<ProductDto>('/products', data).then(r => r.data),
  update: (id: string, data: object) => apiClient.put<ProductDto>(`/products/${id}`, data).then(r => r.data),
  updateStatus: (id: string, status: string) =>
    apiClient.put<ProductDto>(`/products/${id}/status`, { status }).then(r => r.data),
  delete: (id: string) => apiClient.delete(`/products/${id}`),

  getCoverages: (productId: string) =>
    apiClient.get<CoverageDto[]>(`/products/${productId}/coverages`).then(r => r.data),
  createCoverage: (productId: string, data: object) =>
    apiClient.post<CoverageDto>(`/products/${productId}/coverages`, data).then(r => r.data),
  updateCoverage: (id: string, data: object) =>
    apiClient.put<CoverageDto>(`/coverages/${id}`, data).then(r => r.data),
  deleteCoverage: (id: string) => apiClient.delete(`/coverages/${id}`),

  getCovers: (coverageId: string) =>
    apiClient.get<CoverDto[]>(`/coverages/${coverageId}/covers`).then(r => r.data),
  createCover: (coverageId: string, data: object) =>
    apiClient.post<CoverDto>(`/coverages/${coverageId}/covers`, data).then(r => r.data),
  updateCover: (id: string, data: object) =>
    apiClient.put<CoverDto>(`/covers/${id}`, data).then(r => r.data),
  deleteCover: (id: string) => apiClient.delete(`/covers/${id}`),

  getLimits: (coverId: string) => apiClient.get<LimitDto[]>(`/covers/${coverId}/limits`).then(r => r.data),
  createLimit: (coverId: string, data: object) => apiClient.post<LimitDto>(`/covers/${coverId}/limits`, data).then(r => r.data),
  updateLimit: (id: string, data: object) => apiClient.put<LimitDto>(`/limits/${id}`, data).then(r => r.data),
  deleteLimit: (id: string) => apiClient.delete(`/limits/${id}`),

  getDeductibles: (coverId: string) => apiClient.get<DeductibleDto[]>(`/covers/${coverId}/deductibles`).then(r => r.data),
  createDeductible: (coverId: string, data: object) => apiClient.post<DeductibleDto>(`/covers/${coverId}/deductibles`, data).then(r => r.data),
  updateDeductible: (id: string, data: object) => apiClient.put<DeductibleDto>(`/deductibles/${id}`, data).then(r => r.data),
  deleteDeductible: (id: string) => apiClient.delete(`/deductibles/${id}`),

  getPremiums: (coverId: string) => apiClient.get<PremiumDto[]>(`/covers/${coverId}/premiums`).then(r => r.data),
  createPremium: (coverId: string, data: object) => apiClient.post<PremiumDto>(`/covers/${coverId}/premiums`, data).then(r => r.data),
  updatePremium: (id: string, data: object) => apiClient.put<PremiumDto>(`/premiums/${id}`, data).then(r => r.data),
  deletePremium: (id: string) => apiClient.delete(`/premiums/${id}`),

  getModifiers: (params?: { coverId?: string; productId?: string }) =>
    apiClient.get<ModifierDto[]>('/modifiers', { params }).then(r => r.data),
  createModifier: (data: object) => apiClient.post<ModifierDto>('/modifiers', data).then(r => r.data),
  updateModifier: (id: string, data: object) => apiClient.put<ModifierDto>(`/modifiers/${id}`, data).then(r => r.data),
  deleteModifier: (id: string) => apiClient.delete(`/modifiers/${id}`),
};
