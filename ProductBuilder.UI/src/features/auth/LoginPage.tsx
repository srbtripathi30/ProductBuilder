import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../store/AuthContext';
import { authApi } from '../../api/auth.api';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';

type View = 'login' | 'forgot' | 'reset';

export function LoginPage() {
  const [view, setView] = useState<View>('login');

  // Login state
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loginError, setLoginError] = useState('');
  const [loginLoading, setLoginLoading] = useState(false);

  // Forgot password state
  const [forgotEmail, setForgotEmail] = useState('');
  const [forgotLoading, setForgotLoading] = useState(false);
  const [forgotError, setForgotError] = useState('');
  const [resetToken, setResetToken] = useState('');
  const [tokenCopied, setTokenCopied] = useState(false);

  // Reset password state
  const [tokenInput, setTokenInput] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [resetLoading, setResetLoading] = useState(false);
  const [resetError, setResetError] = useState('');
  const [resetSuccess, setResetSuccess] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoginError('');
    setLoginLoading(true);
    try {
      await login(email, password);
      navigate('/dashboard');
    } catch {
      setLoginError('Invalid email or password');
    } finally {
      setLoginLoading(false);
    }
  };

  const handleForgot = async (e: React.FormEvent) => {
    e.preventDefault();
    setForgotError('');
    setForgotLoading(true);
    try {
      const res = await authApi.forgotPassword(forgotEmail);
      if (res.resetToken) {
        setResetToken(res.resetToken);
        setTokenInput(res.resetToken);
      }
      setView('reset');
    } catch {
      setForgotError('Unable to process request. Please try again.');
    } finally {
      setForgotLoading(false);
    }
  };

  const handleReset = async (e: React.FormEvent) => {
    e.preventDefault();
    setResetError('');
    if (newPassword !== confirmPassword) {
      setResetError('Passwords do not match.');
      return;
    }
    if (newPassword.length < 6) {
      setResetError('Password must be at least 6 characters.');
      return;
    }
    setResetLoading(true);
    try {
      await authApi.resetPassword(tokenInput, newPassword);
      setResetSuccess(true);
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setResetError(msg ?? 'Reset failed. The token may be invalid or expired.');
    } finally {
      setResetLoading(false);
    }
  };

  const handleCopyToken = async () => {
    await navigator.clipboard.writeText(resetToken);
    setTokenCopied(true);
    setTimeout(() => setTokenCopied(false), 2000);
  };

  const goBackToLogin = () => {
    setView('login');
    setForgotEmail('');
    setForgotError('');
    setResetToken('');
    setTokenInput('');
    setNewPassword('');
    setConfirmPassword('');
    setResetError('');
    setResetSuccess(false);
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-primary-900 to-primary-700 p-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-2xl">

        {/* Header */}
        <div className="mb-8 text-center">
          <h1 className="text-2xl font-bold text-gray-900">ProductBuilder</h1>
          <p className="mt-1 text-sm text-gray-500">Insurance Underwriting Platform</p>
        </div>

        {/* ── LOGIN VIEW ── */}
        {view === 'login' && (
          <form onSubmit={handleLogin} className="space-y-4">
            <Input id="email" label="Email" type="email" value={email}
              onChange={e => setEmail(e.target.value)} placeholder="you@example.com" required />
            <Input id="password" label="Password" type="password" value={password}
              onChange={e => setPassword(e.target.value)} placeholder="••••••••" required />
            {loginError && (
              <p className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{loginError}</p>
            )}
            <Button type="submit" className="w-full" loading={loginLoading}>Sign In</Button>
            <div className="text-center">
              <button type="button"
                onClick={() => setView('forgot')}
                className="text-sm text-primary-600 hover:text-primary-800 hover:underline focus:outline-none">
                Forgot password?
              </button>
            </div>
          </form>
        )}

        {/* ── FORGOT PASSWORD VIEW ── */}
        {view === 'forgot' && (
          <div className="space-y-4">
            <div>
              <h2 className="text-lg font-semibold text-gray-900">Reset your password</h2>
              <p className="mt-1 text-sm text-gray-500">
                Enter your email address and we'll generate a reset token for you.
              </p>
            </div>
            <form onSubmit={handleForgot} className="space-y-4">
              <Input id="forgot-email" label="Email" type="email" value={forgotEmail}
                onChange={e => setForgotEmail(e.target.value)} placeholder="you@example.com" required />
              {forgotError && (
                <p className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{forgotError}</p>
              )}
              <Button type="submit" className="w-full" loading={forgotLoading}>
                Send Reset Token
              </Button>
            </form>
            <div className="text-center">
              <button type="button" onClick={goBackToLogin}
                className="text-sm text-gray-500 hover:text-gray-700 hover:underline focus:outline-none">
                Back to sign in
              </button>
            </div>
          </div>
        )}

        {/* ── RESET PASSWORD VIEW ── */}
        {view === 'reset' && (
          <div className="space-y-4">
            {resetSuccess ? (
              <div className="space-y-4 text-center">
                <div className="rounded-lg bg-green-50 p-4">
                  <p className="text-sm font-medium text-green-800">
                    Password reset successfully!
                  </p>
                  <p className="mt-1 text-sm text-green-700">
                    You can now sign in with your new password.
                  </p>
                </div>
                <Button className="w-full" onClick={goBackToLogin}>Back to Sign In</Button>
              </div>
            ) : (
              <>
                <div>
                  <h2 className="text-lg font-semibold text-gray-900">Set new password</h2>
                  <p className="mt-1 text-sm text-gray-500">
                    Your reset token is shown below. Enter it along with your new password.
                  </p>
                </div>

                {resetToken && (
                  <div className="rounded-lg border border-amber-200 bg-amber-50 p-3">
                    <p className="mb-1 text-xs font-medium text-amber-800">Your reset token</p>
                    <div className="flex items-center gap-2">
                      <code className="flex-1 break-all rounded bg-white px-2 py-1 text-xs text-gray-800 ring-1 ring-amber-200">
                        {resetToken}
                      </code>
                      <button type="button" onClick={handleCopyToken}
                        className="shrink-0 rounded px-2 py-1 text-xs font-medium text-amber-700 hover:bg-amber-100 focus:outline-none">
                        {tokenCopied ? 'Copied!' : 'Copy'}
                      </button>
                    </div>
                    <p className="mt-1 text-xs text-amber-600">
                      In production, this token would be sent to your email.
                    </p>
                  </div>
                )}

                <form onSubmit={handleReset} className="space-y-4">
                  <Input id="reset-token" label="Reset Token" type="text" value={tokenInput}
                    onChange={e => setTokenInput(e.target.value)} placeholder="Paste token here" required />
                  <Input id="new-password" label="New Password" type="password" value={newPassword}
                    onChange={e => setNewPassword(e.target.value)} placeholder="Min. 6 characters" required />
                  <Input id="confirm-password" label="Confirm Password" type="password" value={confirmPassword}
                    onChange={e => setConfirmPassword(e.target.value)} placeholder="Repeat new password" required />
                  {resetError && (
                    <p className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{resetError}</p>
                  )}
                  <Button type="submit" className="w-full" loading={resetLoading}>
                    Reset Password
                  </Button>
                </form>
                <div className="text-center">
                  <button type="button" onClick={goBackToLogin}
                    className="text-sm text-gray-500 hover:text-gray-700 hover:underline focus:outline-none">
                    Back to sign in
                  </button>
                </div>
              </>
            )}
          </div>
        )}

      </div>
    </div>
  );
}
