import { Badge, Card, Grid, Spacer, Text } from '@nextui-org/react';
import { Button, Select, Spin, Table } from 'antd';
import { useEffect } from 'react';
import { useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { RoomApis } from '../../../apis/ListApi';
import { MdEdit } from 'react-icons/md';
import { CenterApis } from '../../../apis/ListApi';
import classes from './RoomScreen.module.css';

const RoomScreen = () => {
  const [listRooms, setListRooms] = useState([]);
  const [listCenters, setListCenters] = useState([]);
  const [isFetchingTable, setIsFetchingTable] = useState(false);

  const getListRooms = (centerId) => {
    let params = null;
    if (centerId !== null && centerId !== undefined) {
      params = {
        centerId: centerId,
      };
    }

    setIsFetchingTable(true);
    FetchApi(RoomApis.getAllRoom, null, params, null).then((res) => {
      const data = res.data.map((e, index) => {
        return {
          key: e.id,
          ...e,
          index: index + 1,
          roome_name_type: e.room_type.value,
          status_room: 'Đang sử dụng',
        };
      });
      setListRooms(data);
      setIsFetchingTable(false);
    });
  };

  const getListCenter = () => {
    FetchApi(CenterApis.getAllCenter).then((res) => {
      const data = res.data.map((e) => {
        return {
          key: e.id,
          ...e,
        };
      });
      setListCenters(data);
    });
  };

  const handleChangeCenter = (value) => {
    getListRooms(value)
  };

  useEffect(() => {
    getListRooms(null);
    getListCenter();
    getListCenter();
    getListCenter();
    getListCenter();
    getListCenter();
    getListCenter();
    getListCenter();
    getListCenter();
  }, []);

  return (
    <Grid.Container justify="center">
      <Grid
        xs={8}
        css={{
          flexDirection: 'column',
        }}
      >
        <Card>
          <Card.Body
            css={{
              display: 'flex',
              flexDirection: 'row',
              justifyContent: 'space-between',
            }}
          >
            <Select
              style={{ width: 200 }}
              placeholder="Chọn cơ sở"
              disabled={listCenters.length === 0}
              onChange={handleChangeCenter}
            >
              {listCenters.map((e) => (
                <Select.Option key={e.key} value={e.id}>
                  {e.name}
                </Select.Option>
              ))}
            </Select>
            <Button type="primary">Tạo phòng mới</Button>
          </Card.Body>
        </Card>
        <Spacer y={1} />
        <Card>
          <Card.Header>
            <Text size={14} b css={{ textAlign: 'center', width: '100%' }}>
              Danh sách các phòng
            </Text>
          </Card.Header>
          <Table
            loading={isFetchingTable}
            dataSource={listRooms}
            pagination={{ position: ['bottomCenter'], size: 'default' }}
            size={'middle'}
          >
            <Table.Column title="STT" dataIndex="index" />
            <Table.Column title="Tên" dataIndex="name" />
            <Table.Column title="Cơ sở" dataIndex="center_name" />
            <Table.Column title="Loại phòng" dataIndex="roome_name_type" />
            <Table.Column title="Sức chứa" dataIndex="capacity" />
            <Table.Column
              title="Trạng thái"
              dataIndex="status_room"
              render={() => {
                return (
                  <Badge
                    css={{
                      margin: '0',
                    }}
                    size="md"
                    color="error"
                    variant="flat"
                  >
                    Đang sử dụng
                  </Badge>
                );
              }}
            />
            <Table.Column
              title=""
              dataIndex="control"
              render={() => {
                return <MdEdit style={{ cursor: 'pointer' }} color="0a579f" />;
              }}
            />
          </Table>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default RoomScreen;
