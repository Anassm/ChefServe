import styles from './FileTree.module.css';
import { useState } from 'react';

export type TreeItem = {
    id: string,
    name: string,
    isFolder: boolean,
    folderPath: string | null,
    children?: TreeItem[],
};

export function TreeFile({
    item,
    level = 0,
    selectedId,
    folderPath,
}: {
    item: TreeItem;
    level?: number;
    selectedId?: string;
    folderPath: string | null;
}) {
    const [isOpen, setIsOpen] = useState(false);
    const imageSource: string = '/icons/folder.webp';

    const handleToggle = () => {
        setIsOpen(!isOpen);
    };


    return (
        <div className={styles.treeItem}>
            <div
                className={`${styles.row} ${selectedId === item.id ? styles.selected : styles.unselected}`}
                onClick={() => { alert("Open folder from tree!") }}
                style={{ paddingLeft: `${level * 16}px` }}
            >
                <button className={styles.toggle} onClick={(e) => { e.stopPropagation(); handleToggle(); }}>
                    {isOpen ? 'v' : '>'}
                </button>
                <img
                    src={imageSource}
                    className={styles.icon}
                />
                <span className={styles.folderName}>{item.name}</span>
            </div>

            {isOpen && item.children && (
                <div>
                    {item.children.map((child) => (
                        <TreeFile
                            key={child.id}
                            item={child}
                            level={level + 1}
                            selectedId={selectedId}
                            folderPath={child.folderPath}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}




export function FileTree({ root }: { root: TreeItem | null }) {
    const [selectedFile, setSelectedFile] = useState<TreeItem | null>(null);

    if (!root) return null;
    return (
        <div className={styles.tree}>
            <TreeFile
                item={root}
                selectedId={selectedFile?.id}
                folderPath={root.folderPath ? root.folderPath : null}
            />
        </div>
    );
}