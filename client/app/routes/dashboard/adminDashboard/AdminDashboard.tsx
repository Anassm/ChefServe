import { Navigate, NavLink, useNavigate } from "react-router";
import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";
import { useUser } from "~/helper/UserContext";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Admin | dashboard" },
    { name: "description", content: "Admin dashboard route" },
  ];
}

export default function AdminDashboard() {
  const user = useUser()
  let navigate = useNavigate()
  if(user.user?.role.toLowerCase() != "admin"){
    navigate(-1)
  }
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
