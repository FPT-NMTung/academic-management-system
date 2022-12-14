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
      <div className={classes.back1}/>
      <div className={classes.back2}/>
      <div className={classes.back3}/>
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
                ????ng nh???p
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
                  Ch??o m???ng b???n ?????n v???i h??? th???ng qu???n l?? ????o t???o c???a Aptech, vui
                  l??ng ????ng nh???p ????? ti???p t???c s??? d???ng h??? th???ng. N???u g???p b???t k???
                  v???n ????? n??o, vui l??ng li??n h??? v???i ng?????i qu???n l?? l???p c???a b???n.
                </i>
              </Text>
              <Spacer y={1.5} />
              <GoogleLogin
                fetchBasicProfile={false}
                clientId="518989199582-9ul4cerv67mgmg777fpl0jl4lb4nsnji.apps.googleusercontent.com"
                isSignedIn={false}
                render={(propsButton) => {
                  return (
                    <Button
                      css={{
                        width: '220px',
                        margin: '0 auto',
                      }}
                      icon={<FcGoogle />}
                      auto
                      disabled={propsButton.disabled}
                      flat
                      color={isLoginFail ? 'error' : 'primary'}
                      onPress={() => {
                        setIsLogining(true);
                        setIsLoginFail(false);
                        console.log(propsButton);
                        propsButton.onClick();
                      }}
                    >
                      {!isLogining && !isLoginFail && '????ng nh???p b???ng Google'}
                      {isLogining && '??ang ????ng nh???p...'}
                      {isLoginFail && '????ng nh???p th???t b???i'}
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
        ?? {new Date().getFullYear()} Aptech Computer Education. B???n quy???n thu???c
        v??? Aptech Computer Education.
      </Text>
    </div>
  );
};

export default Login;
