import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Plus, Eye } from 'lucide-react';
import { quotesApi } from '../../api/quotes.api';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { PageSpinner } from '../../components/ui/Spinner';
import { formatCurrency, formatDate } from '../../utils/formatters';

export function QuoteListPage() {
  const navigate = useNavigate();
  const { data: quotes, isLoading } = useQuery({ queryKey: ['quotes'], queryFn: () => quotesApi.getAll() });

  if (isLoading) return <PageSpinner />;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div><h1 className="text-2xl font-bold text-gray-900">Quotes</h1><p className="text-sm text-gray-500">Manage insurance quotes</p></div>
        <Button onClick={() => navigate('/quotes/new')}><Plus className="mr-2 h-4 w-4" />New Quote</Button>
      </div>
      <div className="rounded-xl border border-gray-200 bg-white shadow-sm overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>{['Insured', 'Product', 'Broker', 'Status', 'Total Premium', 'Valid Until', 'Created', ''].map(h => (
              <th key={h} className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{h}</th>
            ))}</tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {quotes?.map(q => (
              <tr key={q.id} className="hover:bg-gray-50">
                <td className="px-4 py-4">
                  <div className="font-medium text-gray-900">{q.insuredName}</div>
                  {q.insuredEmail && <div className="text-xs text-gray-400">{q.insuredEmail}</div>}
                </td>
                <td className="px-4 py-4 text-sm text-gray-600">{q.productName}</td>
                <td className="px-4 py-4 text-sm text-gray-600">{q.brokerName ?? '—'}</td>
                <td className="px-4 py-4"><Badge status={q.status} /></td>
                <td className="px-4 py-4 text-sm font-medium text-gray-900">{formatCurrency(q.totalPremium, q.currency)}</td>
                <td className="px-4 py-4 text-sm text-gray-500">{formatDate(q.validUntil)}</td>
                <td className="px-4 py-4 text-sm text-gray-500">{formatDate(q.createdAt)}</td>
                <td className="px-4 py-4">
                  <button onClick={() => navigate(`/quotes/${q.id}`)} className="text-primary-600 hover:text-primary-800 transition-colors">
                    <Eye className="h-4 w-4" />
                  </button>
                </td>
              </tr>
            ))}
            {!quotes?.length && <tr><td colSpan={8} className="px-6 py-8 text-center text-sm text-gray-400">No quotes yet. Create your first quote.</td></tr>}
          </tbody>
        </table>
      </div>
    </div>
  );
}
