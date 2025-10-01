import type { Route } from "../../../+types/root";
import styles from "./UserDashboard.module.css";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "User | dashboard" },
    { name: "description", content: "User dashboard route" },
  ];
}

export default function UserDashboard() {
  return <div>This is a user dashboard block</div>;
}
