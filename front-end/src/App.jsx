import { Text } from '@nextui-org/react';
import { Routes, Route, Outlet, Navigate } from 'react-router-dom';
import NoRequireAuth from './authRoute/NoRequireAuth';
import RequireAuth from './authRoute/RequireAuth';
import LoginScreen from './screens/LoginScreen';
import NotFoundScreen from './screens/NotFoundScreen/NotFoundScreen';
import { withNamespaces } from 'react-i18next';

const App = ({t}) => {
  return (
    <Routes>
      <Route path={'/login'} element={<NoRequireAuth><LoginScreen /></NoRequireAuth>} />
      <Route path={'/'} element={<RequireAuth />}/>
      {/* Routers for role student */}
      <Route path={'/student'} element={<RequireAuth role={'student'}/>}>
        <Route index element={<Navigate to="/student/first-page" replace />} />
        <Route path='/student/first-page' element={<p>{t('welcome')}</p>} />
      </Route>

      {/* Routers for role teacher */}
      <Route path={'/teacher'}>
        <Route index element={<RequireAuth role={'teacher'}><Text p>Teacher</Text></RequireAuth>} />
      </Route>

      {/* Routers for role sro */}
      <Route path={'/sro'}>
        <Route index element={<RequireAuth role={'sro'}><Text p>SRO</Text></RequireAuth>} />
      </Route>

      {/* Routers for role admin */}
      <Route path={'/admin'}>
        <Route index element={<RequireAuth role={'admin'}><Text p>Admin</Text></RequireAuth>} />
      </Route>
      <Route path="*" element={<NotFoundScreen />} />
    </Routes>
  );
};

export default withNamespaces()(App);
