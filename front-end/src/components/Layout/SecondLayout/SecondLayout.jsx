import { Layout, Menu } from 'antd';
import Logo from '../../../images/logo_1.webp';
import classes from './SecondLayout.module.css';
const { Sider } = Layout;

const SecondLayout = ({ children }) => {
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
        <Menu theme="dark" mode="inline" />
      </Sider>
      <Layout
        className="site-layout"
        style={{
          marginLeft: 250,
          position: 'relative',
        }}
      >
        <div className={classes.header}>Header</div>
        <div className={classes.content}>asdasd</div>
      </Layout>
    </Layout>
  );
};

export default SecondLayout;
