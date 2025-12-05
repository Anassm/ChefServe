import { Navigate } from "react-router";
import { useLoaderData } from "react-router";
import type { LoaderFunctionArgs } from "react-router";
import { useUser } from "~/helper/UserContext";
import AdminDashboard from "./adminDashboard/AdminDashboard";
import UserDashboard from "./userDashboard/UserDashboard";
import type { fileItem } from "~/components/FileItem/FileItem";
import type { Route } from "./+types/Dashboard";
import type { TreeItem } from "~/components/FileTree/FileTree";
import { useContext } from "react";

export async function clientLoader({ params }: LoaderFunctionArgs) {
  const fullPath = params["*"] ?? "";
  const url = fullPath
    ? `http://localhost:5175/api/file/getfiles?parentPath=${fullPath}`
    : `http://localhost:5175/api/file/getfiles`;
  const url2 = `http://localhost:5175/api/file/GetFileTree`


  const response = await fetch(url, {
    method: 'GET',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include'
  });
  console.log(response)

  const response2 = await fetch(url2, {
    method: 'GET',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include'
  });

  try {
    const textFiles = await response.text()
    console.log(textFiles)
    const textTree = await response2.text();
    const jsonFiles = textFiles ? JSON.parse(textFiles) : null;
    console.log(jsonFiles)
    const files: fileItem[] = jsonFiles?.returnData ?? [];
    console.log(files)
    const rootFolder: TreeItem | null = textTree ? JSON.parse(textTree) : null;

    console.log("Fetched files:", files);
    console.log("Fetched folder tree:", rootFolder);

    return [files, rootFolder];
  } catch (err) {
    console.error("Failed to parse JSON");
    return [[], null];
  }
}


export function HydrateFallback() {
  return <div>Serving your files...</div>;
}


export default function Dashboard() {
  const { user } = useUser();
  const [files, rootFolder] = useLoaderData<[fileItem[], TreeItem]>();



  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (user.role === "admin") {
    return <AdminDashboard />;
  }

  if (user.role === "user") {
    return <UserDashboard files={files} />;
  }

  return (
    <>
      <h2>Something went wrong.</h2>
      <p>No user role found for dashboard.</p>
    </>
  );
}
