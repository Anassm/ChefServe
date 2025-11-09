import {
  type RouteConfig,
  index,
  layout,
  route,
} from "@react-router/dev/routes";

export default [
  layout("components/MainLayout/MainLayout.tsx", [
    index("routes/dashboard/Dashboard.tsx"),
    route("/:parentpath/*", "routes/dashboard/Dashboard.tsx", {
      id: "dashboard-with-parentpath",
    }),
  ]),

  route("login", "routes/login/Login.tsx"),
  route("logout", "routes/logout.tsx"),
] satisfies RouteConfig;
