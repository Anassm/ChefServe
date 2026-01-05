import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import FileCount from "~/components/AdminDashboard/Files/FileCount";
import FolderCount from "~/components/AdminDashboard/Files/FolderCount";
import FileTypeCount from "~/components/AdminDashboard/Files/FileTypeCount";
import FoldersWithContentCount from "~/components/AdminDashboard/Files/FoldersWithContentCount";
import EmptyFolderCount from "~/components/AdminDashboard/Files/EmptyFolderCount";
import FileTypeStats from "~/components/AdminDashboard/Files/FileTypeStats";

export default function Files() {
    
    return <div>
        <div className={styles.countersContainer}>
            <div><FileCount /></div>
            <div><FolderCount /></div>
            <div><FileTypeCount /></div>
            <div><FoldersWithContentCount /></div>
            <div><EmptyFolderCount /></div>
        </div>
        <FileTypeStats />
    </div>;

}