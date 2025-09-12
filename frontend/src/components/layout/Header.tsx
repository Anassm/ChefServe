'use client';

import { useAuth } from '@/contexts/AuthContext';
import { LogOut, User } from 'lucide-react';

export default function Header() {
  const { user, logout } = useAuth();

  return (
    <header className="bg-white border-b border-gray-200 px-6 py-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <h1 className="text-2xl font-bold text-blue-600">ChefServe</h1>
          <span className="text-gray-500">Serving files like dishes!</span>
        </div>

        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2 text-gray-700">
            <User className="w-5 h-5" />
            <span>{user?.firstName} {user?.lastName}</span>
            <span className="text-gray-500">({user?.email})</span>
          </div>

          <button
            onClick={logout}
            className="inline-flex items-center gap-2 px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
          >
            <LogOut className="w-4 h-4" />
            Logout
          </button>
        </div>
      </div>
    </header>
  );
}