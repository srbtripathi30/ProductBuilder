import React from 'react';
import { cn } from '../../utils/cn';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  loading?: boolean;
}

export function Button({ variant = 'primary', size = 'md', loading, className, children, disabled, ...props }: ButtonProps) {
  const base = 'inline-flex items-center justify-center font-medium rounded-lg transition-all duration-150 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed';
  const variants = {
    primary: 'bg-primary-600 text-white shadow-sm hover:bg-primary-700 active:bg-primary-800 focus:ring-primary-500',
    secondary: 'bg-white text-gray-700 border border-gray-200 shadow-sm hover:bg-gray-50 hover:border-gray-300 focus:ring-primary-500',
    ghost: 'text-gray-600 hover:bg-gray-100 hover:text-gray-900 focus:ring-primary-500',
    danger: 'bg-red-600 text-white shadow-sm hover:bg-red-700 active:bg-red-800 focus:ring-red-500',
  };
  const sizes = { sm: 'px-3 py-1.5 text-sm gap-1.5', md: 'px-4 py-2 text-sm gap-2', lg: 'px-6 py-3 text-base gap-2' };
  return (
    <button className={cn(base, variants[variant], sizes[size], className)} disabled={disabled || loading} {...props}>
      {loading && <span className="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />}
      {children}
    </button>
  );
}
