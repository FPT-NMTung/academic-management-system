import { Grid, Card, Text, Spacer } from '@nextui-org/react';
import { Button, Form, Input, Select, DatePicker, InputNumber } from 'antd';
import classes from './TeacherCreate.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import {
  CenterApis,
  GenderApis,
  AddressApis,
  ManageTeacherApis,
} from '../../../../apis/ListApi';
import FetchApi from '../../../../apis/FetchApi';
import { Validater } from '../../../../validater/Validater';
import moment from 'moment';
import { ErrorCodeApi } from '../../../../apis/ErrorCodeApi';
import { Fragment } from 'react';

const translateWorkingTime = {
  1: 'Sáng',
  2: 'Chiều',
  3: 'Tối',
  4: 'Sáng, Chiều',
  5: 'Sáng, Tối',
  6: 'Chiều, Tối',
};

const TeacherCreate = ({ modeUpdate }) => {
  console.log('modeUpdate', modeUpdate);
  const [listCenter, setListCenter] = useState([]);
  const [listGender, setListGender] = useState([]);
  const [listProvince, setListProvince] = useState([]);
  const [listDistrict, setListDistrict] = useState([]);
  const [listWard, setListWard] = useState([]);
  const [listWorkingTime, setListWorkingTime] = useState([]);
  const [listTeacherType, setListTeacherType] = useState([]);
  const [isCreatingOrUpdating, setIsCreatingOrUpdating] = useState(false);
  const [messageFailed, setMessageFailed] = useState(undefined);
  const [isGettingInformationTeacher, setIsGettingInformationTeacher] =
    useState(true);

  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { id } = useParams();

  const getListCenter = () => {
    FetchApi(CenterApis.getAllCenter, null, null, null).then((res) => {
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

  const getListWorkingTime = () => {
    FetchApi(ManageTeacherApis.getWorkingTime).then((res) => {
      setListWorkingTime(
        res.data.map((item) => {
          return {
            id: item.id,
            value: translateWorkingTime[item.id],
          };
        })
      );
    });
  };

  const getListTeacherType = () => {
    FetchApi(ManageTeacherApis.getTeacherType).then((res) => {
      setListTeacherType(res.data);
    });
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

  const getInformationTeacher = () => {
    setIsGettingInformationTeacher(true);

    FetchApi(ManageTeacherApis.getInformationTeacher, null, null, [
      `${id}`,
    ]).then((res) => {
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
        teacher_type_id: data.teacher_type.id,
        working_time_id: data.working_time.id,
        nickname: data.nickname,
        company_address: data.company_address,
        start_working_date: moment(data.start_working_date),
        salary: data.salary,
        tax_code: data.tax_code,
      });

      setIsGettingInformationTeacher(false);
      getListDistrictForUpdate();
      getListWardForUpdate();
    });
  };

  const handleSubmitForm = () => {
    const data = form.getFieldsValue();
    setIsCreatingOrUpdating(true);
    setMessageFailed(undefined);

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
      birthday: data.birthday.add(7, 'hours').toDate(),
      center_id: data.center_id,
      citizen_identity_card_no: data.citizen_identity_card_no.trim(),
      citizen_identity_card_published_date:
        data.citizen_identity_card_published_date.add(7, 'hours').toDate(),
      citizen_identity_card_published_place:
        data.citizen_identity_card_published_place.trim(),
      teacher_type_id: data.teacher_type_id,
      working_time_id: data.working_time_id,
      nickname: data.nickname.trim(),
      company_address: data.company_address.trim(),
      start_working_date: data.start_working_date.add(7, 'hours').toDate(),
      salary: data.salary,
      tax_code: data.tax_code.trim(),
    };

    const api = modeUpdate
      ? ManageTeacherApis.updateTeacher
      : ManageTeacherApis.createTeacher;
    const params = modeUpdate ? [`${id}`] : null;
    FetchApi(api, body, null, params)
      .then((res) => {
        navigate(`/admin/account/teacher/${res.data.user_id}`);
      })
      .catch((err) => {
        setIsCreatingOrUpdating(false);
        setMessageFailed(ErrorCodeApi[err.type_error]);
      });
  };

  useEffect(() => {
    getListCenter();
    getListGender();
    getListProvince();
    getListWorkingTime();
    getListTeacherType();

    if (modeUpdate) {
      getInformationTeacher();
    }
  }, []);

  return (
    <Form
      labelCol={{ span: 7 }}
      wrapperCol={{ span: 15 }}
      form={form}
      onFinish={handleSubmitForm}
      disabled={modeUpdate && isGettingInformationTeacher}
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
                {!modeUpdate && 'Tạo giáo viên mới'}
                {modeUpdate && 'Cập nhật thông tin giáo viên'}
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
                  <Select placeholder="Cơ sở">
                    {listCenter.map((item) => (
                      <Select.Option value={item.id}>{item.name}</Select.Option>
                    ))}
                  </Select>
                </Form.Item>
              </div>
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
                        if (
                          Validater.isContaintSpecialCharacterForName(
                            value.trim()
                          )
                        ) {
                          return Promise.reject(
                            'Trường này không được chứa ký tự đặc biệt'
                          );
                        }
                        if (value.trim().length < 2) {
                          return Promise.reject(
                            new Error('Trường phải có ít nhất 2 ký tự')
                          );
                        }
                        return Promise.resolve();
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
                        if (
                          Validater.isContaintSpecialCharacterForName(
                            value.trim()
                          )
                        ) {
                          return Promise.reject(
                            'Trường này không được chứa ký tự đặc biệt'
                          );
                        }
                        if (value.trim().length < 2) {
                          return Promise.reject(
                            new Error('Trường phải có ít nhất 2 ký tự')
                          );
                        }
                        return Promise.resolve();
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
                  <Select placeholder="Giới tính">
                    {listGender.map((item) => (
                      <Select.Option value={item.id}>
                        {item.value}
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
                        if (
                          Validater.isContaintSpecialCharacterForName(
                            value.trim()
                          )
                        ) {
                          return Promise.reject(
                            'Trường này không được chứa ký tự đặc biệt'
                          );
                        }
                        if (value.trim().length < 2) {
                          return Promise.reject(
                            new Error('Trường phải có ít nhất 2 ký tự')
                          );
                        }
                        return Promise.resolve();
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
              <div className={classes.layout}>
                <Form.Item
                  label="Biệt danh"
                  name="nickname"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Trường này không được để trống'
                          );
                        }
                        if (
                          Validater.isContaintSpecialCharacterForName(
                            value.trim()
                          )
                        ) {
                          return Promise.reject(
                            'Trường này không được chứa ký tự đặc biệt'
                          );
                        }
                        if (value.trim().length < 2) {
                          return Promise.reject(
                            new Error('Trường phải có ít nhất 2 ký tự')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Trường không được chứa khoảng trắng',
                    },
                  ]}
                >
                  <Input placeholder="Biệt danh" />
                </Form.Item>
                <Form.Item
                  label="Ngày bắt đầu"
                  name="start_working_date"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy nhập ngày bắt đầu',
                    },
                  ]}
                >
                  <DatePicker format={'DD/MM/YYYY'} />
                </Form.Item>
                <Form.Item
                  label="Nơi công tác"
                  name="company_address"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Trường này không được để trống'
                          );
                        }
                        if (
                          Validater.isContaintSpecialCharacterForName(
                            value.trim()
                          )
                        ) {
                          return Promise.reject(
                            'Trường này không được chứa ký tự đặc biệt'
                          );
                        }
                        if (value.trim().length < 2) {
                          return Promise.reject(
                            new Error('Trường phải có ít nhất 2 ký tự')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Trường không được chứa khoảng trắng',
                    },
                  ]}
                >
                  <Input placeholder="Nơi công tác" />
                </Form.Item>
                <Form.Item
                  label="Loại hợp đồng"
                  name="teacher_type_id"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy chọn loại hợp đồng',
                    },
                  ]}
                >
                  <Select placeholder="Loại hợp đồng">
                    {listTeacherType.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.value}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Mã số thuế"
                  name="tax_code"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (Validater.isTaxCode(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('Mã số thuế không hợp lệ')
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="Mã số thuế" />
                </Form.Item>
                <Form.Item
                  label="Thời gian dạy"
                  name="working_time_id"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy chọn thời gian dạy',
                    },
                  ]}
                >
                  <Select placeholder="Thời gian dạy">
                    {listWorkingTime.map((item) => (
                      <Select.Option key={item.id} value={item.id}>
                        {item.value}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Mức lương"
                  name="salary"
                  rules={[
                    {
                      required: true,
                      message: 'Hãy nhập mức lương',
                    },
                  ]}
                >
                  <InputNumber
                    placeholder="Mức lương"
                    formatter={(value) =>
                      `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
                    }
                    parser={(value) => value.replace(/\$\s?|(,*)/g, '')}
                    style={{ width: '100%' }}
                    addonAfter="VNĐ"
                  />
                </Form.Item>
              </div>
              <div className={classes.buttonCreate}>
                <Button
                  type="primary"
                  htmlType="submit"
                  loading={isCreatingOrUpdating}
                >
                  {!modeUpdate && 'Tạo mới'}
                  {modeUpdate && 'Cập nhật'}
                </Button>
                {!isCreatingOrUpdating && messageFailed !== undefined && (
                  <Fragment>
                    <Spacer x={0.5} />
                    <Text color="error" size={15}>
                      {messageFailed}, vui lòng thử lại
                    </Text>
                  </Fragment>
                )}
              </div>
            </Card.Body>
          </Card>
        </Grid>
      </Grid.Container>
    </Form>
  );
};

export default TeacherCreate;
