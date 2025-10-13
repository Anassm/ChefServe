// import { redirect, useLoaderData } from "react-router";
// import AdminDashboard from "./adminDashboard/AdminDashboard";
// import UserDashboard from "./userDashboard/UserDashboard";

// export async function loader() {
//   const user = await functiontogetuserfromsession();

//   if (!user) {
//     throw redirect("/login");
//   }

//   return { role: user.role };
// }

// export default function Dashboard() {
//   const { role } = useLoaderData<typeof loader>();

//   if (role === "admin") {
//     return <AdminDashboard />;
//   }

//   if (role === "user") {
//     return <UserDashboard />;
//   }

//   return (
//     <>
//       <h2>Something went wrong.</h2>
//       <p>No user role found for dashboard.</p>
//     </>
//   );
// }


import type { Props } from "../../components/FileItem/FileItem";
import { FileItem } from "../../components/FileItem/FileItem";
import FileDisplayer from "~/components/FileItem/FileDisplayer";
export default function Dashboard() {

}
