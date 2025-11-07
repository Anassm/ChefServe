import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import UserTable from "../../../components/Users/UserTable";
import { TbUserPlus, TbRefresh } from "react-icons/tb";

export async function clientLoader({ request }: Route.LoaderArgs) {
    const response = await fetch("http://localhost:5175/api/admin/users", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    if (!response.ok) {
        console.error("Failed to fetch users:", response.statusText);
        return [];
    }
    const users = await response.json();
    return users;
}

export function HydrateFallback() {
    return <div>Loading user data...</div>;
}

function refreshUsers() {
    window.location.reload();
}


export default function Users({ loaderData }: { loaderData?: any[] }) {
    if (!loaderData || loaderData.length === 0) {
        return <div>No users found.</div>;
    }
    return(
        <div> 
            <div className={styles.userListHeader}>
                <h2 className={styles.userListTitle}>User List</h2>
                <div className={styles.buttonGroup}>
                    <button className={styles.addUserButton}>
                        <TbUserPlus size={30} style={{ marginTop: "20px" }}/>
                    </button>
                    <button className={styles.refreshButton} onClick={refreshUsers}>
                        <TbRefresh size={30} style={{ marginTop: "20px" }}/>
                    </button>
                </div>
            </div>
            <UserTable users={loaderData} />
        </div>
    )
}