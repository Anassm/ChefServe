import { Outlet } from "react-router";
import Header from "./Header/Header";
import Sidebar from "./Sidebar/Sidebar";
import styles from "./MainLayout.module.css";
import type { fileItem } from "../FileItem/FileItem";
import FileDisplayer from "../FileItem/FileDisplayer";
import { useState } from "react";
import { selectedFileContext } from "~/context/SelectedFileContext";

export default function MainLayout() {
  const [selectedFile, setSelectedFile] = useState<fileItem | null>(null);
  return (
    <selectedFileContext.Provider value={{selectedFile, setSelectedFile}}>
      <div>
        <Header />
        <div className={styles.row}>
          <Sidebar />
          <main className={styles.main}>
            <Outlet />
          </main>
        </div>
      </div>
    </selectedFileContext.Provider>
  );
}
