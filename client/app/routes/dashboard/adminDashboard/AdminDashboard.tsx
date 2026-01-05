import { NavLink, Outlet } from "react-router";
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
    <div>
      <div>This is an admin dashboard block</div>
    </div>
  );
}
