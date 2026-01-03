import styles from "./FileItem.module.css";
import { useState } from "react";
import { useRef } from "react";
import BaseModal from "../BaseModal/BaseModal";

export type fileItem = {
  id: string;
  name: string;
  extension: string;
  isFolder: boolean;
  path: string;
  hasContent: boolean;
  isSelected?: boolean;
  onSelect?: () => void;
  onOpen?: () => void;
};

type FileMetaData = {
  name: string;
  path: string;
  parentPath: string;
  extension: string;
  isFolder: boolean;
  createdAt: string;
  updatedAt: string;
  sizeInBytes: number;
  sizeInMB: number;
};

export function FileItem({
  id,
  name,
  extension,
  isFolder,
  path,
  hasContent,
  isSelected = false,
  onSelect,
  onOpen,
}: fileItem) {
  const ext = extension || ".folder";
  const filetype = ext.substring(1);
  const timer = useRef<NodeJS.Timeout | null>(null);
  const delay = 400;

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [fileMetaData, setFileMetaData] = useState<FileMetaData | null>(null);

  const handleClick = () => {
    if (onSelect) onSelect();
    if (timer.current) {
      clearTimeout(timer.current);
      timer.current = null;
      if (onOpen) {
        onOpen();
      }
    } else {
      timer.current = setTimeout(() => {
        timer.current = null;
      }, delay);
    }
  };

  async function handleRightClick(e: React.MouseEvent<HTMLDivElement>) {
    e.preventDefault();
    setIsModalOpen(true);
    setFileMetaData(null);

    try {
      const response = await fetch(
        `http://localhost:5175/api/File/GetFileInfo?fileID=${id.toUpperCase()}`,
        {
          method: "GET",
          credentials: "include",
          headers: {
            Accept: "application/json",
          },
        }
      );

      if (!response.ok) {
        throw new Error(
          (await response.text()) || "Failed to retrieve file info"
        );
      }

      const responseData = await response.json();
      setFileMetaData(responseData.data);
    } catch (err) {
      console.error(err);
      alert("Failed to retrieve file info");
    }
  }

  let imageSource: string = "";
  if (isFolder) {
    imageSource = hasContent ? "/icons/folderfull.webp" : "/icons/folder.webp";
  } else {
    imageSource = `/icons/${filetype}.webp`;
  }

  const wrapperClass = `${styles.wrapper} ${isSelected ? styles.selected : styles.unselected}`;

  return (
    <div
      className={wrapperClass}
      onClick={handleClick}
      onContextMenu={handleRightClick}
    >
      {isModalOpen && (
        <BaseModal
          title={`${fileMetaData?.name} properties`}
          onClose={() => setIsModalOpen(false)}
        >
          {fileMetaData ? (
            <>
              <span>
                <b>Size in bytes:</b> {fileMetaData.sizeInBytes}
              </span>
              <span>
                <b>Size in mb:</b> {fileMetaData.sizeInMB}
              </span>
              <span>
                <b>Extension:</b>{" "}
                {fileMetaData.isFolder ? "N/A" : fileMetaData.extension}
              </span>
              <span>
                <b>Type:</b> {fileMetaData.isFolder ? "Folder" : "File"}
              </span>
              <span>
                <b>Created at:</b> {fileMetaData.createdAt}
              </span>
              <span>
                <b>Updated at:</b> {fileMetaData.updatedAt}
              </span>
            </>
          ) : (
            <span>Loading...</span>
          )}
        </BaseModal>
      )}

      <button className={styles.button}>
        <img src={imageSource} alt="img" className="image" />
      </button>
      <p className={styles.name}>{name}</p>
    </div>
  );
}
