import { Card, Grid, Button, Text, Input, Spacer } from '@nextui-org/react';
import classes from './Login.module.css';
import { Fragment, useEffect, useState } from 'react';
import { GoogleLogin } from 'react-google-login';
import { gapi } from 'gapi-script';
import { FcGoogle } from 'react-icons/fc';
import Logo from '../../images/logo_1.webp';
import FetchApi from '../../apis/FetchApi';
import { AuthApis } from '../../apis/ListApi';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const [isLogining, setIsLogining] = useState(false);
  const [isLoginFail, setIsLoginFail] = useState(false);

  const navigate = useNavigate();

  const handleLogin = (e) => {
    setIsLogining(true);
    setIsLoginFail(false);

    const tokenId = e.tokenId;
    const apiLogin = AuthApis.login;
    const body = {
      token_google: tokenId,
    };

    FetchApi(apiLogin, body)
      .then((res) => {
        setIsLogining(false);
        setIsLoginFail(false);

        const { access_token, refresh_token, role_id } = res.data;

        switch (role_id) {
          case 1:
            localStorage.setItem('role', 'admin');
            break;
          case 2:
            localStorage.setItem('role', 'sro');
            break;
          case 3:
            localStorage.setItem('role', 'teacher');
            break;
          case 4:
            localStorage.setItem('role', 'student');
            break;
          default:
            return;
        }

        localStorage.setItem('access_token', access_token);
        localStorage.setItem('refresh_token', refresh_token);

        navigate('/');
      })
      .catch((err) => {
        setIsLogining(false);
        setIsLoginFail(true);
      });
  };

  useEffect(() => {
    const initClient = () => {
      gapi.client.init({
        clientId:
          '518989199582-9ul4cerv67mgmg777fpl0jl4lb4nsnji.apps.googleusercontent.com',
        scope: '',
      });
    };
    window.gapi.load('client:auth2', initClient);
  }, []);

  useEffect(() => {
    if (isLoginFail) {
      setIsLoginFail(true);
      setTimeout(() => {
        setIsLoginFail(false);
      }, 1500);
    }
  }, [isLoginFail]);

  return (
    <div className={classes.main}>
      <img className={classes.logo} src={Logo} alt="logo" />
      <Spacer y={1} />
      <Card
        css={{
          width: '480px',
          height: '270px',
        }}
      >
        <Grid.Container css={{ height: '100%' }} justify="center">
          <Grid xs={12}>
            <div className={classes.form}>
              <Text h3 css={{ width: '100%', textAlign: 'center' }}>
                Đăng nhập
              </Text>
              <Spacer y={0.5} />
              <Text
                p
                size={12}
                css={{
                  width: '100%',
                  textAlign: 'center',
                  padding: '0 40px',
                }}
              >
                <i>
                  Chào mừng bạn đến với hệ thống quản lý đào tạo của Aptech, vui
                  lòng đăng nhập để tiếp tục sử dụng hệ thống. Nếu gặp bất kỳ
                  vấn đề nào, vui lòng liên hệ với người quản lý lớp của bạn.
                </i>
              </Text>
              <Spacer y={1.5} />
              <GoogleLogin
                clientId="518989199582-9ul4cerv67mgmg777fpl0jl4lb4nsnji.apps.googleusercontent.com"
                render={(propsButton) => {
                  return (
                    <Button
                      css={{
                        width: '220px',
                        margin: '0 auto',
                      }}
                      icon={<FcGoogle />}
                      auto
                      flat
                      color={isLoginFail ? 'error' : 'primary'}
                      onPress={() => {
                        setIsLogining(true);
                        setIsLoginFail(false);
                        propsButton.onClick();
                      }}
                    >
                      {!isLogining && !isLoginFail && 'Đăng nhập bằng Google'}
                      {isLogining && 'Đang đăng nhập...'}
                      {isLoginFail && 'Đăng nhập thất bại'}
                    </Button>
                  );
                }}
                cookiePolicy="single_host_origin"
                ux_mode={'popup'}
                onSuccess={handleLogin}
                onFailure={(res) => {
                  setIsLogining(false);
                  setIsLoginFail(true);
                }}
              />
            </div>
          </Grid>
        </Grid.Container>
      </Card>
      <Spacer y={1} />
      <Text p size={10}>
        © {new Date().getFullYear()} Aptech Computer Education. Bản quyền thuộc
        về Aptech Computer Education.
      </Text>
    </div>
  );
};

export default Login;
