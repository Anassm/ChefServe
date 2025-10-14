import { Outlet } from "react-router";
import Header from "./Header/Header";
import Sidebar from "./Sidebar/Sidebar";
import styles from "./MainLayout.module.css";
import FileDisplayer from "../FileItem/FileDisplayer";

export default function MainLayout() {
  return (
    <div>
      <Header />
      <div className={styles.row}>
        <Sidebar />
        <main className={styles.main}>
          <Outlet />
        </main>
      </div>
    </div>
  );
}
