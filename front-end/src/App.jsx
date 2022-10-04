import { Text } from '@nextui-org/react';
import { Routes, Route, Outlet, Navigate } from 'react-router-dom';
import NoRequireAuth from './authRoute/NoRequireAuth';
import RequireAuth from './authRoute/RequireAuth';
import LoginScreen from './screens/LoginScreen';
import NotFoundScreen from './screens/NotFoundScreen/NotFoundScreen';
import FirstLayout from './components/Layout/FirstLayout/FirstLayout';
import Schedule from './screens/Student/ScheduleScreen/ScheduleScreen';
import SecondLayout from './components/Layout/SecondLayout/SecondLayout';
import CenterScreen from './screens/Admin/Center/CenterScreen';
import CourseFamily from './screens/Admin/Course Family/CourseFamily';
import Course from './screens/Admin/Course/Course';
import Module from './screens/Admin/Module/Module';

const App = () => {
  return (
    <Routes>
      <Route path={'/login'} element={<NoRequireAuth><LoginScreen/></NoRequireAuth>} />
      <Route path={'/'} element={<RequireAuth />}/>

      {/* Routers for role student */}
      <Route path={'/student'} element={<FirstLayout><RequireAuth role={'student'}/></FirstLayout>}>
        <Route index element={<Navigate to="/student/schedule" replace />} />
        <Route path='/student/schedule' element={<Schedule/>} />
        <Route path='/student/attendance' element={<p>attendance</p>} />
        <Route path='/student/grade' element={<p>grade</p>}/>
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
      <Route path={'/admin'} element={<SecondLayout><RequireAuth role={'admin'} /></SecondLayout>} >
        <Route index element={<p>Trang chủ</p>} />
        <Route path='/admin/center' element={<CenterScreen/>}/>
        <Route path='/admin/account/sro' element={<p>Quản lý SRO</p>}/>
        <Route path='/admin/account/sro/:id' element={<p>Quản lý SRO detail</p>}/>
        <Route path='/admin/manage-course/course-family' element={<CourseFamily/>}/>
        <Route path='/admin/manage-course/course' element={<Course/>}/>
        <Route path='/admin/manage-course/module' element={<Module/>}/>
      </Route>
      <Route path="*" element={<NotFoundScreen />} />
    </Routes>
  );
};

export default App;
