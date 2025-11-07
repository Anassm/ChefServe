import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import UserTable from "../../../components/Users/UserTable";

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


export default function Users({ loaderData }: { loaderData?: any[] }) {
    if (!loaderData || loaderData.length === 0) {
        return <div>No users found.</div>;
    }
    return(
        <div> 
            <h2 className={styles.userListTitle}>User List</h2>
            <UserTable users={loaderData} />
        </div>
    )
}