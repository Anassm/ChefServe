import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import Counter from "~/components/Counter/Counter";

export default function Files() {
    
    return <div>
        Admin file management
        <div className={styles.countersContainer}>
            <Counter title="Total Files" count={1500} />
            <Counter title="Total Folders" count={300} />
            <Counter title="Unique File Types" count={25} />
        </div>
    </div>;

}