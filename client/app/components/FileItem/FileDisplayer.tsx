import { useContext, useState } from "react";
import { useSearchParams, useNavigate, useParams } from "react-router";
import { selectedFileContext } from "~/context/SelectedFileContext";
import styles from "./FileDisplayer.module.css";
import type { fileItem } from "./FileItem";
import { FileItem } from "./FileItem";

export default function FileDisplayer({ files }: { files: fileItem[] }) {
  const context = useContext(selectedFileContext);
  const [params] = useSearchParams();
  const searchQuery = (params.get("search") || "").toLowerCase();

  const navigate = useNavigate();
  const { parentpath } = useParams();

  const selectedFile = context?.selectedFile;
  const setSelectedFile = context?.setSelectedFile;

  const [viewerFile, setViewerFile] = useState<{ url: string; name: string; extension: string } | null>(null);

  if (!files || files === undefined || files === null || files.length === 0) {
    return <div>No files to display</div>;
  }

  const filteredFiles = files.filter((file) =>
    file.name.toLowerCase().includes(searchQuery)
  );

  function handleSelect(file: fileItem) {
    if (setSelectedFile) {
      if (!selectedFile || selectedFile.id !== file.id) {
        setSelectedFile(file);
      } else {
        setSelectedFile(null);
      }
    }
  }

  async function handleOpenFile(file: fileItem) {
    try {
      const viewUrl = `http://localhost:5175/api/file/ViewFile?fileID=${file.id}`;
      window.open(viewUrl, '_blank');
    } catch (error) {
      console.error('Error opening file:', error);
      alert('Error opening file');
    }
  }



  function handleOpenFolder(file: fileItem) {
    console.log("FILE:", file);
    console.log("FILE.PATH:", file.path);
    navigate("/" + file.path);
  }

  return (
    <div className={styles.body}>
      {filteredFiles.map((file) => (
        <FileItem
          id={file.id}
          key={file.id}
          name={file.name}
          extension={file.extension}
          isFolder={file.isFolder}
          path={file.path}
          hasContent={file.hasContent}
          isSelected={selectedFile?.id === file.id}
          onSelect={() => handleSelect(file)}
          onOpen={() =>
            file.isFolder ? handleOpenFolder(file) : handleOpenFile(file)
          }
        />
      ))}
    </div>
  );
}
