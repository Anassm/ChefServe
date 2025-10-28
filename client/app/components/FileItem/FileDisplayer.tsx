import styles from './FileItem.module.css';
import type { fileItem } from './FileItem';
import { FileItem } from './FileItem';

export default function FileDisplayer({ files }: { files: fileItem[] }) {
    const displayItems = files.map(Item => <FileItem name={Item.name} extension={Item.extension} isFolder={Item.isFolder} hasContent={Item.hasContent} />)
    return displayItems;
}
