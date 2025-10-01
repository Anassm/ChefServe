import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Admin | dashboard" },
    { name: "description", content: "Admin dashboard route" },
  ];
}

export default function AdminDashboard() {
  return <div>This is a admin dashboard block</div>;
}
