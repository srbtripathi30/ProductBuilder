export function formatCurrency(amount: number | undefined | null, currency = 'USD'): string {
  if (amount == null) return '-';
  return new Intl.NumberFormat('en-US', { style: 'currency', currency }).format(amount);
}

export function formatDate(date: string | undefined | null): string {
  if (!date) return '-';
  return new Date(date).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
}

export function statusColor(status: string): string {
  const map: Record<string, string> = {
    Draft: 'bg-gray-100 text-gray-700',
    Active: 'bg-green-100 text-green-700',
    Inactive: 'bg-yellow-100 text-yellow-700',
    Archived: 'bg-red-100 text-red-700',
    Submitted: 'bg-blue-100 text-blue-700',
    Bound: 'bg-amber-100 text-amber-700',
    Approved: 'bg-green-100 text-green-700',
    Declined: 'bg-red-100 text-red-700',
    Expired: 'bg-gray-100 text-gray-700',
  };
  return map[status] ?? 'bg-gray-100 text-gray-700';
}
