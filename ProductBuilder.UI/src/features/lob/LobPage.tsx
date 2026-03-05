import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { lobApi } from '../../api/lob.api';
import { Button } from '../../components/ui/Button';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
import { PageHeader } from '../../components/ui/PageHeader';
import { PageSpinner } from '../../components/ui/Spinner';
import type { LobDto } from '../../types';

export function LobPage() {
  const qc = useQueryClient();
  const { data: lobs, isLoading } = useQuery({ queryKey: ['lobs'], queryFn: lobApi.getAll });
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<LobDto | null>(null);
  const [form, setForm] = useState({ name: '', code: '', description: '' });

  const createMutation = useMutation({
    mutationFn: lobApi.create,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['lobs'] }); setOpen(false); }
  });
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Parameters<typeof lobApi.update>[1] }) => lobApi.update(id, data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['lobs'] }); setOpen(false); setEditing(null); }
  });
  const deleteMutation = useMutation({
    mutationFn: lobApi.delete,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['lobs'] }),
    onError: (err: unknown) => {
      const message = (err as any)?.response?.data?.message ?? 'Failed to delete LOB';
      window.alert(message);
    },
  });

  const openCreate = () => { setEditing(null); setForm({ name: '', code: '', description: '' }); setOpen(true); };
  const openEdit = (lob: LobDto) => { setEditing(lob); setForm({ name: lob.name, code: lob.code, description: lob.description ?? '' }); setOpen(true); };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editing) updateMutation.mutate({ id: editing.id, data: { name: form.name, description: form.description, isActive: editing.isActive } });
    else createMutation.mutate(form);
  };

  if (isLoading) return <PageSpinner />;
  const visibleLobs = (lobs ?? []).filter(l => l.isActive);

  return (
    <div className="space-y-6">
      <PageHeader
        title="Lines of Business"
        subtitle="Manage insurance lines of business"
        action={<Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />New LOB</Button>}
      />
      <div className="rounded-xl border border-gray-100 bg-white shadow-card overflow-hidden">
        <table className="min-w-full divide-y divide-gray-100">
          <thead className="bg-slate-50">
            <tr>
              {['Code', 'Name', 'Description', 'Status', 'Actions'].map(h => (
                <th key={h} className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-50">
            {visibleLobs.map(lob => (
              <tr key={lob.id} className="hover:bg-slate-50 transition-colors">
                <td className="px-6 py-3.5 text-xs font-mono font-semibold text-gray-600 tracking-wide">{lob.code}</td>
                <td className="px-6 py-3.5 text-sm font-medium text-gray-900">{lob.name}</td>
                <td className="px-6 py-3.5 text-sm text-gray-500">{lob.description ?? '-'}</td>
                <td className="px-6 py-3.5"><Badge status={lob.isActive ? 'Active' : 'Inactive'} /></td>
                <td className="px-6 py-3.5">
                  <div className="flex items-center gap-1">
                    <button aria-label={`Edit ${lob.name}`} onClick={() => openEdit(lob)} className="rounded p-1 text-gray-400 transition-colors hover:bg-primary-50 hover:text-primary-600"><Pencil className="h-4 w-4" /></button>
                    <button aria-label={`Delete ${lob.name}`} onClick={() => deleteMutation.mutate(lob.id)} className="rounded p-1 text-gray-400 transition-colors hover:bg-red-50 hover:text-red-600"><Trash2 className="h-4 w-4" /></button>
                  </div>
                </td>
              </tr>
            ))}
            {!visibleLobs.length && <tr><td colSpan={5} className="px-6 py-10 text-center text-sm text-gray-400">No lines of business found</td></tr>}
          </tbody>
        </table>
      </div>
      <Modal open={open} onClose={() => setOpen(false)} title={editing ? 'Edit LOB' : 'New Line of Business'}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input label="Name" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required />
          <Input label="Code" value={form.code} onChange={e => setForm(f => ({ ...f, code: e.target.value }))} required disabled={!!editing} placeholder="e.g. PROP, MOTOR" />
          <Input label="Description" value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} />
          <div className="flex justify-end gap-3 pt-2">
            <Button variant="secondary" type="button" onClick={() => setOpen(false)}>Cancel</Button>
            <Button type="submit" loading={createMutation.isPending || updateMutation.isPending}>{editing ? 'Update' : 'Create'}</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
