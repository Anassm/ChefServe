import styles from "./FileDisplayer.module.css";
import type { fileItem } from "./FileItem";
import { FileItem } from "./FileItem";
import { useContext } from "react";
import { selectedFileContext } from "~/context/SelectedFileContext";
import { useState } from "react";
import { useSearchParams } from "react-router";

export default function FileDisplayer({ files }: { files: fileItem[] }) {
  const context = useContext(selectedFileContext);
  if (!context) return null;
  const { selectedFile, setSelectedFile } = context;

  if (!files || files === undefined || files === null || files.length === 0) {
    return <div>No files to display</div>;
  }

  const [params] = useSearchParams();
  const searchQuery = (params.get("q") || "").toLowerCase();

  const filteredFiles = files.filter((file) =>
    file.name.toLowerCase().includes(searchQuery)
  );

  const handleSelect = (file: fileItem) => {
    if (!selectedFile || selectedFile.id !== file.id) {
      setSelectedFile(file);
    } else {
      setSelectedFile(null);
    }
  };

  const openFile: () => void = () => {
    alert("Open file!");
  };

  const openFolder: () => void = () => {
    alert("Open folder!");
  };

  return (
    <div className={styles.body}>
      {filteredFiles.map((file) => (
        <FileItem
          id={file.id}
          key={file.id}
          name={file.name}
          extension={file.extension}
          isFolder={file.isFolder}
          hasContent={file.hasContent}
          isSelected={selectedFile?.id === file.id}
          onSelect={() => handleSelect(file)}
          onOpen={file.isFolder ? openFolder : openFile}
        />
      ))}
    </div>
  );
}
