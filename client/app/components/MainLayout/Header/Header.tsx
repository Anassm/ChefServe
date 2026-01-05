import { useContext, useRef, useState } from "react";
import {
  AiOutlineDownload,
  AiOutlineFileAdd,
  AiOutlineFolderAdd,
} from "react-icons/ai";
import { BsArrow90DegUp } from "react-icons/bs";
import { IoFileTrayStackedOutline } from "react-icons/io5";
import { MdDriveFileRenameOutline } from "react-icons/md";
import { TbUserShield } from "react-icons/tb";
import { TiDocumentDelete } from "react-icons/ti";
import {
  NavLink,
  useLocation,
  useNavigate,
  useRevalidator,
} from "react-router";
import BaseModal from "~/components/BaseModal/BaseModal";
import Searchbar from "~/components/Searchbar/Searchbar";
import TextInput from "~/components/TextInput/TextInput";
import {
  refreshSidebarContext,
  selectedFileContext,
} from "~/context/SelectedFileContext";
import { useUser } from "~/helper/UserContext";
import styles from "../MainLayout.module.css";

export default function Header() {
  const location = useLocation();
  const navigate = useNavigate();
  const isRoot = location.pathname === "/" || location.pathname === "";
  const context = useContext(selectedFileContext);
  const refreshContext = useContext(refreshSidebarContext);
  const [pendingDestinationPath, setPendingDestinationPath] = useState("");
  const [adminMode, setAdminMode] = useState<
    "userManagement" | "fileManagement"
  >("fileManagement");
  const { user } = useUser();
  const isAdminMenuOpen = adminMode === "userManagement";

  if (!refreshContext) return null;
  const { refresh, setRefresh } = refreshContext;

  if (!context) return null;
  const { selectedFile, setSelectedFile } = context;

  const revalidator = useRevalidator();
  const uploadRef = useRef<HTMLInputElement>(null);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newFolderName, setNewFolderName] = useState("");

  const [showConflictModal, setShowConflictModal] = useState(false);
  const [conflictFile, setConflictFile] = useState<File | null>(null);

  const [renameModal, setRenameModal] = useState(false);
  const [renameData, setRenameData] = useState("");

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
        throw new Error((await response.text()) || "Failed to delete file.");
      }

      setSelectedFile(null);

      revalidator.revalidate();
      setRefresh((prev) => !prev);
    } catch (err) {
      console.error(err);
      alert("Failed to delete file");
    }
  }

  async function renameFile() {
    if (!selectedFile || !renameData.trim()) return;

    const body = {
      fileID: selectedFile.id,
      newName: renameData,
    };

    try {
      const response = await fetch(
        `http://localhost:5175/api/File/RenameFile`,
        {
          method: "PUT",
          body: JSON.stringify(body),
          credentials: "include",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
          },
        }
      );

      if (!response.ok) {
        throw new Error((await response.text()) || "Failed to rename file.");
      }

      setRenameData("");
      setRenameModal(false);
      setSelectedFile(null);
      revalidator.revalidate();
    } catch (err) {
      console.error(err);
      alert("Failed to rename file.");
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
      const response = await fetch(
        `http://localhost:5175/api/File/UploadFile`,
        {
          method: "POST",
          body: formData,
          credentials: "include",
          headers: { Accept: "application/json" },
        }
      );

      if (!response.ok) {
        throw new Error((await response.text()) || "Failed to upload file.");
      }

      revalidator.revalidate();
      setRefresh((prev) => !prev);
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
      } else if (response.status !== 201) {
        throw new Error(result.message || "Failed to upload file.");
      } else {
        revalidator.revalidate();
        setRefresh((prev) => !prev);
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
        throw new Error((await response.text()) || "Failed to create folder.");
      }

      setNewFolderName("");
      setIsModalOpen(false);
      revalidator.revalidate();
      setRefresh((prev) => !prev);
    } catch (err) {
      console.error(err);
      alert("Failed to create folder");
    }
  }

  return (
    <header className={styles.header}>
      <div className={styles.fileActions}>
        {user?.role === "admin" ? (
          adminMode == "userManagement" ? (
            <NavLink
              className={styles.button}
              to="/"
              onClick={() => setAdminMode("fileManagement")}
              style={{ marginRight: "12px" }}
            >
              <IoFileTrayStackedOutline size={25} />
            </NavLink>
          ) : (
            <NavLink
              className={styles.button}
              to="/admin/"
              onClick={() => setAdminMode("userManagement")}
              style={{ marginRight: "12px" }}
            >
              <TbUserShield size={28} />
            </NavLink>
          )
        ) : null}

        <Searchbar />

        <BsArrow90DegUp
          size={40}
          style={{
            cursor:
              adminMode === "userManagement" || isRoot
                ? "not-allowed"
                : "pointer",
          }}
          opacity={adminMode === "userManagement" || isRoot ? 0.5 : 1}
          onClick={() => {
            if (adminMode !== "userManagement" && !isRoot) {
              const parentPath =
                location.pathname.substring(
                  0,
                  location.pathname.lastIndexOf("/")
                ) || "/";
              navigate(parentPath);
            }
          }}
        />

        {/* Create folder */}
        <AiOutlineFolderAdd
          size={40}
          onClick={() => {
            if (adminMode !== "userManagement") {
              setIsModalOpen(true);
            }
          }}
          style={{
            cursor: adminMode === "userManagement" ? "not-allowed" : "pointer",
          }}
          opacity={adminMode === "userManagement" ? 0.5 : 1}
        />

        {/* Upload file */}
        <AiOutlineFileAdd
          size={40}
          onClick={() => {
            if (adminMode !== "userManagement") {
              uploadRef.current?.click();
            }
          }}
          style={{
            cursor: adminMode === "userManagement" ? "not-allowed" : "pointer",
          }}
          opacity={adminMode === "userManagement" ? 0.5 : 1}
        />
        <input
          ref={uploadRef}
          type="file"
          multiple
          // directory="true"
          style={{ display: "none" }}
          disabled={adminMode === "userManagement"}
          onChange={onUpload}
        />

        {/* Download file */}
        <AiOutlineDownload
          size={40}
          onClick={
            adminMode === "userManagement" ||
            !selectedFile ||
            selectedFile.isFolder
              ? () => {}
              : () => {
                  window.open(
                    `http://localhost:5175/api/File/DownloadFile?fileID=${selectedFile.id}`,
                    "_blank"
                  );
                }
          }

          opacity={
            adminMode === "userManagement" ||
            !selectedFile ||
            selectedFile.isFolder
              ? 0.5
              : 1
          }
          style={
            adminMode === "userManagement" ||
            !selectedFile ||
            selectedFile.isFolder
              ? { cursor: "not-allowed" }
              : { cursor: "pointer" }
          }
        />

        {/* Rename file */}
        <MdDriveFileRenameOutline
          size={40}
          onClick={
            !selectedFile || adminMode === "userManagement"
              ? () => {}
              : () => setRenameModal(true)
          }
          opacity={!selectedFile || adminMode === "userManagement" ? 0.5 : 1}
          style={
            !selectedFile || adminMode === "userManagement"
              ? { cursor: "not-allowed" }
              : { cursor: "pointer" }
          }
        />

        {/* Delete file */}
        <TiDocumentDelete
          size={40}
          onClick={
            !selectedFile || adminMode === "userManagement"
              ? () => {}
              : onFileDelete
          }
          opacity={!selectedFile || adminMode === "userManagement" ? 0.5 : 1}
          style={
            !selectedFile || adminMode === "userManagement"
              ? { cursor: "not-allowed" }
              : { cursor: "pointer" }
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
          <p>
            File "{conflictFile.name}" already exists. What do you want to do?
          </p>
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

      {renameModal && (
        <BaseModal
          title={`Rename ${selectedFile?.isFolder ? "folder" : "file"}`}
          onClose={() => setRenameModal(false)}
        >
          <TextInput
            label="New name"
            placeholder="A very cool new name"
            value={renameData}
            onChange={(e) => setRenameData(e.target.value)}
          />
          <div className={styles.modalButtons}>
            <button
              className={styles.submitButton}
              type="button"
              onClick={renameFile}
            >
              Create
            </button>
            <button
              className={styles.cancelButton}
              onClick={() => {
                setRenameModal(false);
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
