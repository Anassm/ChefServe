import { useContext, useRef, useState } from "react";
import styles from "../MainLayout.module.css";
import { selectedFileContext } from "~/context/SelectedFileContext";
import { TiDocumentDelete } from "react-icons/ti";
import { useRevalidator } from "react-router";
import { AiOutlineFileAdd, AiOutlineFolderAdd } from "react-icons/ai";
import Searchbar from "~/components/Searchbar/Searchbar";
import BaseModal from "~/components/BaseModal/BaseModal";
import TextInput from "~/components/TextInput/TextInput";

export default function Header() {
  const context = useContext(selectedFileContext);
  if (!context) return null;
  const { selectedFile, setSelectedFile } = context;

  const revalidator = useRevalidator();
  const uploadRef = useRef<HTMLInputElement>(null);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newFolderName, setNewFolderName] = useState("");

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

    const formData = new FormData();

    for (const file of Array.from(files)) {
      formData.append("files", file);
      formData.append("paths", file.webkitRelativePath || file.name);
    }

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

      if (!response.ok) {
        throw new Error((await response.text()) || "Failed to upload file(s)");
      }

      revalidator.revalidate();
    } catch (err) {
      console.error(err);
      alert("Failed to upload file(s)");
    }
  }

  async function onCreateFolder() {
    const currentPath = window.location.pathname || "/";

    const folderData = {
      folderName: newFolderName,
      parentPath: currentPath,
    };

    try {
      const response = await fetch(
        "http://localhost:5175/api/File/CreateFolder",
        {
          method: "POST",
          body: JSON.stringify(folderData),
          credentials: "include",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
          },
        }
      );

      if (!response.ok) {
        throw new Error((await response.text()) || "Failed to create folder");
      }

      setNewFolderName("");
      setIsModalOpen(false);
      revalidator.revalidate();
    } catch (err) {
      console.error(err);
      alert("Failed to create folder");
    }
  }

  return (
    <header className={styles.header}>
      <div className={styles.fileActions}>
        <Searchbar />

        {/* Delete file */}
        <TiDocumentDelete
          size={40}
          onClick={!selectedFile ? () => {} : onFileDelete}
          opacity={!selectedFile ? 0.5 : 1}
          style={
            !selectedFile ? { cursor: "not-allowed" } : { cursor: "pointer" }
          }
        />

        {/* Create folder */}
        <AiOutlineFolderAdd
          size={40}
          onClick={() => setIsModalOpen(true)}
          style={{ cursor: "pointer" }}
        />

        {/* Upload file */}
        <AiOutlineFileAdd
          size={40}
          onClick={() => uploadRef.current?.click()}
          style={{ cursor: "pointer" }}
        />
        <input
          ref={uploadRef}
          type="file"
          multiple
          webkitdirectory="true"
          directory="true"
          style={{ display: "none" }}
          onChange={onUpload}
        />
      </div>

      {isModalOpen && (
        <BaseModal title="Create folder" onClose={() => setIsModalOpen(false)}>
          <TextInput
            label="Folder name"
            placeholder="Homework"
            value={newFolderName}
            onChange={(e) => {
              setNewFolderName(e.target.value);
            }}
          />
          <div className={styles.modalButtons}>
            <button
              className={styles.submitButton}
              type="button"
              onClick={onCreateFolder}
            >
              Create
            </button>
            <button
              className={styles.cancelButton}
              onClick={() => {
                setIsModalOpen(false);
              }}
            >
              Cancel
            </button>
          </div>
        </BaseModal>
      )}
    </header>
  );
}
