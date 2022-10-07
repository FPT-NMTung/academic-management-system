import { Grid, Card, Image, Text, Spacer } from '@nextui-org/react';
import { useEffect, useState } from 'react';
import { BsFillPersonFill } from 'react-icons/bs';
import classes from './SroDetail.module.css';
import { useParams, useNavigate } from 'react-router-dom';
import FetchApi from '../../../../apis/FetchApi';
import { ManageSroApis } from '../../../../apis/ListApi';
import { Spin } from 'antd';

const SroDetail = () => {
  const [dataUser, setDataUser] = useState({});
  const [isLoading, setIsLoading] = useState(true);

  const { id } = useParams();
  const navigate = useNavigate();

  const getDataUser = () => {
    FetchApi(ManageSroApis.getDetailSro, null, null, [`${id}`])
      .then((res) => {
        setDataUser(res.data);
        setIsLoading(false);
      })
      .catch(() => {
        navigate('/404');
      });
  };

  useEffect(() => {
    getDataUser();
  }, []);

  return (
    <Grid.Container justify="center">
      <Grid sm={8}>
        {isLoading && (
          <div className={classes.loading}>
            <Spin />
          </div>
        )}
        {!isLoading && (
          <Grid.Container gap={2}>
            <Grid
              sm={3.5}
              css={{
                display: 'flex',
                alignItems: 'center',
                flexDirection: 'column',
                width: '100%',
              }}
            >
              <div className={classes.logo}>
                <BsFillPersonFill size={60} color="white" />
              </div>
              <Spacer y={1} />
              <Text h3 size={20} b>
                {dataUser.first_name} {dataUser.last_name}
              </Text>
              <Text p size={15}>{dataUser.mobile_phone}</Text>
              <Text p size={15}>{dataUser.email_organization}</Text>
            </Grid>
            <Grid sm={8.5}>
              <Card>
                <Card.Body></Card.Body>
              </Card>
            </Grid>
          </Grid.Container>
        )}
      </Grid>
    </Grid.Container>
  );
};

export default SroDetail;
