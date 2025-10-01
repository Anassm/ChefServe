import type { Route } from "../../+types/root";
import styles from "./login.module.css";
import { Form } from "react-router";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "ChefServe | Login" },
    { name: "description", content: "Login to access" },
  ];
}

export default function Login() {
  return (
    <div className={styles.container}>
      <Form className={styles.login} action="/dashboard" method="post">
        <h2>Login</h2>
        <input name="username" type="text" placeholder="Dikke boktor" />
        <input name="password" type="password" placeholder="password123" />
        <button type="submit">Submit</button>
      </Form>
    </div>
  );
}
