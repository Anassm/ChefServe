import {
  type RouteConfig,
  index,
  layout,
  route,
} from "@react-router/dev/routes";

export default [
  layout("components/MainLayout/MainLayout.tsx", [
    index("routes/dashboard/Dashboard.tsx"),
  ]),

  route("login", "routes/login/Login.tsx"),
] satisfies RouteConfig;
