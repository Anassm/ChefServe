import { useEffect, useState } from "react";
import styles from "~/routes/dashboard/adminDashboard/AdminDashboard.module.css";

type FileTypeStat = {
    extension: string;
    count: number;
};

export default function FileTypeStats() {
    const [stats, setStats] = useState<FileTypeStat[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch("http://localhost:5175/api/admin/filetypes/stats", {
                    method: "GET",
                    credentials: "include",
                });
                if (!response.ok) {
                    setIsLoading(false);
                    return;
                }
                const data = await response.json();
                if (Array.isArray(data)) {
                    setStats(data);
                }
            } catch (error) {
                console.error("Failed to fetch file type statistics", error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchData();
        const interval = setInterval(fetchData, 10000);
        return () => clearInterval(interval);
    }, []);

    if (isLoading && stats.length === 0) {
        return <div className={styles.statsCard}>Loading file type statistics...</div>;
    }

    if (!isLoading && stats.length === 0) {
        return <div className={styles.statsCard}>No file types found.</div>;
    }

    return (
        <div className={styles.statsCard}>
            <h3 className={styles.statsTitle}>File type breakdown</h3>
            <ul className={styles.statsList}>
                {stats.map((item) => (
                    <li key={item.extension} className={styles.statsItem}>
                        <span>{item.extension || "No Extension"}</span>
                        <span>{item.count}</span>
                    </li>
                ))}
            </ul>
        </div>
    );
}
