import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Plus, Eye } from 'lucide-react';
import { quotesApi } from '../../api/quotes.api';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { PageHeader } from '../../components/ui/PageHeader';
import { PageSpinner } from '../../components/ui/Spinner';
import { formatCurrency, formatDate } from '../../utils/formatters';

export function QuoteListPage() {
  const navigate = useNavigate();
  const { data: quotes, isLoading } = useQuery({ queryKey: ['quotes'], queryFn: () => quotesApi.getAll() });

  if (isLoading) return <PageSpinner />;

  return (
    <div className="space-y-6">
      <PageHeader
        title="Quotes"
        subtitle="Manage insurance quotes"
        action={<Button onClick={() => navigate('/quotes/new')}><Plus className="mr-2 h-4 w-4" />New Quote</Button>}
      />
      <div className="rounded-xl border border-gray-100 bg-white shadow-card overflow-hidden">
        <table className="min-w-full divide-y divide-gray-100">
          <thead className="bg-slate-50">
            <tr>{['Insured', 'Product', 'Broker', 'Status', 'Total Premium', 'Valid Until', 'Created', ''].map(h => (
              <th key={h} className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">{h}</th>
            ))}</tr>
          </thead>
          <tbody className="divide-y divide-gray-50">
            {quotes?.map(q => (
              <tr key={q.id} className="hover:bg-slate-50 transition-colors">
                <td className="px-4 py-3.5">
                  <div className="text-sm font-medium text-gray-900">{q.insuredName}</div>
                  {q.insuredEmail && <div className="text-xs text-gray-400">{q.insuredEmail}</div>}
                </td>
                <td className="px-4 py-3.5 text-sm text-gray-600">{q.productName}</td>
                <td className="px-4 py-3.5 text-sm text-gray-600">{q.brokerName ?? '—'}</td>
                <td className="px-4 py-3.5"><Badge status={q.status} /></td>
                <td className="px-4 py-3.5 text-sm font-semibold text-gray-900">{formatCurrency(q.totalPremium, q.currency)}</td>
                <td className="px-4 py-3.5 text-sm text-gray-500">{formatDate(q.validUntil)}</td>
                <td className="px-4 py-3.5 text-sm text-gray-500">{formatDate(q.createdAt)}</td>
                <td className="px-4 py-3.5">
                  <button aria-label={`View quote for ${q.insuredName}`} onClick={() => navigate(`/quotes/${q.id}`)} className="rounded p-1 text-gray-400 transition-colors hover:bg-primary-50 hover:text-primary-600">
                    <Eye className="h-4 w-4" />
                  </button>
                </td>
              </tr>
            ))}
            {!quotes?.length && <tr><td colSpan={8} className="px-6 py-10 text-center text-sm text-gray-400">No quotes yet. Create your first quote.</td></tr>}
          </tbody>
        </table>
      </div>
    </div>
  );
}
