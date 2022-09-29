import { Text } from '@nextui-org/react';
import { Layout, Menu, Dropdown, Avatar } from 'antd';
import Logo from '../../../images/logo_1.webp';
import classes from './SecondLayout.module.css';
import { FaPowerOff } from 'react-icons/fa';
import { BsPersonFill } from 'react-icons/bs';
import { Fragment } from 'react';
import { useNavigate, useLocation, useMatch } from 'react-router-dom';
import MenuLayout from '../MenuLayout/MenuLayout';
const { Sider } = Layout;

const SecondLayout = ({ children }) => {
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('role');
    navigate('/login');
  };

  const menuAccount = (
    <Menu
      items={[
        {
          key: '1',
          label: (
            <Text p size={14}>
              <strong>Admin:</strong> Nguyễn Mạnh Tùng
            </Text>
          ),
        },
        {
          type: 'divider',
        },
        {
          key: '2',
          label: (
            <div className={classes.menuItemLayout}>
              <FaPowerOff />
              <Text
                onClick={handleLogout}
                css={{
                  paddingLeft: 10,
                }}
                p
                size={14}
              >
                Đăng xuất
              </Text>
            </div>
          ),
        },
      ]}
    />
  );

  return (
    <Layout hasSider>
      <Sider
        width={250}
        style={{
          overflow: 'auto',
          height: '100vh',
          position: 'fixed',
          left: 0,
          top: 0,
          bottom: 0,
        }}
      >
        <div className={classes.containterLogo}>
          <img className={classes.logoMain} src={Logo} />
        </div>
        <MenuLayout/>
      </Sider>
      <Layout
        className="site-layout"
        style={{
          marginLeft: 250,
          position: 'relative',
        }}
      >
        <div className={classes.header}>
          <Text p color="#f0f2f5">
            <strong>Academic Management System</strong>
          </Text>
          <Dropdown
            overlay={menuAccount}
            placement="bottomRight"
            arrow
            trigger={['click']}
          >
            <Avatar size={40} icon={<BsPersonFill size={20} />} />
          </Dropdown>
        </div>
        <div className={classes.content}>{children}</div>
      </Layout>
    </Layout>
  );
};

export default SecondLayout;
