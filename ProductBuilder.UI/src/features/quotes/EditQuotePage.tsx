import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import { ArrowLeft, Save } from 'lucide-react';
import { quotesApi } from '../../api/quotes.api';
import { productsApi } from '../../api/products.api';
import { brokersApi, underwritersApi } from '../../api/stakeholders.api';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { AmountInput } from '../../components/ui/AmountInput';
import { PageSpinner } from '../../components/ui/Spinner';

const CURRENCIES = ['USD', 'EUR', 'GBP', 'AED', 'SAR'];

interface CoverInput {
  coverId: string;
  coverName: string;
  isMandatory: boolean;
  isSelected: boolean;
  basisValue?: number;
  selectedLimit?: number;
  selectedDeductible?: number;
}

export function EditQuotePage() {
  const { quoteId } = useParams<{ quoteId: string }>();
  const navigate = useNavigate();

  const { data: quote, isLoading: loadingQuote, isFetching: fetchingQuote } = useQuery({
    queryKey: ['quote', quoteId],
    queryFn: () => quotesApi.getById(quoteId!),
  });

  const { data: coverages, isLoading: loadingCoverages } = useQuery({
    queryKey: ['coverages', quote?.productId],
    queryFn: () => productsApi.getCoverages(quote!.productId),
    enabled: !!quote?.productId,
  });
  const { data: brokers } = useQuery({
    queryKey: ['brokers'],
    queryFn: brokersApi.getAll,
  });
  const { data: underwriters } = useQuery({
    queryKey: ['underwriters'],
    queryFn: underwritersApi.getAll,
  });

  const [details, setDetails] = useState({
    brokerId: '',
    underwriterId: '',
    insuredName: '', insuredEmail: '', insuredPhone: '',
    currency: 'USD', validUntil: '', notes: '',
  });
  const [covers, setCovers] = useState<CoverInput[]>([]);
  const [initialized, setInitialized] = useState(false);

  useEffect(() => {
    if (!quote || !coverages || initialized) return;
    setDetails({
      brokerId: quote.brokerId ?? '',
      underwriterId: quote.underwriterId ?? '',
      insuredName: quote.insuredName,
      insuredEmail: quote.insuredEmail ?? '',
      insuredPhone: quote.insuredPhone ?? '',
      currency: quote.currency,
      validUntil: quote.validUntil ?? '',
      notes: quote.notes ?? '',
    });
    setCovers(
      coverages.flatMap(cov =>
        cov.covers.map(cv => {
          const existing = quote.covers.find(qc => qc.coverId === cv.id);
          return {
            coverId: cv.id,
            coverName: cv.name,
            isMandatory: cv.isMandatory,
            isSelected: existing?.isSelected ?? cv.isMandatory,
            basisValue: existing?.basisValue,
            selectedLimit: existing?.selectedLimit,
            selectedDeductible: existing?.selectedDeductible,
          };
        })
      )
    );
    setInitialized(true);
  }, [quote, coverages, initialized]);

  const [saveError, setSaveError] = useState('');

  const saveMutation = useMutation({
    mutationFn: (data: object) => quotesApi.update(quoteId!, data),
    onSuccess: async () => {
      try { await quotesApi.calculate(quoteId!); } catch { /* best effort */ }
      navigate(`/quotes/${quoteId}`);
    },
    onError: (err: unknown) => {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setSaveError(msg ?? 'Failed to save quote. Please try again.');
    },
  });

  const updateCover = (coverId: string, patch: Partial<CoverInput>) =>
    setCovers(prev => prev.map(c => c.coverId === coverId ? { ...c, ...patch } : c));

  const handleSave = () => {
    saveMutation.mutate({
      brokerId: details.brokerId || undefined,
      underwriterId: details.underwriterId || undefined,
      insuredName: details.insuredName,
      insuredEmail: details.insuredEmail || undefined,
      insuredPhone: details.insuredPhone || undefined,
      currency: details.currency,
      validUntil: details.validUntil || undefined,
      notes: details.notes || undefined,
      covers: covers.map(c => ({
        coverId: c.coverId,
        isSelected: c.isSelected,
        basisValue: c.basisValue,
        selectedLimit: c.selectedLimit,
        selectedDeductible: c.selectedDeductible,
      })),
    });
  };

  // Wait for any in-flight refetch (e.g. after Revise Bind) before checking status,
  // otherwise stale "Bound" data would immediately redirect the user away.
  if (loadingQuote || fetchingQuote || loadingCoverages || !initialized) return <PageSpinner />;
  if (!quote) return <div className="p-6 text-gray-500">Quote not found</div>;

  if (quote.status !== 'Draft') {
    navigate(`/quotes/${quoteId}`);
    return null;
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <button onClick={() => navigate(`/quotes/${quoteId}`)} className="text-gray-400 hover:text-gray-600 transition-colors">
          <ArrowLeft className="h-5 w-5" />
        </button>
        <div className="flex-1">
          <h1 className="text-2xl font-bold text-gray-900">Edit Quote</h1>
          <p className="text-sm text-gray-500">{quote.productName}</p>
        </div>
        <Button onClick={handleSave} loading={saveMutation.isPending}>
          <Save className="mr-2 h-4 w-4" />Save & Recalculate
        </Button>
      </div>

      {/* Insured Details */}
      <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
        <h2 className="font-semibold text-gray-900 mb-4">Insured Details</h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <Select label="Broker" value={details.brokerId} onChange={e => setDetails(d => ({ ...d, brokerId: e.target.value }))}>
            <option value="">Select broker (optional)…</option>
            {brokers?.filter(b => b.isActive).map(b => (
              <option key={b.id} value={b.id}>{b.companyName}</option>
            ))}
          </Select>
          <Select label="Underwriter" value={details.underwriterId} onChange={e => setDetails(d => ({ ...d, underwriterId: e.target.value }))}>
            <option value="">Select underwriter (optional)…</option>
            {underwriters?.map(u => (
              <option key={u.id} value={u.id}>{u.userName}</option>
            ))}
          </Select>
          <Input label="Insured Name" value={details.insuredName} onChange={e => setDetails(d => ({ ...d, insuredName: e.target.value }))} required />
          <Input label="Email" type="email" value={details.insuredEmail} onChange={e => setDetails(d => ({ ...d, insuredEmail: e.target.value }))} />
          <Input label="Phone" value={details.insuredPhone} onChange={e => setDetails(d => ({ ...d, insuredPhone: e.target.value }))} />
          <Select label="Currency" value={details.currency} onChange={e => setDetails(d => ({ ...d, currency: e.target.value }))}>
            {CURRENCIES.map(c => <option key={c}>{c}</option>)}
          </Select>
          <Input label="Valid Until" type="date" value={details.validUntil} onChange={e => setDetails(d => ({ ...d, validUntil: e.target.value }))} />
        </div>
        <div className="mt-4 space-y-1">
          <label className="block text-sm font-medium text-gray-700">Notes</label>
          <textarea
            className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm placeholder-gray-400 focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
            rows={3}
            value={details.notes}
            onChange={e => setDetails(d => ({ ...d, notes: e.target.value }))}
            placeholder="Any additional notes…"
          />
        </div>
      </div>

      {/* Cover Configuration */}
      <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
        <h2 className="font-semibold text-gray-900 mb-4">Cover Configuration</h2>
        <div className="space-y-3">
          {covers.map(cover => (
            <div key={cover.coverId} className={`rounded-lg border p-3 transition-colors ${cover.isSelected ? 'border-primary-200 bg-primary-50/40' : 'border-gray-100 bg-gray-50'}`}>
              <div className="flex items-center gap-3 mb-2">
                <input
                  type="checkbox"
                  checked={cover.isSelected}
                  disabled={cover.isMandatory}
                  onChange={e => updateCover(cover.coverId, { isSelected: e.target.checked })}
                  className="h-4 w-4 rounded text-primary-600"
                />
                <span className="font-medium text-sm text-gray-900">{cover.coverName}</span>
                {cover.isMandatory && (
                  <span className="text-xs bg-blue-100 text-blue-700 px-2 py-0.5 rounded-full">Mandatory</span>
                )}
              </div>
              {cover.isSelected && (
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 mt-3 ml-7">
                  <AmountInput label="Basis Value (SI / Revenue)" placeholder="0"
                    value={cover.basisValue} onChange={v => updateCover(cover.coverId, { basisValue: isNaN(v) ? undefined : v })} />
                  <AmountInput label="Selected Limit" placeholder="Optional"
                    value={cover.selectedLimit} onChange={v => updateCover(cover.coverId, { selectedLimit: isNaN(v) ? undefined : v })} />
                  <AmountInput label="Selected Deductible" placeholder="Optional"
                    value={cover.selectedDeductible} onChange={v => updateCover(cover.coverId, { selectedDeductible: isNaN(v) ? undefined : v })} />
                </div>
              )}
            </div>
          ))}
        </div>
      </div>

      {saveError && (
        <p className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{saveError}</p>
      )}
      <div className="flex justify-end gap-3">
        <Button variant="secondary" onClick={() => navigate(`/quotes/${quoteId}`)}>Cancel</Button>
        <Button onClick={handleSave} loading={saveMutation.isPending}>
          <Save className="mr-2 h-4 w-4" />Save & Recalculate
        </Button>
      </div>
    </div>
  );
}
