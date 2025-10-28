import { redirect, type ActionFunctionArgs } from "react-router";

export async function action({ request }: ActionFunctionArgs) {
  console.log("Logging out user...");
  const res = await fetch("http://localhost:5175/api/auth/logout", {
    method: "POST",
    credentials: "include",
  });
  if (!res.ok) {
    console.error("Logout failed:", res.statusText);
  }
  return redirect("/login");
}
