import {
  Grid,
  Card,
  Text,
  Spacer,
  Button,
  Loading,
  Switch,
  Badge,
} from '@nextui-org/react';
import { UploadOutlined } from '@ant-design/icons';
import {
  Form,
  Input,
  Select,
  DatePicker,
  InputNumber,
  Descriptions,
  Divider,
  Image,
  Upload,
  message,
} from 'antd';
import { useState } from 'react';
import { useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import classes from '../../Student/StudentUpdate/StudentUpdate.module.css';
import toast from 'react-hot-toast';
import FetchApi from '../../../../apis/FetchApi';
import { Validater } from '../../../../validater/Validater';
import {
  CenterApis,
  GenderApis,
  AddressApis,
  CourseApis,
  ManageClassApis,
} from '../../../../apis/ListApi';
import ManImage from '../../../../images/3d-fluency-businessman-1.png';
import WomanImage from '../../../../images/3d-fluency-businesswoman-1.png';
import moment from 'moment';
import { ErrorCodeApi } from '../../../../apis/ErrorCodeApi';
const translateStatusStudent = {
  1: 'Studying',
  2: 'Delay',
  3: 'Dropout',
  4: 'Finished',
};

const AddStudentToClass = () => {
  const [listGender, setListGender] = useState([]);
  const [listCourses, setListCourses] = useState([]);
  const [listProvince, setListProvince] = useState([]);
  const [listDistrict, setListDistrict] = useState([]);
  const [listWard, setListWard] = useState([]);
  const [isCreatingOrUpdating, setIsCreatingOrUpdating] = useState(false);
  const [messageFailed, setMessageFailed] = useState(undefined);
  const [isGettingInformationStudent, setIsGettingInformationStudent] =
    useState(true);
  const [dataUser, setDataUser] = useState(undefined);

  const navigate = useNavigate();
  const [form] = Form.useForm();
  const { id } = useParams();
  const getListCourse = () => {
    FetchApi(CourseApis.getAllCourse).then((res) => {
      setListCourses(res.data);
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
    const data = form.getFieldsValue();

    setIsCreatingOrUpdating(true);
    setMessageFailed(undefined);

    const body = {
      first_name: data.first_name?.trim(),
      last_name: data.last_name?.trim(),
      mobile_phone: data.mobile_phone?.trim(),
      email: data.email?.trim(),
      email_organization: data.email_organization?.trim(),
      province_id: data.province_id,
      district_id: data.district_id,
      ward_id: data.ward_id,
      gender_id: data.gender_id,
      birthday: moment.utc(data.birthday).local().format(),
      citizen_identity_card_no: data.citizen_identity_card_no?.trim(),
      citizen_identity_card_published_date: moment
        .utc(data.citizen_identity_card_published_date)
        .local()
        .format(),
      citizen_identity_card_published_place:
        data.citizen_identity_card_published_place,
      status: data.status,
      contact_phone: data.mobile_phone,
      parental_phone: data.parental_phone,
      parental_name: data.parental_name,
      application_date: moment.utc(data.application_date).local().format(),
      fee_plan: data.fee_plan,
      promotion: data.promotion,
      contact_address: data.contact_address,
      parental_relationship: data.parental_relationship,
      course_code: data.course_code,
      application_document: data.application_document
        ? data.application_document
        : null,
      high_school: data.high_school ? data.high_school : null,
      university: data.university ? data.university : null,
      facebook_url: data.facebook_url ? data.facebook_url : null,
      portfolio_url: data.portfolio_url ? data.portfolio_url : null,
      working_company: data.working_company ? data.working_company : null,
      company_salary: data.company_salary ? data.company_salary : null,
      company_position: data.company_position ? data.company_position : null,
      company_address: data.company_address ? data.company_address : null,
      enroll_number: data.enroll_number ? data.enroll_number : null,
      home_phone: data.parental_phone ? data.parental_phone : null,
    };
    console.log(body);

    const api = ManageClassApis.addStudentToClass;
    const params = [`${id}`];
    console.log(params);

    toast.promise(FetchApi(api, body, null, params), {
      loading: '??ang x??? l??',
      success: (res) => {
        setIsCreatingOrUpdating(false);
        navigate(`/sro/manage-class/${id}`);
        return 'Th??nh c??ng';
      },
      error: (err) => {
        setMessageFailed(ErrorCodeApi[err.type_error]);
        setIsCreatingOrUpdating(false);
        if (err?.type_error) {
          return ErrorCodeApi[err.type_error];
        }

        return 'Th???t b???i';
      },
    });
  };
  const handleCancel = () => {
    navigate(`/sro/manage-class/${id}`);
  };

  useEffect(() => {
    getListGender();
    getListProvince();
    // getListWorkingTime();
    // getListTeacherType();
    getListCourse();
  }, []);
  return (
    <Form
      labelCol={{ span: 7 }}
      wrapperCol={{ span: 15 }}
      form={form}
      onFinish={handleSubmitForm}
      // disabled={modeUpdate && isGettingInformationStudent}
      initialValues={{
        status: 1,
      }}
    >
      <Grid.Container justify="center" gap={2}>
        <Grid sm={6.5} direction={'column'} css={{ rowGap: 20 }}>
          <Card variant="bordered">
            <Card.Header css={{ margin: '12px 0 0 0' }}>
              <Text
                b
                size={17}
                p
                css={{
                  width: '100%',
                  textAlign: 'center',
                }}
              >
                T???o h???c vi??n m???i
              </Text>
            </Card.Header>
            <Card.Body>
              <div className={classes.layout}>
                {/* Th??ng tin c?? nh??n */}
                <Form.Item
                  label="H??? & t??n"
                  name="first_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Tr?????ng ph???i t??? 1 ?????n 255 k?? t???'
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="H??? v?? t??n" />
                </Form.Item>
                <Form.Item
                  label="T??n"
                  name="last_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Tr?????ng ph???i t??? 1 ?????n 255 k?? t???'
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="T??n" />
                </Form.Item>
                <Form.Item
                  label="Gi???i t??nh"
                  name="gender_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y nh???p gi???i t??nh',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="Gi???i t??nh"
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listGender.map((item, index) => (
                      <Select.Option key={index} value={item.id}>
                        {item.value}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Ng??y sinh"
                  name="birthday"
                  rules={[
                    {
                      required: true,
                      message: 'H??y nh???p ng??y sinh',
                    },
                  ]}
                >
                  <DatePicker format={'DD/MM/YYYY'} />
                </Form.Item>
              </div>

              <Divider
                orientation="left"
                style={{ marginTop: 0, marginBottom: 24 }}
              >
                <Text
                  b
                  p
                  size={15}
                  css={{
                    width: '100%',
                    textAlign: 'center',
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Th??ng tin li??n h???
                </Text>
              </Divider>
              <div></div>
              <div className={classes.layout}>
                {/* Th??ng tin li??n h??? */}
                <Form.Item
                  label="T???nh/Th??nh"
                  name="province_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y ch???n t???nh/th??nh ph???',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="T???nh/Th??nh ph???"
                    loading={listProvince.length === 0}
                    onChange={() => {
                      getListDistrict();
                    }}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listProvince.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Qu???n/Huy???n"
                  name="district_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y ch???n qu???n/huy???n',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="Qu???n/Huy???n"
                    disabled={listDistrict.length === 0}
                    loading={listDistrict.length === 0}
                    onChange={() => {
                      getListWard();
                    }}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listDistrict.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {`${e.prefix} ${e.name}`}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Ph?????ng/X??"
                  name="ward_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y ch???n ph?????ng/x??',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="Ph?????ng/X??"
                    disabled={listWard.length === 0}
                    loading={listWard.length === 0}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listWard.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {`${e.prefix} ${e.name}`}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="?????a ch??? c??? th???"
                  name="contact_address"
                  style={{
                    // margin: "auto",
                    width: '100%',
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ''
                        ) {
                          return Promise.reject('Tr?????ng kh??ng ???????c ????? tr???ng');
                        }
                        if (
                          Validater.isContaintSpecialCharacterForAddress(
                            value.trim()
                          )
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="?????a ch??? li??n h??? c??? th???" />
                </Form.Item>
                <Form.Item
                  label="Email c?? nh??n"
                  name="email"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value === ''
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ????? tr???ng'
                          );
                        }
                        if (Validater.isEmail(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Email kh??ng h???p l???'));
                      },
                    },
                  ]}
                >
                  <Input placeholder="example@gmail.com" />
                </Form.Item>
                <Form.Item
                  label="Email t??? ch???c"
                  name="email_organization"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value === ''
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ????? tr???ng'
                          );
                        }
                        if (Validater.isEmail(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Email kh??ng h???p l???'));
                      },
                    },
                  ]}
                >
                  <Input type="email" placeholder="example@domain.com" />
                </Form.Item>
                <Form.Item
                  label="S??? ??i???n tho???i"
                  name="mobile_phone"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value === ''
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ????? tr???ng'
                          );
                        }
                        // check regex phone number viet nam
                        if (Validater.isPhone(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('S??? ??i???n tho???i kh??ng h???p l???')
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="0891234567" />
                </Form.Item>
              </div>

              <Divider
                orientation="left"
                style={{ marginTop: 0, marginBottom: 24 }}
              >
                <Text
                  b
                  p
                  size={15}
                  css={{
                    width: '100%',
                    textAlign: 'center',
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Th??ng tin ph??? huynh
                </Text>
              </Divider>
              <div></div>
              <div className={classes.layout}>
                <Form.Item
                  label="H??? & t??n"
                  name="parental_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Tr?????ng ph???i t??? 1 ?????n 255 k?? t???'
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="H??? v?? t??n" />
                </Form.Item>

                <Form.Item
                  label="S??? ??i???n tho???i"
                  name="parental_phone"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value === ''
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ????? tr???ng'
                          );
                        }
                        // check regex phone number viet nam
                        if (Validater.isPhone(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('S??? ??i???n tho???i kh??ng h???p l???')
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="0891234567" />
                </Form.Item>
                <Form.Item
                  label="Quan h???"
                  name="parental_relationship"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Tr?????ng ph???i t??? 1 ?????n 255 k?? t???'
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="L?? ... c???a h???c vi??n"
                    dropdownStyle={{ zIndex: 9999 }}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    <Select.Option key={9000} value="B???">
                      B???
                    </Select.Option>
                    <Select.Option key={9001} value="M???">
                      M???
                    </Select.Option>
                    <Select.Option key={9002} value="??ng">
                      ??ng
                    </Select.Option>
                    <Select.Option key={9003} value="B??">
                      B??
                    </Select.Option>
                    <Select.Option key={9004} value="Anh">
                      Anh
                    </Select.Option>
                    <Select.Option key={9005} value="Ch???">
                      Ch???
                    </Select.Option>
                    <Select.Option key={9006} value="Kh??c">
                      Kh??c
                    </Select.Option>
                  </Select>
                </Form.Item>
                <div></div>
              </div>
              <Divider
                orientation="left"
                style={{ marginTop: 0, marginBottom: 24 }}
              >
                <Text
                  b
                  p
                  size={15}
                  css={{
                    width: '100%',
                    textAlign: 'center',
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Th??ng tin li??n quan ?????n h???c vi???n
                </Text>
              </Divider>
              <div></div>
              <div className={classes.layout}>
                <Form.Item
                  label="H???c b???ng"
                  name="promotion"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value === ''
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ????? tr???ng'
                          );
                        }
                        const salary = value.toString();
                        if (Validater.isNumber(salary)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Ph???i l?? s???'));

                        // check regex phone number viet nam
                      },
                    },
                  ]}
                >
                  <Input placeholder="20" />
                </Form.Item>
                <Form.Item
                  label="K??? ho???ch ph??"
                  name="fee_plan"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value === ''
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ????? tr???ng'
                          );
                        }

                        const salary = value.toString();
                        if (Validater.isNumber(salary)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Ph???i l?? s???'));

                        // check regex phone number viet nam
                      },
                    },
                  ]}
                >
                  <Input placeholder="20" />
                </Form.Item>
                <Form.Item
                  label="H??? s??"
                  name="application_document"
                  style={{
                    // margin: "auto",
                    width: '100%',
                    // textAlign: "left",
                  }}
                  // rules={[
                  //   {
                  //     required: true,
                  //     message: "H??y nh???p ???????ng d???n c???a h??? s??",
                  //   },
                  // ]}
                >
                  <Input placeholder="???????ng d???n t???i h??? s??" />
                </Form.Item>
                <Form.Item
                  label="Ng??y n???p"
                  name="application_date"
                  style={{
                    // margin: "auto",
                    width: '100%',
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: true,
                      message: 'H??y nh???p ng??y n???p h??? s??',
                    },
                  ]}
                >
                  <DatePicker
                    placeholder="Ng??y n???p h??? s??"
                    format={'DD/MM/YYYY'}
                  />
                </Form.Item>
                <div></div>
              </div>
              <Divider
                orientation="left"
                style={{ marginTop: 0, marginBottom: 24 }}
              >
                <Text
                  b
                  p
                  size={15}
                  css={{
                    width: '100%',
                    textAlign: 'center',
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  H???c v???n v?? c??ng vi???c
                </Text>
              </Divider>
              <div></div>
              <div className={classes.layout}>
                <Form.Item
                  label="Tr?????ng c???p 3"
                  name="high_school"
                  style={{
                    // margin: "auto",
                    width: '100%',
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ''
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="T??n tr?????ng h???c" />
                </Form.Item>
                <Form.Item
                  label="Tr?????ng ?????i h???c"
                  name="university"
                  style={{
                    // margin: "auto",
                    width: '100%',
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ''
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="T??n tr?????ng h???c" />
                </Form.Item>
                <Form.Item
                  label="C??ng ty"
                  name="working_company"
                  style={{
                    // margin: "auto",
                    width: '100%',
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ''
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="T??n c??ng ty" />
                </Form.Item>
                <Form.Item
                  label="?????a ch??? c??ng ty"
                  name="company_address"
                  style={{
                    // margin: "auto",
                    width: '100%',
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ''
                        ) {
                          return Promise.resolve();
                        }
                        if (
                          Validater.isContaintSpecialCharacter(value.trim())
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="?????a ch??? c??ng ty" />
                </Form.Item>
                <Form.Item
                  label="Ch???c v???"
                  name="company_position"
                  style={{
                    // margin: "auto",
                    width: '100%',
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ''
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="Ch???c v??? trong c??ng ty" />
                </Form.Item>
                <Form.Item
                  label="M???c l????ng"
                  name="company_salary"
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value === ''
                        ) {
                          return Promise.resolve();
                        }
                        const salary = value.toString();
                        if (Validater.isNumber(salary)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Ph???i l?? s???'));
                      },
                    },
                    // {
                    //   whitespace: true,
                    //   message: "Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng",
                    // },
                  ]}
                >
                  <InputNumber
                    min={0}
                    placeholder="5000000"
                    formatter={(value) =>
                      `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
                    }
                    parser={(value) => value.replace(/\$\s?|(,*)/g, '')}
                    style={{ width: '100%' }}
                  />
                </Form.Item>

                <div></div>
              </div>
              <Spacer y={0.2} />
            </Card.Body>
          </Card>
        </Grid>

        <Grid
          sm={4.5}
          direction={'column'}
          css={{ rowGap: 20, position: 'relative' }}
        >
          <Card
            variant="bordered"
            css={{ backgroundColor: 'transparent', border: 'none' }}
          >
            <Card.Header css={{ margin: '12px 0 0 0' }}>
              <Text
                p
                b
                size={17}
                css={{
                  width: '100%',
                  textAlign: 'center',
                  margin: '0',
                }}
              >
                ???nh ?????i di???n
              </Text>
            </Card.Header>
            <Card.Body
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              <div className={classes.contantLogo}>
                <div className={classes.logo}>
                  <Image
                    preview={false}
                    className={classes.avatarDefault}
                    width={250}
                    src="https://cdn-icons-png.flaticon.com/512/149/149071.png"
                  />
                  {/* {dataStudent.avatar && (
                      <img className={classes.avatar} src={dataStudent.avatar} />
                    )} */}

                  {/* {!dataStudent.avatar && (
                      <img
                        className={classes.avatarMini}
                        src={
                          dataStudent.gender.id === 1
                            ? ManImage
                            : dataStudent.gender.id === 2
                            ? WomanImage
                            : ""
                        }
                      />
                    )} */}
                </div>
              </div>
              {/* <Upload disabled={true}>
                <Button
                  disabled={true}
                  css={{
                    fontSize: '12px',
                    height: '28px',
                    margin: '16px 0 0 0',
                  }}
                  auto
                  flat
                  icon={<UploadOutlined />}
                >
                  T???i l??n
                </Button>
              </Upload> */}

              {/* </div> */}
            </Card.Body>
          </Card>
          <Card variant="bordered">
            <Card.Header css={{ margin: '0px 0 0 0' }}>
              <Text
                b
                p
                size={15}
                css={{
                  width: '100%',
                  textAlign: 'center',
                  //   margin: '0',
                  //   padding: '0',
                }}
              >
                Th??ng tin quan tr???ng
              </Text>
            </Card.Header>
            <Card.Body
              css={{
                width: '100%',
                //   textAlign: "center",
              }}
            >
              <Form.Item
                name="enroll_number"
                label="M?? s??? sinh vi??n"
                labelWrap={true}
                style={{
                  // margin: "auto",
                  width: '100%',
                  textAlign: 'left',
                }}
                rules={[
                  {
                    required: true,
                    validator: (_, value) => {
                      if (
                        value === null ||
                        value === undefined ||
                        value === ''
                      ) {
                        return Promise.reject('Tr?????ng ph???i t??? 1 ?????n 100 k?? t???');
                      }

                      if (Validater.isContaintSpecialCharacter(value.trim())) {
                        return Promise.reject(
                          'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                        );
                      }
                      if (
                        value.trim().length < 1 ||
                        value.trim().length > 100
                      ) {
                        return Promise.reject(
                          new Error('Tr?????ng ph???i t??? 1 ?????n 100 k?? t???')
                        );
                      } else {
                        return Promise.resolve();
                      }
                    },
                  },
                  {
                    whitespace: true,
                    message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                  },
                ]}
              >
                <Input placeholder="H??y nh???p m?? s??? sinh vi??n" />
              </Form.Item>
              <Form.Item
                name="status"
                label="T??nh tr???ng"
                labelWrap={true}
                style={{
                  // margin: "auto",
                  width: '100%',
                  textAlign: 'left',
                }}
              >
                <Select
                  showSearch
                  placeholder="T??nh tr???ng h???c vi??n"
                  dropdownStyle={{ zIndex: 9999 }}
                  filterOption={(input, option) =>
                    option.children.toLowerCase().includes(input.toLowerCase())
                  }
                  defaultValue={1}
                  disabled={true}
                >
                  <Select.Option key={100} value={1}>
                    Studying
                  </Select.Option>
                  <Select.Option key={101} value={2}>
                    Delay
                  </Select.Option>
                  <Select.Option key={102} value={3}>
                    Dropout
                  </Select.Option>
                  <Select.Option key={106} value={4}>
                    Finished
                  </Select.Option>
                </Select>
              </Form.Item>
              <Form.Item
                name="course_code"
                label="M?? kh??a h???c"
                labelWrap={true}
                style={{
                  // margin: "auto",
                  width: '100%',
                  textAlign: 'left',
                }}
                rules={[
                  {
                    required: true,
                    message: 'H??y ch???n kh??a h???c',
                  },
                ]}
              >
                <Select
                  showSearch
                  dropdownStyle={{ zIndex: 9999 }}
                  placeholder="Ch???n kh??a h???c"
                  onChange={getListCourse}
                  filterOption={(input, option) =>
                    option.children.toLowerCase().includes(input.toLowerCase())
                  }
                >
                  {listCourses.map((e, index) => (
                    <Select.Option key={index} value={e.code}>
                      {e.code}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </Card.Body>
          </Card>
          <Card variant="bordered">
            <Card.Header css={{ margin: '0px 0 0 0' }}>
              <Text
                b
                p
                size={15}
                css={{
                  width: '100%',
                  textAlign: 'center',
                  //   margin: '0',
                  //   padding: '0',
                }}
              >
                Th??ng tin CMND/CCCD
              </Text>
            </Card.Header>
            <Card.Body
              css={{
                width: '100%',
                //   textAlign: "center",
              }}
            >
              <Form.Item
                label="S??? CMND/CCCD"
                name="citizen_identity_card_no"
                rules={[
                  {
                    required: true,
                    validator: (_, value) => {
                      if (
                        value === null ||
                        value === undefined ||
                        value === ''
                      ) {
                        return Promise.reject('Tr?????ng n??y kh??ng ???????c ????? tr???ng');
                      }
                      if (Validater.isCitizenId(value)) {
                        return Promise.resolve();
                      }
                      return Promise.reject(
                        new Error('S??? CMND/CCCD kh??ng h???p l???')
                      );
                    },
                  },
                ]}
              >
                <Input placeholder="CMND/CCCD" />
              </Form.Item>
              <Form.Item
                label="Ng??y c???p"
                name="citizen_identity_card_published_date"
                rules={[
                  {
                    required: true,
                    message: 'H??y nh???p ng??y c???p',
                  },
                ]}
              >
                <DatePicker format={'DD/MM/YYYY'} />
              </Form.Item>
              <Form.Item
                label="N??i c???p"
                name="citizen_identity_card_published_place"
                rules={[
                  {
                    required: true,
                    validator: (_, value) => {
                      if (value === null || value === undefined) {
                        return Promise.reject('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???');
                      }
                      if (
                        Validater.isContaintSpecialCharacterForName(
                          value.trim()
                        )
                      ) {
                        return Promise.reject(
                          'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                        );
                      }
                      if (
                        value.trim().length < 1 ||
                        value.trim().length > 255
                      ) {
                        return Promise.reject(
                          new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                        );
                      }
                      return Promise.resolve();
                    },
                  },
                  {
                    whitespace: true,
                    message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                  },
                ]}
              >
                <Input placeholder="N??i c???p" />
              </Form.Item>
            </Card.Body>
          </Card>
          <Card variant="bordered">
            <Card.Header css={{ margin: '0px 0 0 0' }}>
              <Text
                b
                p
                size={15}
                css={{
                  width: '100%',
                  textAlign: 'center',
                  //   margin: '0',
                  //   padding: '0',
                }}
              >
                Th??ng tin b??? sung
              </Text>
            </Card.Header>
            <Card.Body
              css={{
                width: '100%',
                //   textAlign: "center",
              }}
            >
              <Form.Item
                label="Facebook"
                name="facebook_url"
                style={{
                  // margin: "auto",
                  width: '100%',
                  // textAlign: "left",
                }}
                // rules={[
                //   {
                //     // required: true,

                //     message: "H??y nh???p ???????ng d???n facebook",
                //   },
                // ]}
              >
                <Input placeholder="???????ng d???n t???i facebook" />
              </Form.Item>

              <Form.Item
                label="CV"
                name="portfolio_url"
                style={{
                  // margin: "auto",
                  width: '100%',
                  // textAlign: "left",
                }}
                // rules={[
                //   {
                //     // required: true,
                //     message: "H??y nh???p ???????ng d???n t???i cv",
                //   },
                // ]}
              >
                <Input placeholder="???????ng d???n t???i cv" />
              </Form.Item>
            </Card.Body>
          </Card>
          <Form.Item
            style={{
              display: 'inline-block',
              textAlign: 'right',
              width: '100%',
            }}
          >
            {/* {!isCreatingOrUpdating && messageFailed !== undefined && (
                  <Text
                    size={14}
                    css={{
                      background: "#fff",
                      color: "red",
                    }}
                  >
                    {messageFailed}
                  </Text>
                )} */}
          </Form.Item>

          <div className={classes.buttonCreate}>
            <Button
              flat
              auto
              css={{
                width: '150px',
                position: 'absolute',
                right: '10px',
                bottom: '10px',
              }}
              type="primary"
              htmlType="submit"
              // disabled={isCreatingOrUpdating}
            >
              T???o m???i
            </Button>
            <Button
              flat
              auto
              color="error"
              css={{
                width: '150px',
                position: 'absolute',
                right: '180px',
                bottom: '10px',
              }}
              onPress={() => {
                handleCancel();
              }}
              // disabled={isCreatingOrUpdating}
            >
              H???y
            </Button>
          </div>
        </Grid>
      </Grid.Container>
    </Form>
  );
};
export default AddStudentToClass;
