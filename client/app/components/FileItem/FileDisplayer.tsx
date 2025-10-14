import styles from './FileItem.module.css';
import type { fileItem } from './FileItem';
import { FileItem } from './FileItem';

export function FileDisplayer(props : fileItem[]) {
    const displayItems = props.map(Item => <FileItem name={Item.name} extension={Item.extension} isFolder={Item.isFolder} hasContent={Item.hasContent} />)
    return (displayItems);
}

export default FileDisplayer;