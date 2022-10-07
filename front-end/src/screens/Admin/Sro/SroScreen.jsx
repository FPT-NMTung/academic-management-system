import { Card, Grid, Text } from '@nextui-org/react';
import { Button, Form, Input, Table } from 'antd';
import { Fragment } from 'react';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ManageSroApis } from '../../../apis/ListApi';
import { MdEdit } from 'react-icons/md';
import classes from './SroScreen.module.css';
import { useNavigate } from 'react-router-dom';
import ColumnGroup from 'antd/lib/table/ColumnGroup';

const SroScreen = () => {
  const [listSro, setListSro] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  const navigate = useNavigate();
  const [form] = Form.useForm();

  const getAllSro = () => {
    const param = {
      firstName: form.getFieldValue('first_name'),
      lastName: form.getFieldValue('last_name'),
      mobilePhone: form.getFieldValue('mobile_phone'),
      email: form.getFieldValue('email'),
      emailOrganization: form.getFieldValue('email_organization'),
    };

    FetchApi(ManageSroApis.searchSro, null, param, null).then((res) => {
      const data = res.data.map((item, index) => {
        return {
          key: item.id,
          ...item,
          index: index + 1,
        };
      });
      setListSro(data);
      setIsLoading(false);
    });
  };

  const handleClearInput = () => {
    form.resetFields();
    getAllSro();
  };

  const handleSubmitForm = () => {
    getAllSro();
  };

  useEffect(() => {
    getAllSro();
  }, []);

  return (
    <Grid.Container gap={2}>
      <Grid sm={12}>
        <Card>
          <Card.Body>
            <Form
              name="basic"
              layout="inline"
              form={form}
              initialValues={{
                first_name: '',
                last_name: '',
                mobile_phone: '',
                email: '',
                email_organization: '',
              }}
              onFinish={handleSubmitForm}
            >
              <Form.Item
                name="first_name"
                style={{ width: 'calc(17% - 16px)' }}
              >
                <Input placeholder="Họ" />
              </Form.Item>
              <Form.Item name="last_name" style={{ width: 'calc(17% - 16px)' }}>
                <Input placeholder="Tên" />
              </Form.Item>
              <Form.Item
                name="mobile_phone"
                style={{ width: 'calc(17% - 16px)' }}
              >
                <Input placeholder="Số điện thoại" />
              </Form.Item>
              <Form.Item name="email" style={{ width: 'calc(17% - 16px)' }}>
                <Input placeholder="Email cá nhân" />
              </Form.Item>
              <Form.Item
                name="email_organization"
                style={{ width: 'calc(17% - 16px)' }}
              >
                <Input placeholder="Email tổ chức" />
              </Form.Item>
              <Form.Item style={{ width: 'calc(9% - 16px)' }}>
                <Button
                  type="primary"
                  htmlType="submit"
                  style={{ width: '100%' }}
                >
                  Tìm kiếm
                </Button>
              </Form.Item>
              <Form.Item style={{ width: '6%', marginRight: 0 }}>
                <Button
                  type="default"
                  style={{
                    width: '100%',
                  }}
                  onClick={handleClearInput}
                >
                  Huỷ
                </Button>
              </Form.Item>
            </Form>
          </Card.Body>
        </Card>
      </Grid>
      <Grid sm={12}>
        <Card>
          <Card.Header>
            <Grid.Container>
              <Grid sm={1}></Grid>
              <Grid sm={10}>
                <Text
                  b
                  size={14}
                  p
                  css={{
                    width: '100%',
                    textAlign: 'center',
                  }}
                >
                  Danh sách SRO
                </Text>
              </Grid>
              <Grid sm={1}>
                <Button
                  type="primary"
                  style={{
                    width: '100%',
                  }}
                  onClick={() => {
                    navigate('/admin/account/sro/create');
                  }}
                >
                  + Tạo mới
                </Button>
              </Grid>
            </Grid.Container>
          </Card.Header>
          <Table
            loading={isLoading}
            bordered
            size="middle"
            dataSource={listSro}
            // columns={designColumns}
            scroll={{
              x: 1500,
            }}
            pagination={{
              size: 'default',
              position: ['bottomCenter'],
            }}
          >
            <Table.Column
              title="STT"
              dataIndex="index"
              key="index"
              width={60}
              fixed={'left'}
            />
            <ColumnGroup title="Tên tài khoản">
              <Table.Column
                title="Họ"
                dataIndex="first_name"
                key="username"
                width={180}
                fixed={'left'}
              />
              <Table.Column
                title="Tên"
                dataIndex="last_name"
                key="name"
                width={100}
                fixed={'left'}
              />
            </ColumnGroup>
            <ColumnGroup title="Email">
              <Table.Column
                title="Cá nhân"
                dataIndex="email"
                key="username"
                width={250}
              />
              <Table.Column
                title="Tổ chức"
                dataIndex="email_organization"
                key="name"
                width={250}
              />
            </ColumnGroup>
            <Table.Column
              title="Số điện thoại"
              dataIndex="mobile_phone"
              key="mobile_phone"
              width={130}
            />
            <Table.Column
              title="Ngày sinh"
              dataIndex="birthday"
              key="birthday"
              width={100}
              render={(text) => {
                return (
                  <Fragment>
                    {new Date(text).toLocaleDateString('vi-VN')}
                  </Fragment>
                );
              }}
            />
            <Table.Column
              title="Giới tính"
              dataIndex="gender"
              key="gender"
              width={90}
              render={(data) => {
                return <Fragment>{data.id === 1 ? 'Nam' : 'Nữ'}</Fragment>;
              }}
            />
            <ColumnGroup title="Địa chỉ">
              <Table.Column
                title="Tỉnh/Thành phố"
                dataIndex="province"
                key="province"
                width={180}
                render={(data) => {
                  return <Fragment>{data.name}</Fragment>;
                }}
              />
              <Table.Column
                title="Quận/Huyện"
                dataIndex="district"
                key="district"
                width={180}
                render={(data) => {
                  return <Fragment>{data.prefix} {data.name}</Fragment>;
                }}
              />
              <Table.Column
                title="Phường/Xã"
                dataIndex="ward"
                key="ward"
                width={180}
                render={(data) => {
                  return <Fragment>{data.prefix} {data.name}</Fragment>;
                }}
              />
            </ColumnGroup>
            <ColumnGroup title="Thông tin thẻ CMT/CCCD">
              <Table.Column
                title="Số thẻ"
                dataIndex="citizen_identity_card_no"
                key="citizen_identity_card_no"
                width={160}
              />
              <Table.Column
                title="Ngày cấp"
                dataIndex="citizen_identity_card_published_date"
                key="citizen_identity_card_published_date"
                width={160}
                render={(text) => {
                  return (
                    <Fragment>
                      {new Date(text).toLocaleDateString('vi-VN')}
                    </Fragment>
                  );
                }}
              />
              <Table.Column
                title="Nơi cấp"
                dataIndex="citizen_identity_card_published_place"
                key="citizen_identity_card_published_place"
                width={200}
              />
            </ColumnGroup>
            <Table.Column
              width={50}
              title=""
              dataIndex=""
              key="action"
              fixed={'right'}
              render={(_, data) => {
                return (
                  <div className={classes.logoEdit}>
                    <MdEdit
                      color="0a579f"
                      style={{ cursor: 'pointer' }}
                      onClick={() => {
                        navigate(`/admin/account/sro/${data.user_id}`);
                      }}
                    />
                  </div>
                );
              }}
            />
          </Table>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default SroScreen;
