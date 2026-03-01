import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Pencil } from 'lucide-react';
import { underwritersApi, usersApi } from '../../api/stakeholders.api';
import { Button } from '../../components/ui/Button';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { AmountInput } from '../../components/ui/AmountInput';
import { PageSpinner } from '../../components/ui/Spinner';
import { formatCurrency, formatDate } from '../../utils/formatters';
import type { UnderwriterDto } from '../../types';

interface UnderwriterForm {
  userId: string;
  licenseNo: string;
  specialization: string;
  authorityLimit?: number;
}

export function UnderwritersPage() {
  const qc = useQueryClient();
  const { data: underwriters, isLoading } = useQuery({ queryKey: ['underwriters'], queryFn: underwritersApi.getAll });
  const { data: users } = useQuery({ queryKey: ['users'], queryFn: usersApi.getAll });

  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<UnderwriterDto | null>(null);
  const [form, setForm] = useState<UnderwriterForm>({ userId: '', licenseNo: '', specialization: '', authorityLimit: undefined });

  const createMutation = useMutation({
    mutationFn: underwritersApi.create,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['underwriters'] }); setOpen(false); },
  });
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: object }) => underwritersApi.update(id, data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['underwriters'] }); setOpen(false); },
  });

  const openCreate = () => {
    setEditing(null);
    setForm({ userId: '', licenseNo: '', specialization: '', authorityLimit: undefined });
    setOpen(true);
  };

  const openEdit = (u: UnderwriterDto) => {
    setEditing(u);
    setForm({ userId: u.userId, licenseNo: u.licenseNo ?? '', specialization: u.specialization ?? '', authorityLimit: u.authorityLimit });
    setOpen(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editing) {
      updateMutation.mutate({ id: editing.id, data: { licenseNo: form.licenseNo, specialization: form.specialization, authorityLimit: form.authorityLimit } });
    } else {
      createMutation.mutate(form);
    }
  };

  if (isLoading) return <PageSpinner />;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Underwriters</h1>
          <p className="text-sm text-gray-500">Manage underwriting staff and their authority limits</p>
        </div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />New Underwriter</Button>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white shadow-sm overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Name', 'Email', 'License No', 'Specialization', 'Authority Limit', 'Created', ''].map(h => (
                <th key={h} className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {underwriters?.map(u => (
              <tr key={u.id} className="hover:bg-gray-50">
                <td className="px-4 py-4 text-sm font-medium text-gray-900">{u.userName}</td>
                <td className="px-4 py-4 text-sm text-gray-500">{u.userEmail}</td>
                <td className="px-4 py-4 text-sm font-mono text-gray-700">{u.licenseNo ?? '-'}</td>
                <td className="px-4 py-4 text-sm text-gray-500">{u.specialization ?? '-'}</td>
                <td className="px-4 py-4 text-sm text-gray-700">{u.authorityLimit != null ? formatCurrency(u.authorityLimit) : '-'}</td>
                <td className="px-4 py-4 text-sm text-gray-400">{formatDate(u.createdAt)}</td>
                <td className="px-4 py-4">
                  <button onClick={() => openEdit(u)} className="text-gray-400 hover:text-primary-600 transition-colors">
                    <Pencil className="h-4 w-4" />
                  </button>
                </td>
              </tr>
            ))}
            {!underwriters?.length && (
              <tr><td colSpan={7} className="px-6 py-8 text-center text-sm text-gray-400">No underwriters found</td></tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal open={open} onClose={() => setOpen(false)} title={editing ? 'Edit Underwriter' : 'New Underwriter'} size="lg">
        <form onSubmit={handleSubmit} className="space-y-4">
          {!editing ? (
            <Select label="User" value={form.userId} onChange={e => setForm(f => ({ ...f, userId: e.target.value }))} required>
              <option value="">Select a user</option>
              {users?.map(u => (
                <option key={u.id} value={u.id}>{u.firstName} {u.lastName} ({u.email})</option>
              ))}
            </Select>
          ) : (
            <div className="rounded-lg bg-gray-50 px-4 py-3">
              <p className="text-xs text-gray-500 mb-0.5">User</p>
              <p className="text-sm font-medium text-gray-900">{editing.userName}</p>
              <p className="text-xs text-gray-400">{editing.userEmail}</p>
            </div>
          )}
          <div className="grid grid-cols-2 gap-4">
            <Input label="License No" value={form.licenseNo} onChange={e => setForm(f => ({ ...f, licenseNo: e.target.value }))} placeholder="e.g. UW-2024-001" />
            <Input label="Specialization" value={form.specialization} onChange={e => setForm(f => ({ ...f, specialization: e.target.value }))} placeholder="e.g. Marine, Property" />
          </div>
          <AmountInput
            label="Authority Limit"
            value={form.authorityLimit}
            onChange={v => setForm(f => ({ ...f, authorityLimit: isNaN(v) ? undefined : v }))}
            placeholder="e.g. 1m for 1,000,000"
          />
          <div className="flex justify-end gap-3 pt-2">
            <Button variant="secondary" type="button" onClick={() => setOpen(false)}>Cancel</Button>
            <Button type="submit" loading={createMutation.isPending || updateMutation.isPending}>
              {editing ? 'Update' : 'Create'}
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
