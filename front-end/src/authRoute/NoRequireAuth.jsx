import { useLocation, Navigate } from "react-router-dom";

function NoRequireAuth({ children }) {
  let location = useLocation();

    console.log("NoRequireAuth");
  if (
    localStorage.getItem("access_token") &&
    localStorage.getItem("refresh_token") &&
    localStorage.getItem("role")
  ) {
    return <Navigate to="/" state={{ from: location }} replace />;
  }

  localStorage.removeItem("access_token");
  localStorage.removeItem("refresh_token");
  localStorage.removeItem("role");
  return children;
}

export default NoRequireAuth;