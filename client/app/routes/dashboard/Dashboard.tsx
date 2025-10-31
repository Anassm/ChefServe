import { Navigate } from "react-router";
import { useLoaderData } from "react-router";
import type { LoaderFunctionArgs } from "react-router";
import { useUser } from "~/helper/UserContext";
import AdminDashboard from "./adminDashboard/AdminDashboard";
import UserDashboard from "./userDashboard/UserDashboard";
import type { fileItem } from "~/components/FileItem/FileItem";
import type { Route } from "./+types/Dashboard";


export async function clientLoader({ params }: LoaderFunctionArgs) {
  const url = params?.parentpath
    ? `http://localhost:5175/api/file/getfiles?parentPath=${params.parentpath}`
    : `http://localhost:5175/api/file/getfiles`;

  const response = await fetch(url, {
    method: 'GET',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include'
  });

  if (!response.ok) {
    console.error("Failed to fetch files:", response.statusText);
    return [];
  }

  const text = await response.text();
  if (!text) {
    console.warn("Empty response from API");
    return []; 
  }

  try {
    const files: fileItem[] = JSON.parse(text);
    console.log("Fetched files:", files);
    return files;
  } catch (err) {
    console.error("Failed to parse JSON:", text);
    return [];
  }
}


export function HydrateFallback() {
  return <div>Serving your files...</div>;
}


export default function Dashboard({
    loaderData }: Route.ComponentProps) {
    const { user } = useUser();


    if (!user) {
        return <Navigate to="/login" replace />;
    }

    if (user.role === "admin") {
        return <AdminDashboard />;
    }

    if (user.role === "user") {
        return <UserDashboard files={loaderData} />;
    }

    return (
        <>
            <h2>Something went wrong.</h2>
            <p>No user role found for dashboard.</p>
        </>
    );
}
