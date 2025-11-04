import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import { TbUserEdit } from "react-icons/tb";
import { TbUserMinus } from "react-icons/tb";

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
            <table className={styles.userTable}>
                <thead>
                    <tr>
                        <th>User ID</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Email</th>
                        <th>Role</th>
                        <th>Created At</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        loaderData.map((user: any) => (
                            <tr key={user.id}>
                                <td>{user.id}</td>
                                <td>{user.firstName}</td>
                                <td>{user.lastName}</td>
                                <td>{user.email}</td>
                                <td>{user.role}</td>
                                <td>{user.createdAt}</td>
                                <td className={styles.actionButtons}>

                                    <button className={styles.editButton} title="Edit User" onClick={() => alert("Edit user")}>
                                        <TbUserEdit size={25}/>
                                    </button>
                                    <button className={styles.deleteButton} title="Delete User" onClick={() => alert("Delete user")}>
                                        <TbUserMinus size={25}/>
                                    </button>
                                </td>
                            </tr>
                        ))
                    }
                    
                </tbody>
            </table>
        </div>
    )
}