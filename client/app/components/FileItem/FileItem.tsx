import styles from './FileItem.module.css';
export type Props = {
    id: string;
    name: string;
    extension: string;
    isFolder: boolean;
};

export function FileItem({ id, name, extension, isFolder }: Props) {
    const filetype = extension.substring(1);
    const imageSource : string = `/images/${filetype}.webp`;
  return (
    <div className={styles.wrapper}>
      <button className={styles.button}>
        {isFolder ? "📁" : <img src={imageSource} alt={filetype} className="image" />}
      </button>
      <p className={styles.name}>{name}</p>
    </div>
  );
}


