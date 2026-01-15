import { useState } from "react";

import style from "./FileTable.module.css";
import { TbFileMinus} from "react-icons/tb";

export default function FileTable({ files, onFileDeleted }: { files: any[]; onFileDeleted?: () => void }) {
    const [selectedFileId, setSelectedFileId] = useState<string | null>(null);
    const filesList: any[] = Array.isArray(files) ? files : (files ? Object.values(files) : []);
    const handleRowClick = (fileId: string) => {
        setSelectedFileId(fileId === selectedFileId ? null : fileId);
    }
    const deleteFile = (fileId: string, ownerId: string) => {
        console.log("Delete file with ID:", fileId);
            const confirmed = window.confirm("Are you sure you want to delete this file? This action cannot be undone.");
            if (!confirmed) return;

            fetch(`http://localhost:5175/api/admin/files/${fileId}/${ownerId}`, {
                method: "DELETE",
                credentials: "include",
            }).then(response => {
                if (response.ok) {
                    console.log("File deleted successfully");
                    onFileDeleted?.();
                } else {
                    console.error("Failed to delete file");
                }
            });
    }

    return (
    <table className={style.fileTable}>
        <thead>
            <tr>
                <th>File ID</th>
                <th>Filename</th>
                <th>Size in MB</th>
                <th>Uploaded At</th>
                <th>Owner name</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>     
            {
                filesList.map((file: any) => (
                    <tr
                        key={file.id}
                        onClick={() => handleRowClick(file.id)}
                        className={`${style.tableRow} ${
                            selectedFileId === file.id ? style.selectedRow : ""
                        }`}
                    >
                        <td>{file.id}</td>
                        <td>{file.name ?? file.filename ?? file.fileName}</td>
                        <td>{file.size ? (file.size / (1024 * 1024)).toFixed(2) : (file.Size ? (file.Size / (1024 * 1024)).toFixed(2) : (file.sizeInBytes ? (file.sizeInBytes / (1024 * 1024)).toFixed(2) : '-'))}</td>
                        <td>{(file.createdAt || file.uploadedAt) ? new Date(file.createdAt ?? file.uploadedAt).toLocaleString() : '-'}</td>
                        <td>{file.ownerName ?? file.owner?.username ?? file.owner?.email ?? '-'}</td>
                        <td>
                            <button
                                className={style.actionButton}
                                onClick={(e) => {
                                    e.stopPropagation();
                                    deleteFile(file.id, file.ownerId ?? file.owner?.id ?? "");
                                }
                            }>
                                <TbFileMinus size={20} color="red" />
                            </button>
                        </td>
                    </tr>
                ))
            }
        </tbody>
    </table>
    );
}