import {
  type RouteConfig,
  index,
  layout,
  route,
} from "@react-router/dev/routes";

export default [
  layout("components/MainLayout/MainLayout.tsx", [
    index("routes/dashboard/Dashboard.tsx"),
    route("admin", "routes/dashboard/adminDashboard/AdminDashboard.tsx"),
    route("admin/overview", "routes/dashboard/adminDashboard/Overview.tsx"),
    route("admin/users", "routes/dashboard/adminDashboard/Users.tsx"),
    route("admin/files", "routes/dashboard/adminDashboard/Files.tsx"),
    route("admin/settings", "routes/dashboard/adminDashboard/Settings.tsx"),
    //route("/files/:parentpath*", "routes/dashboard/userDashboard/UserDashboard.tsx")
  ]),

  route("login", "routes/login/Login.tsx"),
  route("logout", "routes/logout.tsx"),
] satisfies RouteConfig;
