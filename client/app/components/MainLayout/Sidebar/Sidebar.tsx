import React from "react";
import { useState, useEffect, useRef } from "react"; import styles from "./Sidebar.module.css";
import { CiLogout, CiSettings } from "react-icons/ci";
import { IoFileTrayStackedOutline } from "react-icons/io5";
import { Form } from "react-router";


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
      <span>Navigation</span>
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
  const sidebarRef = useRef(null);
  const [isResizing, setIsResizing] = useState(false);
  const [sidebarWidth, setSidebarWidth] = useState(268);

  const startResizing = React.useCallback((mouseDownEvent) => {
    setIsResizing(true);
    mouseDownEvent.preventDefault()
  }, []);

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
          <div className={styles.treeContent}>
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
        </div>
      </div>
      <div className={styles.sidebarResizer} onMouseDown={startResizing} />
    </div>
  );
}
