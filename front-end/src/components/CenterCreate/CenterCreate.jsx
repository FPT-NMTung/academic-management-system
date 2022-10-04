import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../apis/ListApi';

const CenterCreate = ({onCreateSuccess}) => {
  const [listProvince, setListProvince] = useState([]);
  const [listDistrict, setListDistrict] = useState([]);
  const [listWard, setListWard] = useState([]);

  const [isCreating, setIsCreating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);

  const [form] = Form.useForm();

  const getListProvince = () => {
    FetchApi(AddressApis.getListProvince).then((res) => {
      setListProvince(res.data);
    });
    setListDistrict([]);
    setListWard([]);
  };

  const getListDistrict = () => {
    const provinceId = form.getFieldValue('province');

    FetchApi(AddressApis.getListDistrict, null, null, [`${provinceId}`]).then(
      (res) => {
        setListDistrict(res.data);
      }
    );
    setListWard([]);
    form.resetFields(['district', 'ward']);
  };

  const getListWard = () => {
    const provinceId = form.getFieldValue('province');
    const districtId = form.getFieldValue('district');

    FetchApi(AddressApis.getListWard, null, null, [
      `${provinceId}`,
      `${districtId}`,
    ]).then((res) => {
      setListWard(res.data);
    });

    form.resetFields(['ward']);
  };

  useEffect(() => {
    getListProvince();
  }, []);

  const handleSubmitForm = (e) => {
    setIsCreating(true);
    FetchApi(
      CenterApis.createCenter,
      {
        name: e.name.trim(),
        province_id: e.province,
        district_id: e.district,
        ward_id: e.ward,
      },
      null,
      null
    )
      .then(() => {
        onCreateSuccess();
      })
      .catch(() => {
        setIsCreating(false);
        setIsFailed(true);
      });
  };

  return (
    <Grid xs={4}>
      <Card
        css={{
          width: '100%',
          height: 'fit-content',
        }}
      >
        <Card.Header>
          <Text size={14}>
            Thông tin cơ sở: <b></b>
          </Text>
        </Card.Header>
        <Card.Divider />
        <Card.Body>
          <Form
            labelCol={{ span: 6 }}
            wrapperCol={{ span: 14 }}
            layout="horizontal"
            onFinish={handleSubmitForm}
            form={form}
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
                disabled={listProvince.length === 0}
                placeholder="Chọn tỉnh/thành phố"
                optionFilterProp="children"
                onChange={getListDistrict}
              >
                {listProvince.map((e) => (
                  <Select.Option key={e.id} value={e.id}>
                    {e.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={'district'}
              label={'Quận/Huyện'}
              rules={[
                {
                  required: true,
                  message: 'Hãy chọn quận/huyện',
                },
              ]}
            >
              <Select
                allowClear
                showSearch
                disabled={listDistrict.length === 0}
                placeholder="Chọn quận/huyện"
                optionFilterProp="children"
                onChange={getListWard}
              >
                {listDistrict.map((e) => (
                  <Select.Option key={e.id} value={e.id}>
                    {e.prefix} {e.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={'ward'}
              label={'Phường/Xã'}
              rules={[
                {
                  required: true,
                  message: 'Hãy chọn phường/xã',
                },
              ]}
            >
              <Select
                showSearch
                disabled={listWard.length === 0}
                placeholder="Chọn phường/xã"
                optionFilterProp="children"
              >
                {listWard.map((e) => (
                  <Select.Option key={e.id} value={e.id}>
                    {e.prefix} {e.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item wrapperCol={{ offset: 6, span: 10 }}>
              <Button type="primary" htmlType="submit" loading={isCreating}>
                Tạo mới
              </Button>
            </Form.Item>
          </Form>
          {!isCreating && isFailed && (
            <Text
              size={14}
              css={{
                color: 'red',
                textAlign: 'center',
              }}
            >
              Tạo cơ sở thuất bại, kiểm tra lại thông tin và thử lại
            </Text>
          )}
        </Card.Body>
      </Card>
    </Grid>
  );
};

export default CenterCreate;
