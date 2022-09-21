import { Card, Grid, Button, Text, Input, Spacer } from '@nextui-org/react';
import { Carousel } from 'antd';
import classes from './Login.module.css';
import { Fragment, useState } from 'react';
import FlagVn from '../../images/flag_vn.png';
import FlagUk from '../../images/flag_uk.png';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';

const contentStyle = {
  height: '400px',
  color: '#fff',
  textAlign: 'center',
  background: '#000',
};

const listImage = [
  'https://aptechvietnam.com.vn/sites/default/files/banner-pc_1_11.png',
  'https://aptechvietnam.com.vn/sites/default/files/banner-pc_90.png',
  'https://aptechvietnam.com.vn/sites/default/files/pc-t09.png',
];

const Login = () => {
  const [isForgotPassword, setIsForgotPassword] = useState(false);
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();

  const handleChangeLanguage = () => {
    const currentLanguage = i18n.language;
    i18n.changeLanguage(currentLanguage === 'vi' ? 'en' : 'vi');
    localStorage.setItem('lang', currentLanguage === 'vi' ? 'en' : 'vi');
  };

  const handleForgotPassword = () => {
    setIsForgotPassword(!isForgotPassword);
  };

  return (
    <div className={classes.main}>
      <div className={classes.language}>
        <img
          onClick={handleChangeLanguage}
          src={i18n.language !== 'vi' ? FlagVn : FlagUk}
          alt="flag"
        />
      </div>
      <Card
        css={{
          width: '1200px',
          height: '400px',
        }}
      >
        <Grid.Container css={{ height: '100%' }} justify="center">
          <Grid css={{ height: '100%' }} xs={7.5}>
            <div className={classes.contain}>
              <Carousel autoplay autoplaySpeed={5000}>
                {listImage.map((item, index) => (
                  <div key={index}>
                    <h3 style={contentStyle}>
                      <img className={classes.images} src={item} />
                    </h3>
                  </div>
                ))}
              </Carousel>
            </div>
          </Grid>
          <Grid xs={4.5}>
            <div className={classes.form}>
              <Text h3>
                {isForgotPassword ? t('forgot_title') : t('login_title')}
              </Text>
              <Spacer y={1.2} />
              <form className={classes.formInside}>
                <Input
                  label={t('login_email')}
                  required
                  type={'email'}
                  placeholder="example@domain.com"
                  css={{ width: '310px' }}
                />
                {!isForgotPassword && (
                  <Fragment>
                    <Spacer y={0.8} />
                    <Input.Password
                      label={t('login_password')}
                      required
                      css={{ width: '310px' }}
                      placeholder="********"
                    />
                  </Fragment>
                )}
                {isForgotPassword && (
                  <Fragment>
                    <Spacer y={0.8} />
                    <Text
                      size={13}
                      css={{ cursor: 'pointer', textAlign: 'center' }}
                      p
                    >
                      {t('forgot_content')}
                    </Text>
                    <Spacer y={0.8} />
                  </Fragment>
                )}
                <Text
                  css={{ cursor: 'pointer', textAlign: 'right' }}
                  p
                  size={13}
                  color={'primary'}
                  onClick={handleForgotPassword}
                >
                  {!isForgotPassword && t('login_forgot')}
                  {isForgotPassword && t('forgot_back')}
                </Text>
                <Spacer y={0.4} />
                <Text css={{ textAlign: 'center' }} p size={14} color={'error'}>
                  {!isForgotPassword ? t('login_error') : t('forgot_error')}
                </Text>
                <Spacer y={0.4} />
                <Button
                  css={{
                    width: '150px',
                    margin: '0 auto',
                  }}
                  auto
                  onClick={() => {
                    localStorage.setItem('access_token', '123456');
                    localStorage.setItem('refresh_token', '123456');
                    localStorage.setItem('role', 'student');
                    navigate('/');
                  }}
                >
                  {!isForgotPassword ? t('login_button') : t('forgot_button')}
                </Button>
              </form>
            </div>
          </Grid>
        </Grid.Container>
      </Card>
    </div>
  );
};

export default Login;
