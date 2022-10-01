import { Card, Grid, Text, Table } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button } from 'antd';

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
              <Text size={14}>Danh sách các cơ sở</Text>
            </Card.Header>
            <Card.Divider />
            <Table
              aria-label="Example static collection table"
              css={{
                height: 'auto',
                minWidth: '100%',
              }}
              selectionMode="single"
              lined
              headerLined
              shadow={false}
            >
              <Table.Header>
                <Table.Column>ID</Table.Column>
                <Table.Column>Tên cơ sở</Table.Column>
                <Table.Column>Địa chỉ</Table.Column>
              </Table.Header>
              <Table.Body>
                <Table.Row key="1">
                  <Table.Cell>1</Table.Cell>
                  <Table.Cell>Hà Nội 1</Table.Cell>
                  <Table.Cell>Active</Table.Cell>
                </Table.Row>
                <Table.Row key="2">
                  <Table.Cell>2</Table.Cell>
                  <Table.Cell>Hà Nội 2</Table.Cell>
                  <Table.Cell>Paused</Table.Cell>
                </Table.Row>
                <Table.Row key="3">
                  <Table.Cell>3</Table.Cell>
                  <Table.Cell>Hà Nội 3</Table.Cell>
                  <Table.Cell>Active</Table.Cell>
                </Table.Row>
                <Table.Row key="4">
                  <Table.Cell>4</Table.Cell>
                  <Table.Cell>Hà Nội 4</Table.Cell>
                  <Table.Cell>Vacation</Table.Cell>
                </Table.Row>
              </Table.Body>
            </Table>
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
                  rules={[
                    { required: true, message: 'Hãy nhập tên cơ sở' },
                  ]}
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
