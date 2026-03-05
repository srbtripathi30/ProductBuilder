import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Package, FileText, Building2, TrendingUp } from 'lucide-react';
import { productsApi } from '../../api/products.api';
import { quotesApi } from '../../api/quotes.api';
import { insurersApi } from '../../api/stakeholders.api';
import { PageSpinner } from '../../components/ui/Spinner';
import { Badge } from '../../components/ui/Badge';
import { PageHeader } from '../../components/ui/PageHeader';
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
    {
      label: 'Active Products', value: activeProducts,
      icon: <Package className="h-5 w-5" />,
      gradient: 'from-primary-500 to-primary-600',
    },
    {
      label: 'Draft Quotes', value: draftQuotes,
      icon: <FileText className="h-5 w-5" />,
      gradient: 'from-amber-400 to-amber-500',
    },
    {
      label: 'Submitted Quotes', value: submittedQuotes,
      icon: <TrendingUp className="h-5 w-5" />,
      gradient: 'from-emerald-400 to-emerald-500',
    },
    {
      label: 'Active Insurers', value: insurers?.filter(i => i.isActive).length ?? 0,
      icon: <Building2 className="h-5 w-5" />,
      gradient: 'from-violet-400 to-violet-500',
    },
  ];

  return (
    <div className="space-y-6">
      <PageHeader title="Dashboard" subtitle="Insurance Product Builder Overview" />

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map(stat => (
          <div
            key={stat.label}
            className={`rounded-xl bg-gradient-to-br ${stat.gradient} p-6 shadow-card-md`}
          >
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-white/75">{stat.label}</p>
                <p className="mt-1.5 text-3xl font-bold text-white">{stat.value}</p>
              </div>
              <div className="rounded-xl bg-white/20 p-3 text-white">{stat.icon}</div>
            </div>
          </div>
        ))}
      </div>

      <div className="rounded-xl bg-gradient-to-br from-slate-800 to-slate-900 p-6 shadow-card-md">
        <p className="text-sm font-medium text-slate-400">Total Premium Portfolio</p>
        <p className="mt-2 text-4xl font-bold text-white">{formatCurrency(totalPremium)}</p>
        <p className="mt-1 text-sm text-slate-400">Across {quotes?.length ?? 0} quotes</p>
      </div>

      <div className="rounded-xl border border-gray-100 bg-white shadow-card">
        <div className="flex items-center justify-between border-b border-gray-100 px-6 py-4">
          <h2 className="text-base font-semibold text-gray-900">Recent Quotes</h2>
          <span className="text-xs text-gray-400">{quotes?.length ?? 0} total</span>
        </div>
        <div className="divide-y divide-gray-50">
          {quotes?.slice(0, 5).map(q => (
            <div key={q.id} className="flex items-center justify-between px-6 py-4 hover:bg-slate-50 transition-colors">
              <div>
                <p className="text-sm font-medium text-gray-900">{q.insuredName}</p>
                <p className="mt-0.5 text-xs text-gray-400">{q.productName} · {formatDate(q.createdAt)}</p>
              </div>
              <div className="flex items-center gap-3">
                <span className="text-sm font-semibold text-gray-900">{formatCurrency(q.totalPremium)}</span>
                <Badge status={q.status} />
              </div>
            </div>
          ))}
          {!quotes?.length && (
            <p className="px-6 py-8 text-center text-sm text-gray-400">No quotes yet</p>
          )}
        </div>
      </div>
    </div>
  );
}
