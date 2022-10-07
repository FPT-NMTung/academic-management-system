import { Modal, Text } from '@nextui-org/react';
import { Button, Form, Select, Spin, Table, Input, InputNumber } from 'antd';
import { CenterApis, RoomApis, RoomTypeApis } from '../../apis/ListApi';
import FetchApi from '../../apis/FetchApi';
import { useState, useEffect } from 'react';
import { ErrorCodeApi } from '../../apis/ErrorCodeApi';

const CreateRoom = ({ onCreateRoomSuccess }) => {
  const [listCenters, setListCenters] = useState([]);
  const [listTypeRoom, setListTypeRoom] = useState([]);

  const [isGetListCenter, setIsGetListCenter] = useState(false);
  const [isGetListTypeRoom, setIsGetListTypeRoom] = useState(false);

  const [isCreatingRoom, setIsCreatingRoom] = useState(false);
  const [isFailedCreateRoom, setIsFailedCreateRoom] = useState(false);

  const [errorValue, setErrorValue] = useState(undefined);

  const getListCenter = () => {
    setIsGetListCenter(true);
    FetchApi(CenterApis.getAllCenter).then((res) => {
      const data = res.data.map((e) => {
        return {
          key: e.id,
          ...e,
        };
      });
      setListCenters(data);
      setIsGetListCenter(false);
    });
  };

  const getListTypeRoom = () => {
    setIsGetListTypeRoom(true);
    FetchApi(RoomTypeApis.getAllRoomType).then((res) => {
      const data = res.data.map((e) => {
        return {
          key: e.id,
          ...e,
        };
      });
      setListTypeRoom(data);
      setIsGetListTypeRoom(false);
    });
  };

  useEffect(() => {
    getListCenter();
    getListTypeRoom();
  }, []);

  const handleSubmitForm = (e) => {
    const body = {
      center_id: e.center_id,
      room_type_id: e.room_type_id,
      name: e.name.trim(),
      capacity: e.capacity,
    };

    setIsCreatingRoom(true);
    setErrorValue(undefined);
    FetchApi(RoomApis.createRoom, body, null, null)
      .then((res) => {
        setIsCreatingRoom(false);
        onCreateRoomSuccess();
      })
      .catch((err) => {
        setIsCreatingRoom(false);
        setIsFailedCreateRoom(true);
        setErrorValue(ErrorCodeApi[err.type_error]);
      });
  };

  return (
    <Form
      labelCol={{ span: 6 }}
      wrapperCol={{ span: 16 }}
      initialValues={{
        centerId: null,
        name: null,
        type_room: null,
        capacity: 24,
      }}
      onFinish={handleSubmitForm}
    >
      <Form.Item
        name={'center_id'}
        label="Cơ sở"
        wrapperCol={{ span: 10 }}
        rules={[
          {
            required: true,
            message: 'Hãy chọn cơ sở',
          },
        ]}
      >
        <Select
          placeholder="Chọn cơ sở"
          style={{ width: '100%' }}
          dropdownStyle={{ zIndex: 9999 }}
          loading={isGetListCenter}
        >
          {listCenters.map((e) => (
            <Select.Option key={e.key} value={e.id}>
              {e.name}
            </Select.Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item
        name={'name'}
        label="Tên"
        rules={[
          {
            required: true,
            message: 'Hãy nhập tên phòng',
          },
          {
            validator: (_, value) => {
              if (value.trim().length >= 2) {
                return Promise.resolve();
              }
              return Promise.reject(
                new Error('Tên phòng phải có ít nhất 2 ký tự')
              );
            },
          },
          {
            whitespace: true,
            message: 'Tên phòng không được chứa khoảng trắng',
          },
        ]}
      >
        <Input placeholder="Nhập tên phòng" />
      </Form.Item>
      <Form.Item
        name={'room_type_id'}
        label="Loại phòng"
        wrapperCol={{ span: 10 }}
        rules={[
          {
            required: true,
            message: 'Hãy chọn loại phòng',
          },
        ]}
      >
        <Select
          placeholder="Chọn loại phòng"
          style={{ width: '100%' }}
          dropdownStyle={{ zIndex: 9999 }}
          loading={isGetListTypeRoom}
        >
          {listTypeRoom.map((e) => (
            <Select.Option key={e.key} value={e.id}>
              {e.value}
            </Select.Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item
        name={'capacity'}
        label="Sức chứa"
        rules={[
          {
            required: true,
            message: 'Hãy nhập sức chứa',
          },
        ]}
      >
        <InputNumber
          min={20}
          max={100}
          style={{ width: '100px' }}
          placeholder="24"
        />
      </Form.Item>
      <Form.Item wrapperCol={{ offset: 6, span: 99 }}>
        <Button type="primary" htmlType="submit">
          Thêm
        </Button>
      </Form.Item>
      {!isCreatingRoom && isFailedCreateRoom && (
        <Text
          color="error"
          size={15}
          p
          css={{ width: '100%', textAlign: 'center' }}
        >
          {errorValue}, hãy thử lại!
        </Text>
      )}
    </Form>
  );
};

export default CreateRoom;
