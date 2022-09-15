import { useLocation, Navigate, Outlet } from 'react-router-dom';

function RequireAuth({ children, role }) {
  let location = useLocation();

  if (
    !localStorage.getItem('access_token') ||
    !localStorage.getItem('refresh_token') ||
    !localStorage.getItem('role')
  ) {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('role');
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  const localRole = localStorage.getItem('role');

  if (role === undefined) {
    return <Navigate to={`/${localRole}`} state={{ from: location }} replace />;
  }

  if (role !== localRole) {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('role');
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet/>;
}

export default RequireAuth;
