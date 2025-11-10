import { Outlet } from "react-router";
import Header from "./Header/Header";
import Sidebar from "./Sidebar/Sidebar";
import styles from "./MainLayout.module.css";
import type { fileItem } from "../FileItem/FileItem";
import FileDisplayer from "../FileItem/FileDisplayer";
import { useState } from "react";
import { selectedFileContext } from "~/context/SelectedFileContext";
import type { TreeItem } from "../FileTree/FileTree";
import { useEffect } from "react";

export default function MainLayout() {
  const [selectedFile, setSelectedFile] = useState<fileItem | null>(null);
  const [rootFolder, setRootFolder] = useState<TreeItem | null>(null);

  useEffect(() => {
    async function fetchTree() {
      const res = await fetch("http://localhost:5175/api/file/GetFileTree", { credentials: "include" });
      const data: TreeItem = await res.json();
      setRootFolder(data);
    }
    fetchTree();
  }, []);

  return (
    <selectedFileContext.Provider value={{ selectedFile, setSelectedFile }}>
      <div>
        <Header />
        <div className={styles.row}>
          <Sidebar rootFolder={rootFolder}/>
          <main className={styles.main}>
            <Outlet />
          </main>
        </div>
      </div>
    </selectedFileContext.Provider>
  );
}
