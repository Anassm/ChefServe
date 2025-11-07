import { TbUserEdit, TbUserMinus } from "react-icons/tb";
import styles from "./UserTable.module.css";

export default function UserTable({ users }: { users: any[] }) {
    return (
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
                users.map((user: any) => (
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
    );
}