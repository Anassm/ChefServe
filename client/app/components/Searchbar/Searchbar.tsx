import { IoIosSearch } from "react-icons/io";
import { useSearchParams } from "react-router";
import styles from "./Searchbar.module.css";

export default function Searchbar() {
  const [searchInput, setSearchInput] = useSearchParams();

  function handleChange(event: React.ChangeEvent<HTMLInputElement>) {
    const search = event.target.value;

    const newSearchInput = new URLSearchParams(searchInput);

    if (search) {
      newSearchInput.set("search", search);
    } else {
      newSearchInput.delete("search");
    }

    setSearchInput(newSearchInput);
  }

  const search = searchInput.get("search") || "";

  return (
    <div className={styles.searchbarContainer}>
      <IoIosSearch size={24} className={styles.icon} />
      <input
        className={styles.searchbar}
        type="text"
        placeholder={`Search...`}
        value={search}
        onChange={handleChange}
      />
    </div>
  );
}
