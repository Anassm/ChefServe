import { useState, useEffect } from "react";
import BaseModal from "~/components/BaseModal/BaseModal";
import styles from './UserFormModal.module.css';

export default function EditUserModal({ initialUser, onClose, onSubmit }: { initialUser: any; onClose: () => void; onSubmit: (data: any) => void }) {
  const [formData, setFormData] = useState({
    username: "",
    firstName: "",
    lastName: "",
    email: "",
    role: "user"
  });

  useEffect(() => {
    if (initialUser) {
      setFormData({
        username: initialUser.username ?? initialUser.name ?? "",
        firstName: initialUser.firstName ?? "",
        lastName: initialUser.lastName ?? "",
        email: initialUser.email ?? "",
        role: initialUser.role ?? "user"
      });
    }
  }, [initialUser]);

  const handleChange = (e: any) => setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSubmit = (e: any) => {
    e.preventDefault();
    onSubmit({ ID: initialUser.id, Username: formData.username, FirstName: formData.firstName, LastName: formData.lastName, Email: formData.email, Role: formData.role });
  };

  return (
    <BaseModal title={`Edit ${initialUser?.username ?? 'User'}`} onClose={onClose}>
      <form onSubmit={handleSubmit} className={styles.form}>
        <label>Username</label>
        <input type="text" name="username" value={formData.username} onChange={handleChange} required />

        <label>First Name</label>
        <input type="text" name="firstName" value={formData.firstName} onChange={handleChange} />

        <label>Last Name</label>
        <input type="text" name="lastName" value={formData.lastName} onChange={handleChange} />

        <label>Email</label>
        <input type="email" name="email" value={formData.email} onChange={handleChange} required />

        <label>Role</label>
        <select name="role" value={formData.role} onChange={handleChange}>
          <option value="user">User</option>
          <option value="admin">Admin</option>
        </select>

        <div className={styles.buttons}>
          <button type="submit" className={styles.submitButton}>Save</button>
          <button type="button" className={styles.cancelButton} onClick={onClose}>Cancel</button>
        </div>
      </form>
    </BaseModal>
  );
}
