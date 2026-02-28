import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../store/AuthContext';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';

export function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(email, password);
      navigate('/dashboard');
    } catch {
      setError('Invalid email or password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-primary-900 to-primary-700 p-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-2xl">
        <div className="mb-8 text-center">
          <h1 className="text-2xl font-bold text-gray-900">ProductBuilder</h1>
          <p className="mt-1 text-sm text-gray-500">Insurance Underwriting Platform</p>
        </div>
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input id="email" label="Email" type="email" value={email}
            onChange={e => setEmail(e.target.value)} placeholder="admin@productbuilder.com" required />
          <Input id="password" label="Password" type="password" value={password}
            onChange={e => setPassword(e.target.value)} placeholder="••••••••" required />
          {error && <p className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{error}</p>}
          <Button type="submit" className="w-full" loading={loading}>Sign In</Button>
        </form>
        <p className="mt-4 text-center text-xs text-gray-400">
          Default: admin@productbuilder.com / Admin@123
        </p>
      </div>
    </div>
  );
}
