import styles from "./TextInput.module.css";

type TextInputProps = {
  label?: string;
  error?: string;
  className?: string;
} & React.InputHTMLAttributes<HTMLInputElement>;

export default function TextInput({
  label,
  error,
  className,
  ...props
}: TextInputProps) {
  return (
    <div className={styles.wrapper}>
      {label && <label className="styles.label">{label}</label>}

      <input
        className={`${styles.input} ${error ? styles.error : ""} ${className ?? ""}`}
        {...props}
      />

      {error && <p className={styles.errorMessage}>{error}</p>}
    </div>
  );
}
