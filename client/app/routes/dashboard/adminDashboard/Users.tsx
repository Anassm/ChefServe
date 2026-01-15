import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import UserTable from "../../../components/AdminDashboard/Users/UserTable";
import { TbUserPlus, TbRefresh } from "react-icons/tb";
import { useState } from "react";
import UserFormModal from "~/components/AdminDashboard/Users/UserFormModal";
import UserUsageTable from "~/components/AdminDashboard/Users/UserUsageTable";
import { NavLink } from "react-router";

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

    const handleAddUser = async (data: any) => {
        const response = await fetch("http://localhost:5175/api/admin/users", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            credentials: "include",
            body: JSON.stringify(data),
        });
        if (!response.ok) {
            console.error("Failed to add user:", response.statusText);
            return;
        }
        return await response.json();
    }


    if (!loaderData || loaderData.length === 0) {
        return <div>No users found.</div>;
    }
    return(
        <div> 
            <div style={{ height: "20px" }}></div>
            <div className={styles.navContainer}>
                <NavLink to="/admin" className={styles.navButton}>
                Go back
                </NavLink>
            </div>
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
            <UserTable users={users} onUserDeleted={refreshUsers} />
            
            {isModalOpen && (
                <UserFormModal
                    onClose={() => setIsModalOpen(false)}
                    onSubmit={handleAddUser}
                />
            )}

            <div style={{ height: "50px" }}></div>

            <div className={styles.userListHeader}>
                <h2 className={styles.userListTitle}>User usage statistics</h2>
                <button className={styles.refreshButton} onClick={refreshUsers}>
                    <TbRefresh size={30} style={{ marginTop: "20px" }} />
                </button>
            </div>
            <UserUsageTable usageData={users.map(user => ({
                userId: user.id,
                username: user.username,
                totalFiles: user.totalFiles,
                totalStorageUsed: user.totalStorageUsed
            }))} />
        </div>
    )
}