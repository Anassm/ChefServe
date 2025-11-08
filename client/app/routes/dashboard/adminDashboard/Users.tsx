import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import UserTable from "../../../components/Users/UserTable";
import { TbUserPlus, TbRefresh } from "react-icons/tb";
import { useState } from "react";
import UserFormModal from "~/components/Users/UserFormModal";

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

    return await response.json();
}

export function HydrateFallback() {
    return <div>Loading user data...</div>;
}

export default function Users({ loaderData }: { loaderData?: any[] }) {
    const [users, setUsers] = useState(loaderData || []);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const refreshUsers = async () => {
        const response = await fetch("http://localhost:5175/api/admin/users", {
            credentials: "include",
        });
        const updated = await response.json();
        setUsers(updated);
    }

    if (!loaderData || loaderData.length === 0) {
        return <div>No users found.</div>;
    }
    return(
        <div> 
            <div className={styles.userListHeader}>
                <h2 className={styles.userListTitle}>User List</h2>
                <div className={styles.buttonGroup}>
                    <button className={styles.addUserButton} onClick={() => setIsModalOpen(true)}>
                        <TbUserPlus size={30} style={{ marginTop: "20px" }}/>
                    </button>
                    <button className={styles.refreshButton} onClick={refreshUsers}>
                        <TbRefresh size={30} style={{ marginTop: "20px" }}/>
                    </button>
                </div>
            </div>
            <UserTable users={loaderData} />
            {isModalOpen && (
                <UserFormModal
                onClose={() => setIsModalOpen(false)}
                onSubmit={alert}
                />
            )}
        </div>
    )
}