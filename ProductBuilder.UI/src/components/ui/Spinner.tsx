import React from 'react';
import { cn } from '../../utils/cn';

export function Spinner({ className }: { className?: string }) {
  return (
    <div className={cn('h-8 w-8 animate-spin rounded-full border-4 border-gray-200 border-t-primary-600', className)} />
  );
}

export function PageSpinner() {
  return (
    <div className="flex h-64 items-center justify-center">
      <Spinner />
    </div>
  );
}
