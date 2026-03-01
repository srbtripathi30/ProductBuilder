import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ArrowLeft, Calculator, Send, Pencil, Anchor, RotateCcw, RefreshCw } from 'lucide-react';
import { quotesApi } from '../../api/quotes.api';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { PageSpinner } from '../../components/ui/Spinner';
import { formatCurrency, formatDate } from '../../utils/formatters';

export function QuoteDetailPage() {
  const { quoteId } = useParams<{ quoteId: string }>();
  const navigate = useNavigate();
  const qc = useQueryClient();

  const { data: quote, isLoading } = useQuery({
    queryKey: ['quote', quoteId],
    queryFn: () => quotesApi.getById(quoteId!)
  });
  const invalidate = () => qc.invalidateQueries({ queryKey: ['quote', quoteId] });

  const calcMutation     = useMutation({ mutationFn: () => quotesApi.calculate(quoteId!),   onSuccess: invalidate });
  const submitMutation   = useMutation({ mutationFn: () => quotesApi.submit(quoteId!),      onSuccess: invalidate });
  const bindMutation     = useMutation({ mutationFn: () => quotesApi.bind(quoteId!),        onSuccess: invalidate });
  const reviseMutation   = useMutation({ mutationFn: () => quotesApi.revise(quoteId!),      onSuccess: invalidate });
  const reviseBindMutation = useMutation({
    mutationFn: () => quotesApi.reviseBind(quoteId!),
    onSuccess: () => { invalidate(); navigate(`/quotes/${quoteId}/edit`); }
  });

  if (isLoading) return <PageSpinner />;
  if (!quote) return <div className="text-center text-gray-500 p-6">Quote not found</div>;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <button onClick={() => navigate('/quotes')} className="text-gray-400 hover:text-gray-600 transition-colors">
          <ArrowLeft className="h-5 w-5" />
        </button>
        <div className="flex-1">
          <div className="flex items-center gap-3 flex-wrap">
            <h1 className="text-2xl font-bold text-gray-900">{quote.insuredName}</h1>
            <Badge status={quote.status} />
          </div>
          <p className="text-sm text-gray-500">{quote.productName} · {quote.currency}</p>
        </div>
        <div className="flex gap-2 flex-wrap">
          {quote.status === 'Draft' && <>
            <Button variant="secondary" onClick={() => navigate(`/quotes/${quoteId}/edit`)}>
              <Pencil className="mr-2 h-4 w-4" />Edit
            </Button>
            <Button variant="secondary" onClick={() => calcMutation.mutate()} loading={calcMutation.isPending}>
              <Calculator className="mr-2 h-4 w-4" />Recalculate
            </Button>
            <Button onClick={() => submitMutation.mutate()} loading={submitMutation.isPending}>
              <Send className="mr-2 h-4 w-4" />Submit
            </Button>
          </>}
          {quote.status === 'Submitted' && <>
            <Button variant="secondary" onClick={() => reviseMutation.mutate()} loading={reviseMutation.isPending}>
              <RotateCcw className="mr-2 h-4 w-4" />Revise
            </Button>
            <Button onClick={() => bindMutation.mutate()} loading={bindMutation.isPending}>
              <Anchor className="mr-2 h-4 w-4" />Bind
            </Button>
          </>}
          {quote.status === 'Bound' && (
            <Button variant="secondary" onClick={() => reviseBindMutation.mutate()} loading={reviseBindMutation.isPending}>
              <RefreshCw className="mr-2 h-4 w-4" />Revise Bind
            </Button>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Cover Table */}
        <div className="lg:col-span-2 space-y-4">
          <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
            <h2 className="font-semibold text-gray-900 mb-4">Cover Details</h2>
            {quote.covers.length > 0 ? (
              <div className="overflow-x-auto">
                <table className="min-w-full text-sm">
                  <thead>
                    <tr className="border-b">
                      {['Cover', 'Limit', 'Deductible', 'Basis Value', 'Premium'].map(h => (
                        <th key={h} className="pb-2 pr-4 text-left text-xs font-medium text-gray-500">{h}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-50">
                    {quote.covers.filter(c => c.isSelected).map(c => (
                      <tr key={c.id}>
                        <td className="py-3 pr-4 font-medium text-gray-900">{c.coverName}</td>
                        <td className="py-3 pr-4 text-gray-600">{formatCurrency(c.selectedLimit)}</td>
                        <td className="py-3 pr-4 text-gray-600">{formatCurrency(c.selectedDeductible)}</td>
                        <td className="py-3 pr-4 text-gray-600">{formatCurrency(c.basisValue)}</td>
                        <td className="py-3 font-medium text-gray-900">{formatCurrency(c.calculatedPremium)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <p className="text-sm text-gray-400">No covers configured</p>
            )}
          </div>

          {quote.modifiers.length > 0 && (
            <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
              <h2 className="font-semibold text-gray-900 mb-4">Modifiers</h2>
              <div className="divide-y">
                {quote.modifiers.map(m => (
                  <div key={m.id} className="flex justify-between py-2">
                    <span className="text-gray-700 text-sm">{m.modifierName}</span>
                    <span className={`text-sm font-medium ${m.premiumImpact && m.premiumImpact < 0 ? 'text-green-600' : 'text-red-600'}`}>
                      {m.premiumImpact != null ? formatCurrency(m.premiumImpact) : '—'}
                    </span>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-4">
          <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
            <h2 className="font-semibold text-gray-900 mb-4">Premium Summary</h2>
            <div className="space-y-3 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-500">Base Premium</span>
                <span className="font-medium">{formatCurrency(quote.basePremium, quote.currency)}</span>
              </div>
              {quote.modifiers.map(m => (
                <div key={m.id} className="flex justify-between text-xs">
                  <span className="text-gray-400">{m.modifierName}</span>
                  <span className={m.premiumImpact && m.premiumImpact < 0 ? 'text-green-600' : 'text-amber-600'}>
                    {m.premiumImpact != null ? formatCurrency(m.premiumImpact) : '—'}
                  </span>
                </div>
              ))}
              <div className="flex justify-between border-t pt-3">
                <span className="font-semibold text-gray-900">Total Premium</span>
                <span className="text-xl font-bold text-primary-700">{formatCurrency(quote.totalPremium, quote.currency)}</span>
              </div>
            </div>
          </div>

          <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm text-sm space-y-3">
            <h2 className="font-semibold text-gray-900">Quote Information</h2>
            {[
              { label: 'Broker', value: quote.brokerName ?? '—' },
              { label: 'Underwriter', value: quote.underwriterName ?? '—' },
              { label: 'Email', value: quote.insuredEmail ?? '—' },
              { label: 'Phone', value: quote.insuredPhone ?? '—' },
              { label: 'Valid Until', value: formatDate(quote.validUntil) },
              { label: 'Created', value: formatDate(quote.createdAt) },
            ].map(item => (
              <div key={item.label}>
                <span className="text-xs text-gray-400 block">{item.label}</span>
                <span className="text-gray-800">{item.value}</span>
              </div>
            ))}
            {quote.notes && (
              <div>
                <span className="text-xs text-gray-400 block">Notes</span>
                <span className="text-gray-800">{quote.notes}</span>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
