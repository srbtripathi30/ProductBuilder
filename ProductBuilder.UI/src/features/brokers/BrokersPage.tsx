import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { brokersApi, usersApi, insurersApi } from '../../api/stakeholders.api';
import { Button } from '../../components/ui/Button';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { Badge } from '../../components/ui/Badge';
import { PageSpinner } from '../../components/ui/Spinner';
import { formatDate } from '../../utils/formatters';
import type { BrokerDto } from '../../types';

interface BrokerForm {
  userId: string;
  companyName: string;
  insurerId: string;
  licenseNo: string;
  commissionRate: string;
  isActive: boolean;
}

export function BrokersPage() {
  const qc = useQueryClient();
  const { data: brokers, isLoading } = useQuery({ queryKey: ['brokers'], queryFn: brokersApi.getAll });
  const { data: users } = useQuery({ queryKey: ['users'], queryFn: usersApi.getAll });
  const { data: insurers } = useQuery({ queryKey: ['insurers'], queryFn: insurersApi.getAll });

  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<BrokerDto | null>(null);
  const [form, setForm] = useState<BrokerForm>({ userId: '', companyName: '', insurerId: '', licenseNo: '', commissionRate: '', isActive: true });

  const createMutation = useMutation({
    mutationFn: brokersApi.create,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['brokers'] }); setOpen(false); },
  });
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: object }) => brokersApi.update(id, data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['brokers'] }); setOpen(false); },
  });
  const deleteMutation = useMutation({
    mutationFn: brokersApi.delete,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['brokers'] }); },
    onError: (err: unknown) => {
      const message = (err as any)?.response?.data?.message ?? 'Failed to delete broker';
      window.alert(message);
    },
  });

  const openCreate = () => {
    setEditing(null);
    setForm({ userId: '', companyName: '', insurerId: '', licenseNo: '', commissionRate: '', isActive: true });
    setOpen(true);
  };

  const openEdit = (b: BrokerDto) => {
    setEditing(b);
    setForm({
      userId: b.userId,
      companyName: b.companyName,
      insurerId: b.insurerId ?? '',
      licenseNo: b.licenseNo ?? '',
      commissionRate: b.commissionRate != null ? String(b.commissionRate) : '',
      isActive: b.isActive,
    });
    setOpen(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const payload = {
      ...(editing ? {} : { userId: form.userId }),
      companyName: form.companyName,
      insurerId: form.insurerId || null,
      licenseNo: form.licenseNo,
      commissionRate: form.commissionRate ? parseFloat(form.commissionRate) : null,
      ...(editing ? { isActive: form.isActive } : {}),
    };
    if (editing) updateMutation.mutate({ id: editing.id, data: payload });
    else createMutation.mutate(payload);
  };

  const brokerUserIds = new Set((brokers ?? []).map(b => b.userId));
  const availableUsers = (users ?? []).filter(
    u => u.isActive && u.roleName === 'Broker' && !brokerUserIds.has(u.id)
  );

  if (isLoading) return <PageSpinner />;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Brokers</h1>
          <p className="text-sm text-gray-500">Manage brokers and their commission rates</p>
        </div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />New Broker</Button>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white shadow-sm overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Company', 'Contact', 'Insurer', 'License No', 'Commission', 'Status', ''].map(h => (
                <th key={h} className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {brokers?.map(b => (
              <tr key={b.id} className="hover:bg-gray-50">
                <td className="px-4 py-4 text-sm font-medium text-gray-900">{b.companyName}</td>
                <td className="px-4 py-4">
                  <p className="text-sm font-medium text-gray-900">{b.userName}</p>
                  <p className="text-xs text-gray-400">{b.userEmail}</p>
                </td>
                <td className="px-4 py-4 text-sm text-gray-500">{b.insurerName ?? '-'}</td>
                <td className="px-4 py-4 text-sm font-mono text-gray-700">{b.licenseNo ?? '-'}</td>
                <td className="px-4 py-4 text-sm text-gray-700">{b.commissionRate != null ? `${b.commissionRate}%` : '-'}</td>
                <td className="px-4 py-4"><Badge status={b.isActive ? 'Active' : 'Inactive'} /></td>
                <td className="px-4 py-4">
                  <div className="flex items-center gap-2">
                    <button onClick={() => openEdit(b)} className="text-gray-400 hover:text-primary-600 transition-colors">
                      <Pencil className="h-4 w-4" />
                    </button>
                    <button
                      onClick={() => {
                        if (window.confirm(`Delete broker ${b.companyName}?`)) deleteMutation.mutate(b.id);
                      }}
                      className="text-gray-400 hover:text-red-600 transition-colors"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
            {!brokers?.length && (
              <tr><td colSpan={7} className="px-6 py-8 text-center text-sm text-gray-400">No brokers found</td></tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal open={open} onClose={() => setOpen(false)} title={editing ? 'Edit Broker' : 'New Broker'} size="lg">
        <form onSubmit={handleSubmit} className="space-y-4">
          {!editing ? (
            <Select label="User" value={form.userId} onChange={e => setForm(f => ({ ...f, userId: e.target.value }))} required>
              <option value="">Select a user</option>
              {availableUsers.map(u => (
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
            <Input label="Company Name" value={form.companyName} onChange={e => setForm(f => ({ ...f, companyName: e.target.value }))} required />
            <Input label="License No" value={form.licenseNo} onChange={e => setForm(f => ({ ...f, licenseNo: e.target.value }))} placeholder="e.g. BR-2024-001" />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <Select label="Insurer" value={form.insurerId} onChange={e => setForm(f => ({ ...f, insurerId: e.target.value }))}>
              <option value="">None (independent)</option>
              {insurers?.map(i => (
                <option key={i.id} value={i.id}>{i.name}</option>
              ))}
            </Select>
            <Input
              label="Commission Rate (%)"
              type="number"
              step="0.01"
              min="0"
              max="100"
              value={form.commissionRate}
              onChange={e => setForm(f => ({ ...f, commissionRate: e.target.value }))}
              placeholder="e.g. 15"
            />
          </div>
          {editing && (
            <label className="flex items-center gap-2 text-sm text-gray-700 cursor-pointer select-none">
              <input
                type="checkbox"
                checked={form.isActive}
                onChange={e => setForm(f => ({ ...f, isActive: e.target.checked }))}
                className="h-4 w-4 rounded text-primary-600"
              />
              Active
            </label>
          )}
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
