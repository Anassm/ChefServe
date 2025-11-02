import { useContext } from "react";
import styles from "../MainLayout.module.css";
import { selectedFileContext } from "~/context/SelectedFileContext";

export default function Header() {
  const selectedFile = useContext(selectedFileContext)
  return (
    <header className={styles.header}>
      <span>This is a header</span>
      <button className={styles.testButton} onClick={() => alert(selectedFile ? JSON.stringify(selectedFile, null, 2) : "No file selected")}>Test</button>
    </header>
  );
}
