import { describe, it, expect } from 'vitest';
import { cn } from './cn';

describe('cn', () => {
  it('returns a single class unchanged', () => {
    expect(cn('foo')).toBe('foo');
  });

  it('merges two class names', () => {
    expect(cn('foo', 'bar')).toBe('foo bar');
  });

  it('filters out falsy values', () => {
    expect(cn('base', false && 'hidden', 'visible')).toBe('base visible');
  });

  it('filters out undefined', () => {
    expect(cn('base', undefined, 'extra')).toBe('base extra');
  });

  it('filters out null', () => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    expect(cn('base', null as any, 'extra')).toBe('base extra');
  });

  it('deduplicates conflicting Tailwind classes — last wins', () => {
    // tailwind-merge resolves text color conflicts, keeping the last one
    const result = cn('text-red-500', 'text-blue-500');
    expect(result).toBe('text-blue-500');
  });

  it('deduplicates conflicting padding classes', () => {
    const result = cn('p-2', 'p-4');
    expect(result).toBe('p-4');
  });

  it('keeps non-conflicting classes', () => {
    const result = cn('flex', 'items-center', 'gap-2');
    expect(result).toBe('flex items-center gap-2');
  });

  it('handles conditional object syntax', () => {
    const isActive = true;
    const result = cn('base', { 'bg-blue-500': isActive, 'bg-gray-100': !isActive });
    expect(result).toBe('base bg-blue-500');
  });

  it('returns empty string for no input', () => {
    expect(cn()).toBe('');
  });

  it('handles array input', () => {
    const result = cn(['foo', 'bar']);
    expect(result).toBe('foo bar');
  });
});
