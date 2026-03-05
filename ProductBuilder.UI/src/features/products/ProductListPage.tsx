import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Eye, Trash2 } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { productsApi } from '../../api/products.api';
import { lobApi } from '../../api/lob.api';
import { insurersApi } from '../../api/stakeholders.api';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { PageHeader } from '../../components/ui/PageHeader';
import { PageSpinner } from '../../components/ui/Spinner';
import { formatDate } from '../../utils/formatters';

export function ProductListPage() {
  const navigate = useNavigate();
  const qc = useQueryClient();
  const { data: products, isLoading } = useQuery({ queryKey: ['products'], queryFn: () => productsApi.getAll() });
  const { data: lobs } = useQuery({ queryKey: ['lobs'], queryFn: lobApi.getAll });
  const { data: insurers } = useQuery({ queryKey: ['insurers'], queryFn: insurersApi.getAll });
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ lobId: '', insurerId: '', name: '', code: '', description: '', version: '1.0', effectiveDate: '', expiryDate: '' });

  const createMutation = useMutation({
    mutationFn: productsApi.create,
    onSuccess: (p: any) => { qc.invalidateQueries({ queryKey: ['products'] }); setOpen(false); navigate(`/products/${p.id}`); }
  });
  const deleteMutation = useMutation({
    mutationFn: productsApi.delete,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }),
    onError: (err: unknown) => {
      const message = (err as any)?.response?.data?.message ?? 'Failed to delete product';
      window.alert(message);
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    createMutation.mutate({ ...form, expiryDate: form.expiryDate || undefined });
  };

  if (isLoading) return <PageSpinner />;
  const visibleProducts = (products ?? []).filter(p => p.status !== 'Archived');

  return (
    <div className="space-y-6">
      <PageHeader
        title="Products"
        subtitle="Manage insurance products"
        action={<Button onClick={() => setOpen(true)}><Plus className="mr-2 h-4 w-4" />New Product</Button>}
      />
      <div className="rounded-xl border border-gray-100 bg-white shadow-card overflow-hidden">
        <table className="min-w-full divide-y divide-gray-100">
          <thead className="bg-slate-50">
            <tr>{['Code', 'Name', 'LOB', 'Insurer', 'Version', 'Status', 'Effective', ''].map(h => (
              <th key={h} className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">{h}</th>
            ))}</tr>
          </thead>
          <tbody className="divide-y divide-gray-50">
            {visibleProducts.map(p => (
              <tr key={p.id} className="hover:bg-slate-50 transition-colors">
                <td className="px-4 py-3.5 text-xs font-mono font-semibold text-gray-600 tracking-wide">{p.code}</td>
                <td className="px-4 py-3.5 text-sm font-medium text-gray-900">{p.name}</td>
                <td className="px-4 py-3.5 text-sm text-gray-500">{p.lobName}</td>
                <td className="px-4 py-3.5 text-sm text-gray-500">{p.insurerName}</td>
                <td className="px-4 py-3.5 text-sm text-gray-500">{p.version}</td>
                <td className="px-4 py-3.5"><Badge status={p.status} /></td>
                <td className="px-4 py-3.5 text-sm text-gray-500">{formatDate(p.effectiveDate)}</td>
                <td className="px-4 py-3.5">
                  <div className="flex items-center gap-2">
                    <button aria-label={`View ${p.name}`} onClick={() => navigate(`/products/${p.id}`)} className="rounded p-1 text-gray-400 transition-colors hover:bg-primary-50 hover:text-primary-600">
                      <Eye className="h-4 w-4" />
                    </button>
                    <button
                      aria-label={`Delete ${p.name}`}
                      onClick={() => {
                        if (window.confirm(`Delete product ${p.name}?`)) deleteMutation.mutate(p.id);
                      }}
                      className="rounded p-1 text-gray-400 transition-colors hover:bg-red-50 hover:text-red-600"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
            {!visibleProducts.length && <tr><td colSpan={8} className="px-6 py-10 text-center text-sm text-gray-400">No products found. Create your first product.</td></tr>}
          </tbody>
        </table>
      </div>

      <Modal open={open} onClose={() => setOpen(false)} title="New Product" size="lg">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Select label="Line of Business" value={form.lobId} onChange={e => setForm(f => ({ ...f, lobId: e.target.value }))} required>
              <option value="">Select LOB</option>
              {lobs?.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}
            </Select>
            <Select label="Insurer" value={form.insurerId} onChange={e => setForm(f => ({ ...f, insurerId: e.target.value }))} required>
              <option value="">Select Insurer</option>
              {insurers?.map(i => <option key={i.id} value={i.id}>{i.name}</option>)}
            </Select>
            <Input label="Product Name" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required />
            <Input label="Code" value={form.code} onChange={e => setForm(f => ({ ...f, code: e.target.value }))} required placeholder="e.g. HOME-COMP-01" />
            <Input label="Version" value={form.version} onChange={e => setForm(f => ({ ...f, version: e.target.value }))} required />
            <Input label="Effective Date" type="date" value={form.effectiveDate} onChange={e => setForm(f => ({ ...f, effectiveDate: e.target.value }))} required />
            <Input label="Expiry Date" type="date" value={form.expiryDate} onChange={e => setForm(f => ({ ...f, expiryDate: e.target.value }))} />
          </div>
          <Input label="Description" value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} />
          <div className="flex justify-end gap-3 pt-2">
            <Button variant="secondary" type="button" onClick={() => setOpen(false)}>Cancel</Button>
            <Button type="submit" loading={createMutation.isPending}>Create Product</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
