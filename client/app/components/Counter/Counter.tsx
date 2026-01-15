import styles from "./Counter.module.css";

export default function Counter({ title, count }: { title: string; count: number }) {
    return (
        <div className={styles.counterTile}>
            <h3 className={styles.counterTitle}>{title}</h3>
            <p className={styles.counterNumber}>{count}</p>
        </div>
    );
}  