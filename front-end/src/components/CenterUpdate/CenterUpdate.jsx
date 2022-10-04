import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button } from 'antd';
import { Fragment } from 'react';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../apis/ListApi';

const CenterUpdate = ({ data, onCreateSuccess }) => {
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
  };

  return (
    <Fragment>
      <Form
        labelCol={{ span: 7 }}
        wrapperCol={{ span: 16 }}
        layout="horizontal"
        onFinish={handleSubmitForm}
        form={form}
        initialValues={{
          name: data?.name,
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
            disabled={listProvince.length === 0}
            placeholder="Chọn tỉnh/thành phố"
            optionFilterProp="children"
            onChange={getListDistrict}
            dropdownStyle={{ zIndex: 9999 }}
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
            dropdownStyle={{ zIndex: 9999 }}
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
            dropdownStyle={{ zIndex: 9999 }}
          >
            {listWard.map((e) => (
              <Select.Option key={e.id} value={e.id}>
                {e.prefix} {e.name}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item wrapperCol={{ offset: 7, span: 99 }}>
          <Button type="primary" htmlType="submit" loading={isCreating}>
            Cập nhật
          </Button>
          <Button
            style={{ marginLeft: 10}}
            type="primary"
            htmlType="button"
            danger
            disabled
          >
            Xoá
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
    </Fragment>
  );
};

export default CenterUpdate;
