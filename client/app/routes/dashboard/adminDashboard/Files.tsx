import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import Counter from "~/components/Counter/Counter";
import FileCount from "~/components/AdminDashboard/Files/FileCount";
import FolderCount from "~/components/AdminDashboard/Files/FolderCount";
import FileTypeCount from "~/components/AdminDashboard/Files/FileTypeCount";

export default function Files() {
    
    return <div>
        <div className={styles.countersContainer}>
            <div><FileCount /></div>
            <div><FolderCount /></div>
            <div><FileTypeCount /></div>
        </div>
    </div>;

}