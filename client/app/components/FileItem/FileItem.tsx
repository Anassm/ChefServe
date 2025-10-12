import styles from './FileItem.module.css';
type Props = {
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



// export function FileItem({ id, name, isFolder }: Props) {

//     var object = 
//     <div className="file-item-wrapper">
//         <div className="icon">
//             <button className="imageButton"><img src="/images/txt.webp" width="64" height="64"alt="ALT"/></button>
//         </div>
//         <div className="name">
//             <p>{name}</p>
//         </div>

//     </div>;

//     return object;
// }

// C:\Users\jensk\Desktop\HR\J2S2\Web Dev\ChefServe\client\app\components\FileItem\images\txt.webp
// C:\Users\jensk\Desktop\HR\J2S2\Web Dev\ChefServe\client\app\components\FileItem\FileItem.tsx