import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ChevronDown, ChevronRight, Plus, Trash2, ArrowLeft } from 'lucide-react';
import { productsApi } from '../../api/products.api';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { PageSpinner } from '../../components/ui/Spinner';
import { formatDate, formatCurrency } from '../../utils/formatters';

export function ProductDetailPage() {
  const { productId } = useParams<{ productId: string }>();
  const navigate = useNavigate();
  const qc = useQueryClient();

  const { data: product, isLoading: lp } = useQuery({
    queryKey: ['product', productId],
    queryFn: () => productsApi.getById(productId!),
    enabled: !!productId
  });
  const { data: coverages, isLoading: lc } = useQuery({
    queryKey: ['coverages', productId],
    queryFn: () => productsApi.getCoverages(productId!),
    enabled: !!productId
  });

  const statusMutation = useMutation({
    mutationFn: (status: string) => productsApi.updateStatus(productId!, status),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['product', productId] })
  });

  const [expandedCoverages, setExpandedCoverages] = useState<Set<string>>(new Set());
  const [expandedCovers, setExpandedCovers] = useState<Set<string>>(new Set());
  const [coverageModal, setCoverageModal] = useState(false);
  const [coverModal, setCoverModal] = useState<string | null>(null);
  const [form, setForm] = useState({ name: '', code: '', description: '', isMandatory: false, sequenceNo: 0 });

  const createCoverageMutation = useMutation({
    mutationFn: (data: object) => productsApi.createCoverage(productId!, data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['coverages', productId] }); setCoverageModal(false); resetForm(); }
  });
  const createCoverMutation = useMutation({
    mutationFn: ({ coverageId, data }: { coverageId: string; data: object }) => productsApi.createCover(coverageId, data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['coverages', productId] }); setCoverModal(null); resetForm(); }
  });
  const deleteCoverageMutation = useMutation({
    mutationFn: productsApi.deleteCoverage,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['coverages', productId] })
  });

  const resetForm = () => setForm({ name: '', code: '', description: '', isMandatory: false, sequenceNo: 0 });
  const toggleCoverage = (id: string) => setExpandedCoverages(prev => { const n = new Set(prev); n.has(id) ? n.delete(id) : n.add(id); return n; });
  const toggleCover = (id: string) => setExpandedCovers(prev => { const n = new Set(prev); n.has(id) ? n.delete(id) : n.add(id); return n; });

  if (lp || lc) return <PageSpinner />;
  if (!product) return <div className="p-6 text-center text-gray-500">Product not found</div>;

  const statusTransitions: Record<string, string[]> = {
    Draft: ['Active'], Active: ['Inactive'], Inactive: ['Active', 'Archived'], Archived: []
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <button onClick={() => navigate('/products')} className="text-gray-400 hover:text-gray-600 transition-colors">
          <ArrowLeft className="h-5 w-5" />
        </button>
        <div className="flex-1">
          <div className="flex items-center gap-3 flex-wrap">
            <h1 className="text-2xl font-bold text-gray-900">{product.name}</h1>
            <Badge status={product.status} />
            <span className="text-sm text-gray-400">v{product.version}</span>
          </div>
          <p className="text-sm text-gray-500">{product.lobName} · {product.insurerName} · {product.code}</p>
        </div>
        <div className="flex gap-2">
          {statusTransitions[product.status]?.map(s => (
            <Button key={s} variant="secondary" size="sm" onClick={() => statusMutation.mutate(s)}
              loading={statusMutation.isPending}>
              Set {s}
            </Button>
          ))}
        </div>
      </div>

      {/* Info Cards */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        {[
          { label: 'Effective Date', value: formatDate(product.effectiveDate) },
          { label: 'Expiry Date', value: formatDate(product.expiryDate) },
          { label: 'Last Updated', value: formatDate(product.updatedAt) },
          { label: 'Description', value: product.description ?? '—' },
        ].map(item => (
          <div key={item.label} className="rounded-lg border border-gray-200 bg-white p-4">
            <p className="text-xs text-gray-500">{item.label}</p>
            <p className="mt-1 text-sm font-medium text-gray-900">{item.value}</p>
          </div>
        ))}
      </div>

      {/* Coverage Tree */}
      <div className="rounded-xl border border-gray-200 bg-white shadow-sm">
        <div className="flex items-center justify-between border-b p-4">
          <h2 className="font-semibold text-gray-900">Coverages & Covers</h2>
          <Button size="sm" onClick={() => { resetForm(); setCoverageModal(true); }}>
            <Plus className="mr-1 h-3 w-3" />Add Coverage
          </Button>
        </div>

        <div className="divide-y">
          {coverages?.map(coverage => (
            <div key={coverage.id}>
              {/* Coverage Row */}
              <div
                className="flex items-center gap-3 p-4 cursor-pointer hover:bg-gray-50 transition-colors"
                onClick={() => toggleCoverage(coverage.id)}
              >
                {expandedCoverages.has(coverage.id)
                  ? <ChevronDown className="h-4 w-4 text-gray-400 flex-shrink-0" />
                  : <ChevronRight className="h-4 w-4 text-gray-400 flex-shrink-0" />}
                <div className="flex-1">
                  <span className="font-medium text-gray-900">{coverage.name}</span>
                  <span className="ml-2 text-xs text-gray-400 font-mono">{coverage.code}</span>
                  {coverage.isMandatory && <span className="ml-2 rounded-full bg-blue-100 px-2 py-0.5 text-xs text-blue-700">Mandatory</span>}
                </div>
                <span className="text-xs text-gray-400">{coverage.covers.length} cover{coverage.covers.length !== 1 ? 's' : ''}</span>
                <button
                  onClick={e => { e.stopPropagation(); deleteCoverageMutation.mutate(coverage.id); }}
                  className="text-gray-300 hover:text-red-500 transition-colors ml-2"
                >
                  <Trash2 className="h-4 w-4" />
                </button>
              </div>

              {/* Covers */}
              {expandedCoverages.has(coverage.id) && (
                <div className="ml-8 border-l-2 border-gray-100 bg-gray-50/50">
                  <div className="flex justify-end p-2 pr-4">
                    <Button size="sm" variant="secondary" onClick={() => { resetForm(); setCoverModal(coverage.id); }}>
                      <Plus className="mr-1 h-3 w-3" />Add Cover
                    </Button>
                  </div>
                  {coverage.covers.map(cover => (
                    <div key={cover.id}>
                      <div
                        className="flex items-center gap-3 px-4 py-3 cursor-pointer hover:bg-gray-100 transition-colors"
                        onClick={() => toggleCover(cover.id)}
                      >
                        {expandedCovers.has(cover.id)
                          ? <ChevronDown className="h-4 w-4 text-gray-400 flex-shrink-0" />
                          : <ChevronRight className="h-4 w-4 text-gray-400 flex-shrink-0" />}
                        <span className="font-medium text-gray-800 text-sm">{cover.name}</span>
                        <span className="text-xs text-gray-400 font-mono">{cover.code}</span>
                        {cover.isMandatory && <span className="rounded-full bg-blue-100 px-2 py-0.5 text-xs text-blue-700">Mandatory</span>}
                      </div>
                      {expandedCovers.has(cover.id) && <CoverFinancialPanel coverId={cover.id} />}
                    </div>
                  ))}
                  {!coverage.covers.length && (
                    <p className="px-6 py-3 text-sm text-gray-400">No covers yet — add the first one</p>
                  )}
                </div>
              )}
            </div>
          ))}
          {!coverages?.length && (
            <p className="p-8 text-center text-sm text-gray-400">No coverages yet. Add the first coverage to get started.</p>
          )}
        </div>
      </div>

      {/* Coverage Modal */}
      <Modal open={coverageModal} onClose={() => setCoverageModal(false)} title="Add Coverage">
        <form onSubmit={e => { e.preventDefault(); createCoverageMutation.mutate(form); }} className="space-y-4">
          <Input label="Name" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required />
          <Input label="Code" value={form.code} onChange={e => setForm(f => ({ ...f, code: e.target.value }))} required placeholder="e.g. TPL, FIRE" />
          <Input label="Description" value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} />
          <Input label="Sequence No" type="number" value={String(form.sequenceNo)} onChange={e => setForm(f => ({ ...f, sequenceNo: parseInt(e.target.value) }))} />
          <label className="flex items-center gap-2 text-sm cursor-pointer">
            <input type="checkbox" checked={form.isMandatory} onChange={e => setForm(f => ({ ...f, isMandatory: e.target.checked }))} className="rounded" />
            Mandatory Coverage
          </label>
          <div className="flex justify-end gap-3 pt-2">
            <Button variant="secondary" type="button" onClick={() => setCoverageModal(false)}>Cancel</Button>
            <Button type="submit" loading={createCoverageMutation.isPending}>Add Coverage</Button>
          </div>
        </form>
      </Modal>

      {/* Cover Modal */}
      <Modal open={!!coverModal} onClose={() => setCoverModal(null)} title="Add Cover">
        <form onSubmit={e => { e.preventDefault(); if (coverModal) createCoverMutation.mutate({ coverageId: coverModal, data: form }); }} className="space-y-4">
          <Input label="Name" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required />
          <Input label="Code" value={form.code} onChange={e => setForm(f => ({ ...f, code: e.target.value }))} required />
          <Input label="Description" value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} />
          <Input label="Sequence No" type="number" value={String(form.sequenceNo)} onChange={e => setForm(f => ({ ...f, sequenceNo: parseInt(e.target.value) }))} />
          <label className="flex items-center gap-2 text-sm cursor-pointer">
            <input type="checkbox" checked={form.isMandatory} onChange={e => setForm(f => ({ ...f, isMandatory: e.target.checked }))} className="rounded" />
            Mandatory Cover
          </label>
          <div className="flex justify-end gap-3 pt-2">
            <Button variant="secondary" type="button" onClick={() => setCoverModal(null)}>Cancel</Button>
            <Button type="submit" loading={createCoverMutation.isPending}>Add Cover</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}

function CoverFinancialPanel({ coverId }: { coverId: string }) {
  const [tab, setTab] = useState<'limits' | 'deductibles' | 'premiums' | 'modifiers'>('limits');
  const qc = useQueryClient();
  const [addOpen, setAddOpen] = useState(false);
  const [form, setForm] = useState<Record<string, string | number | boolean>>({});

  const { data: limits } = useQuery({ queryKey: ['limits', coverId], queryFn: () => productsApi.getLimits(coverId) });
  const { data: deductibles } = useQuery({ queryKey: ['deductibles', coverId], queryFn: () => productsApi.getDeductibles(coverId) });
  const { data: premiums } = useQuery({ queryKey: ['premiums', coverId], queryFn: () => productsApi.getPremiums(coverId) });
  const { data: modifiers } = useQuery({ queryKey: ['modifiers-cover', coverId], queryFn: () => productsApi.getModifiers({ coverId }) });

  const limitMutation = useMutation({ mutationFn: (d: object) => productsApi.createLimit(coverId, d), onSuccess: () => { qc.invalidateQueries({ queryKey: ['limits', coverId] }); setAddOpen(false); } });
  const deductibleMutation = useMutation({ mutationFn: (d: object) => productsApi.createDeductible(coverId, d), onSuccess: () => { qc.invalidateQueries({ queryKey: ['deductibles', coverId] }); setAddOpen(false); } });
  const premiumMutation = useMutation({ mutationFn: (d: object) => productsApi.createPremium(coverId, d), onSuccess: () => { qc.invalidateQueries({ queryKey: ['premiums', coverId] }); setAddOpen(false); } });
  const modifierMutation = useMutation({ mutationFn: (d: object) => productsApi.createModifier(d), onSuccess: () => { qc.invalidateQueries({ queryKey: ['modifiers-cover', coverId] }); setAddOpen(false); } });

  const tabs = [
    { id: 'limits' as const, label: `Limits (${limits?.length ?? 0})` },
    { id: 'deductibles' as const, label: `Deductibles (${deductibles?.length ?? 0})` },
    { id: 'premiums' as const, label: `Premiums (${premiums?.length ?? 0})` },
    { id: 'modifiers' as const, label: `Modifiers (${modifiers?.length ?? 0})` },
  ];

  const handleAdd = (e: React.FormEvent) => {
    e.preventDefault();
    if (tab === 'limits') limitMutation.mutate(form);
    else if (tab === 'deductibles') deductibleMutation.mutate(form);
    else if (tab === 'premiums') premiumMutation.mutate(form);
    else modifierMutation.mutate({ ...form, coverId });
  };

  return (
    <div className="ml-4 mr-4 mb-4 rounded-lg border border-gray-200 bg-white p-4">
      <div className="flex flex-wrap gap-2 mb-4">
        {tabs.map(t => (
          <button key={t.id} onClick={() => setTab(t.id)}
            className={`px-3 py-1 rounded-full text-xs font-medium transition-colors ${tab === t.id ? 'bg-primary-600 text-white' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'}`}>
            {t.label}
          </button>
        ))}
      </div>

      <div className="space-y-2 min-h-[40px]">
        {tab === 'limits' && limits?.map(l => (
          <div key={l.id} className="flex items-center justify-between rounded-lg bg-gray-50 px-3 py-2 text-sm border border-gray-100">
            <span className="font-medium text-gray-800">{l.limitType}</span>
            <span className="text-gray-500 text-xs">Default: {formatCurrency(l.defaultAmount, l.currency)} · Max: {formatCurrency(l.maxAmount, l.currency)}</span>
          </div>
        ))}
        {tab === 'deductibles' && deductibles?.map(d => (
          <div key={d.id} className="flex items-center justify-between rounded-lg bg-gray-50 px-3 py-2 text-sm border border-gray-100">
            <span className="font-medium text-gray-800">{d.deductibleType}</span>
            <span className="text-gray-500 text-xs">Default: {formatCurrency(d.defaultAmount, d.currency)}</span>
          </div>
        ))}
        {tab === 'premiums' && premiums?.map(p => (
          <div key={p.id} className="flex items-center justify-between rounded-lg bg-gray-50 px-3 py-2 text-sm border border-gray-100">
            <span className="font-medium text-gray-800">{p.premiumType}</span>
            <span className="text-gray-500 text-xs">
              {p.flatAmount ? `Flat: ${formatCurrency(p.flatAmount)}` : `Rate: ${p.baseRate} × ${p.calculationBasis}`}
            </span>
          </div>
        ))}
        {tab === 'modifiers' && modifiers?.map(m => (
          <div key={m.id} className="flex items-center justify-between rounded-lg bg-gray-50 px-3 py-2 text-sm border border-gray-100">
            <span className="font-medium text-gray-800">{m.name}</span>
            <span className="text-gray-500 text-xs">{m.modifierType} · {m.valueType} · {m.minValue}–{m.maxValue}</span>
          </div>
        ))}
      </div>

      <button onClick={() => { setForm({}); setAddOpen(true); }}
        className="mt-3 flex items-center gap-1 text-xs text-primary-600 hover:text-primary-800 transition-colors">
        <Plus className="h-3 w-3" />Add {tab.slice(0, -1)}
      </button>

      <Modal open={addOpen} onClose={() => setAddOpen(false)} title={`Add ${tab.slice(0, -1)}`} size="sm">
        <form onSubmit={handleAdd} className="space-y-3">
          {tab === 'limits' && <>
            <Select label="Limit Type" value={String(form.limitType ?? '')} onChange={e => setForm(f => ({ ...f, limitType: e.target.value }))} required>
              <option value="">Select type</option>
              {['PerOccurrence', 'Aggregate', 'PerItem'].map(t => <option key={t}>{t}</option>)}
            </Select>
            <Input label="Min Amount" type="number" value={String(form.minAmount ?? 0)} onChange={e => setForm(f => ({ ...f, minAmount: parseFloat(e.target.value) }))} required />
            <Input label="Max Amount" type="number" value={String(form.maxAmount ?? '')} onChange={e => setForm(f => ({ ...f, maxAmount: parseFloat(e.target.value) }))} required />
            <Input label="Default Amount" type="number" value={String(form.defaultAmount ?? '')} onChange={e => setForm(f => ({ ...f, defaultAmount: parseFloat(e.target.value) }))} required />
            <Input label="Currency" value={String(form.currency ?? 'USD')} onChange={e => setForm(f => ({ ...f, currency: e.target.value }))} />
          </>}
          {tab === 'deductibles' && <>
            <Select label="Deductible Type" value={String(form.deductibleType ?? '')} onChange={e => setForm(f => ({ ...f, deductibleType: e.target.value }))} required>
              <option value="">Select type</option>
              {['Fixed', 'Percentage', 'Franchise'].map(t => <option key={t}>{t}</option>)}
            </Select>
            <Input label="Min Amount" type="number" value={String(form.minAmount ?? 0)} onChange={e => setForm(f => ({ ...f, minAmount: parseFloat(e.target.value) }))} required />
            <Input label="Max Amount" type="number" value={String(form.maxAmount ?? '')} onChange={e => setForm(f => ({ ...f, maxAmount: parseFloat(e.target.value) }))} required />
            <Input label="Default Amount" type="number" value={String(form.defaultAmount ?? '')} onChange={e => setForm(f => ({ ...f, defaultAmount: parseFloat(e.target.value) }))} required />
          </>}
          {tab === 'premiums' && <>
            <Select label="Premium Type" value={String(form.premiumType ?? '')} onChange={e => setForm(f => ({ ...f, premiumType: e.target.value }))} required>
              <option value="">Select type</option>
              {['Flat', 'RateBased', 'PerUnit'].map(t => <option key={t}>{t}</option>)}
            </Select>
            {form.premiumType === 'Flat'
              ? <Input label="Flat Amount" type="number" value={String(form.flatAmount ?? '')} onChange={e => setForm(f => ({ ...f, flatAmount: parseFloat(e.target.value) }))} required />
              : <>
                <Input label="Base Rate" type="number" step="0.000001" value={String(form.baseRate ?? '')} onChange={e => setForm(f => ({ ...f, baseRate: parseFloat(e.target.value) }))} required />
                <Input label="Calculation Basis" value={String(form.calculationBasis ?? '')} onChange={e => setForm(f => ({ ...f, calculationBasis: e.target.value }))} placeholder="SumInsured, Revenue…" required />
              </>}
            <Input label="Min Premium" type="number" value={String(form.minPremium ?? '')} onChange={e => setForm(f => ({ ...f, minPremium: parseFloat(e.target.value) }))} />
          </>}
          {tab === 'modifiers' && <>
            <Input label="Name" value={String(form.name ?? '')} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required />
            <Input label="Code" value={String(form.code ?? '')} onChange={e => setForm(f => ({ ...f, code: e.target.value }))} required />
            <Select label="Modifier Type" value={String(form.modifierType ?? '')} onChange={e => setForm(f => ({ ...f, modifierType: e.target.value }))} required>
              <option value="">Select type</option>
              {['Loading', 'Discount', 'Adjustment'].map(t => <option key={t}>{t}</option>)}
            </Select>
            <Select label="Value Type" value={String(form.valueType ?? '')} onChange={e => setForm(f => ({ ...f, valueType: e.target.value }))} required>
              <option value="">Select type</option>
              {['Percentage', 'Fixed'].map(t => <option key={t}>{t}</option>)}
            </Select>
            <div className="grid grid-cols-2 gap-2">
              <Input label="Min Value" type="number" value={String(form.minValue ?? 0)} onChange={e => setForm(f => ({ ...f, minValue: parseFloat(e.target.value) }))} required />
              <Input label="Max Value" type="number" value={String(form.maxValue ?? '')} onChange={e => setForm(f => ({ ...f, maxValue: parseFloat(e.target.value) }))} required />
            </div>
          </>}
          <div className="flex justify-end gap-2 pt-2">
            <Button variant="secondary" type="button" size="sm" onClick={() => setAddOpen(false)}>Cancel</Button>
            <Button type="submit" size="sm">Add</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
