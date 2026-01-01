import { useContext, useRef, useState } from "react";
import styles from "../MainLayout.module.css";
import { selectedFileContext } from "~/context/SelectedFileContext";
import { TiDocumentDelete } from "react-icons/ti";
import { useRevalidator } from "react-router";
import { AiOutlineFileAdd, AiOutlineFolderAdd, AiOutlineDownload } from "react-icons/ai";
import Searchbar from "~/components/Searchbar/Searchbar";
import BaseModal from "~/components/BaseModal/BaseModal";
import TextInput from "~/components/TextInput/TextInput";
import { refreshSidebarContext } from "~/context/SelectedFileContext";


export default function Header() {
  const context = useContext(selectedFileContext);
  const refreshContext = useContext(refreshSidebarContext);
  const [conflictFile, setConflictFile] = useState<File | null>(null);
  const [showConflictModal, setShowConflictModal] = useState(false);
  const [pendingDestinationPath, setPendingDestinationPath] = useState("");
  if (!refreshContext) return null;
  const { refresh, setRefresh } = refreshContext;

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
      setRefresh(prev => !prev);
    } catch (err) {
      console.error(err);
      alert("Failed to delete file");
    }
  }

  async function handleConflictMode(mode: "Overwrite" | "Suffix" | "Cancel") {
    if (!conflictFile) return;

    setShowConflictModal(false);

    if (mode === "Cancel") {
      setConflictFile(null);
      return;
    }

    const formData = new FormData();
    formData.append("FileName", conflictFile.name);
    formData.append("Content", conflictFile);
    formData.append("DestinationPath", pendingDestinationPath);
    formData.append("ConflictMode", mode);

    try {
      const response = await fetch(`http://localhost:5175/api/File/UploadFile`, {
        method: "POST",
        body: formData,
        credentials: "include",
        headers: { Accept: "application/json" },
      });

      if (!response.ok) {
        throw new Error((await response.text()) || "Failed to upload file");
      }

      revalidator.revalidate();
      setRefresh(prev => !prev);
    } catch (err) {
      console.error(err);
      alert("Failed to upload file");
    } finally {
      setConflictFile(null);
      if (uploadRef.current) uploadRef.current.value = "";
    }
  }

  async function onUpload(event: React.ChangeEvent<HTMLInputElement>) {
    const files = event.target.files;
    if (!files || files.length === 0) return;

    const file = files[0];
    let desPath = decodeURIComponent(window.location.pathname);
    setPendingDestinationPath(desPath);


    const formData = new FormData();
    formData.append("FileName", file.name);
    formData.append("Content", file);
    formData.append("DestinationPath", desPath);
    console.log("Uploading to path:", desPath);

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

      const result = await response.json();
      if (response.status === 409) {
        setConflictFile(file);
        setShowConflictModal(true);
      } else if (response.status !== 200) {
        throw new Error(result.message || "Failed to upload file");
      } else {
        revalidator.revalidate();
        setRefresh(prev => !prev);
      }
    } catch (err) {
      console.error(err);
      alert("Failed to upload file");
    } finally {
      if (uploadRef.current) {
        uploadRef.current.value = "";
      }
    }
  }



  async function onCreateFolder() {
    const currentPath = decodeURIComponent(window.location.pathname) || "/";

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
      setRefresh(prev => !prev);
    } catch (err) {
      console.error(err);
      alert("Failed to create folder");
    }
  }

  return (
    <header className={styles.header}>
      <div className={styles.fileActions}>
        <Searchbar />

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
          // directory="true"
          style={{ display: "none" }}
          onChange={onUpload}
        />

        {/* Download file */}
        <AiOutlineDownload
          size={40}
          onClick={!selectedFile || selectedFile.isFolder ? () => { } : () => {
            window.open(`http://localhost:5175/api/File/DownloadFile?fileID=${selectedFile.id}`, "_blank");
          }}
          opacity={!selectedFile || selectedFile.isFolder ? 0.5 : 1}
          style={
            !selectedFile || selectedFile.isFolder ? { cursor: "not-allowed" } : { cursor: "pointer" }
          }
        />

        {/* Delete file */}
        <TiDocumentDelete
          size={40}
          onClick={!selectedFile ? () => { } : onFileDelete}
          opacity={!selectedFile ? 0.5 : 1}
          style={
            !selectedFile ? { cursor: "not-allowed" } : { cursor: "pointer" }
          }
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

      {showConflictModal && conflictFile && (
        <BaseModal
          title="File Conflict"
          onClose={() => setShowConflictModal(false)}
        >
          <p>File "{conflictFile.name}" already exists. What do you want to do?</p>
          <div
            className={styles.modalButtons}
            style={{ justifyContent: "space-between", marginTop: "1rem" }}
          >
            <button
              className={styles.submitButton}
              onClick={() => handleConflictMode("Overwrite")}
            >
              Overwrite
            </button>
            <button
              className={styles.submitButton}
              onClick={() => handleConflictMode("Suffix")}
            >
              Suffix
            </button>
            <button
              className={styles.cancelButton}
              onClick={() => handleConflictMode("Cancel")}
            >
              Cancel
            </button>
          </div>
        </BaseModal>
      )}
    </header>
  );
}
