import React, { useState } from "react";
import BaseModal from "~/components/BaseModal/BaseModal";

import styles from './UserFormModal.module.css';



export default function UserFormModal({ 
  onClose, 
  onSubmit 
}: {
  onClose: () => void;
  onSubmit: (data: any) => void;
}) {
  const [formData, setFormData] = useState({
    username: "",
    firstName: "",
    lastName: "",
    email: "",
    passwordHash: "",
    role: "user"
  });

  const handleChange = (e: any) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSubmit = (e: any) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <BaseModal title="Add New User" onClose={onClose}>
      <form onSubmit={handleSubmit} className={styles.form}>
        <label>Username</label>
        <input type="text" name="username" value={formData.username} onChange={handleChange} required />

        <label>First Name</label>
        <input type="text" name="firstName" value={formData.firstName} onChange={handleChange} />

        <label>Last Name</label>
        <input type="text" name="lastName" value={formData.lastName} onChange={handleChange} />

        <label>Email</label>
        <input type="email" name="email" value={formData.email} onChange={handleChange} required />

        <label>Password</label>
        <input name="passwordHash" type="password" value={formData.passwordHash} onChange={handleChange} required />

        <label>Role</label>
        <select name="role" value={formData.role} onChange={handleChange}>
          <option value="user">User</option>
          <option value="admin">Admin</option>
        </select>

        <div className={styles.buttons}>
          <button type="submit" className={styles.submitButton}>Create</button>
          <button type="button" className={styles.cancelButton} onClick={onClose}>Cancel</button>
        </div>
      </form>
    </BaseModal>
  );
}