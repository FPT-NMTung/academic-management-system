import {
  Card,
  Grid,
  Text,
  Tooltip,
  Button,
  Badge,
  Table,
  Loading,
} from '@nextui-org/react';
import { Form, Input } from 'antd';
import { Fragment } from 'react';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ManageSroApis } from '../../../apis/ListApi';
import { RiEyeFill } from 'react-icons/ri';
import classes from './SroScreen.module.css';
import { useNavigate } from 'react-router-dom';
import ColumnGroup from 'antd/lib/table/ColumnGroup';

const SroScreen = () => {
  const [listSro, setListSro] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  const navigate = useNavigate();
  const [form] = Form.useForm();

  const getAllSro = () => {
    setIsLoading(true);
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

  useEffect(() => {
    getAllSro();
  }, []);

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
                  flat
                  auto
                  type="primary"
                  htmlType="submit"
                  style={{ width: '100%' }}
                >
                  Tìm kiếm
                </Button>
              </Form.Item>
              <Form.Item style={{ width: '6%', marginRight: 0 }}>
                <Button
                  flat
                  auto
                  type="default"
                  style={{
                    width: '100%',
                  }}
                  color="error"
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
        <Card
          variant="bordered"
          css={{
            minHeight: '300px',
          }}
        >
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
                  flat
                  auto
                  type="primary"
                  style={{
                    width: '100%',
                  }}
                  onPress={() => {
                    navigate('/admin/account/sro/create');
                  }}
                >
                  + Tạo mới
                </Button>
              </Grid>
            </Grid.Container>
          </Card.Header>
          {isLoading && <Loading/>}
          {!isLoading && <Table aria-label="">
            <Table.Header>
              <Table.Column width={60}>STT</Table.Column>
              <Table.Column width={320}>Họ và tên</Table.Column>
              <Table.Column width={150}>Cơ sở</Table.Column>
              <Table.Column width={250}>Email cá nhân</Table.Column>
              <Table.Column width={250}>Email tổ chức</Table.Column>
              <Table.Column>Số điện thoại</Table.Column>
              <Table.Column>Giới tính</Table.Column>
              <Table.Column width={20}></Table.Column>
            </Table.Header>
            <Table.Body>
              {listSro.map((data, index) => (
                <Table.Row key={data.user_id}>
                  <Table.Cell>{index + 1}</Table.Cell>
                  <Table.Cell>
                    <b>
                      {data.first_name} {data.last_name}
                    </b>
                  </Table.Cell>
                  <Table.Cell>{data.center_name}</Table.Cell>
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
                        navigate(`/admin/account/sro/${data.user_id}`);
                      }}
                    />
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
            <Table.Pagination shadow noMargin align="center" rowsPerPage={9} />
          </Table>}
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default SroScreen;
