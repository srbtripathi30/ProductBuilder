import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import { ArrowLeft, ArrowRight, Check } from 'lucide-react';
import { productsApi } from '../../api/products.api';
import { quotesApi } from '../../api/quotes.api';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { PageSpinner } from '../../components/ui/Spinner';
import type { ProductDto, CoverageDto } from '../../types';

const STEPS = ['Select Product', 'Configure Covers', 'Modifiers & Notes', 'Review & Submit'];

interface CoverInput {
  coverId: string;
  coverName: string;
  isMandatory: boolean;
  isSelected: boolean;
  basisValue?: number;
  selectedLimit?: number;
  selectedDeductible?: number;
}

export function QuoteWizard() {
  const navigate = useNavigate();
  const [step, setStep] = useState(0);
  const [selectedProduct, setSelectedProduct] = useState<ProductDto | null>(null);
  const [coverages, setCoverages] = useState<CoverageDto[]>([]);
  const [covers, setCovers] = useState<CoverInput[]>([]);
  const [quoteData, setQuoteData] = useState({
    insuredName: '', insuredEmail: '', insuredPhone: '',
    currency: 'USD', validUntil: '', notes: ''
  });

  const { data: products, isLoading } = useQuery({
    queryKey: ['products-active'],
    queryFn: () => productsApi.getAll({ status: 'Active' })
  });

  const createMutation = useMutation({
    mutationFn: (data: object) => quotesApi.create(data),
    onSuccess: async (quote: any) => {
      try { await quotesApi.calculate(quote.id); } catch { /* best effort */ }
      navigate(`/quotes/${quote.id}`);
    }
  });

  const handleProductSelect = async (productId: string) => {
    const product = products?.find(p => p.id === productId) ?? null;
    setSelectedProduct(product);
    if (productId) {
      const cvgs = await productsApi.getCoverages(productId);
      setCoverages(cvgs);
      setCovers(cvgs.flatMap(c =>
        c.covers.map(cv => ({
          coverId: cv.id, coverName: cv.name, isMandatory: cv.isMandatory,
          isSelected: cv.isMandatory, basisValue: undefined, selectedLimit: undefined, selectedDeductible: undefined
        }))
      ));
    }
  };

  const updateCover = (coverId: string, patch: Partial<CoverInput>) =>
    setCovers(prev => prev.map(c => c.coverId === coverId ? { ...c, ...patch } : c));

  const handleSubmit = () => {
    createMutation.mutate({
      productId: selectedProduct!.id,
      insuredName: quoteData.insuredName,
      insuredEmail: quoteData.insuredEmail || undefined,
      insuredPhone: quoteData.insuredPhone || undefined,
      currency: quoteData.currency,
      validUntil: quoteData.validUntil || undefined,
      notes: quoteData.notes || undefined,
      covers: covers.map(c => ({ coverId: c.coverId, isSelected: c.isSelected, basisValue: c.basisValue, selectedLimit: c.selectedLimit, selectedDeductible: c.selectedDeductible })),
      modifiers: []
    });
  };

  const canProceed = () => {
    if (step === 0) return !!selectedProduct && !!quoteData.insuredName;
    return true;
  };

  if (isLoading) return <PageSpinner />;

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <button onClick={() => navigate('/quotes')} className="text-gray-400 hover:text-gray-600"><ArrowLeft className="h-5 w-5" /></button>
        <h1 className="text-2xl font-bold text-gray-900">New Quote</h1>
      </div>

      {/* Stepper */}
      <div className="flex items-center">
        {STEPS.map((s, i) => (
          <React.Fragment key={s}>
            <div className="flex items-center gap-2">
              <div className={`flex h-8 w-8 items-center justify-center rounded-full text-sm font-medium transition-colors
                ${i < step ? 'bg-green-500 text-white' : i === step ? 'bg-primary-600 text-white' : 'bg-gray-200 text-gray-500'}`}>
                {i < step ? <Check className="h-4 w-4" /> : i + 1}
              </div>
              <span className={`text-sm font-medium hidden sm:block ${i === step ? 'text-primary-700' : 'text-gray-400'}`}>{s}</span>
            </div>
            {i < STEPS.length - 1 && <div className={`mx-3 h-px flex-1 transition-colors ${i < step ? 'bg-green-400' : 'bg-gray-200'}`} />}
          </React.Fragment>
        ))}
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
        {/* Step 0 */}
        {step === 0 && (
          <div className="space-y-6">
            <h2 className="text-lg font-semibold text-gray-900">Select Product & Insured Details</h2>
            <Select label="Active Product" value={selectedProduct?.id ?? ''} onChange={e => handleProductSelect(e.target.value)} required>
              <option value="">Select a product…</option>
              {products?.map(p => <option key={p.id} value={p.id}>{p.name} ({p.code}) — {p.insurerName}</option>)}
            </Select>
            {selectedProduct && (
              <div className="rounded-lg bg-blue-50 p-3 text-sm text-blue-800">
                <strong>{selectedProduct.name}</strong> · {selectedProduct.lobName} · v{selectedProduct.version}
              </div>
            )}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <Input label="Insured Name *" value={quoteData.insuredName} onChange={e => setQuoteData(d => ({ ...d, insuredName: e.target.value }))} required />
              <Input label="Insured Email" type="email" value={quoteData.insuredEmail} onChange={e => setQuoteData(d => ({ ...d, insuredEmail: e.target.value }))} />
              <Input label="Insured Phone" value={quoteData.insuredPhone} onChange={e => setQuoteData(d => ({ ...d, insuredPhone: e.target.value }))} />
              <Input label="Valid Until" type="date" value={quoteData.validUntil} onChange={e => setQuoteData(d => ({ ...d, validUntil: e.target.value }))} />
              <Select label="Currency" value={quoteData.currency} onChange={e => setQuoteData(d => ({ ...d, currency: e.target.value }))}>
                {['USD', 'EUR', 'GBP', 'AED', 'SAR'].map(c => <option key={c}>{c}</option>)}
              </Select>
            </div>
          </div>
        )}

        {/* Step 1 */}
        {step === 1 && (
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900">Configure Covers</h2>
            <p className="text-sm text-gray-500">Select covers and enter basis values (sum insured, revenue, etc.) for premium calculation.</p>
            {coverages.map(cov => (
              <div key={cov.id} className="rounded-xl border border-gray-200 p-4">
                <h3 className="font-semibold text-gray-900 mb-3">{cov.name}</h3>
                <div className="space-y-3">
                  {covers.filter(c => cov.covers.some(cv => cv.id === c.coverId)).map(cover => (
                    <div key={cover.coverId} className={`rounded-lg border p-3 transition-colors ${cover.isSelected ? 'border-primary-200 bg-primary-50/40' : 'border-gray-100 bg-gray-50'}`}>
                      <div className="flex items-center gap-3 mb-2">
                        <input type="checkbox" checked={cover.isSelected} disabled={cover.isMandatory}
                          onChange={e => updateCover(cover.coverId, { isSelected: e.target.checked })}
                          className="h-4 w-4 rounded text-primary-600" />
                        <span className="font-medium text-sm text-gray-900">{cover.coverName}</span>
                        {cover.isMandatory && <span className="text-xs bg-blue-100 text-blue-700 px-2 py-0.5 rounded-full">Mandatory</span>}
                      </div>
                      {cover.isSelected && (
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 mt-3 ml-7">
                          <Input label="Basis Value (SI / Revenue)" type="number" placeholder="0"
                            value={String(cover.basisValue ?? '')} onChange={e => updateCover(cover.coverId, { basisValue: parseFloat(e.target.value) })} />
                          <Input label="Selected Limit" type="number" placeholder="Optional"
                            value={String(cover.selectedLimit ?? '')} onChange={e => updateCover(cover.coverId, { selectedLimit: parseFloat(e.target.value) })} />
                          <Input label="Selected Deductible" type="number" placeholder="Optional"
                            value={String(cover.selectedDeductible ?? '')} onChange={e => updateCover(cover.coverId, { selectedDeductible: parseFloat(e.target.value) })} />
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Step 2 */}
        {step === 2 && (
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900">Notes</h2>
            <p className="text-sm text-gray-500">Add any additional notes for this quote. Modifiers can be applied after creation.</p>
            <textarea
              className="w-full rounded-lg border border-gray-300 p-3 text-sm focus:outline-none focus:ring-1 focus:ring-primary-500"
              rows={5} placeholder="Enter any notes or special conditions…"
              value={quoteData.notes} onChange={e => setQuoteData(d => ({ ...d, notes: e.target.value }))}
            />
          </div>
        )}

        {/* Step 3 */}
        {step === 3 && (
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900">Review & Submit</h2>
            <div className="rounded-xl bg-gray-50 border border-gray-200 p-5 space-y-4">
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div><span className="text-gray-500 block text-xs">Product</span><span className="font-medium">{selectedProduct?.name}</span></div>
                <div><span className="text-gray-500 block text-xs">Insured</span><span className="font-medium">{quoteData.insuredName}</span></div>
                <div><span className="text-gray-500 block text-xs">Currency</span><span className="font-medium">{quoteData.currency}</span></div>
                <div><span className="text-gray-500 block text-xs">Valid Until</span><span className="font-medium">{quoteData.validUntil || '—'}</span></div>
              </div>
              <div className="border-t pt-3">
                <p className="text-xs text-gray-500 mb-2">Selected Covers ({covers.filter(c => c.isSelected).length})</p>
                <div className="space-y-1">
                  {covers.filter(c => c.isSelected).map(c => (
                    <div key={c.coverId} className="text-sm text-gray-700 flex items-center gap-2">
                      <Check className="h-3 w-3 text-green-500" />{c.coverName}
                      {c.basisValue ? <span className="text-gray-400 text-xs">· Basis: {c.basisValue.toLocaleString()}</span> : null}
                    </div>
                  ))}
                </div>
              </div>
              {quoteData.notes && <div className="border-t pt-3 text-sm"><span className="text-gray-500 text-xs block">Notes</span>{quoteData.notes}</div>}
            </div>
            <p className="text-xs text-gray-400">Premium will be automatically calculated after creation.</p>
          </div>
        )}

        {/* Navigation */}
        <div className="mt-8 flex justify-between">
          <Button variant="secondary" onClick={() => step > 0 ? setStep(s => s - 1) : navigate('/quotes')} disabled={createMutation.isPending}>
            <ArrowLeft className="mr-2 h-4 w-4" />{step === 0 ? 'Cancel' : 'Back'}
          </Button>
          {step < STEPS.length - 1 ? (
            <Button onClick={() => setStep(s => s + 1)} disabled={!canProceed()}>
              Next <ArrowRight className="ml-2 h-4 w-4" />
            </Button>
          ) : (
            <Button onClick={handleSubmit} loading={createMutation.isPending}>
              Create & Calculate Quote
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
