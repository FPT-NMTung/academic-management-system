import { Card, Grid, Text, Button, Loading } from '@nextui-org/react';
import { Form, Select, Input, Spin } from 'antd';
import { Fragment } from 'react';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../apis/ListApi';
import classes from './CenterUpdate.module.css';
import { Validater } from '../../validater/Validater';
import toast from 'react-hot-toast';

const CenterUpdate = ({ data, onUpdateSuccess }) => {
  const [listProvince, setListProvince] = useState([]);
  const [listDistrict, setListDistrict] = useState([]);
  const [listWard, setListWard] = useState([]);

  const [isUpdating, setIsUpdating] = useState(false);
  const [canDelete, setCanDelete] = useState(false);

  const [isLoadingProvince, setIsLoadingProvince] = useState(true);
  const [isLoadingDistrict, setIsLoadingDistrict] = useState(true);
  const [isLoadingWard, setIsLoadingWard] = useState(true);

  const [form] = Form.useForm();

  const getListProvince = () => {
    FetchApi(AddressApis.getListProvince).then((res) => {
      setListProvince(res.data);
      setIsLoadingProvince(false);
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
    form.setFieldsValue({ district: null, ward: null });
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

    form.setFieldsValue({ ward: null });
  };

  useEffect(() => {
    getListProvince();
    FetchApi(AddressApis.getListDistrict, null, null, [
      `${data?.province.id}`,
    ]).then((res) => {
      setListDistrict(res.data);
      setIsLoadingDistrict(false);
    });
    FetchApi(AddressApis.getListWard, null, null, [
      `${data?.province.id}`,
      `${data?.district.id}`,
    ]).then((res) => {
      setListWard(res.data);
      setIsLoadingWard(false);
    });
    FetchApi(CenterApis.checkCanDeleteCenter, null, null, [String(data.id)])
      .then((res) => {
        setCanDelete(res.data.can_delete);
      })
      .catch((err) => {
        toast.error(
          'Không thể kiểm tra xem có thể xóa trung tâm này hay không'
        );
      });
  }, []);

  const handleSubmitForm = (e) => {
    setIsUpdating(true);

    const body = {
      name: e.name.trim(),
      province_id: e.province,
      district_id: e.district,
      ward_id: e.ward,
    };

    toast.promise(
      FetchApi(CenterApis.updateCenter, body, null, [`${data.id}`]),
      {
        loading: 'Đang cập nhật...',
        success: (res) => {
          onUpdateSuccess();
          return 'Cập nhật thành công';
        }, 
        error: (err) => {
          return 'Cập nhật thất bại';
        },
      }
    );
  };

  const handleDeleteCenter = () => {
    toast.promise(
      FetchApi(CenterApis.deleteCenter, null, null, [`${data.id}`]),
      {
        loading: 'Đang xóa...',
        success: (res) => {
          onUpdateSuccess();
          return 'Xóa thành công';
        },
        error: (err) => {
          return 'Xóa thất bại';
        },
      }
    )
  };

  return (
    <Fragment>
      {(isLoadingDistrict || isLoadingProvince || isLoadingWard) && (
        <div className={classes.loading}>
          <Spin />
        </div>
      )}
      {!isLoadingDistrict && !isLoadingProvince && !isLoadingWard && (
        <Form
          labelCol={{ span: 7 }}
          wrapperCol={{ span: 16 }}
          layout="horizontal"
          onFinish={handleSubmitForm}
          form={form}
          initialValues={{
            name: data?.name,
            province: data?.province.id,
            district: data?.district.id,
            ward: data?.ward.id,
          }}
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
                  if (value.trim().length < 1 || value.trim().length > 100) {
                    return Promise.reject(
                      new Error('Trường phải từ 1 đến 100 ký tự')
                    );
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <Input />
          </Form.Item>

          <Fragment>
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
          </Fragment>
          <Form.Item wrapperCol={{ offset: 7, span: 99 }}>
            <div
              style={{
                display: 'flex',
                gap: '10px',
              }}
            >
              <Button
                flat
                auto
                css={{
                  width: '120px',
                }}
                type="primary"
                htmlType="submit"
                disabled={isUpdating}
              >
                Cập nhật
              </Button>
              <Button
                flat
                auto
                css={{
                  width: '80px',
                }}
                color={'error'}
                disabled={
                  canDelete === false || canDelete === undefined || isUpdating
                }
                onPress={handleDeleteCenter}
              >
                {canDelete === undefined && <Loading size="xs" />}
                {canDelete !== undefined && 'Xoá'}
              </Button>
            </div>
          </Form.Item>
        </Form>
      )}
    </Fragment>
  );
};

export default CenterUpdate;
