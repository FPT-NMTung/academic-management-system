import { Text } from '@nextui-org/react';
import { Routes, Route, Navigate } from 'react-router-dom';
import NoRequireAuth from './authRoute/NoRequireAuth';
import RequireAuth from './authRoute/RequireAuth';
import LoginScreen from './screens/LoginScreen';
import NotFoundScreen from './screens/NotFoundScreen/NotFoundScreen';
import FirstLayout from './components/Layout/FirstLayout/FirstLayout';
import Schedule from './screens/Student/ScheduleScreen/ScheduleScreen';
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
import ManageClass from './screens/Sro/Manage Class/ManageClass';
import DetailClass from './screens/Sro/Manage Class/DetailClass/DetailClass';
import StudentScreen from './screens/Sro/Student/StudentScreen';
import ClassCreate from './screens/Sro/Manage Class/ClassCreate/ClassCreate';
import StudentDetail from './screens/Sro/Student/StudentDetail/StudentDetail';
import StudentUpdate from './screens/Sro/Student/StudentUpdate/StudentUpdate';
import AddStudentToClass from './screens/Sro/Manage Class/AddStudentToClass/AddStudentToClass';
import ManageSchedule from './screens/Sro/Manage Class/Schedule/ManageSchedule';
import ScheduleDetail from './screens/Sro/Manage Class/ScheduleDetail/ScheduleDetail';
import ScheduleEmpty from './screens/Sro/Manage Class/ScheduleEmpty/ScheduleEmpty';
import ManageDayOff from './screens/Sro/DayOff/ManageDayOff';

const App = () => {
  return (
    <Routes>
      <Route path={'/login'} element={<NoRequireAuth><LoginScreen /></NoRequireAuth>}/>
      <Route path={'/'} element={<RequireAuth />} />

      {/* Routers for role student */}
      <Route path={'/student'} element={<FirstLayout><RequireAuth role={'student'} /></FirstLayout>}>
        <Route index element={<Navigate to="/student/schedule" replace />} />
        <Route path="/student/schedule" element={<Schedule />} />
        <Route path="/student/attendance" element={<p>attendance</p>} />
        <Route path="/student/grade" element={<p>grade</p>} />
      </Route>

      {/* Routers for role teacher */}
      <Route path={'/teacher'}>
        <Route index element={<ThirdLayout><RequireAuth role={'teacher'}><Text p>Teacher</Text></RequireAuth></ThirdLayout>}/>
      </Route>

      {/* Routers for role sro */}
      <Route path={'/sro'} element={<ThirdLayout><RequireAuth role={'sro'} /></ThirdLayout>}>
        <Route index element={<Navigate to="/sro/manage-class" />} />
        <Route path="/sro/manage-class" element={<ManageClass />} />
        <Route path="/sro/manage-class/:id" element={<DetailClass />} />
        <Route path="/sro/manage-class/create" element={<ClassCreate modeUpdate={false} />} />
        <Route path="/sro/manage-class/:id/update" element={<ClassCreate modeUpdate={true} />} />
        <Route path="/sro/manage-class/:id/schedule" element={<ManageSchedule />} >
          <Route index element={<ScheduleEmpty />} />
          <Route path="/sro/manage-class/:id/schedule/module/:moduleId" element={<ScheduleDetail />} />
        </Route>
        <Route path="/sro/manage/student" element={<StudentScreen />} />
        <Route path="/sro/manage/student/:id" element={<StudentDetail />} />
        <Route path="/sro/manage/student/:id/update" element={<StudentUpdate />} />
        <Route path="/sro/manage-class/:id/add" element={<AddStudentToClass />} />
        {/* <Route path="/sro/manage/student/:id/update" element={<p>grade</p>} /> */}
        <Route path="/sro/manage/day-off" element={<ManageDayOff />} />
      </Route>

      {/* Routers for role admin */}
      <Route path={'/admin'} element={<ThirdLayout><RequireAuth role={'admin'} /></ThirdLayout>}>
        <Route index element={<Navigate to="/admin/center" />} />
        <Route path="/admin/center" element={<CenterScreen />} />
        <Route path="/admin/room" element={<RoomScreen />} />
        <Route path="/admin/manage-course/course-family" element={<CourseFamily />} />
        <Route path="/admin/manage-course/courses" element={<Course />} />
        <Route path="/admin/manage-course/module" element={<Module />} />
        <Route path="/admin/manage-course/module/:id/update" element={<ModuleUpdate />} />
        <Route path="/admin/account/sro" element={<SroScreen />} />
        <Route path="/admin/account/sro/:id" element={<SroDetail />} />
        <Route path="/admin/account/sro/create" element={<SroCreate modeUpdate={false} />} />
        <Route path="/admin/account/sro/:id/update" element={<SroCreate modeUpdate={true} />} />
        <Route path="/admin/account/teacher" element={<TeacherScreen />} />
        <Route path="/admin/account/teacher/:id" element={<TeacherDetail />} />
        <Route path="/admin/account/teacher/create" element={<TeacherCreate modeUpdate={false} />} />
        <Route path="/admin/account/teacher/:id/update" element={<TeacherCreate modeUpdate={true} />} />
      </Route>
      <Route path="*" element={<NotFoundScreen />} />
    </Routes>
  );
};

export default App;
