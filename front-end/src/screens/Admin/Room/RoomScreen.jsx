import {
  Badge,
  Card,
  Grid,
  Modal,
  Spacer,
  Text,
  Button,
  Table,
  Loading,
  Switch,
} from '@nextui-org/react';
import { Form, Select, Spin } from 'antd';
import { Fragment, useEffect } from 'react';
import { useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { RoomApis } from '../../../apis/ListApi';
import { MdEdit } from 'react-icons/md';
import { CenterApis } from '../../../apis/ListApi';
import classes from './RoomScreen.module.css';
import CreateRoom from '../../../components/CreateRoom/CreateRoom';
import UpdateRoom from '../../../components/UpdateRoom/UpdateRoom';
import { FaPen } from 'react-icons/fa';
import toast from 'react-hot-toast';
import ChangeAvatar from '../../../components/ChangeAvatar/ChangeAvatar';

const RoomScreen = () => {
  const [listRooms, setListRooms] = useState([]);
  const [listCenters, setListCenters] = useState([]);
  const [isFetchingTable, setIsFetchingTable] = useState(false);
  const [selectRoomId, setSelectRoomId] = useState(undefined);
  const [isOpenCreateRoom, setIsOpenCreateRoom] = useState(false);

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
    getListRooms(value);
  };

  const handleCreateRoomSuccess = () => {
    setIsOpenCreateRoom(false);
    getListRooms();
  };

  const handleUpdateSuccess = () => {
    setSelectRoomId(undefined);
    getListRooms();
  };

  const handleChangeStatus = (data) => {
    toast.promise(
      FetchApi(RoomApis.changeActiveRoom, null, null, [String(data.id)]),
      {
        loading: 'Đang thay đổi ...',
        success: () => {
          const temp = listRooms.find((e) => e.id === data.id);
          temp.is_active = !temp.is_active;
          setListRooms([...listRooms]);

          return 'Thay đổi trạng thái thành công';
        },
        error: () => {
          return 'Thay đổi trạng thái thất bại';
        },
      }
    );
  };

  const renderTypeRoom = (id) => {
    return (
      <Badge variant="flat" color={id === 1 ? 'success' : 'warning'}>
        {id === 1 ? 'Lý thuyết' : 'Thực hành'}
      </Badge>
    );
  };

  useEffect(() => {
    getListRooms(null);
    getListCenter();
  }, []);

  return (
    <Fragment>
      <ChangeAvatar/>
      <Modal open={isOpenCreateRoom} blur closeButton={true} width={500}>
        <Modal.Header>
          <Text b size={14}>
            Thêm phòng mới
          </Text>
        </Modal.Header>
        <Modal.Body>
          <CreateRoom onCreateRoomSuccess={handleCreateRoomSuccess} />
        </Modal.Body>
      </Modal>
      <Modal
        open={selectRoomId !== undefined}
        onClose={() => {
          setSelectRoomId(undefined);
        }}
        blur
        closeButton={true}
        width={500}
      >
        <Modal.Header>
          <Text b size={14}>
            Chỉnh sửa thông tin phòng
          </Text>
        </Modal.Header>
        <Modal.Body>
          <UpdateRoom
            data={listRooms.find((e) => e.id === selectRoomId)}
            onUpdateSuccess={handleUpdateSuccess}
          />
        </Modal.Body>
      </Modal>
      <Grid.Container justify="center">
        <Grid
          xs={8}
          css={{
            flexDirection: 'column',
          }}
        >
          <Card
            variant="bordered"
            css={{
              minHeight: '300px',
            }}
          >
            <Card.Header>
              <Grid.Container>
                <Grid xs={4}>
                  <Select
                    showSearch
                    style={{ width: 200 }}
                    placeholder="Chọn cơ sở"
                    disabled={listCenters.length === 0}
                    onChange={handleChangeCenter}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listCenters.map((e) => (
                      <Select.Option key={e.key} value={e.id}>
                        {e.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Grid>
                <Grid xs={4}>
                  <Text
                    size={14}
                    b
                    css={{ textAlign: 'center', width: '100%' }}
                  >
                    Danh sách các phòng
                  </Text>
                </Grid>
                <Grid xs={4} justify="flex-end">
                  <Button
                    auto
                    flat
                    type="primary"
                    onClick={setIsOpenCreateRoom}
                  >
                    + Tạo phòng mới
                  </Button>
                </Grid>
              </Grid.Container>
            </Card.Header>
            {isFetchingTable && <Loading />}
            {!isFetchingTable && (
              <Table>
                <Table.Header>
                  <Table.Column>STT</Table.Column>
                  <Table.Column>Tên</Table.Column>
                  <Table.Column>Cơ sở</Table.Column>
                  <Table.Column>Loại phòng</Table.Column>
                  <Table.Column>Sức chứa</Table.Column>
                  <Table.Column>Tình trạng</Table.Column>
                  <Table.Column>Trạng thái</Table.Column>
                  <Table.Column width={30}></Table.Column>
                </Table.Header>
                <Table.Body>
                  {listRooms.map((e, index) => (
                    <Table.Row key={index}>
                      <Table.Cell>{index + 1}</Table.Cell>
                      <Table.Cell>{e.name}</Table.Cell>
                      <Table.Cell>{e.center_name}</Table.Cell>
                      <Table.Cell>{renderTypeRoom(e.room_type.id)}</Table.Cell>
                      <Table.Cell>{e.capacity}</Table.Cell>
                      <Table.Cell>
                        <Badge variant={'flat'} color={'error'}>
                          Bận
                        </Badge>
                      </Table.Cell>
                      <Table.Cell>
                        <Switch
                          size={'xs'}
                          onChange={() => handleChangeStatus(e)}
                          checked={e.is_active}
                          color={'success'}
                        />
                      </Table.Cell>
                      <Table.Cell>
                        <FaPen
                          size={14}
                          color="5EA2EF"
                          style={{ cursor: 'pointer' }}
                          onClick={() => {
                            setSelectRoomId(e.id);
                          }}
                        />
                      </Table.Cell>
                    </Table.Row>
                  ))}
                </Table.Body>
                <Table.Pagination
                  shadow
                  noMargin
                  align="center"
                  rowsPerPage={9}
                />
              </Table>
            )}
          </Card>
        </Grid>
      </Grid.Container>
    </Fragment>
  );
};

export default RoomScreen;
