import { useState } from "react";
import { TbUserEdit, TbUserMinus } from "react-icons/tb";
import styles from "./UserTable.module.css";
import EditUserModal from "./EditUserModal";

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

    const [isEditOpen, setIsEditOpen] = useState(false);
    const [editingUser, setEditingUser] = useState<any | null>(null);

    const openEditModal = (user: any) => {
        setEditingUser(user);
        setIsEditOpen(true);
    };

    const handleUpdateUser = async (data: any) => {
        try {
            const response = await fetch("http://localhost:5175/api/admin/users", {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(data),
            });
            if (!response.ok) {
                const txt = await response.text();
                console.error("Failed to update user:", txt);
                alert("Failed to update user");
                return;
            }
            alert("User updated successfully");
            onUserDeleted?.();
            setIsEditOpen(false);
            setEditingUser(null);
        } catch (err) {
            console.error(err);
            alert("Error while updating user");
        }
    };

    return (
    <>
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
                                onClick={() => openEditModal(user)}
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
    {isEditOpen && editingUser && (
        <EditUserModal initialUser={editingUser} onClose={() => setIsEditOpen(false)} onSubmit={handleUpdateUser} />
    )}
    </>
    );
}