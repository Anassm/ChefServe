import React, { useRef, useState } from "react";
import { CiLogout, CiSettings } from "react-icons/ci";
import { IoFileTrayStackedOutline } from "react-icons/io5";
import { NavLink } from "react-router";
import { Form } from "react-router";
import { FileTree } from "~/components/FileTree/FileTree";
import type { TreeItem } from "~/components/FileTree/FileTree";
import styles from "./Sidebar.module.css";

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
            <NavLink to="/admin/settings">Navigation</NavLink>
          </button>
        </li>
      </ul>
    </>
  );
}

function Settings() {
  return (
    <>
      <span>Navigation</span>
    </>
  );
}

export default function Sidebar({
  rootFolder,
}: {
  rootFolder: TreeItem | null;
}) {
  const [mode, setMode] = useState<"navigation" | "settings">("navigation");
  const sidebarRef = useRef(null);
  const [isResizing, setIsResizing] = useState(false);
  const [sidebarWidth, setSidebarWidth] = useState(268);

  const startResizing = React.useCallback(
    (mouseDownEvent) => {
      setIsResizing(true);
      mouseDownEvent.preventDefault();
    },
    []
  );

  const stopResizing = React.useCallback(() => {
    setIsResizing(false);
  }, []);

  const resize = React.useCallback(
    (mouseMoveEvent) => {
      if (isResizing) {
        setSidebarWidth(
          mouseMoveEvent.clientX -
          sidebarRef.current.getBoundingClientRect().left
        );
      }
    },
    [isResizing]
  );

  React.useEffect(() => {
    window.addEventListener("mousemove", resize);
    window.addEventListener("mouseup", stopResizing);
    return () => {
      window.removeEventListener("mousemove", resize);
      window.removeEventListener("mouseup", stopResizing);
    };
  }, [resize, stopResizing]);

  return (
    <div className={styles.sidebarWrapper}>
      <div
        ref={sidebarRef}
        className={styles.sidebar}
        style={{ width: sidebarWidth }}
      >
        <div className={styles.sidebarContent}>
          <div className={styles.contentBody}>
            <div className={styles.treeContentTop}>
              {mode == "navigation" ? <Navigation /> : <Settings />}
            </div>
            <div className={styles.contentBodyBottom}>
              <FileTree root={rootFolder} />
            </div>
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
                <IoFileTrayStackedOutline />

              ) : (
                <CiSettings size={25} />
              )}
              {mode === "navigation" ? "Navigation" : "Settings"}
            </button>

            <button
              type="submit"
              onClick={handleLogout}
              className={styles.button}
            >
              <CiLogout size={25} /> Logout
            </button>

            <span className={styles.love}>Made with ❤ by team Chef</span>
          </div>
        </div>
      </div>
      <div className={styles.sidebarResizer} onMouseDown={startResizing} />
    </div>
  );
}
