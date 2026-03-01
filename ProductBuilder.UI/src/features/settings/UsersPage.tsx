import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Pencil, Shield, AlertCircle, Trash2 } from 'lucide-react';
import { usersApi } from '../../api/stakeholders.api';
import { useAuth } from '../../store/AuthContext';
import { Button } from '../../components/ui/Button';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { Badge } from '../../components/ui/Badge';
import { PageSpinner } from '../../components/ui/Spinner';
import { formatDate } from '../../utils/formatters';
import type { UserDetailDto } from '../../types';

const ROLES = [
  { id: 1, name: 'Admin' },
  { id: 2, name: 'Underwriter' },
  { id: 3, name: 'Broker' },
  { id: 4, name: 'Insurer' },
];

const ROLE_COLORS: Record<string, string> = {
  Admin:       'bg-red-100 text-red-700',
  Underwriter: 'bg-blue-100 text-blue-700',
  Broker:      'bg-purple-100 text-purple-700',
  Insurer:     'bg-green-100 text-green-700',
};

interface CreateForm {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  roleId: number;
}

interface EditForm {
  firstName: string;
  lastName: string;
  isActive: boolean;
}

function apiErrorMessage(err: unknown, fallback: string): string {
  if (!err) return fallback;
  const msg = (err as any)?.response?.data?.message;
  return msg ?? fallback;
}

export function UsersPage() {
  const qc = useQueryClient();
  const { user: currentUser } = useAuth();

  const { data: users, isLoading, isError, error } = useQuery({
    queryKey: ['users'],
    queryFn: usersApi.getAll,
  });

  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<UserDetailDto | null>(null);
  const [createForm, setCreateForm] = useState<CreateForm>({ email: '', password: '', firstName: '', lastName: '', roleId: 2 });
  const [editForm, setEditForm] = useState<EditForm>({ firstName: '', lastName: '', isActive: true });

  const createMutation = useMutation({
    mutationFn: usersApi.create,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['users'] }); setOpen(false); },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: object }) => usersApi.update(id, data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['users'] }); setOpen(false); },
  });
  const deleteMutation = useMutation({
    mutationFn: usersApi.delete,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['users'] }); },
    onError: (err: unknown) => {
      const message = (err as any)?.response?.data?.message ?? 'Failed to delete user';
      window.alert(message);
    },
  });

  const openCreate = () => {
    setEditing(null);
    setCreateForm({ email: '', password: '', firstName: '', lastName: '', roleId: 2 });
    createMutation.reset();
    setOpen(true);
  };

  const openEdit = (u: UserDetailDto) => {
    setEditing(u);
    setEditForm({ firstName: u.firstName, lastName: u.lastName, isActive: u.isActive });
    updateMutation.reset();
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    createMutation.reset();
    updateMutation.reset();
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editing) updateMutation.mutate({ id: editing.id, data: editForm });
    else createMutation.mutate(createForm);
  };

  const isSelf = (u: UserDetailDto) => u.email === currentUser?.email;
  const visibleUsers = (users ?? []).filter(u => u.isActive);
  const activeError = editing ? updateMutation.error : createMutation.error;

  if (isLoading) return <PageSpinner />;

  if (isError) {
    return (
      <div className="space-y-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">User Management</h1>
          <p className="text-sm text-gray-500">Manage system users and their roles</p>
        </div>
        <div className="flex items-start gap-3 rounded-xl border border-red-200 bg-red-50 p-6 text-red-700">
          <AlertCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
          <div>
            <p className="font-medium">Failed to load users</p>
            <p className="text-sm text-red-600 mt-1">
              {apiErrorMessage(error, 'Ensure you are logged in as an Admin and the server is running.')}
            </p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">User Management</h1>
          <p className="text-sm text-gray-500">Manage system users and their roles</p>
        </div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />New User</Button>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white shadow-sm overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Name', 'Email', 'Role', 'Status', 'Created', ''].map(h => (
                <th key={h} className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {visibleUsers.map(u => (
              <tr key={u.id} className="hover:bg-gray-50">
                <td className="px-4 py-4">
                  <div className="flex items-center gap-2">
                    <span className="text-sm font-medium text-gray-900">{u.firstName} {u.lastName}</span>
                    {isSelf(u) && (
                      <span className="text-xs bg-primary-100 text-primary-700 px-1.5 py-0.5 rounded-full">You</span>
                    )}
                  </div>
                </td>
                <td className="px-4 py-4 text-sm text-gray-500">{u.email}</td>
                <td className="px-4 py-4">
                  <span className={`inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-medium ${ROLE_COLORS[u.roleName] ?? 'bg-gray-100 text-gray-700'}`}>
                    <Shield className="h-3 w-3" />{u.roleName}
                  </span>
                </td>
                <td className="px-4 py-4">
                  <Badge status={u.isActive ? 'Active' : 'Inactive'} />
                </td>
                <td className="px-4 py-4 text-sm text-gray-400">{formatDate(u.createdAt)}</td>
                <td className="px-4 py-4">
                  <div className="flex items-center gap-2">
                    <button onClick={() => openEdit(u)} className="text-gray-400 hover:text-primary-600 transition-colors">
                      <Pencil className="h-4 w-4" />
                    </button>
                    {!isSelf(u) && (
                      <button
                        onClick={() => {
                          if (window.confirm(`Delete user ${u.firstName} ${u.lastName}?`)) deleteMutation.mutate(u.id);
                        }}
                        className="text-gray-400 hover:text-red-600 transition-colors"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
            {!visibleUsers.length && (
              <tr><td colSpan={6} className="px-6 py-8 text-center text-sm text-gray-400">No users found</td></tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal open={open} onClose={handleClose} title={editing ? 'Edit User' : 'New User'} size="lg">
        <form onSubmit={handleSubmit} className="space-y-4">
          {!editing ? (
            <>
              <div className="grid grid-cols-2 gap-4">
                <Input label="First Name" value={createForm.firstName} onChange={e => setCreateForm(f => ({ ...f, firstName: e.target.value }))} required />
                <Input label="Last Name" value={createForm.lastName} onChange={e => setCreateForm(f => ({ ...f, lastName: e.target.value }))} required />
              </div>
              <Input label="Email" type="email" value={createForm.email} onChange={e => setCreateForm(f => ({ ...f, email: e.target.value }))} required />
              <Input label="Password" type="password" value={createForm.password} onChange={e => setCreateForm(f => ({ ...f, password: e.target.value }))} required />
              <Select label="Role" value={String(createForm.roleId)} onChange={e => setCreateForm(f => ({ ...f, roleId: parseInt(e.target.value) }))} required>
                {ROLES.map(r => <option key={r.id} value={r.id}>{r.name}</option>)}
              </Select>
            </>
          ) : (
            <>
              <div className="rounded-lg bg-gray-50 px-4 py-3 grid grid-cols-2 gap-3 text-sm">
                <div>
                  <p className="text-xs text-gray-500 mb-0.5">Email</p>
                  <p className="font-medium text-gray-900">{editing.email}</p>
                </div>
                <div>
                  <p className="text-xs text-gray-500 mb-0.5">Role</p>
                  <span className={`inline-flex items-center gap-1 rounded-full px-2 py-0.5 text-xs font-medium ${ROLE_COLORS[editing.roleName] ?? 'bg-gray-100 text-gray-700'}`}>
                    <Shield className="h-3 w-3" />{editing.roleName}
                  </span>
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <Input label="First Name" value={editForm.firstName} onChange={e => setEditForm(f => ({ ...f, firstName: e.target.value }))} required />
                <Input label="Last Name" value={editForm.lastName} onChange={e => setEditForm(f => ({ ...f, lastName: e.target.value }))} required />
              </div>
              <label className={`flex items-center gap-2 text-sm select-none ${isSelf(editing) ? 'opacity-50 cursor-not-allowed' : 'text-gray-700 cursor-pointer'}`}>
                <input
                  type="checkbox"
                  checked={editForm.isActive}
                  disabled={isSelf(editing)}
                  onChange={e => setEditForm(f => ({ ...f, isActive: e.target.checked }))}
                  className="h-4 w-4 rounded text-primary-600"
                />
                Active
                {isSelf(editing) && (
                  <span className="text-xs text-gray-400 ml-1">(cannot deactivate your own account)</span>
                )}
              </label>
            </>
          )}

          {activeError && (
            <div className="flex items-center gap-2 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
              <AlertCircle className="h-4 w-4 flex-shrink-0" />
              {apiErrorMessage(activeError, 'Something went wrong. Please try again.')}
            </div>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <Button variant="secondary" type="button" onClick={handleClose}>Cancel</Button>
            <Button type="submit" loading={createMutation.isPending || updateMutation.isPending}>
              {editing ? 'Update' : 'Create'}
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
