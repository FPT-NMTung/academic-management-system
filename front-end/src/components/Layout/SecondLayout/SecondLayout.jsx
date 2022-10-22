import { Text } from '@nextui-org/react';
import { Layout, Menu, Dropdown, Avatar } from 'antd';
import Logo from '../../../images/logo_1.webp';
import classes from './SecondLayout.module.css';
import { FaPowerOff } from 'react-icons/fa';
import { BsPersonFill } from 'react-icons/bs';
import { useNavigate, useLocation, useMatch } from 'react-router-dom';
import MenuLayout from '../MenuLayout/MenuLayout';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { UserApis } from '../../../apis/ListApi';
import { Fragment } from 'react';
const { Sider } = Layout;

const ROLE = {
  1: 'Admin',
  2: 'SRO',
  3: 'Teacher',
  4: 'Student',
};

const SecondLayout = ({ children }) => {
  const [dataUser, setDataUser] = useState({});

  const navigate = useNavigate();
  const location = useLocation();

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

  const menuAccount = (
    <Menu
      items={[
        {
          key: '1',
          label: (
            <div>
              <Text p size={14}>
                <strong>{dataUser.role}:</strong> {dataUser.name}
              </Text>
            </div>
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
          backgroundColor: '#fff',
        }}
      >
        <div className={classes.containterLogo}>
          <img className={classes.logoMain} src={Logo} />
        </div>
        <MenuLayout />
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
            <Avatar
              src={dataUser.avatar}
              size={40}
              icon={<BsPersonFill size={20} />}
            />
          </Dropdown>
        </div>
        <div className={classes.content}>{children}</div>
      </Layout>
    </Layout>
  );
};

export default SecondLayout;
