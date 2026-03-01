import React, { useState, useEffect, useRef } from 'react';
import { cn } from '../../utils/cn';

const MULTIPLIERS: Record<string, number> = {
  k: 1_000,
  m: 1_000_000,
  l: 100_000,
};

export function parseAmountString(raw: string): number | null {
  const match = raw.trim().match(/^(-?\d*\.?\d+)\s*([kml])?$/i);
  if (!match) return null;
  const num = parseFloat(match[1]);
  const suffix = match[2]?.toLowerCase();
  return isNaN(num) ? null : suffix ? num * MULTIPLIERS[suffix] : num;
}

interface AmountInputProps {
  label?: string;
  error?: string;
  value?: number | string;
  onChange?: (value: number) => void;
  placeholder?: string;
  required?: boolean;
  className?: string;
  id?: string;
}

export function AmountInput({ label, error, value, onChange, placeholder, required, className, id }: AmountInputProps) {
  const toDisplay = (v: number | string | undefined): string => {
    if (v === undefined || v === '' || v === null) return '';
    const n = Number(v);
    return isNaN(n) ? '' : String(n);
  };

  const [displayValue, setDisplayValue] = useState(() => toDisplay(value));
  const isFocusedRef = useRef(false);

  useEffect(() => {
    if (!isFocusedRef.current) {
      setDisplayValue(toDisplay(value));
    }
  }, [value]);

  const applyConversion = (raw: string) => {
    if (!raw.trim()) {
      onChange?.(NaN);
      return;
    }
    const parsed = parseAmountString(raw);
    if (parsed !== null) {
      setDisplayValue(String(parsed));
      onChange?.(parsed);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Tab') {
      applyConversion(displayValue);
    }
  };

  const handleBlur = () => {
    isFocusedRef.current = false;
    applyConversion(displayValue);
  };

  const handleFocus = () => {
    isFocusedRef.current = true;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setDisplayValue(e.target.value);
  };

  return (
    <div className="space-y-1">
      {label && (
        <label htmlFor={id} className="block text-sm font-medium text-gray-700">
          {label}
          <span className="ml-1 text-xs font-normal text-gray-400">(k / m / l)</span>
        </label>
      )}
      <input
        id={id}
        type="text"
        inputMode="decimal"
        value={displayValue}
        onChange={handleChange}
        onKeyDown={handleKeyDown}
        onBlur={handleBlur}
        onFocus={handleFocus}
        placeholder={placeholder}
        required={required}
        className={cn(
          'block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm placeholder-gray-400',
          'focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500',
          error && 'border-red-300 focus:border-red-500 focus:ring-red-500',
          className
        )}
      />
      {error && <p className="text-xs text-red-600">{error}</p>}
    </div>
  );
}
