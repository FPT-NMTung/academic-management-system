import { Divider, Spacer, Text, Tooltip } from '@nextui-org/react';
import classes from './FirstLayout.module.css';
import { HiUserCircle, HiCheckCircle, HiCalendar } from 'react-icons/hi';
import { RiShutDownLine, RiMedalFill } from 'react-icons/ri';
import { Link, useLocation, useMatches, useNavigate } from 'react-router-dom';
import { Fragment } from 'react';
import { PageHeader } from 'antd';
import { MdManageAccounts,MdFeedback } from 'react-icons/md';

const navItem = [
  {
    name: 'Thời khóa biểu',
    description: 'Xem thời khóa biểu của bạn',
    path: '/student/schedule',
    icon: <HiCalendar size={30} color={'#fff'} />,
    role: 'student',
  },
  {
    name: 'Điểm danh',
    description: 'Kiểm tra tình trạng điểm danh của bạn',
    path: '/student/attendance',
    icon: <HiCheckCircle size={30} color={'#fff'} />,
    role: 'student',
  },
  {
    name: 'Điểm',
    description: 'Xem điểm của bạn với từng môn học',
    path: '/student/grade',
    icon: <RiMedalFill size={30} color={'#fff'} />,
    role: 'student',
  },
  {
    name: 'Điểm',
    description: 'Xem điểm của bạn với từng môn học',
    path: '/student/grade',
    icon: <RiMedalFill size={30} color={'#fff'} />,
    role: 'teacher',
  },
  // {
  //   name: 'Ý kiến về việc giảng dậy',
  //   description: 'Đánh giá về việc giảng dậy của giáo viên',
  //   path: '/student/feedback',
  //   icon: <MdFeedback size={30} color={'#fff'} />,
  //   role: 'student',
  // },

];

const FirstLayout = ({ children }) => {
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('role');
    navigate('/');
  };

  return (
    <div className={classes.main}>
      <div className={classes.nav}>
        <div className={classes.contentNav}>
          <div className={classes.navItem}>
            <HiUserCircle size={30} color={'#fff'} />
            <Spacer y={1.8} />
            <div className={classes.navMenu}>
              {navItem
                .filter((item) => item.role === localStorage.getItem('role'))
                .map((item) => {
                  return (
                    <Fragment>
                      <Tooltip
                        placement="right"
                        color={'default'}
                        content={item.name}
                      >
                        <Link to={item.path} className={classes.navIcon}>
                          {item.icon}
                        </Link>
                      </Tooltip>
                      <Spacer y={0.5} />
                    </Fragment>
                  );
                })}
            </div>
          </div>
          <Tooltip placement="right" color={'default'} content={'Đăng xuất'}>
            <RiShutDownLine
              className={classes.iconLogout}
              onClick={handleLogout}
              size={28}
              color={'#fff'}
            />
          </Tooltip>
        </div>
      </div>
      <div className={classes.content}>
        <div className={classes.children}>
          <div className={classes.header}>
            <PageHeader
              title={
                navItem
                  .filter((item) => item.role === localStorage.getItem('role'))
                  .find((item) => item.path === location.pathname)?.name
              }
              subTitle={
                navItem
                  .filter((item) => item.role === localStorage.getItem('role'))
                  .find((item) => item.path === location.pathname)?.description
              }
            />
            <Divider />
          </div>
          <div className={classes.body}>{children}</div>
        </div>
        <div className={classes.copyright}>
          <Text p size={12}>
            © {new Date().getFullYear()} Aptech Computer Education. All rights
            reserved.
          </Text>
        </div>
      </div>
    </div>
  );
};

export default FirstLayout;
