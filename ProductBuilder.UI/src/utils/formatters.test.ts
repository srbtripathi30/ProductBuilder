import { describe, it, expect } from 'vitest';
import { formatCurrency, formatDate, statusColor } from './formatters';

describe('formatCurrency', () => {
  it('formats a positive USD amount', () => {
    expect(formatCurrency(1000)).toBe('$1,000.00');
  });

  it('formats zero', () => {
    expect(formatCurrency(0)).toBe('$0.00');
  });

  it('returns "-" for null', () => {
    expect(formatCurrency(null)).toBe('-');
  });

  it('returns "-" for undefined', () => {
    expect(formatCurrency(undefined)).toBe('-');
  });

  it('includes currency symbol for EUR', () => {
    const result = formatCurrency(500, 'EUR');
    expect(result).toContain('500');
    expect(result).toContain('€');
  });

  it('formats large amounts with commas', () => {
    expect(formatCurrency(1_000_000)).toBe('$1,000,000.00');
  });

  it('formats negative amounts', () => {
    const result = formatCurrency(-250);
    expect(result).toContain('250');
    expect(result).toContain('-');
  });
});

describe('formatDate', () => {
  it('formats a valid ISO date string', () => {
    const result = formatDate('2024-06-15T00:00:00Z');
    expect(result).toContain('2024');
  });

  it('returns "-" for null', () => {
    expect(formatDate(null)).toBe('-');
  });

  it('returns "-" for undefined', () => {
    expect(formatDate(undefined)).toBe('-');
  });

  it('returns "-" for empty string', () => {
    expect(formatDate('')).toBe('-');
  });

  it('includes the month abbreviation', () => {
    const result = formatDate('2024-01-20T00:00:00Z');
    expect(result).toMatch(/Jan/);
  });
});

describe('statusColor', () => {
  it('returns green classes for Active', () => {
    expect(statusColor('Active')).toContain('green');
  });

  it('returns green classes for Approved', () => {
    expect(statusColor('Approved')).toContain('green');
  });

  it('returns blue classes for Submitted', () => {
    expect(statusColor('Submitted')).toContain('blue');
  });

  it('returns gray classes for Draft', () => {
    expect(statusColor('Draft')).toContain('gray');
  });

  it('returns gray classes for Expired', () => {
    expect(statusColor('Expired')).toContain('gray');
  });

  it('returns yellow classes for Inactive', () => {
    expect(statusColor('Inactive')).toContain('yellow');
  });

  it('returns red classes for Archived', () => {
    expect(statusColor('Archived')).toContain('red');
  });

  it('returns red classes for Declined', () => {
    expect(statusColor('Declined')).toContain('red');
  });

  it('returns default gray for unknown status', () => {
    expect(statusColor('Unknown')).toBe('bg-gray-100 text-gray-700');
  });

  it('returns default gray for empty string', () => {
    expect(statusColor('')).toBe('bg-gray-100 text-gray-700');
  });
});
