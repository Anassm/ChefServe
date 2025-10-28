import { useState } from "react";
import styles from "../MainLayout.module.css";
import { CiLogout, CiSettings } from "react-icons/ci";
import { IoFileTrayStackedOutline } from "react-icons/io5";
import { Form } from "react-router";

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

        <Form method="POST" action="/logout">
          <button type="submit" className={styles.button}>
            <CiLogout size={25} /> Logout
          </button>
        </Form>

        <span className={styles.love}>Made with ❤ by team Chef</span>
      </div>
    </aside>
  );
}
