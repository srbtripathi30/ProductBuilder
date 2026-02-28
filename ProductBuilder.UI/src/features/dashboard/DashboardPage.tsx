import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Package, FileText, Building2, TrendingUp } from 'lucide-react';
import { productsApi } from '../../api/products.api';
import { quotesApi } from '../../api/quotes.api';
import { insurersApi } from '../../api/stakeholders.api';
import { PageSpinner } from '../../components/ui/Spinner';
import { Badge } from '../../components/ui/Badge';
import { formatCurrency, formatDate } from '../../utils/formatters';

export function DashboardPage() {
  const { data: products, isLoading: lp } = useQuery({ queryKey: ['products'], queryFn: () => productsApi.getAll() });
  const { data: quotes, isLoading: lq } = useQuery({ queryKey: ['quotes'], queryFn: () => quotesApi.getAll() });
  const { data: insurers, isLoading: li } = useQuery({ queryKey: ['insurers'], queryFn: () => insurersApi.getAll() });

  if (lp || lq || li) return <PageSpinner />;

  const activeProducts = products?.filter(p => p.status === 'Active').length ?? 0;
  const draftQuotes = quotes?.filter(q => q.status === 'Draft').length ?? 0;
  const submittedQuotes = quotes?.filter(q => q.status === 'Submitted').length ?? 0;
  const totalPremium = quotes?.reduce((sum, q) => sum + (q.totalPremium ?? 0), 0) ?? 0;

  const stats = [
    { label: 'Active Products', value: activeProducts, icon: <Package className="h-6 w-6 text-primary-600" />, bg: 'bg-primary-50' },
    { label: 'Draft Quotes', value: draftQuotes, icon: <FileText className="h-6 w-6 text-yellow-600" />, bg: 'bg-yellow-50' },
    { label: 'Submitted Quotes', value: submittedQuotes, icon: <TrendingUp className="h-6 w-6 text-green-600" />, bg: 'bg-green-50' },
    { label: 'Active Insurers', value: insurers?.filter(i => i.isActive).length ?? 0, icon: <Building2 className="h-6 w-6 text-purple-600" />, bg: 'bg-purple-50' },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-sm text-gray-500">Insurance Product Builder Overview</p>
      </div>

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map(stat => (
          <div key={stat.label} className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">{stat.label}</p>
                <p className="mt-1 text-3xl font-bold text-gray-900">{stat.value}</p>
              </div>
              <div className={`rounded-xl p-3 ${stat.bg}`}>{stat.icon}</div>
            </div>
          </div>
        ))}
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900">Total Premium Portfolio</h2>
        <p className="mt-2 text-4xl font-bold text-primary-700">{formatCurrency(totalPremium)}</p>
        <p className="text-sm text-gray-500">Across {quotes?.length ?? 0} quotes</p>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Recent Quotes</h2>
        <div className="divide-y">
          {quotes?.slice(0, 5).map(q => (
            <div key={q.id} className="flex items-center justify-between py-3">
              <div>
                <p className="font-medium text-gray-900">{q.insuredName}</p>
                <p className="text-sm text-gray-500">{q.productName} · {formatDate(q.createdAt)}</p>
              </div>
              <div className="flex items-center gap-3">
                <span className="font-medium text-gray-900">{formatCurrency(q.totalPremium)}</span>
                <Badge status={q.status} />
              </div>
            </div>
          ))}
          {!quotes?.length && <p className="py-4 text-center text-sm text-gray-400">No quotes yet</p>}
        </div>
      </div>
    </div>
  );
}
