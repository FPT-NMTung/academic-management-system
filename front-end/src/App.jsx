import { Text } from '@nextui-org/react';
import { Routes, Route, Outlet, Navigate } from 'react-router-dom';
import NoRequireAuth from './authRoute/NoRequireAuth';
import RequireAuth from './authRoute/RequireAuth';
import LoginScreen from './screens/LoginScreen';
import NotFoundScreen from './screens/NotFoundScreen/NotFoundScreen';
import FirstLayout from './components/Layout/FirstLayout/FirstLayout';
import Schedule from './screens/Student/ScheduleScreen/ScheduleScreen';
import SecondLayout from './components/Layout/SecondLayout/SecondLayout';
import ThirdLayout from './components/Layout/ThirdLayout/ThirdLayout';
import CenterScreen from './screens/Admin/Center/CenterScreen';
import RoomScreen from './screens/Admin/Room/RoomScreen';
import CourseFamily from './screens/Admin/Course Family/CourseFamily';
import Course from './screens/Admin/Course/Course';
import Module from './screens/Admin/Module/Module';
import SroScreen from './screens/Admin/Sro/SroScreen';
import SroDetail from './screens/Admin/Sro/SroDetail/SroDetail';
import SroCreate from './screens/Admin/Sro/SroCreate/SroCreate';
import ModuleUpdate from './components/ModuleUpdate/ModuleUpdate';
import TeacherScreen from './screens/Admin/Teacher/TeacherScreen';
import TeacherDetail from './screens/Admin/Teacher/TeacherDetail/TeacherDetail';
import TeacherCreate from './screens/Admin/Teacher/TeacherCreate/TeacherCreate';

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
        <Route index element={<ThirdLayout><RequireAuth role={'teacher'}><Text p>Teacher</Text></RequireAuth></ThirdLayout>} />
      </Route>

      {/* Routers for role sro */}
      <Route path={'/sro'}>
        <Route index element={<ThirdLayout><RequireAuth role={'sro'}><Text p>SRO</Text></RequireAuth></ThirdLayout>} />
      </Route>

      {/* Routers for role admin */}
      <Route path={'/admin'} element={<ThirdLayout><RequireAuth role={'admin'} /></ThirdLayout>} >
        <Route index element={<Navigate to="/admin/center"/>} />
        <Route path='/admin/center' element={<CenterScreen/>}/>
        <Route path="/admin/room" element={<RoomScreen />} />
        <Route path='/admin/manage-course/course-family' element={<CourseFamily/>}/>
        <Route path='/admin/manage-course/course' element={<Course/>}/>
        <Route path='/admin/manage-course/module' element={<Module/>}/>
        <Route path='/admin/manage-course/module/:id/update' element={<ModuleUpdate/>}/>
        <Route path='/admin/account/sro' element={<SroScreen />}/>
        <Route path='/admin/account/sro/:id' element={<SroDetail />}/>
        <Route path='/admin/account/sro/create' element={<SroCreate modeUpdate={false}/>}/>
        <Route path='/admin/account/sro/:id/update' element={<SroCreate modeUpdate={true}/>}/>
        <Route path='/admin/account/teacher' element={<TeacherScreen />}/>
        <Route path='/admin/account/teacher/:id' element={<TeacherDetail />}/>
        <Route path='/admin/account/teacher/create' element={<TeacherCreate modeUpdate={false}/>}/>
        <Route path='/admin/account/teacher/:id/update' element={<TeacherCreate modeUpdate={true}/>}/>
      </Route>
      <Route path="*" element={<NotFoundScreen />} />
    </Routes>
  );
};

export default App;
