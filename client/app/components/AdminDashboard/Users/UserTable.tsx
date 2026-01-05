import { useState } from "react";
import { TbUserEdit, TbUserMinus } from "react-icons/tb";
import styles from "./UserTable.module.css";

export default function UserTable({ users, onUserDeleted }: { users: any[]; onUserDeleted?: () => void }) {
    const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

    const handleRowClick = (userId: string) => {
        setSelectedUserId(userId === selectedUserId ? null : userId);
    }

    const deleteUser = (userId: string) => {
        console.log("Delete user with ID:", userId);
        const confirmed = window.confirm("Are you sure you want to delete this user? This action cannot be undone.");
        if (!confirmed) return;

        fetch(`http://localhost:5175/api/admin/users/${userId}`, {
            method: "DELETE",
            credentials: "include",
        }).then(response => {
            if (response.ok) {
                console.log("User deleted successfully");
                onUserDeleted?.();
            } else {
                console.error("Failed to delete user");
            }
        });
    }

    const editUser = (userId: string) => {
        console.log("Edit user with ID:", userId);

        

    }

    return (
    <table className={styles.userTable}>
        <thead>
            <tr>
                <th>User ID</th>
                <th>Username</th>
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
                users.map((user: any) => (
                    <tr 
                        key={user.id}
                        onClick={() => handleRowClick(user.id)}
                        className={`${styles.tableRow} ${
                            selectedUserId === user.id ? styles.selectedRow : ""
                        }`}
                    >
                        <td>{user.id}</td>
                        <td>{user.username}</td>
                        <td>{user.firstName}</td>
                        <td>{user.lastName}</td>
                        <td>{user.email}</td>
                        <td>{user.role}</td>
                        <td>{user.createdAt}</td>
                        <td 
                            className={styles.actionButtons}
                            onClick={(e) => e.stopPropagation()}
                        >
                            <button 
                                className={styles.editButton}
                                title="Edit User" 
                                onClick={() => alert("Edit user")}
                            >
                                <TbUserEdit size={25}/>
                            </button>
                            <button 
                                className={styles.deleteButton} 
                                title="Delete User" 
                                onClick={() => deleteUser(user.id) }
                            >
                                <TbUserMinus size={25}/>
                            </button>
                        </td>
                    </tr>
                ))
            }
            
        </tbody>
    </table>
    );
}