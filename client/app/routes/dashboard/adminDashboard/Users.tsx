import type { Route } from "../../../+types/root";
import styles from "./AdminDashboard.module.css";

export async function loader({ request }: Route.LoaderArgs) {
    return null;
}

export function HydrateFallback() {
    return <div>Loading user data...</div>;
}


export default function Users() {
  return(
    <div>
        <table className={styles.userTable}>
            <thead>
                <tr>
                    <th>User ID</th>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Role</th>
                </tr>
            </thead>
            <tbody>
                
            </tbody>
        </table>
    </div>
  )
}