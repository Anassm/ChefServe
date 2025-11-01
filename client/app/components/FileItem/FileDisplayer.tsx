import styles from './FileDisplayer.module.css';
import type { fileItem } from './FileItem';
import { FileItem } from './FileItem';
import { useState } from 'react';

export default function FileDisplayer({ files }: { files: fileItem[] }) {
    const [selectedFile, setSelectedFile] = useState<string | null>(null);

    if (!files || files === undefined || files === null || files.length === 0) {
        return <div>No files to display</div>;
    }

    const handleSelect = (fileId: string) => {
        setSelectedFile(previousId => previousId === fileId ? null : fileId)
    }

    const openFile : () => void = () => {
        alert("Open file!")
    }

    const openFolder : () => void = () => {
        alert("Open folder!")
    }

    return (
        <div className={styles.body}>
            {files.map(file => (
                <FileItem
                    id={file.id}
                    key={file.name}
                    name={file.name}
                    extension={file.extension}
                    isFolder={file.isFolder}
                    hasContent={file.hasContent}
                    isSelected={selectedFile === file.id}
                    onSelect={() => handleSelect(file.id)}
                    onOpen ={file.isFolder ? openFolder : openFile}
                />
            ))}
        </div>
    );
}
