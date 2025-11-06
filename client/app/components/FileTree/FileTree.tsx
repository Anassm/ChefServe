import styles from './FileTree.module.css';
import type { fileItem } from '../FileItem/FileItem';
import { FileItem } from '../FileItem/FileItem';
import { useContext } from 'react';
import { selectedFileContext } from '~/context/SelectedFileContext';
import { useState } from 'react';

export type TreeItem = {
    id: string,
    name: string,
    isFolder: boolean,
    isSelected: boolean,
    onOpen: () => void
};

export function TreeFile({ id, name, isFolder, isSelected, onOpen }: TreeItem) {
    if (!isFolder) return null;
    const imageSource: string = '/icons/folder.webp';
    const [button, setButton] = useState('>');
    const [isOpen, setIsOpen] = useState(false);

    const handleClick = () => {
        if (isOpen) {
            setButton('\u2335');
            setIsOpen(true);
        }
        else {
            setButton('>');
            setIsOpen(false);
        }
    }

    const wrapperClass = `${styles.wrapper} ${isSelected ? styles.selected : styles.unselected}`;

    return (
        <div className={wrapperClass}>
            <button onClick={handleClick}>{button}</button>
            <button onClick={onOpen}>
                <img src={imageSource} className={styles.folderImage}></img>
                <span className={styles.folderName}>{name}</span>
            </button>
        </div>
    );
}




export function FileTree(files: TreeItem[]) {
    const [selectedFile, setSelectedFile] = useState<TreeItem | null>(null)

    const handleSelect = (file: TreeItem) => {
        if (!selectedFile || selectedFile.id !== file.id) {
            setSelectedFile(file);
        } else {
            setSelectedFile(null);
        }
    };
    const openFile: () => void = () => {
        alert("Open file!")
    }

    return (
        <div className={styles.tree}>
            {files.map(file => (
                <TreeFile
                    id={file.id}
                    name={file.name}
                    isFolder={file.isFolder}
                    isSelected={selectedFile?.id === file.id}
                    onSelect={() => handleSelect(file)}
                    onOpen={openFile}
                />

            ))}
        </div>
    );
}