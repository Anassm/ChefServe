'use client';

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/contexts/AuthContext';
import "./globals.css";
import { useState } from 'react';

// Remove metadata export since this is now a client component
// We'll handle metadata in a server component wrapper

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const [queryClient] = useState(() => new QueryClient());

  return (
    <html lang="en">
      <head>
        <title>ChefServe - Cloud Storage</title>
        <meta name="description" content="Serving files like dishes! Cloud storage application." />
      </head>
      <body className="antialiased">
        <QueryClientProvider client={queryClient}>
          <AuthProvider>
            {children}
          </AuthProvider>
        </QueryClientProvider>
      </body>
    </html>
  );
}
