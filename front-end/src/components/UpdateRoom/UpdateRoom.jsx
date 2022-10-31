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
        loading: 'Đang xóa phòng...',
        success: (res) => {
          onUpdateSuccess();
          return 'Xóa phòng thành công';
        },
        error: (err) => {
          return 'Xóa phòng thất bại';
        },
      }
    )
  }

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
      loading: 'Đang cập nhật phòng',
      success: () => {
        setIsCreatingRoom(false);
        onUpdateSuccess();
        return 'Cập nhật phòng thành công';
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
          label="Tên"
          rules={[
            {
              required: true,
              message: 'Hãy nhập tên phòng',
            },
            {
              validator: (_, value) => {
                if (value === null || value === undefined) {
                  return Promise.reject('Trường này không được để trống');
                }
                if (Validater.isContaintSpecialCharacterForName(value.trim())) {
                  return Promise.reject(
                    'Trường này không được chứa ký tự đặc biệt'
                  );
                }
                if (value.trim().length < 1 || value.trim().length > 255) {
                  return Promise.reject(
                    new Error('Trường phải từ 1 đến 255 ký tự')
                  );
                }
                return Promise.resolve();
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
              {!isCreatingRoom && 'Cập nhật'}
            </Button>
            <Button flat auto disabled={canDelete} onPress={handleDelete} color="error">
              Xoá
            </Button>
          </div>
        </Form.Item>
      </Form>
    </Fragment>
  );
};

export default UpdateRoom;
