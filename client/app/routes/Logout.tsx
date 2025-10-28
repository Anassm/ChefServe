import { redirect, type ActionFunctionArgs } from "react-router";

export async function action({ request }: ActionFunctionArgs) {
  await fetch("http://localhost:5175/api/auth/logout", {
    method: "POST",
    credentials: "include",
  });

  localStorage.removeItem("authToken");

  return redirect("/login");
}
