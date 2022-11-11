import classes from './ThirdLayout.module.css';
import Logo from '../../../images/logo_1.webp';
import { NavLink } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { IoHome } from 'react-icons/io5';
import { Badge, Dropdown, Spacer, Text, User } from '@nextui-org/react';
import { useNavigate } from 'react-router-dom';
import FetchApi from '../../../apis/FetchApi';
import { UserApis } from '../../../apis/ListApi';
import { Fragment } from 'react';
import { FaPowerOff, FaDoorOpen } from 'react-icons/fa';
import { MdManageAccounts } from 'react-icons/md';
import { ImBooks, ImBook, ImLibrary } from 'react-icons/im';
import {IoPeopleSharp} from 'react-icons/io5';
import {
  MdMeetingRoom,
  MdSupervisedUserCircle,
  MdMenuBook,
} from 'react-icons/md';
import { Toaster } from 'react-hot-toast';

const ROLE = {
  1: 'Admin',
  2: 'SRO',
  3: 'Teacher',
  4: 'Student',
};

const menu = {
  admin: [
    {
      key: '1',
      label: 'Thông tin các cơ sở',
      icon: <IoHome size={16}/>,
      url: '/admin/center',
    },
    {
      key: '2',
      label: 'Tài khoản giáo viên',
      icon: <MdManageAccounts size={20}/>,
      url: '/admin/account/teacher',
    },
    {
      key: '3',
      label: 'Tài khoản SRO',
      icon: <MdSupervisedUserCircle size={20}/>,
      url: '/admin/account/sro',
    },
    {
      key: '4',
      label: 'Chương Trình Học',
      icon: <ImLibrary size={18}/>,
      url: '/admin/manage-course/course-family',
    },
    {
      key: '5',
      label: 'Thông Tin Khóa Học',
      icon: <ImBook size={18}/>,
      url: '/admin/manage-course/courses',
    },
    {
      key: '6',
      label: 'Thông Tin Môn Học',
      icon: <MdMenuBook size={18}/>,
      url: '/admin/manage-course/module',
    },

    {
      key: '7',
      label: 'Quản lý phòng học',
      icon: <FaDoorOpen size={18}/>,
      url: '/admin/room',
    },
  ],
  sro: [
    {
      key: '8',
      label: 'Quản lý lớp học',
      icon: <IoHome size={16}/>,
      url: '/sro/manage-class',
    },
    {
      key: '9',
      label: 'Quản lý học viên',
      icon: <IoPeopleSharp size={16}/>,
      url: '/sro/manage/student',
    },
  ],
  teacher: [],
  student: [],
};

const ThirdLayout = ({children}) => {
  const [dataUser, setDataUser] = useState({});
  const [isOpenDropdown, setIsOpenDropdown] = useState(true);

  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('role');
    navigate('/login');
  };

  useEffect(() => {
    FetchApi(UserApis.getInformationUser, null, null, null)
      .then((res) => {
        const data = res.data;
        setDataUser({
          name: data.first_name + ' ' + data.last_name,
          role: ROLE[data.role.id],
          avatar: data.avatar,
          emailOrganization: data.email_organization,
          mobilePhone: data.mobile_phone,
          centerName: data.center_name,
        });
      })
      .catch(() => {
        handleLogout();
      });
  }, []);

  return (
    <div className={classes.foundation}>
      <Toaster containerStyle={{
        zIndex: 99999999,
      }}/>
      <div className={classes.main}>
        <div className={classes.leftSide}>
          <div className={classes.logo}>
            <img src={Logo} alt="logo"/>
          </div>
          <div className={classes.menu}>
            {menu[localStorage.getItem('role')].map((item) => (
              <NavLink
                key={item.key}
                to={item.url}
                className={({isActive}) => {
                  return isActive
                    ? `${classes.menuItem} ${classes.menuItemActive}`
                    : classes.menuItem;
                }}
              >
                <div className={classes.menuContent}>
                  <div className={classes.menuItemIcon}>{item.icon}</div>
                  <div className={classes.menuItemLabel}>{item.label}</div>
                </div>
              </NavLink>
            ))}
          </div>
        </div>
        <div className={classes.rightSide}>
          <div className={classes.header}>
            {dataUser?.role !== 'Admin' && <Badge variant={'flat'} color="success" size="md">
              {dataUser.centerName}
            </Badge>}
            <Dropdown placement="bottom">
              <Dropdown.Trigger css={{cursor: 'pointer'}}>
                <User
                  size="lg"
                  src={dataUser.avatar}
                  color={
                    dataUser.role === 'Admin'
                      ? 'error'
                      : dataUser.role === 'SRO'
                        ? 'warning'
                        : dataUser.role === 'Teacher'
                          ? 'secondary'
                          : 'success'
                  }
                  bordered
                  squared
                  name={dataUser.name}
                  description={
                    <Fragment>
                      <b>{dataUser.role}</b> của @Aptech
                    </Fragment>
                  }
                />
              </Dropdown.Trigger>
              <Dropdown.Menu
                onAction={(e) => {
                  switch (e) {
                    case 'logout':
                      handleLogout();
                      break;
                    default:
                      break;
                  }
                }}
                color="secondary"
                aria-label="Avatar Actions"
              >
                <Dropdown.Item
                  key="logout"
                  color="error"
                  icon={<FaPowerOff/>}
                  description={'Đăng xuất tài khoản'}
                >
                  Đăng xuất
                </Dropdown.Item>
              </Dropdown.Menu>
            </Dropdown>
          </div>
          <div className={classes.foundationContain}>{children}</div>
        </div>
      </div>
    </div>
  );
};

export default ThirdLayout;
