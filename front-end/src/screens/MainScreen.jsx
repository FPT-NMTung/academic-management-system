import { Outlet, useNavigate } from 'react-router-dom';

const MainScreen = () => {
  const navigate = useNavigate();

  if (localStorage.getItem('role') === 'student') {
    navigate('/student');
  }

  if (localStorage.getItem('role') === 'teacher') {
    navigate('/teacher');
  }

  if (localStorage.getItem('role') === 'sro') {
    navigate('/sro');
  }

  if (localStorage.getItem('role') === 'admin') {
    navigate('/admin');
  }

  return (
    <div>
      <Outlet />
    </div>
  );
};

export default MainScreen;
