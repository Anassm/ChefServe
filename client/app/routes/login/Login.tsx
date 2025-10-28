import { useNavigate } from "react-router";
import styles from "./Login.module.css";
import { useUser } from "~/helper/UserContext";
import { useEffect } from "react";

export default function Login() {
  const navigate = useNavigate();
  const { setUser } = useUser();

  useEffect(() => {
    async function checkLogin() {
      try {
        const res = await fetch("http://localhost:5175/api/auth/me", {
          method: "GET",
          credentials: "include", // sends cookie if backend sets it
        });
        if (res.ok) {
          const dto = await res.json();
          setUser({
            username: dto.username,
            firstname: dto.firstname,
            lastname: dto.lastname,
            role: dto.role,
          });
          navigate("/");
        }
      } catch (err) {
        console.error("Failed to check login:", err);
      }
    }
    checkLogin();
  }, [navigate, setUser]);


  async function attemptLogin(username: any, password: any, invalidateAll = false) {
    const res = await fetch("http://localhost:5175/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, password, invalidateAll }),
      credentials: "include",
    });
    return res;
  }

  async function handleLogin(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);

    const username = formData.get("username");
    const password = formData.get("password");

    let res = await attemptLogin(username, password);

    if (res.status === 409) {
      const confirmLogout = window.confirm(
        "You are already logged in on another device. Logging in here will log you out everywhere else. Continue?"
      );
      if (!confirmLogout) return;
      res = await attemptLogin(username, password, true);
    }

    if (res.ok) {
      const dto = await res.json();

      setUser({
        username: dto.username,
        firstname: dto.firstname,
        lastname: dto.lastname,
        role: dto.role,
      });

      navigate("/");
    } else {
      alert(await res.text());
    }
  }

  return (
    <div className={styles.container}>
      <form className={styles.login} onSubmit={handleLogin}>
        <input name="username" placeholder="Username" />
        <input name="password" type="password" placeholder="Password" />
        <button type="submit">Login</button>
      </form>
    </div>
  );
}