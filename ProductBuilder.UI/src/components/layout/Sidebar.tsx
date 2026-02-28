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
  { to: '/products', icon: <Package className="h-5 w-5" />, label: 'Products' },
  { to: '/quotes', icon: <FileText className="h-5 w-5" />, label: 'Quotes' },
  { to: '/insurers', icon: <Building2 className="h-5 w-5" />, label: 'Insurers' },
  { to: '/underwriters', icon: <UserCheck className="h-5 w-5" />, label: 'Underwriters' },
  { to: '/brokers', icon: <Briefcase className="h-5 w-5" />, label: 'Brokers' },
  { to: '/settings/users', icon: <Settings className="h-5 w-5" />, label: 'Users', roles: ['Admin'] },
];

export function Sidebar() {
  const { user } = useAuth();
  const visible = navItems.filter(item => !item.roles || (user && item.roles.includes(user.role)));

  return (
    <aside className="flex h-screen w-64 flex-col border-r border-gray-200 bg-white">
      <div className="flex h-16 items-center px-6 border-b border-gray-200">
        <span className="text-xl font-bold text-primary-700">ProductBuilder</span>
      </div>
      <nav className="flex-1 overflow-y-auto px-3 py-4 space-y-1">
        {visible.map(item => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              cn('flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                isActive ? 'bg-primary-50 text-primary-700' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900')
            }
          >
            {item.icon}
            {item.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
