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
          });
          navigate("/");
        }
      } catch (err) {
        console.error("Failed to check login:", err);
      }
    }
    checkLogin();
  }, [navigate, setUser]);

  async function handleLogin(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);

    const username = formData.get("username");
    const password = formData.get("password");

    const res = await fetch("http://localhost:5175/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, password }),
      credentials: "include", // stores cookie if backend sets it
    });

    if (res.ok) {
      const dto = await res.json();

      setUser({
        username: dto.username,
        firstname: dto.firstname,
        lastname: dto.lastname,
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