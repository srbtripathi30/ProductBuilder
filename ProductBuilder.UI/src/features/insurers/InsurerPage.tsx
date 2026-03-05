import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { insurersApi } from '../../api/stakeholders.api';
import { Button } from '../../components/ui/Button';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
import { PageHeader } from '../../components/ui/PageHeader';
import { PageSpinner } from '../../components/ui/Spinner';
import type { InsurerDto } from '../../types';

export function InsurerPage() {
  const qc = useQueryClient();
  const { data: insurers, isLoading } = useQuery({ queryKey: ['insurers'], queryFn: insurersApi.getAll });
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<InsurerDto | null>(null);
  const [form, setForm] = useState({ name: '', code: '', licenseNo: '', address: '', phone: '', email: '' });

  const createMutation = useMutation({ mutationFn: insurersApi.create, onSuccess: () => { qc.invalidateQueries({ queryKey: ['insurers'] }); setOpen(false); } });
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: object }) => insurersApi.update(id, data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['insurers'] }); setOpen(false); }
  });
  const deleteMutation = useMutation({
    mutationFn: insurersApi.delete,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['insurers'] }),
    onError: (err: unknown) => {
      const message = (err as any)?.response?.data?.message ?? 'Failed to delete insurer';
      window.alert(message);
    },
  });

  const openCreate = () => { setEditing(null); setForm({ name: '', code: '', licenseNo: '', address: '', phone: '', email: '' }); setOpen(true); };
  const openEdit = (i: InsurerDto) => { setEditing(i); setForm({ name: i.name, code: i.code, licenseNo: i.licenseNo ?? '', address: i.address ?? '', phone: i.phone ?? '', email: i.email ?? '' }); setOpen(true); };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editing) updateMutation.mutate({ id: editing.id, data: { ...form, isActive: editing.isActive } });
    else createMutation.mutate(form);
  };

  if (isLoading) return <PageSpinner />;
  const visibleInsurers = (insurers ?? []).filter(i => i.isActive);

  return (
    <div className="space-y-6">
      <PageHeader
        title="Insurers"
        subtitle="Manage insurance companies"
        action={<Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />New Insurer</Button>}
      />
      <div className="rounded-xl border border-gray-100 bg-white shadow-card overflow-hidden">
        <table className="min-w-full divide-y divide-gray-100">
          <thead className="bg-slate-50">
            <tr>{['Code', 'Name', 'Email', 'Phone', 'Status', ''].map(h => <th key={h} className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">{h}</th>)}</tr>
          </thead>
          <tbody className="divide-y divide-gray-50">
            {visibleInsurers.map(i => (
              <tr key={i.id} className="hover:bg-slate-50 transition-colors">
                <td className="px-4 py-3.5 text-xs font-mono font-semibold text-gray-600 tracking-wide">{i.code}</td>
                <td className="px-4 py-3.5 text-sm font-medium text-gray-900">{i.name}</td>
                <td className="px-4 py-3.5 text-sm text-gray-500">{i.email ?? '-'}</td>
                <td className="px-4 py-3.5 text-sm text-gray-500">{i.phone ?? '-'}</td>
                <td className="px-4 py-3.5"><Badge status={i.isActive ? 'Active' : 'Inactive'} /></td>
                <td className="px-4 py-3.5">
                  <div className="flex items-center gap-1">
                    <button aria-label={`Edit ${i.name}`} onClick={() => openEdit(i)} className="rounded p-1 text-gray-400 transition-colors hover:bg-primary-50 hover:text-primary-600"><Pencil className="h-4 w-4" /></button>
                    <button
                      aria-label={`Delete ${i.name}`}
                      onClick={() => {
                        if (window.confirm(`Delete insurer ${i.name}?`)) deleteMutation.mutate(i.id);
                      }}
                      className="rounded p-1 text-gray-400 transition-colors hover:bg-red-50 hover:text-red-600"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
            {!visibleInsurers.length && <tr><td colSpan={6} className="px-6 py-10 text-center text-sm text-gray-400">No insurers found</td></tr>}
          </tbody>
        </table>
      </div>
      <Modal open={open} onClose={() => setOpen(false)} title={editing ? 'Edit Insurer' : 'New Insurer'} size="lg">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Input label="Name" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required />
            <Input label="Code" value={form.code} onChange={e => setForm(f => ({ ...f, code: e.target.value }))} required disabled={!!editing} />
            <Input label="License No" value={form.licenseNo} onChange={e => setForm(f => ({ ...f, licenseNo: e.target.value }))} />
            <Input label="Phone" value={form.phone} onChange={e => setForm(f => ({ ...f, phone: e.target.value }))} />
            <Input label="Email" type="email" value={form.email} onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
          </div>
          <Input label="Address" value={form.address} onChange={e => setForm(f => ({ ...f, address: e.target.value }))} />
          <div className="flex justify-end gap-3 pt-2">
            <Button variant="secondary" type="button" onClick={() => setOpen(false)}>Cancel</Button>
            <Button type="submit" loading={createMutation.isPending || updateMutation.isPending}>{editing ? 'Update' : 'Create'}</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
