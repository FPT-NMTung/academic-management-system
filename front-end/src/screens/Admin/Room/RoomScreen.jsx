import { Card, Grid, Spacer, Text } from '@nextui-org/react';
import { Spin } from 'antd';
import { useEffect } from 'react';
import { useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { RoomApis } from '../../../apis/ListApi';
import classes from './RoomScreen.module.css';

const RoomScreen = () => {
  const [listRooms, setListRooms] = useState([]);
  const [isFetching, setIsFetching] = useState(false);

  const getListRooms = (centerId) => {
    let params = null;
    if (centerId === undefined) {
      params = {
        centerId: centerId,
      };
    }

    setIsFetching(true);
    FetchApi(RoomApis.getAllRoom, null, params, null).then((res) => {
      setListRooms(res.data);
      setIsFetching(false);
    });
  };

  useEffect(() => {
    getListRooms(null);
  }, []);

  return (
    <Grid.Container justify="center">
      <Grid xs={8}>
        {!isFetching && (
          <Grid.Container gap={2}>
            {listRooms.map((item) => {
              return (
                <Grid xs={3} key={item.id}>
                  <Card>
                    <Card.Body>
                      <Text size={14}><b>Tên: </b>{item.name}</Text>
                      <Text size={14}><b>Cơ sở: </b>{item.center_name}</Text>
                      <Text size={14}><b>Loại phòng: </b>{item.room_type.value}</Text>
                      <Spacer y={1} />
                      <Text size={14}><b>Giá: </b>{item.price}</Text>
                    </Card.Body>
                  </Card>
                </Grid>
              );
            })}
          </Grid.Container>
        )}
        {isFetching && (
          <div className={classes.loading}>
            <Spin />
          </div>
        )}
      </Grid>
    </Grid.Container>
  );
};

export default RoomScreen;
