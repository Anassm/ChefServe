import styles from './FileItem.module.css';
import { useState } from 'react';
import { useRef } from 'react';


export type fileItem = {
    id: string,
    name: string,
    extension: string,
    isFolder: string,
    hasContent: string
    isSelected?: boolean,
    onSelect?: () => void,
    onOpen?: () => void
};

export function FileItem({ name, extension, isFolder, hasContent, isSelected = false, onSelect, onOpen, }: fileItem) {
    const ext = extension || '.folder';
    const filetype = ext.substring(1);
    const timer = useRef<NodeJS.Timeout | null>(null);
    const delay = 200;

    const handleClick = () => {
        if (timer.current) {
            clearTimeout(timer.current);
            timer.current = null;
            if (onOpen){
                if (onSelect) onSelect();
                onOpen();
            }
        } else {
            timer.current = setTimeout(() => {
                if (onSelect) onSelect();
                timer.current = null;
            }, delay);
        }
    };

    let imageSource: string = "";
    if (isFolder) {
        imageSource = hasContent ? '/icons/folderfull.webp' : '/icons/folder.webp';
    }
    else {
        imageSource = `/icons/${filetype}.webp`;
    }

    const wrapperClass = `${styles.wrapper} ${isSelected ? styles.selected : styles.unselected}`;

    return (
        <div className={wrapperClass} onClick={handleClick}>
            <button className={styles.button}>
                <img src={imageSource} alt="img" className="image" />
            </button>
            <p className={styles.name}>{name}</p>
        </div>
    );
}


