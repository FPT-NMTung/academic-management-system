import { Card, Grid, Spacer, Text } from '@nextui-org/react';
import { Spin, Table } from 'antd';
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
      const data = res.data.map((e) => {
        return {
          key: e.id,
          ...e,
          roome_name_type: e.room_type.value,
          status_room: 'Đang hoạt động',
        };
      })
      setListRooms(data);
      setIsFetching(false);
    });
  };

  useEffect(() => {
    getListRooms(null);
  }, []);

  return (
    <Grid.Container justify="center">
      <Grid xs={8}>
        <Card>
          <Card.Header>
            <Text size={14} b css={{ textAlign: 'center', width: '100%' }}>
              Danh sách các phòng
            </Text>
          </Card.Header>
          <Table
            loading={isFetching}
            dataSource={listRooms}
            pagination={{ position: ['bottomCenter'] }}
          >
            <Table.Column title="Tên" dataIndex="name" />
            <Table.Column title="Cơ sở" dataIndex="center_name" />
            <Table.Column title="Loại phòng" dataIndex="roome_name_type" />
            <Table.Column title="Sức chứa" dataIndex="capacity" />
            <Table.Column title="Trạng thái" dataIndex="status_room" />
            <Table.Column title="" dataIndex="control" render={() => {
              return <p>asdad</p>
            }}/>
          </Table>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default RoomScreen;
