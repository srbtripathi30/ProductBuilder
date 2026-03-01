import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { lobApi } from '../../api/lob.api';
import { Button } from '../../components/ui/Button';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
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
      <div className="flex items-center justify-between">
        <div><h1 className="text-2xl font-bold text-gray-900">Lines of Business</h1><p className="text-sm text-gray-500">Manage insurance lines of business</p></div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />New LOB</Button>
      </div>
      <div className="rounded-xl border border-gray-200 bg-white shadow-sm overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Code', 'Name', 'Description', 'Status', 'Actions'].map(h => (
                <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {visibleLobs.map(lob => (
              <tr key={lob.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-mono font-medium text-gray-900">{lob.code}</td>
                <td className="px-6 py-4 text-sm font-medium text-gray-900">{lob.name}</td>
                <td className="px-6 py-4 text-sm text-gray-500">{lob.description ?? '-'}</td>
                <td className="px-6 py-4"><Badge status={lob.isActive ? 'Active' : 'Inactive'} /></td>
                <td className="px-6 py-4">
                  <div className="flex items-center gap-2">
                    <button onClick={() => openEdit(lob)} className="text-gray-400 hover:text-primary-600 transition-colors"><Pencil className="h-4 w-4" /></button>
                    <button onClick={() => deleteMutation.mutate(lob.id)} className="text-gray-400 hover:text-red-600 transition-colors"><Trash2 className="h-4 w-4" /></button>
                  </div>
                </td>
              </tr>
            ))}
            {!visibleLobs.length && <tr><td colSpan={5} className="px-6 py-8 text-center text-sm text-gray-400">No lines of business found</td></tr>}
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
