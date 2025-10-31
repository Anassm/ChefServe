import styles from './FileItem.module.css';
export type fileItem = {
    name: string,
    extension: string,
    isFolder: string,
    hasContent: string
};

export function FileItem({ name, extension, isFolder, hasContent }: fileItem) {
    if (!extension) {
        extension = ".folder";
    }
    const filetype = extension.substring(1);
    let imageSource: string = "";
    if (isFolder) {
        if (hasContent) {
            imageSource = "/icons/folderfull.webp";
        }
        else {
            imageSource = "/icons/folder.webp";
        }
    }
    else {
        imageSource = `/icons/${filetype}.webp`;
    }
    return (
        <div className={styles.wrapper}>
            <button className={styles.button}>
                <img src={imageSource} alt="img" className="image" />
            </button>
            <p className={styles.name}>{name}</p>
        </div>
    );
}


