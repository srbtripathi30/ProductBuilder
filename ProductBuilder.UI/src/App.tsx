import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './store/AuthContext';
import { ProtectedRoute } from './components/layout/ProtectedRoute';
import { MainLayout } from './components/layout/MainLayout';
import { LoginPage } from './features/auth/LoginPage';
import { DashboardPage } from './features/dashboard/DashboardPage';
import { LobPage } from './features/lob/LobPage';
import { ProductListPage } from './features/products/ProductListPage';
import { ProductDetailPage } from './features/products/ProductDetailPage';
import { QuoteListPage } from './features/quotes/QuoteListPage';
import { QuoteWizard } from './features/quotes/QuoteWizard';
import { QuoteDetailPage } from './features/quotes/QuoteDetailPage';
import { EditQuotePage } from './features/quotes/EditQuotePage';
import { InsurerPage } from './features/insurers/InsurerPage';
import { UnderwritersPage } from './features/underwriters/UnderwritersPage';
import { BrokersPage } from './features/brokers/BrokersPage';

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 30_000 } }
});

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route element={<ProtectedRoute />}>
              <Route element={<MainLayout />}>
                <Route path="/" element={<Navigate to="/dashboard" replace />} />
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/lob" element={<LobPage />} />
                <Route path="/products" element={<ProductListPage />} />
                <Route path="/products/:productId" element={<ProductDetailPage />} />
                <Route path="/quotes" element={<QuoteListPage />} />
                <Route path="/quotes/new" element={<QuoteWizard />} />
                <Route path="/quotes/:quoteId" element={<QuoteDetailPage />} />
                <Route path="/quotes/:quoteId/edit" element={<EditQuotePage />} />
                <Route path="/insurers" element={<InsurerPage />} />
                <Route path="/underwriters" element={<UnderwritersPage />} />
                <Route path="/brokers" element={<BrokersPage />} />
                <Route path="/settings/users" element={<div className="p-6 text-gray-500">User Management — coming soon</div>} />
              </Route>
            </Route>
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
}
