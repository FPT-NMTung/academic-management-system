import { Card, Grid } from '@nextui-org/react';
import { Button, Table } from 'antd';
import { Fragment } from 'react';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ManageSroApis } from '../../../apis/ListApi';
import { MdEdit } from 'react-icons/md';
import classes from './SroScreen.module.css';

const designColumns = [
  {
    title: 'STT',
    dataIndex: 'index',
    key: 'index',
    width: 60,
    fixed: 'left',
  },
  {
    title: 'Tên tài khoản',
    children: [
      {
        title: 'Họ',
        dataIndex: 'first_name',
        key: 'first_name',
        width: 180,
        fixed: 'left',
      },
      {
        title: 'Tên',
        dataIndex: 'last_name',
        key: 'last_name',
        width: 100,
        fixed: 'left',
      },
    ],
  },
  {
    title: 'Email',
    dataIndex: 'email',
    key: 'email',
    width: 250,
    fixed: 'left',
  },
  {
    title: 'Số điện thoại',
    dataIndex: 'mobile_phone',
    key: 'mobile_phone',
    width: 130,
  },
  {
    title: 'Ngày sinh',
    dataIndex: 'birthday',
    key: 'birthday',
    render: (text) => {
      return <Fragment>{new Date(text).toLocaleDateString('vi-VN')}</Fragment>;
    },
    width: 100,
  },
  {
    title: 'Giới tính',
    dataIndex: 'gender',
    key: 'gender',
    render: (data) => {
      return <Fragment>{data.id === 1 ? 'Nam' : 'Nữ'}</Fragment>;
    },
    width: 90,
  },
  {
    title: 'Địa chỉ',
    children: [
      {
        title: 'Tỉnh/Thành phố',
        dataIndex: 'province',
        key: 'province',
        render: (data) => {
          return <Fragment>{data.name}</Fragment>;
        },
        width: 160,
      },
      {
        title: 'Quận/Huyện',
        dataIndex: 'district',
        key: 'district',
        render: (data) => {
          return <Fragment>{data.name}</Fragment>;
        },
        width: 160,
      },
      {
        title: 'Phường/Xã',
        dataIndex: 'ward',
        key: 'ward',
        render: (data) => {
          return <Fragment>{data.name}</Fragment>;
        },
        width: 160,
      },
    ],
  },
  {
    title: 'Thông tin thẻ CMT/CCCD',
    children: [
      {
        title: 'Số',
        dataIndex: 'citizen_identity_card_no',
        key: 'citizen_identity_card_no',
        width: 160,
      },
      {
        title: 'Ngày cấp',
        dataIndex: 'citizen_identity_card_published_date',
        key: 'citizen_identity_card_published_date',
        render: (text) => {
          return (
            <Fragment>{new Date(text).toLocaleDateString('vi-VN')}</Fragment>
          );
        },
        width: 160,
      },
      {
        title: 'Nơi cấp',
        dataIndex: 'citizen_identity_card_published_place',
        key: 'citizen_identity_card_published_place',
        width: 160,
      },
    ],
  },
  {
    title: '',
    dataIndex: '',
    key: 'action',
    render: (_, data) => {
      return (
        <div className={classes.logoEdit}>
          <MdEdit
            size={20}
            style={{ cursor: 'pointer' }}
            color="0a579f"
            onClick={() => {
              // setSelectRoomId(data.id);
            }}
          />
        </div>
      );
    },
    width: 50,
    fixed: 'right',
  },
];

const SroScreen = () => {
  const [listSro, setListSro] = useState([]);

  const getAllSro = () => {
    FetchApi(ManageSroApis.getAllSro, null, null, null).then((res) => {
      const data = res.data.map((item, index) => {
        return {
          key: item.id,
          ...item,
          index: index + 1,
        };
      });
      setListSro(data);
    });
  };

  useEffect(() => {
    getAllSro();
  }, []);

  return (
    <Grid.Container gap={2}>
      <Grid sm={10}>
        <Card>
          <Card.Body>asdasd</Card.Body>
        </Card>
      </Grid>
      <Grid sm={2}>
        <Card>
          <Card.Body>
            {/* <Button type="primary">Tạo tài khoản SRO mới</Button> */}
          </Card.Body>
        </Card>
      </Grid>
      <Grid sm={12}>
        <Card>
          <Table
            bordered
            size="middle"
            dataSource={listSro}
            columns={designColumns}
            scroll={{
              x: 1500,
            }}
          />
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default SroScreen;
