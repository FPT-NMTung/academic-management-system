import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../../apis/ListApi';
import classes from './CenterScreen.module.css';
import { MdEdit } from 'react-icons/md';

const CenterScreen = () => {
  const [listCenter, setListCenter] = useState([]);
  const [selectedCenterId, setSelectedCenterId] = useState(null);

  const [listProvince, setListProvince] = useState([]);

  useEffect(() => {
    const apiCanter = CenterApis.getAllCenter;
    FetchApi(apiCanter).then((res) => {
      const data = res.data;
      const mergeAddressRes = data.map((e) => {
        return {
          key: e.id,
          ...e,
          address: `${e.ward.prefix} ${e.ward.name}, ${e.district.prefix} ${e.district.name}, ${e.province.name}`,
        };
      });
      setListCenter(mergeAddressRes);
    });
  }, []);

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
              dataSource={listCenter}
            >
              <Table.Column title="Tên" dataIndex="name" key="name" />
              <Table.Column title="Địa chỉ" dataIndex="address" key="address" />
              <Table.Column
                title=""
                dataIndex="action"
                key="action"
                render={(_, data) => {
                  return (
                    <MdEdit
                      className={classes.editIcon}
                      onClick={() => {
                        setSelectedCenterId(data.id);
                      }}
                    />
                  );
                }}
              />
            </Table>
          </Card>
        </Grid>
        {selectedCenterId && (
          <Grid xs={4}>
            <Card
              css={{
                width: '100%',
                height: 'fit-content',
              }}
            >
              <Card.Header>
                <Text size={14}>
                  Thông tin cơ sở: <b>asd</b>
                </Text>
              </Card.Header>
              <Card.Divider />
              <Card.Body>
                <Form
                  labelCol={{ span: 6 }}
                  wrapperCol={{ span: 10 }}
                  layout="horizontal"
                  initialValues={{
                    name: listCenter.find((e) => e.id === selectedCenterId)
                      ?.name,
                  }}
                >
                  <Form.Item
                    name={'name'}
                    label={'Tên cơ sở'}
                    rules={[
                      {
                        required: true,
                        message: 'Hãy nhập tên cơ sở',
                      },
                    ]}
                  >
                    <Input />
                  </Form.Item>
                  <Form.Item
                    name={'province'}
                    label={'Tỉnh/Thành phố'}
                    rules={[
                      {
                        required: true,
                        message: 'Hãy chọn tỉnh/thành phố',
                      },
                    ]}
                  >
                    <Select
                      showSearch
                      placeholder="Chọn tỉnh/thành phố"
                      optionFilterProp="children"
                      filterOption={(input, option) =>
                        option.children

                          .toLowerCase()
                          .indexOf(input.toLowerCase()) >= 0
                      }
                    >
                      {/* {listProvince.map()} */}
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
        )}
      </Grid.Container>
    </div>
  );
};

export default CenterScreen;
