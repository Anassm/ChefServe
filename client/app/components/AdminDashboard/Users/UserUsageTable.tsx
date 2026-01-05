import styles from "./UserUsageTable.module.css";

export default function UserUsageTable({ usageData }: { usageData: any[] }) {
    return (
    <table className={styles.usageTable}>
        <thead>
            <tr>
                <th>User ID</th>
                <th>Username</th>
                <th>Total Files</th>
                <th>Total Storage Used (MB)</th>
            </tr>
        </thead>
        <tbody>
            {
                usageData.map((data: any) => (
                    <tr className={styles.tableRow} key={data.userId}>
                        <td>{data.userId}</td> 
                        <td>{data.username}</td>
                        <td>{data.totalFiles}</td>
                        <td>{data.totalStorageUsed}</td>
                    </tr>
                ))
            }
        </tbody>
    </table>
    );
}  
