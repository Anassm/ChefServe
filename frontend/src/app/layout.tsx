import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "ChefServe - Cloud Storage",
  description: "Serving files like dishes! Cloud storage application.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="antialiased">
        {children}
      </body>
    </html>
  );
}
