import { Navigate } from "react-router";
import { useUser } from "~/helper/UserContext";
import AdminDashboard from "./adminDashboard/AdminDashboard";
import UserDashboard from "./userDashboard/UserDashboard";

export default function Dashboard() {
    const { user } = useUser();

    if (!user) {
        return <Navigate to="/login" replace />;
    }

    if (user.role === "admin") {
        return <AdminDashboard />;
    }

    if (user.role === "user") {
        return <UserDashboard />;
    }

    return (
        <>
            <h2>Something went wrong.</h2>
            <p>No user role found for dashboard.</p>
        </>
    );
}
