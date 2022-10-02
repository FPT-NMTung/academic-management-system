import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import classes from './CenterScreen.module.css';

const dataSource = [
  {
    key: '1',
    name: 'Mike',
    age: 32,
    address: '10 Downing Street',
  },
  {
    key: '2',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '3',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '4',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '5',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '6',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '7',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '8',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '9',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '10',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '11',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '12',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '13',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '14',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
  {
    key: '15',
    name: 'John',
    age: 42,
    address: '10 Downing Street',
  },
];

const columns = [
  {
    title: 'Name',
    dataIndex: 'name',
    key: 'name',
  },
  {
    title: 'Age',
    dataIndex: 'age',
    key: 'age',
  },
  {
    title: 'Address',
    dataIndex: 'address',
    key: 'address',
  },
];

const CenterScreen = () => {
  return (
    <div>
      <Grid.Container gap={2} justify="center">
        <Grid xs={5}>
          <Card
            css={{
              width: '100%',
              height: 'fit-content',
            }}
          >
            <Card.Header>
              <div className={classes.headerTable}>
                <Text size={14}>Danh sách các cơ sở</Text>
                <Button type="primary">Thêm cơ sở</Button>
              </div>
            </Card.Header>
            <Card.Divider />
            <Table
              pagination={{ position: ['bottomCenter'] }}
              dataSource={dataSource}
              columns={columns}
            />
          </Card>
        </Grid>
        <Grid xs={4}>
          <Card
            css={{
              width: '100%',
              height: 'fit-content',
            }}
          >
            <Card.Header>
              <Text size={14}>
                Thông tin cơ sở: <b>Hà Nội 1</b>
              </Text>
            </Card.Header>
            <Card.Divider />
            <Card.Body>
              <Form
                labelCol={{ span: 6 }}
                wrapperCol={{ span: 10 }}
                layout="horizontal"
              >
                <Form.Item
                  label="Tên cơ sở"
                  name="disabled"
                  valuePropName="checked"
                  rules={[{ required: true, message: 'Hãy nhập tên cơ sở' }]}
                >
                  <Input placeholder="" />
                </Form.Item>
                <Divider plain="left">Thông tin địa chỉ</Divider>
                <Form.Item
                  label="Tỉnh thành"
                  name="disabled"
                  valuePropName="checked"
                  rules={[
                    { required: true, message: 'Vui lòng chọn tỉnh thành' },
                  ]}
                >
                  <Select showSearch>
                    <Select.Option value="jack">Jack</Select.Option>
                    <Select.Option value="lucy">Lucy</Select.Option>
                    <Select.Option value="tom">Tom</Select.Option>
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Quận huyện"
                  name="disabled"
                  valuePropName="checked"
                  rules={[
                    { required: true, message: 'Vui lòng chọn quận huyện' },
                  ]}
                >
                  <Select showSearch>
                    <Select.Option value="jack">Jack</Select.Option>
                    <Select.Option value="lucy">Lucy</Select.Option>
                    <Select.Option value="tom">Tom</Select.Option>
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Phường xã"
                  name="disabled"
                  valuePropName="checked"
                  rules={[
                    { required: true, message: 'Vui lòng chọn phường xã' },
                  ]}
                >
                  <Select showSearch>
                    <Select.Option value="jack">Jack</Select.Option>
                    <Select.Option value="lucy">Lucy</Select.Option>
                    <Select.Option value="tom">Tom</Select.Option>
                  </Select>
                </Form.Item>
                <Form.Item wrapperCol={{ offset: 6, span: 10 }}>
                  <Button type="primary" htmlType="submit">
                    Cập nhật
                  </Button>
                </Form.Item>
              </Form>
            </Card.Body>
          </Card>
        </Grid>
      </Grid.Container>
    </div>
  );
};

export default CenterScreen;
