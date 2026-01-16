import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import FileCount from "~/components/AdminDashboard/Files/FileCount";
import FolderCount from "~/components/AdminDashboard/Files/FolderCount";
import FileTypeCount from "~/components/AdminDashboard/Files/FileTypeCount";
import FoldersWithContentCount from "~/components/AdminDashboard/Files/FoldersWithContentCount";
import EmptyFolderCount from "~/components/AdminDashboard/Files/EmptyFolderCount";
import FileTypeStats from "~/components/AdminDashboard/Files/FileTypeStats";
import TotalStorageCount from "~/components/AdminDashboard/Files/TotalStorageCount";
import { NavLink, useLoaderData } from "react-router";
import FileTable from "~/components/AdminDashboard/Files/FileTable";
import { useState } from "react";

export async function clientLoader({ request }: Route.LoaderArgs) {
    const response = await fetch("http://localhost:5175/api/admin/files", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });
    if (!response.ok) {
        console.error("Failed to fetch files:", response.statusText);
        return [];
    }

    return await response.json();
}

export function HydrateFallback() {
    return <div>Loading file data...</div>;
}


export default function Files() {
    const loaderData = useLoaderData() as any;
    const filesFromLoader: any[] = loaderData?.data ?? loaderData?.returnData ?? loaderData ?? [];
    const [files, setFiles] = useState(filesFromLoader || []);

    const refreshFiles = async () => {
        const response = await fetch("http://localhost:5175/api/admin/files", {
            credentials: "include",
        });
        const updated = await response.json();
        const filesdata = updated?.data ?? updated?.returnData ?? updated ?? [];
        setFiles(filesdata);
    }

    return <div style={{overflowY: "auto"}}>
        <div style={{ height: "20px" }}></div>
        <div className={styles.navContainer}>
        <NavLink to="/admin" className={styles.navButton}>
          Go back
        </NavLink>
      </div>
        <div style={{ height: "20px" }}></div>
        <h1 className={styles.title}>File statistics</h1>
        <div style={{ height: "20px" }}></div>
        <div className={styles.countersContainer} >
            <div><FileCount /></div>
            <div><FolderCount /></div>
            <div><FileTypeCount /></div>
            <div><TotalStorageCount /></div>
        </div>
        <div style={{ height: "20px" }}></div>
        <h1 className={styles.title}>File types</h1>
        <div style={{ height: "20px" }}></div>
        <FileTypeStats />
        <div style={{ height: "20px" }}></div>
        <h1 className={styles.title}>All files</h1>
        <div style={{ height: "20px" }}></div>
        <FileTable files={files} onFileDeleted={refreshFiles} />
        <div style={{ height: "50px" }}></div>
    </div>;

}