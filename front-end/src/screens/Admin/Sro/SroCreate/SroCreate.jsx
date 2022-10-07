import { Grid, Card, Text, Spacer } from '@nextui-org/react';
import { Button, Form, Input, Select, Divider, DatePicker } from 'antd';
import classes from './SroCreate.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import {
  CenterApis,
  GenderApis,
  AddressApis,
  ManageSroApis,
} from '../../../../apis/ListApi';
import FetchApi from '../../../../apis/FetchApi';
import { Validater } from '../../../../validater/Validater';
import moment from 'moment';

const SroCreate = ({ modeUpdate }) => {
  const [listCenter, setListCenter] = useState([]);
  const [listGender, setListGender] = useState([]);
  const [listProvince, setListProvince] = useState([]);
  const [listDistrict, setListDistrict] = useState([]);
  const [listWard, setListWard] = useState([]);
  const [isGetDateUser, setIsGetDateUser] = useState(modeUpdate);
  const [isCreatingOrUpdating, setIsCreatingOrUpdating] = useState(false);

  const navigate = useNavigate();
  const { id } = useParams();
  const [form] = Form.useForm();

  const getListCenter = () => {
    FetchApi(CenterApis.getAllCenter).then((res) => {
      setListCenter(res.data);
    });
  };

  const getListGender = () => {
    FetchApi(GenderApis.getAllGender).then((res) => {
      setListGender(res.data);
    });
  };

  const getListProvince = () => {
    FetchApi(AddressApis.getListProvince).then((res) => {
      setListProvince(res.data);
    });
    setListDistrict([]);
    setListWard([]);
  };

  const getListDistrict = () => {
    const provinceId = form.getFieldValue('province_id');

    FetchApi(AddressApis.getListDistrict, null, null, [`${provinceId}`]).then(
      (res) => {
        setListDistrict(res.data);
      }
    );
    setListWard([]);
    form.setFieldsValue({ district_id: null, ward_id: null });
  };

  const getListWard = () => {
    const provinceId = form.getFieldValue('province_id');
    const districtId = form.getFieldValue('district_id');

    FetchApi(AddressApis.getListWard, null, null, [
      `${provinceId}`,
      `${districtId}`,
    ]).then((res) => {
      setListWard(res.data);
    });

    form.setFieldsValue({ ward_id: null });
  };

  const getListDistrictForUpdate = () => {
    const provinceId = form.getFieldValue('province_id');

    FetchApi(AddressApis.getListDistrict, null, null, [`${provinceId}`]).then(
      (res) => {
        setListDistrict(res.data);
      }
    );
  };

  const getListWardForUpdate = () => {
    const provinceId = form.getFieldValue('province_id');
    const districtId = form.getFieldValue('district_id');

    FetchApi(AddressApis.getListWard, null, null, [
      `${provinceId}`,
      `${districtId}`,
    ]).then((res) => {
      setListWard(res.data);
    });
  };

  const handleSubmitForm = () => {
    setIsCreatingOrUpdating(true);
    const data = form.getFieldsValue();
    const body = {
      first_name: data.first_name.trim(),
      last_name: data.last_name.trim(),
      mobile_phone: data.mobile_phone.trim(),
      email: data.email.trim(),
      email_organization: data.email_organization.trim(),
      province_id: data.province_id,
      district_id: data.district_id,
      ward_id: data.ward_id,
      gender_id: data.gender_id,
      birthday: data.birthday.toDate(),
      center_id: data.center_id,
      citizen_identity_card_no: data.citizen_identity_card_no.trim(),
      citizen_identity_card_published_date:
        data.citizen_identity_card_published_date.toDate(),
      citizen_identity_card_published_place:
        data.citizen_identity_card_published_place.trim(),
    };

    const api = modeUpdate ? ManageSroApis.updateSro : ManageSroApis.createSro;
    const params = modeUpdate ? [`${id}`] : null;
    FetchApi(api, body, null, params).then((res) => {
      const user_id = res.data.user_id;
      navigate(`/admin/account/sro/${user_id}`, { replace: true });
    });
  };

  const getInformationSro = () => {
    FetchApi(ManageSroApis.getDetailSro, null, null, [`${id}`])
      .then((res) => {
        const data = res.data;
        form.setFieldsValue({
          first_name: data.first_name,
          last_name: data.last_name,
          mobile_phone: data.mobile_phone,
          email: data.email,
          email_organization: data.email_organization,
          province_id: data.province.id,
          district_id: data.district.id,
          ward_id: data.ward.id,
          gender_id: data.gender.id,
          birthday: moment(data.birthday),
          center_id: data.center_id,
          citizen_identity_card_no: data.citizen_identity_card_no,
          citizen_identity_card_published_date: moment(
            data.citizen_identity_card_published_date
          ),
          citizen_identity_card_published_place:
            data.citizen_identity_card_published_place,
        });
        setIsGetDateUser(false);

        getListCenter();
        getListGender();
        getListDistrictForUpdate();
        getListWardForUpdate();
      })
      .catch((err) => {
        navigate('/admin/account/sro');
      });
  };

  useEffect(() => {
    getListCenter();
    getListGender();
    getListProvince();
    if (modeUpdate) {
      getInformationSro();
    }
  }, []);

  return (
    <Form
      labelCol={{ span: 7 }}
      wrapperCol={{ span: 15 }}
      form={form}
      onFinish={handleSubmitForm}
      disabled={isGetDateUser}
    >
      <Grid.Container justify="center">
        <Grid xs={7} direction={'column'} css={{ rowGap: 20 }}>
          <Card>
            <Card.Header>
              <Text
                b
                size={16}
                p
                css={{
                  width: '100%',
                  textAlign: 'center',
                }}
              >
                {!modeUpdate && 'Tạo mới SRO'}
                {modeUpdate && 'Cập nhật SRO'}
              </Text>
            </Card.Header>
            <Card.Body>
              <div className={classes.layout}>
                <Form.Item
                  label="Cở sở"
                  name="center_id"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy chọn cơ sở',
                    },
                  ]}
                >
                  <Select
                    disabled={modeUpdate}
                    placeholder="Cơ sở"
                    loading={listCenter.length === 0}
                  >
                    {listCenter.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
              </div>
              <Spacer y={1.5} />
              <div className={classes.layout}>
                <Form.Item
                  label="Họ, tên đệm"
                  name="first_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Trường này không được để trống'
                          );
                        }
                        if (value.trim().length >= 2) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('Trường phải có ít nhất 2 ký tự')
                        );
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Trường không được chứa khoảng trắng',
                    },
                  ]}
                >
                  <Input placeholder="Họ và tên đệm" />
                </Form.Item>
                <Form.Item
                  label="Tên"
                  name="last_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Trường này không được để trống'
                          );
                        }
                        if (value.trim().length >= 2) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('Trường phải có ít nhất 2 ký tự')
                        );
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Trường không được chứa khoảng trắng',
                    },
                  ]}
                >
                  <Input placeholder="Tên" />
                </Form.Item>
                <Form.Item
                  label="Giới tính"
                  name="gender_id"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy nhập giới tính',
                    },
                  ]}
                >
                  <Select
                    placeholder="Giới tính"
                    loading={listGender.length === 0}
                  >
                    {listGender.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.value}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Ngày sinh"
                  name="birthday"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy nhập ngày sinh',
                    },
                  ]}
                >
                  <DatePicker format={'DD/MM/YYYY'} />
                </Form.Item>
                <Form.Item
                  label="Số điện thoại"
                  name="mobile_phone"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        // check regex phone number viet nam
                        if (Validater.isPhone(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('Số điện thoại không hợp lệ')
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="0891234567" />
                </Form.Item>
                <div></div>
                <Form.Item
                  label="Email cá nhân"
                  name="email"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (Validater.isEmail(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Email không hợp lệ'));
                      },
                    },
                  ]}
                >
                  <Input placeholder="example@gmail.com" />
                </Form.Item>
                <Form.Item
                  label="Email tổ chức"
                  name="email_organization"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (Validater.isEmail(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Email không hợp lệ'));
                      },
                    },
                  ]}
                >
                  <Input type="email" placeholder="example@domain.com" />
                </Form.Item>
              </div>
              <Spacer y={1.5} />
              <div className={classes.layout}>
                <Form.Item
                  label="Tỉnh/Thành phố"
                  name="province_id"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy chọn tỉnh/thành phố',
                    },
                  ]}
                >
                  <Select
                    placeholder="Tỉnh/Thành phố"
                    loading={listProvince.length === 0}
                    onChange={() => {
                      getListDistrict();
                    }}
                  >
                    {listProvince.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Quận/Huyện"
                  name="district_id"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy chọn quận/huyện',
                    },
                  ]}
                >
                  <Select
                    placeholder="Quận/Huyện"
                    loading={listDistrict.length === 0}
                    onChange={() => {
                      getListWard();
                    }}
                  >
                    {listDistrict.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.prefix} {e.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Phường/Xã"
                  name="ward_id"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy chọn phường/xã',
                    },
                  ]}
                >
                  <Select
                    placeholder="Phường/Xã"
                    loading={listWard.length === 0}
                  >
                    {listWard.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.prefix} {e.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
              </div>
              <Spacer y={1.5} />
              <div className={classes.layout}>
                <Form.Item
                  label="Số CMND/CCCD"
                  name="citizen_identity_card_no"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (Validater.isCitizenId(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('Số CMND/CCCD không hợp lệ')
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="Số CMND/CCCD" />
                </Form.Item>
                <Form.Item
                  label="Ngày cấp"
                  name="citizen_identity_card_published_date"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy nhập ngày cấp',
                    },
                  ]}
                >
                  <DatePicker format={'DD/MM/YYYY'} />
                </Form.Item>
                <Form.Item
                  label="Nơi cấp"
                  name="citizen_identity_card_published_place"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Trường này không được để trống'
                          );
                        }
                        if (value.trim().length >= 2) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('Trường phải có ít nhất 2 ký tự')
                        );
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Trường không được chứa khoảng trắng',
                    },
                  ]}
                >
                  <Input placeholder="Nơi cấp" />
                </Form.Item>
              </div>
              <Spacer y={1.5} />
              <div className={classes.buttonCreate}>
                <Button type="primary" htmlType="submit" loading={isCreatingOrUpdating}>
                  {!modeUpdate && 'Tạo mới'}
                  {modeUpdate && 'Cập nhật'}
                </Button>
              </div>
            </Card.Body>
          </Card>
        </Grid>
      </Grid.Container>
    </Form>
  );
};

export default SroCreate;
