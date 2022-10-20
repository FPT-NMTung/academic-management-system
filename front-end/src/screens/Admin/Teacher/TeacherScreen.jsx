import {
  Grid,
  Card,
  Text,
  Tooltip,
  Button,
  Table,
  Badge,
} from '@nextui-org/react';
import { Form, Input, message } from 'antd';
import FetchApi from '../../../apis/FetchApi';
import { useNavigate } from 'react-router-dom';
import { ManageTeacherApis } from '../../../apis/ListApi';
import { useState } from 'react';
import { useEffect } from 'react';
import ColumnGroup from 'antd/lib/table/ColumnGroup';
import { Fragment } from 'react';
import classes from '../Sro/SroScreen.module.css';
import { RiEyeFill } from 'react-icons/ri';

const TeacherScreen = () => {
  const [dataSource, setDataSource] = useState([]);
  const [isGetData, setIsGetData] = useState(true);

  const [form] = Form.useForm();
  const navigate = useNavigate();

  const getListTeacher = (param) => {
    setIsGetData(true);
    FetchApi(ManageTeacherApis.searchTeacher, null, param, null)
      .then((res) => {
        setDataSource(
          res.data.map((item, index) => {
            return {
              ...item,
              key: item.id,
              index: index + 1,
            };
          })
        );
        setIsGetData(false);
      })
      .catch(() => {
        setIsGetData(false);
        message.error('Có điều gì đó không đúng 😥');
      });
  };

  const handleSubmitForm = () => {
    const data = form.getFieldsValue();
    const param = {
      firstName: data.first_name.trim(),
      lastName: data.last_name.trim(),
      nickname: data.nickname.trim(),
      email: data.email.trim(),
      mobilePhone: data.mobile_phone.trim(),
      emailOrganization: data.email_organization.trim(),
    };

    getListTeacher(param);
  };

  const handleClearInput = () => {
    form.resetFields();
    getListTeacher();
  };

  useEffect(() => {
    getListTeacher();
  }, []);

  const renderGender = (id) => {
    if (id === 1) {
      return (
        <Badge variant="flat" color="success">
          Nam
        </Badge>
      );
    } else if (id === 2) {
      return (
        <Badge variant="flat" color="error">
          Nữ
        </Badge>
      );
    } else if (id === 3) {
      return (
        <Badge variant="flat" color="default">
          Khác
        </Badge>
      );
    } else {
      return (
        <Badge variant="flat" color="default">
          Không xác định
        </Badge>
      );
    }
  };

  return (
    <Grid.Container gap={2}>
      <Grid sm={12}>
        <Card variant="bordered">
          <Card.Body
            css={{
              padding: '10px',
            }}
          >
            <Form
              name="basic"
              layout="inline"
              form={form}
              initialValues={{
                first_name: '',
                last_name: '',
                mobile_phone: '',
                nickname: '',
                email: '',
                email_organization: '',
              }}
              onFinish={handleSubmitForm}
            >
              <Form.Item
                name="first_name"
                style={{ width: 'calc(15% - 16px)' }}
              >
                <Input placeholder="Họ" />
              </Form.Item>
              <Form.Item name="last_name" style={{ width: 'calc(15% - 16px)' }}>
                <Input placeholder="Tên" />
              </Form.Item>
              <Form.Item name="nickname" style={{ width: 'calc(10% - 16px)' }}>
                <Input placeholder="Biệt danh" />
              </Form.Item>
              <Form.Item
                name="mobile_phone"
                style={{ width: 'calc(15% - 16px)' }}
              >
                <Input placeholder="Số điện thoại" />
              </Form.Item>
              <Form.Item name="email" style={{ width: 'calc(15% - 16px)' }}>
                <Input placeholder="Email cá nhân" />
              </Form.Item>
              <Form.Item
                name="email_organization"
                style={{ width: 'calc(15% - 16px)' }}
              >
                <Input placeholder="Email tổ chức" />
              </Form.Item>
              <Form.Item style={{ width: 'calc(9% - 16px)' }}>
                <Button
                  flat
                  auto
                  type="primary"
                  htmlType="submit"
                  css={{ width: '100%' }}
                >
                  Tìm kiếm
                </Button>
              </Form.Item>
              <Form.Item style={{ width: '6%', marginRight: 0 }}>
                <Button
                  flat
                  auto
                  color={'error'}
                  css={{
                    width: '100%',
                  }}
                  onPress={handleClearInput}
                >
                  Huỷ
                </Button>
              </Form.Item>
            </Form>
          </Card.Body>
        </Card>
      </Grid>
      <Grid sm={12}>
        <Card variant="bordered">
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
                  Danh sách giáo viên
                </Text>
              </Grid>
              <Grid sm={1}>
                <Button
                  flat
                  auto
                  css={{
                    width: '100%',
                  }}
                  type="primary"
                  onPress={() => {
                    navigate('/admin/account/teacher/create');
                  }}
                >
                  + Tạo mới
                </Button>
              </Grid>
            </Grid.Container>
          </Card.Header>
          {/* <Table
            loading={isGetData}
            dataSource={dataSource}
            scroll={{
              x: 1500,
            }}
            bordered
            size="middle"
            pagination={{
              size: 'default',
              position: ['bottomCenter'],
            }}
          >
            <Table.Column
              title="STT"
              dataIndex="index"
              key="index"
              width={50}
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
            <Table.Column
              title="Biệt danh"
              dataIndex="nickname"
              key="nickname"
              width={150}
            />
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
                  return (
                    <Fragment>
                      {data.prefix} {data.name}
                    </Fragment>
                  );
                }}
              />
              <Table.Column
                title="Phường/Xã"
                dataIndex="ward"
                key="ward"
                width={180}
                render={(data) => {
                  return (
                    <Fragment>
                      {data.prefix} {data.name}
                    </Fragment>
                  );
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
                    <Tooltip
                      placement="left"
                      content="Chi tiết"
                      color={'invert'}
                    >
                      <RiEyeFill
                        size={20}
                        color="0a579f"
                        style={{ cursor: 'pointer' }}
                        onClick={() => {
                          navigate(`/admin/account/teacher/${data.user_id}`);
                        }}
                      />
                    </Tooltip>
                  </div>
                );
              }}
            />
          </Table> */}
          <Table aria-label="">
            <Table.Header>
              <Table.Column width={60}>STT</Table.Column>
              <Table.Column width={320}>Họ và tên</Table.Column>
              <Table.Column width={160}>Biệt danh</Table.Column>
              <Table.Column width={250}>Email cá nhân</Table.Column>
              <Table.Column width={250}>Email tổ chức</Table.Column>
              <Table.Column>Số điện thoại</Table.Column>
              <Table.Column>Giới tính</Table.Column>
              <Table.Column width={20}></Table.Column>
            </Table.Header>
            <Table.Body>
              {dataSource.map((data, index) => (
                <Table.Row key={data.user_id}>
                  <Table.Cell>{index + 1}</Table.Cell>
                  <Table.Cell>
                    <b>
                      {data.first_name} {data.last_name}
                    </b>
                  </Table.Cell>
                  <Table.Cell>{data.nickname}</Table.Cell>
                  <Table.Cell>{data.email}</Table.Cell>
                  <Table.Cell>{data.email_organization}</Table.Cell>
                  <Table.Cell>{data.mobile_phone}</Table.Cell>
                  <Table.Cell>{renderGender(data.gender.id)}</Table.Cell>
                  <Table.Cell>
                    <RiEyeFill
                      size={20}
                      color="5EA2EF"
                      style={{ cursor: 'pointer' }}
                      onClick={() => {
                        navigate(`/admin/account/teacher/${data.user_id}`);
                      }}
                    />
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
            <Table.Pagination shadow noMargin align="center" rowsPerPage={10} />
          </Table>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default TeacherScreen;