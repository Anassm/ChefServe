import { Navigate } from "react-router";
import { useUser } from "~/helper/UserContext";
import AdminDashboard from "./adminDashboard/AdminDashboard";
import UserDashboard from "./userDashboard/UserDashboard";
import type { fileItem } from "~/components/FileItem/FileItem";
import { useLoaderData } from "react-router";


export async function loader({ params }: { params?: { parentpath: string } }) {
    const url: string = params == undefined ? `http://localhost:5175/api/file/getfiles` : `http://localhost:5175/api/file/getfiles?parentPath=${params?.parentpath}`;
    const data = await fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        credentials: 'include'
    });
    const files: fileItem[] = await data.json();
    console.log("Fetched files:", files);
    return files;
}

export default function Dashboard() {
    const { user } = useUser();
    const files = useLoaderData<fileItem[]>();


    return <UserDashboard files={files}/>;
    // if (!user) {
    //     return <Navigate to="/login" replace />;
    // }

    // if (user.role === "admin") {
    //     return <AdminDashboard />;
    // }

    // if (user.role === "user") {
    //     return <UserDashboard />;
    // }

    // return (
    //     <>
    //         <h2>Something went wrong.</h2>
    //         <p>No user role found for dashboard.</p>
    //     </>
    // );
}
