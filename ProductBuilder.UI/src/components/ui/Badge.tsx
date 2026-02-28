import React from 'react';
import { cn } from '../../utils/cn';
import { statusColor } from '../../utils/formatters';

interface BadgeProps {
  status: string;
  className?: string;
}

export function Badge({ status, className }: BadgeProps) {
  return (
    <span className={cn('inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium', statusColor(status), className)}>
      {status}
    </span>
  );
}
