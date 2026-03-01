import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { AmountInput, parseAmountString } from './AmountInput';

// ── parseAmountString unit tests ──────────────────────────────────────────────

describe('parseAmountString', () => {
  it('converts k suffix to thousands', () => {
    expect(parseAmountString('5k')).toBe(5_000);
    expect(parseAmountString('1.5k')).toBe(1_500);
    expect(parseAmountString('5K')).toBe(5_000);
  });

  it('converts m suffix to millions', () => {
    expect(parseAmountString('2m')).toBe(2_000_000);
    expect(parseAmountString('1.5m')).toBe(1_500_000);
    expect(parseAmountString('2M')).toBe(2_000_000);
  });

  it('converts l suffix to lakhs (100k)', () => {
    expect(parseAmountString('3l')).toBe(300_000);
    expect(parseAmountString('2.5l')).toBe(250_000);
    expect(parseAmountString('3L')).toBe(300_000);
  });

  it('handles plain numbers without suffix', () => {
    expect(parseAmountString('1000')).toBe(1000);
    expect(parseAmountString('99.5')).toBe(99.5);
  });

  it('returns null for invalid input', () => {
    expect(parseAmountString('')).toBeNull();
    expect(parseAmountString('abc')).toBeNull();
    expect(parseAmountString('5x')).toBeNull();
  });

  it('handles whitespace around the value', () => {
    expect(parseAmountString('  5k  ')).toBe(5_000);
    expect(parseAmountString('2 m')).toBe(2_000_000);
  });
});

// ── AmountInput component tests ───────────────────────────────────────────────

describe('AmountInput', () => {
  describe('label', () => {
    it('shows the hint "(k / m / l)" next to the label', () => {
      render(<AmountInput label="Basis Value" />);
      expect(screen.getByText('Basis Value')).toBeInTheDocument();
      expect(screen.getByText('(k / m / l)')).toBeInTheDocument();
    });

    it('renders without a label', () => {
      render(<AmountInput />);
      expect(screen.queryByText('(k / m / l)')).not.toBeInTheDocument();
    });
  });

  describe('shortcut conversion on Tab', () => {
    it('converts k suffix to thousands', async () => {
      const onChange = vi.fn();
      render(<AmountInput label="Amount" onChange={onChange} />);
      const input = screen.getByRole('textbox');

      await userEvent.type(input, '5k');
      fireEvent.keyDown(input, { key: 'Tab' });

      expect(onChange).toHaveBeenCalledWith(5_000);
      expect(input).toHaveValue('5000');
    });

    it('converts m suffix to millions', async () => {
      const onChange = vi.fn();
      render(<AmountInput label="Amount" onChange={onChange} />);
      const input = screen.getByRole('textbox');

      await userEvent.type(input, '2m');
      fireEvent.keyDown(input, { key: 'Tab' });

      expect(onChange).toHaveBeenCalledWith(2_000_000);
      expect(input).toHaveValue('2000000');
    });

    it('converts l suffix to lakhs', async () => {
      const onChange = vi.fn();
      render(<AmountInput label="Amount" onChange={onChange} />);
      const input = screen.getByRole('textbox');

      await userEvent.type(input, '3l');
      fireEvent.keyDown(input, { key: 'Tab' });

      expect(onChange).toHaveBeenCalledWith(300_000);
      expect(input).toHaveValue('300000');
    });

    it('is case-insensitive (K, M, L)', async () => {
      const onChange = vi.fn();
      render(<AmountInput label="Amount" onChange={onChange} />);
      const input = screen.getByRole('textbox');

      await userEvent.type(input, '1M');
      fireEvent.keyDown(input, { key: 'Tab' });

      expect(onChange).toHaveBeenCalledWith(1_000_000);
    });

    it('handles decimal amounts like 1.5m', async () => {
      const onChange = vi.fn();
      render(<AmountInput label="Amount" onChange={onChange} />);
      const input = screen.getByRole('textbox');

      await userEvent.type(input, '1.5m');
      fireEvent.keyDown(input, { key: 'Tab' });

      expect(onChange).toHaveBeenCalledWith(1_500_000);
      expect(input).toHaveValue('1500000');
    });

    it('passes plain numbers through unchanged', async () => {
      const onChange = vi.fn();
      render(<AmountInput label="Amount" onChange={onChange} />);
      const input = screen.getByRole('textbox');

      await userEvent.type(input, '50000');
      fireEvent.keyDown(input, { key: 'Tab' });

      expect(onChange).toHaveBeenCalledWith(50_000);
      expect(input).toHaveValue('50000');
    });

    it('does not convert on other key presses', async () => {
      const onChange = vi.fn();
      render(<AmountInput label="Amount" onChange={onChange} />);
      const input = screen.getByRole('textbox');

      await userEvent.type(input, '5k');
      fireEvent.keyDown(input, { key: 'Enter' });

      expect(onChange).not.toHaveBeenCalled();
      expect(input).toHaveValue('5k');
    });
  });

  describe('shortcut conversion on blur', () => {
    it('converts suffix when field loses focus', async () => {
      const onChange = vi.fn();
      render(<AmountInput label="Amount" onChange={onChange} />);
      const input = screen.getByRole('textbox');

      await userEvent.type(input, '4k');
      fireEvent.blur(input);

      expect(onChange).toHaveBeenCalledWith(4_000);
      expect(input).toHaveValue('4000');
    });
  });

  describe('external value sync', () => {
    it('displays the initial numeric value', () => {
      render(<AmountInput label="Amount" value={5000} />);
      expect(screen.getByRole('textbox')).toHaveValue('5000');
    });

    it('shows empty string for undefined value', () => {
      render(<AmountInput label="Amount" value={undefined} />);
      expect(screen.getByRole('textbox')).toHaveValue('');
    });
  });

  describe('error state', () => {
    it('shows error message', () => {
      render(<AmountInput label="Amount" error="Required" />);
      expect(screen.getByText('Required')).toBeInTheDocument();
    });
  });
});
