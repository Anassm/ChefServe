import styles from './BaseModal.module.css';

export default function BaseModal({
    title,
    onClose,
    children
}: {
    title: string;
    onClose: () => void;
    children: React.ReactNode;
}) {
    return (
        <div className={styles.overlay} onClick={onClose}>
            <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
                {title && <h2 className={styles.title}>{title}</h2>}
                {children}
            </div>
        </div>
    )
}