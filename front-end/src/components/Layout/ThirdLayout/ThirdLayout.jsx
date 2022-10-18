import classes from './ThirdLayout.module.css';
import Logo from '../../../images/logo_1.webp';
import { NavLink } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { IoHome } from 'react-icons/io5';
import { Dropdown, User } from '@nextui-org/react';
import { useNavigate } from 'react-router-dom';
import FetchApi from '../../../apis/FetchApi';
import { UserApis } from '../../../apis/ListApi';

const ROLE = {
  1: 'Admin',
  2: 'SRO',
  3: 'Teacher',
  4: 'Student',
};

const menu = [
  {
    key: '1',
    label: 'Quản lý cơ sở',
    icon: <IoHome size={16} />,
    url: '/admin/center',
  },
  {
    key: '2',
    label: 'Quản lý tài khoản',
    icon: <IoHome size={16} />,
    url: '/admin/account/teacher',
  },
  {
    key: '3',
    label: 'Quản lý khoá học',
    icon: <IoHome size={16} />,
    url: '/admin/manage-course/course-family',
  },
];

const ThirdLayout = ({ children }) => {
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
        });
      })
      .catch(() => {
        handleLogout();
      });
  }, []);

  return (
    <div className={classes.foundation}>
      <div className={classes.main}>
        <div className={classes.leftSide}>
          <div className={classes.logo}>
            <img src={Logo} alt="logo" />
          </div>
          <div className={classes.menu}>
            {menu.map((item) => (
              <NavLink
                to={item.url}
                className={({ isActive }) => {
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
            <Dropdown placement="bottom-left">
              <Dropdown.Trigger css={{ cursor: 'pointer' }}>
                <User
                  size="lg"
                  src="https://i.pravatar.cc/150?u=a092581d4ef9026700d"
                  color="error"
                  bordered
                  squared
                  name={dataUser.name}
                  description={dataUser.role}
                />
              </Dropdown.Trigger>
              <Dropdown.Menu color="secondary" aria-label="Avatar Actions">
                <Dropdown.Item key="logout" color="error">
                  Log Out
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
