import React from 'react';
import { NavLink } from 'react-router-dom';
import { LayoutDashboard, Layers, Package, FileText, Building2, UserCheck, Briefcase, Settings } from 'lucide-react';
import { cn } from '../../utils/cn';
import { useAuth } from '../../store/AuthContext';

interface NavItem {
  to: string;
  icon: React.ReactNode;
  label: string;
  roles?: string[];
}

const navItems: NavItem[] = [
  { to: '/dashboard', icon: <LayoutDashboard className="h-5 w-5" />, label: 'Dashboard' },
  { to: '/lob', icon: <Layers className="h-5 w-5" />, label: 'Lines of Business', roles: ['Admin', 'Underwriter'] },
  { to: '/products', icon: <Package className="h-5 w-5" />, label: 'Products', roles: ['Admin', 'Underwriter', 'Broker', 'Insurer'] },
  { to: '/quotes', icon: <FileText className="h-5 w-5" />, label: 'Quotes', roles: ['Admin', 'Underwriter', 'Broker', 'Insurer'] },
  { to: '/insurers', icon: <Building2 className="h-5 w-5" />, label: 'Insurers', roles: ['Admin', 'Underwriter'] },
  { to: '/underwriters', icon: <UserCheck className="h-5 w-5" />, label: 'Underwriters', roles: ['Admin', 'Underwriter'] },
  { to: '/brokers', icon: <Briefcase className="h-5 w-5" />, label: 'Brokers', roles: ['Admin', 'Underwriter', 'Broker'] },
  { to: '/settings/users', icon: <Settings className="h-5 w-5" />, label: 'Users', roles: ['Admin'] },
];

export function Sidebar() {
  const { user } = useAuth();
  const visible = navItems.filter(item => !item.roles || (user && item.roles.includes(user.role)));

  return (
    <aside className="flex h-screen w-64 flex-col bg-slate-900">
      <div className="flex h-16 items-center gap-3 px-5">
        <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-primary-500 shadow-sm">
          <span className="text-sm font-bold tracking-tight text-white">PB</span>
        </div>
        <span className="text-sm font-semibold text-white">ProductBuilder</span>
      </div>

      <nav className="flex-1 overflow-y-auto px-3 py-3 space-y-0.5">
        {visible.map(item => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              cn(
                'flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-all duration-150',
                isActive
                  ? 'bg-primary-600 text-white shadow-sm'
                  : 'text-slate-400 hover:bg-slate-800 hover:text-slate-100'
              )
            }
          >
            {item.icon}
            {item.label}
          </NavLink>
        ))}
      </nav>

      <div className="border-t border-slate-800 px-4 py-3">
        <p className="text-xs text-slate-600">Insurance Underwriting Platform</p>
      </div>
    </aside>
  );
}
