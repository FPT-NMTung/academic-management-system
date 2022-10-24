import { Card, Grid, Text, Button, Loading } from '@nextui-org/react';
import { Form, Select, Input } from 'antd';
import { useEffect, useState } from 'react';
import toast from 'react-hot-toast';
import FetchApi from '../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../apis/ListApi';
import { Validater } from '../../validater/Validater';

const CenterCreate = ({ onCreateSuccess }) => {
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

    toast.promise(
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
      ),
      {
        loading: 'Đang tạo trung tâm',
        success: (res) => {
          setIsCreating(false);
          onCreateSuccess();
          return 'Tạo trung tâm thành công';
        },
        error: (err) => {
          setIsCreating(false);
          return 'Tạo trung tâm thất bại';
        },
      }
    );
  };

  return (
    <Grid xs={4}>
      <Card
        css={{
          width: '100%',
          height: 'fit-content',
        }}
        variant="bordered"
      >
        <Card.Header>
          <Text
            b
            p
            size={14}
            css={{
              width: '100%',
              textAlign: 'center',
            }}
          >
            Thông tin cơ sở: <b></b>
          </Text>
        </Card.Header>
        <Card.Body>
          <Form
            labelCol={{ span: 8 }}
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
                  validator: (_, value) => {
                    if (value === null || value === undefined) {
                      return Promise.reject('Trường này không được để trống');
                    }
                    if (
                      Validater.isContaintSpecialCharacterForName(value.trim())
                    ) {
                      return Promise.reject(
                        'Trường này không được chứa ký tự đặc biệt'
                      );
                    }
                    if (value.trim().length < 2 || value.trim().length > 100) {
                      return Promise.reject(
                        new Error('Trường phải từ 2 đến 100 ký tự')
                      );
                    }
                    return Promise.resolve();
                  },
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
            <Form.Item wrapperCol={{ offset: 8, span: 10 }}>
              <Button
                auto
                flat
                css={{
                  width: '120px',
                }}
                type="primary"
                htmlType="submit"
                disabled={isCreating}
              >
                {'Tạo mới'}
              </Button>
            </Form.Item>
          </Form>
        </Card.Body>
      </Card>
    </Grid>
  );
};

export default CenterCreate;
