import { NavLink } from "react-router";
import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Admin | dashboard" },
    { name: "description", content: "Admin dashboard route" },
  ];
}

export default function AdminDashboard() {
  return (
    <div className={styles.page}>
      <h1 className={styles.title}>Admin dashboard</h1>
      <p className={styles.lead}>
        Click here for the different admin pages:
      </p>
      <div className={styles.navContainer}>
        <NavLink to="/admin/users" className={styles.navButton}>
          Go to users
        </NavLink>
        <NavLink to="/admin/files" className={styles.navButton}>
          Go to files
        </NavLink>
      </div>
    </div>
  );
}
