import { useContext, useRef } from "react";
import styles from "../MainLayout.module.css";
import { selectedFileContext } from "~/context/SelectedFileContext";
import { TiDocumentDelete } from "react-icons/ti";
import { useRevalidator } from "react-router";
import { IoMdAdd } from "react-icons/io";
import Searchbar from "~/components/Searchbar/Searchbar";

export default function Header() {
  const context = useContext(selectedFileContext);
  if (!context) return null;
  const { selectedFile, setSelectedFile } = context;

  const revalidator = useRevalidator();
  const uploadRef = useRef<HTMLInputElement>(null);

  async function onFileDelete() {
    if (!selectedFile) {
      alert("No file selected");
      return;
    }

    const confirmDelete = confirm(`Delete file "${selectedFile.name}"?`);
    if (!confirmDelete) return;

    try {
      const response = await fetch(
        `http://localhost:5175/api/File/DeleteFile?fileID=${selectedFile.id}`,
        {
          method: "DELETE",
          credentials: "include",
          headers: {
            Accept: "application/json",
          },
        }
      );

      if (!response.ok) {
        throw new Error((await response.text()) || "Failed to delete file");
      }

      setSelectedFile(null);

      revalidator.revalidate();
    } catch (err) {
      console.error(err);
      alert("Failed to delete file");
    }
  }

  async function onUpload(event: React.ChangeEvent<HTMLInputElement>) {
    const files = event.target.files;
    if (!files || files.length === 0) return;

    const file = files[0];

    const confirmUpload = confirm(`Upload file "${file.name}"?`);
    if (!confirmUpload) return;

    const formData = new FormData();
    formData.append("FileName", file.name);
    formData.append("Content", file);
    formData.append("DestinationPath", "/"); // Voor nu even "/"

    try {
      const response = await fetch(
        `http://localhost:5175/api/File/UploadFile`,
        {
          method: "POST",
          body: formData,
          credentials: "include",
          headers: {
            Accept: "application/json",
          },
        }
      );

      if (response.status != 201) {
        throw new Error((await response.text()) || "Failed to upload file");
      }

      revalidator.revalidate();
    } catch (err) {
      console.error(err);
      alert("Failed to upload file");
    }
  }

  return (
    <header className={styles.header}>
      <div className={styles.fileActions}>
        <Searchbar />
        <TiDocumentDelete
          size={40}
          onClick={!selectedFile ? () => {} : onFileDelete}
          opacity={!selectedFile ? 0.5 : 1}
          style={
            !selectedFile ? { cursor: "not-allowed" } : { cursor: "pointer" }
          }
        />
        <IoMdAdd
          size={40}
          onClick={() => uploadRef.current?.click()}
          style={{ cursor: "pointer" }}
        />
        <input
          ref={uploadRef}
          type="file"
          multiple
          style={{ display: "none" }}
          onChange={onUpload}
        />
      </div>
    </header>
  );
}
