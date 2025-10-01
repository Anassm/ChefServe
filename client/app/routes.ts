import { type RouteConfig, index, route } from "@react-router/dev/routes";

export default [
  index("routes/login/Login.tsx"),
  route("admin", "routes/adminDashboard/AdminDashboard.tsx"),
] satisfies RouteConfig;
