import type { Route } from "../../+types/root";
import styles from "./login.module.css";
import { Form, redirect, type ActionFunctionArgs } from "react-router";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "ChefServe | Login" },
    { name: "description", content: "Login to access" },
  ];
}

export async function action({ request }: ActionFunctionArgs) {
  const loginCreds = await request.formData();
  const username = loginCreds.get("username") as string;
  const password = loginCreds.get("password") as string;

  const response = await fetch("http://localhost:5175/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
    credentials: "include",
  });

  if (response.ok) {
    return redirect("/dashboard");
  } else {
    return { error: (await response.text()) || "Invalid username or password" };
  }
}

export default function Login() {
  return (
    <div className={styles.container}>
      <Form className={styles.login} method="post">
        <h2>Login</h2>
        <input name="username" type="text" placeholder="Dikke boktor" />
        <input name="password" type="password" placeholder="password123" />
        <button type="submit">Submit</button>
      </Form>
    </div>
  );
}
