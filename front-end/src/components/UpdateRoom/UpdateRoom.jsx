import { Modal, Text, Button, Loading } from '@nextui-org/react';
import { Form, Select, Spin, Table, Input, InputNumber } from 'antd';
import { CenterApis, RoomApis, RoomTypeApis } from '../../apis/ListApi';
import FetchApi from '../../apis/FetchApi';
import { useState, useEffect } from 'react';
import { ErrorCodeApi } from '../../apis/ErrorCodeApi';
import { Fragment } from 'react';
import { Validater } from '../../validater/Validater';
import toast from 'react-hot-toast';

const UpdateRoom = ({ data, onUpdateSuccess }) => {
  const [listCenters, setListCenters] = useState([]);
  const [listTypeRoom, setListTypeRoom] = useState([]);

  const [isGetListCenter, setIsGetListCenter] = useState(false);
  const [isGetListTypeRoom, setIsGetListTypeRoom] = useState(false);

  const [isCreatingRoom, setIsCreatingRoom] = useState(false);
  const [isFailedCreateRoom, setIsFailedCreateRoom] = useState(false);

  const [errorValue, setErrorValue] = useState(undefined);
  const [canDelete, setCanDelete] = useState(undefined);

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

  const checkCanDelete = () => {
    FetchApi(RoomApis.checkCanDeleteRoom, null, null, [String(data?.id)])
      .then((res) => {
        setCanDelete(res.data.can_delete);
      })
      .catch((err) => {
        setCanDelete(false);
      });
  };

  const handleDelete = () => {
    toast.promise(
      FetchApi(RoomApis.deleteRoom, null, null, [String(data?.id)]),
      {
        loading: '??ang x??a ph??ng...',
        success: (res) => {
          onUpdateSuccess();
          return 'X??a ph??ng th??nh c??ng';
        },
        error: (err) => {
          return 'X??a ph??ng th???t b???i';
        },
      }
    );
  };

  useEffect(() => {
    getListCenter();
    getListTypeRoom();
    checkCanDelete();
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

    toast.promise(FetchApi(RoomApis.updateRoom, body, null, [`${data?.id}`]), {
      loading: '??ang c???p nh???t ph??ng',
      success: () => {
        setIsCreatingRoom(false);
        onUpdateSuccess();
        return 'C???p nh???t ph??ng th??nh c??ng';
      },
      error: (err) => {
        setIsCreatingRoom(false);
        setIsFailedCreateRoom(true);
        return ErrorCodeApi[err.type_error];
      },
    });
  };

  return (
    <Fragment>
      <Form
        labelCol={{ span: 6 }}
        wrapperCol={{ span: 16 }}
        initialValues={{
          name: data?.name,
          room_type_id: data?.room_type.id,
          capacity: data?.capacity,
        }}
        onFinish={handleSubmitForm}
      >
        <Form.Item
          name={'name'}
          label="T??n"
          rules={[
            {
              required: true,
              message: 'H??y nh???p t??n ph??ng',
            },
            {
              validator: (_, value) => {
                if (value === null || value === undefined) {
                  return Promise.reject('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???');
                }
                if (Validater.isContaintSpecialCharacterForName(value.trim())) {
                  return Promise.reject(
                    'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                  );
                }
                if (value.trim().length < 1 || value.trim().length > 255) {
                  return Promise.reject(
                    new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                  );
                }
                return Promise.resolve();
              },
            },
            {
              whitespace: true,
              message: 'T??n ph??ng kh??ng ???????c ch???a kho???ng tr???ng',
            },
          ]}
        >
          <Input placeholder="Nh???p t??n ph??ng" />
        </Form.Item>
        <Form.Item
          name={'room_type_id'}
          label="Lo???i ph??ng"
          wrapperCol={{ span: 10 }}
          rules={[
            {
              required: true,
              message: 'H??y ch???n lo???i ph??ng',
            },
          ]}
        >
          <Select
            showSearch
            placeholder="Ch???n lo???i ph??ng"
            style={{ width: '100%' }}
            dropdownStyle={{ zIndex: 9999 }}
            loading={isGetListTypeRoom}
            filterOption={(input, option) =>
              option.children.toLowerCase().includes(input.toLowerCase())
            }
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
          label="S???c ch???a"
          rules={[
            {
              required: true,
              message: 'H??y nh???p s???c ch???a',
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
          <div
            style={{
              display: 'flex',
              gap: '10px',
            }}
          >
            <Button
              css={{
                width: '130px',
              }}
              flat
              auto
              type="primary"
              htmlType="submit"
            >
              {isCreatingRoom && <Loading size="xs" />}
              {!isCreatingRoom && 'C???p nh???t'}
            </Button>
            <Button
              flat
              auto
              disabled={canDelete}
              onPress={handleDelete}
              color="error"
            >
              Xo??
            </Button>
          </div>
        </Form.Item>
      </Form>
    </Fragment>
  );
};

export default UpdateRoom;
