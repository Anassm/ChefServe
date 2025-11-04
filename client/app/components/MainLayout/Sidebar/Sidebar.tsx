import { useState } from "react";
import styles from "../MainLayout.module.css";
import { CiLogout, CiSettings } from "react-icons/ci";
import { IoFileTrayStackedOutline } from "react-icons/io5";
import { Form, NavLink } from "react-router";


async function handleLogout() {
  await fetch("http://localhost:5175/api/auth/logout", {
    method: "POST",
    credentials: "include",
  });
  window.location.href = "/login";
}

function Navigation() {
  return (
    <>
      <ul>
        <li>
          <button className={`${styles.button} ${styles.hoverEffect}`}>
            <NavLink to="/admin">Admin dashboard</NavLink>
          </button>
        </li>
        <li>
          <button className={`${styles.button} ${styles.hoverEffect}`}>
            <NavLink to="/admin/overview">Overview</NavLink>
          </button>
        </li>
        <li>
          <button className={`${styles.button} ${styles.hoverEffect}`}>
            <NavLink to="/admin/users">Users</NavLink>
          </button>
        </li>
        <li>
          <button className={`${styles.button} ${styles.hoverEffect}`}>
            <NavLink to="/admin/files">Files</NavLink>
          </button>
        </li>
        <li>
          <button className={`${styles.button} ${styles.hoverEffect}`}>
            <NavLink to="/admin/settings">Settings</NavLink>
          </button>
        </li>
      </ul>
    </>
  );
}

function Settings() {
  return (
    <>
      <span>Settings</span>
    </>
  );
}

export default function Sidebar() {
  const [mode, setMode] = useState<"navigation" | "settings">("navigation");

  return (
    <aside className={styles.sidebar}>
      <div className={styles.content}>
        {mode == "navigation" ? <Navigation /> : <Settings />}
      </div>

      <div className={styles.bottom}>
        <button
          className={styles.button}
          onClick={() =>
            setMode((prevMode) =>
              prevMode === "navigation" ? "settings" : "navigation"
            )
          }
        >
          {mode === "navigation" ? (
            <CiSettings size={25} />
          ) : (
            <IoFileTrayStackedOutline />
          )}
          {mode === "navigation" ? "Settings" : "Navigation"}
        </button>

        <button type="submit" onClick={handleLogout} className={styles.button}>
          <CiLogout size={25} /> Logout
        </button>

        <span className={styles.love}>Made with ❤ by team Chef</span>
      </div>
    </aside>
  );
}
