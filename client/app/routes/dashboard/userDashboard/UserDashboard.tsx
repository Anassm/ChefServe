import type { Route } from "../../../+types/root";
import styles from "./UserDashboard.module.css";
import type { fileItem } from "~/components/FileItem/FileItem";
import { FileItem } from "~/components/FileItem/FileItem";
import FileDisplayer from "~/components/FileItem/FileDisplayer";
import { useLoaderData } from "react-router";

export function meta({ }: Route.MetaArgs) {
  return [
    { title: "User | dashboard" },
    { name: "description", content: "User dashboard route" },
  ];
}

export async function loader({ params }: { params?: { parentpath: string } }) {
  const url: string = params == undefined ? `http://localhost:5175/api/file/getfiles` : `http://localhost:5175/api/file/getfiles?parentPath=${params?.parentpath}`;
  const data = await fetch(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include'
  });
  const files: fileItem[] = await data.json();
  return files;
}

export default function UserDashboard() {
  const files = useLoaderData<fileItem[]>();
  console.log(files);
  return null;
}
