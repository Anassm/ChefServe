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
export default function Dashboard() {
  const exampleData: Props[] = [
    { id: "1", name: "Documenten", extension: "", isFolder: true },
    { id: "2", name: "Afbeeldingen", extension: "", isFolder: true },
    { id: "3", name: "Muziek", extension: "", isFolder: true },
    { id: "4", name: "Video's", extension: "", isFolder: true },
    { id: "5", name: "Downloads", extension: "", isFolder: true },
    { id: "6", name: "Projectplan", extension: "docx", isFolder: false },
    { id: "7", name: "Budget 2025", extension: "xlsx", isFolder: false },
    { id: "8", name: "Logo", extension: "png", isFolder: false },
    { id: "9", name: "Teamfoto", extension: "jpg", isFolder: false },
    { id: "10", name: "Introductie", extension: "mp4", isFolder: false },
    { id: "11", name: "Handleiding", extension: "pdf", isFolder: false },
    { id: "12", name: "Script", extension: "ts", isFolder: false },
    { id: "13", name: "Stijlblad", extension: "css", isFolder: false },
    { id: "14", name: "Configuratie", extension: "json", isFolder: false },
    { id: "15", name: "Index", extension: "html", isFolder: false },
    { id: "16", name: "Readme", extension: "md", isFolder: false },
    { id: "17", name: "Notities", extension: "txt", isFolder: false },
    { id: "18", name: "Iconen", extension: "", isFolder: true },
    { id: "19", name: "Backups", extension: "", isFolder: true },
    { id: "20", name: "Presentatie", extension: "pptx", isFolder: false },
  ];
  const displayItems = exampleData.map(Item => )

  return ({

  });
}
