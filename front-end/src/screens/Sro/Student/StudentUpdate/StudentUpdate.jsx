import {
  Grid,
  Card,
  Text,
  Spacer,
  Button,
  Loading,
  Switch,
  Badge,
} from "@nextui-org/react";
import { UploadOutlined } from "@ant-design/icons";
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
} from "antd";
import { useState } from "react";
import { useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import classes from "./StudentUpdate.module.css";
import toast from "react-hot-toast";
import FetchApi from "../../../../apis/FetchApi";
import { Validater } from "../../../../validater/Validater";
import {
  CenterApis,
  GenderApis,
  AddressApis,
  CourseApis,
  ManageStudentApis,
} from "../../../../apis/ListApi";
import ManImage from "../../../../images/3d-fluency-businessman-1.png";
import WomanImage from "../../../../images/3d-fluency-businesswoman-1.png";
import moment from "moment";
import { ErrorCodeApi } from "../../../../apis/ErrorCodeApi";
const translateStatusStudent = {
  1: "Studying",
  2: "Delay",
  3: "Dropout",
  4: "ClassQueue",
  5: "Transfer",
  6: "Upgrade",
  7: "Finished",
};
// const translateStatusId = {
//      "Studying": 1,
//      "Delay" :2,
//      "Dropout":3,
//      "ClassQueue":4,
//      "Transfer":5,
//      "Upgrade":6,
//      "Finished":7,
//   };

const StudentUpdate = () => {
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
    const provinceId = form.getFieldValue("province_id");

    FetchApi(AddressApis.getListDistrict, null, null, [`${provinceId}`]).then(
      (res) => {
        setListDistrict(res.data);
      }
    );
    setListWard([]);
    form.setFieldsValue({ district_id: null, ward_id: null });
  };

  const getListWard = () => {
    const provinceId = form.getFieldValue("province_id");
    const districtId = form.getFieldValue("district_id");

    FetchApi(AddressApis.getListWard, null, null, [
      `${provinceId}`,
      `${districtId}`,
    ]).then((res) => {
      setListWard(res.data);
    });

    form.setFieldsValue({ ward_id: null });
  };
  const getListDistrictForUpdate = () => {
    const provinceId = form.getFieldValue("province_id");

    FetchApi(AddressApis.getListDistrict, null, null, [`${provinceId}`]).then(
      (res) => {
        setListDistrict(res.data);
      }
    );
  };

  const getListWardForUpdate = () => {
    const provinceId = form.getFieldValue("province_id");
    const districtId = form.getFieldValue("district_id");

    FetchApi(AddressApis.getListWard, null, null, [
      `${provinceId}`,
      `${districtId}`,
    ]).then((res) => {
      setListWard(res.data);
    });
  };
  const getInformationStudent = () => {
    setIsGettingInformationStudent(true);

    FetchApi(ManageStudentApis.getInformationStudent, null, null, [
      `${id}`,
    ]).then((res) => {
      const data = res.data;
      console.log(data);
      setDataUser(data);

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
        citizen_identity_card_no: data.citizen_identity_card_no,
        citizen_identity_card_published_date: moment(
          data.citizen_identity_card_published_date
        ),
        citizen_identity_card_published_place:
          data.citizen_identity_card_published_place,
        status: data.status,
        contact_phone: data.mobile_phone,
        parental_name: data.parental_name,
        // parental_name: `${data.parental_first_name} ${data.parental_last_name}`,
        parental_relationship: data.parental_relationship,
        // contact_address: `${data.ward.prefix}` `${data.ward.name}``${data.district.prefix}``${data.district.name}``${data.province.name}`,

        parental_phone: data.parental_phone,
        application_date: moment(data.application_date),
        fee_plan: data.fee_plan,
        promotion: data.promotion,
        course_code: data.course_code,
        application_document: data.application_document,
        high_school: data.high_school,
        university: data.university,
        facebook_url: data.facebook_url,
        portfolio_url: data.portfolio_url,
        working_company: data.working_company,
        company_salary: data.company_salary,
        company_position: data.company_position,
        company_address: data.company_address,
      });

      setIsGettingInformationStudent(false);
      getListDistrictForUpdate();
      getListWardForUpdate();
    });
  };

  const handleSubmitForm = () => {
    const data = form.getFieldsValue();
    const companysalary = form.getFieldValue("company_salary");

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
      birthday: data.birthday.add(7, "hours").toDate(),
      center_id: data.center_id,
      citizen_identity_card_no: data.citizen_identity_card_no?.trim(),
      citizen_identity_card_published_date:
        data.citizen_identity_card_published_date.add(7, "hours").toDate(),
      citizen_identity_card_published_place:
        data.citizen_identity_card_published_place,
      status: data.status,
      contact_phone: data.mobile_phone,
      parental_phone: data.parental_phone,
      parental_name: data.parental_name,
      application_date: data.application_date.add(7, "hours").toDate(),
      fee_plan: data.fee_plan,
      promotion: data.promotion,
      contact_address: `lolll`,
      parental_relationship: data.parental_relationship,
      course_code: data.course_code,
      application_document: data.application_document,
      high_school: data.high_school,
      university: data.university,
      facebook_url: data.facebook_url,
      portfolio_url: data.portfolio_url,
      working_company: data.working_company ? data.working_company : null,
      company_salary: data.company_salary ? data.company_salary : null,
      company_position: data.company_position,
      company_address: data.company_address,
    };
    console.log(body);

    const api =  ManageStudentApis.updateStudent;
    const params = [`${id}`];

    toast.promise(FetchApi(api, body, null, params), {
      loading: "Đang xử lý",
      success: (res) => {
        setIsCreatingOrUpdating(false);
        navigate(`/sro/manage/student/${res.data.user_id}`);
        return "Thành công";
      },
      error: (err) => {
        setMessageFailed(ErrorCodeApi[err.type_error]);
        setIsCreatingOrUpdating(false);
        if (err?.type_error) {
          return ErrorCodeApi[err.type_error];
        }
        return "Thất bại";
      },
    });
  };

  useEffect(() => {
    getListGender();
    getListProvince();
    // getListWorkingTime();
    // getListTeacherType();
    getListCourse();
    getInformationStudent();
  }, []);
  return (
    <Form
      labelCol={{ span: 7 }}
      wrapperCol={{ span: 15 }}
      form={form}
      onFinish={handleSubmitForm}
      // disabled={modeUpdate && isGettingInformationStudent}
      // initialValues = {{
      //   application_document: null,
      //   high_school: null,
      //   university: null,
      //   facebook_url: null,
      //   portfolio_url: null,
      //   working_company: null,
      //   company_salary: null,
      //   company_position: null,
      //   company_address: null
      // }}
    >
      <Grid.Container justify="center" gap={2}>
        <Grid sm={6.5} direction={"column"} css={{ rowGap: 20 }}>
          <Card variant="bordered">
            <Card.Header css={{ margin: "12px 0 0 0" }}>
              <Text
                b
                size={17}
                p
                css={{
                  width: "100%",
                  textAlign: "center",
                }}
              >
Cập nhật thông tin học viên

              </Text>
            </Card.Header>
            <Card.Body>
              <div className={classes.layout}>
                {/* Thông tin cá nhân */}
                <Form.Item
                  label="Họ & tên đệm"
                  name="first_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            "Trường này không được để trống"
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
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
                            "Trường này không được để trống"
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
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
                      message: "Hãy nhập giới tính",
                    },
                  ]}
                >
                  <Select placeholder="Giới tính">
                    {listGender.map((item, index) => (
                      <Select.Option key={index} value={item.id}>
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
                      message: "Hãy nhập ngày sinh",
                    },
                  ]}
                >
                  <DatePicker format={"DD/MM/YYYY"} />
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
                    width: "100%",
                    textAlign: "center",
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Thông tin liên hệ
                </Text>
              </Divider>
              <div></div>
              <div className={classes.layout}>
                {/* Thông tin liên hệ */}
                <Form.Item
                  label="Tỉnh/Thành"
                  name="province_id"
                  rules={[
                    {
                      required: true,
                      message: "Hãy chọn tỉnh/thành phố",
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
                      message: "Hãy chọn quận/huyện",
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
                      message: "Hãy chọn phường/xã",
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
                        return Promise.reject(new Error("Email không hợp lệ"));
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
                        return Promise.reject(new Error("Email không hợp lệ"));
                      },
                    },
                  ]}
                >
                  <Input type="email" placeholder="example@domain.com" />
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
                          new Error("Số điện thoại không hợp lệ")
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
                    width: "100%",
                    textAlign: "center",
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Thông tin phụ huynh
                </Text>
              </Divider>
              <div></div>
              <div className={classes.layout}>
                <Form.Item
                  label="Họ & tên"
                  name="parental_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            "Trường này không được để trống"
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
                    },
                  ]}
                >
                  <Input placeholder="Họ và tên đệm" />
                </Form.Item>

                <Form.Item
                  label="Số điện thoại"
                  name="parental_phone"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        // check regex phone number viet nam
                        if (Validater.isPhone(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error("Số điện thoại không hợp lệ")
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="0891234567" />
                </Form.Item>
                <Form.Item
                  label="Quan hệ"
                  name="parental_relationship"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            "Trường này không được để trống"
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
                    },
                  ]}
                >
                  <Select
                    placeholder="Là ... của học viên"
                    dropdownStyle={{ zIndex: 9999 }}
                  >
                    <Select.Option key={9000} value="Bố">
                      Bố
                    </Select.Option>
                    <Select.Option key={9001} value="Mẹ">
                      Mẹ
                    </Select.Option>
                    <Select.Option key={9002} value="Ông">
                      Ông
                    </Select.Option>
                    <Select.Option key={9003} value="Bà">
                      Bà
                    </Select.Option>
                    <Select.Option key={9004} value="Anh">
                      Anh
                    </Select.Option>
                    <Select.Option key={9005} value="Chị">
                      Chị
                    </Select.Option>
                    <Select.Option key={9006} value="Khác">
                      Khác
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
                    width: "100%",
                    textAlign: "center",
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Thông tin liên quan đến học viện
                </Text>
              </Divider>
              <div></div>
              <div className={classes.layout}>
                <Form.Item
                  label="Học bổng"
                  name="promotion"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value !== null && value !== undefined) {
                          const salary = value.toString();
                          if (Validater.isNumber(salary)) {
                            return Promise.resolve();
                          }
                          return Promise.reject(new Error("Phải là số"));
                        }

                        // check regex phone number viet nam
                      },
                    },
                  ]}
                >
                  <Input placeholder="20" />
                </Form.Item>
                <Form.Item
                  label="Kế hoạch phí"
                  name="fee_plan"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value !== null && value !== undefined) {
                          const salary = value.toString();
                          if (Validater.isNumber(salary)) {
                            return Promise.resolve();
                          }
                          return Promise.reject(new Error("Phải là số"));
                        }

                        // check regex phone number viet nam
                      },
                    },
                  ]}
                >
                  <Input placeholder="20" />
                </Form.Item>
                <Form.Item
                  label="Hồ sơ"
                  name="application_document"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  // rules={[
                  //   {
                  //     required: true,
                  //     message: "Hãy nhập đường dẫn của hồ sơ",
                  //   },
                  // ]}
                >
                  <Input placeholder="Đường dẫn tới hồ sơ" />
                </Form.Item>
                <Form.Item
                  label="Ngày nộp"
                  name="application_date"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: true,
                      message: "Hãy nhập ngày nộp hồ sơ",
                    },
                  ]}
                >
                  <DatePicker
                    placeholder="Ngày nộp hồ sơ"
                    format={"DD/MM/YYYY"}
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
                    width: "100%",
                    textAlign: "center",
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Học vấn và công việc
                </Text>
              </Divider>
              <div></div>
              <div className={classes.layout}>
                <Form.Item
                  label="Trường cấp 3"
                  name="high_school"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ""
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
                    },
                  ]}
                >
                  <Input placeholder="Tên trường học" />
                </Form.Item>
                <Form.Item
                  label="Trường đại học"
                  name="university"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ""
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
                    },
                  ]}
                >
                  <Input placeholder="Tên trường học" />
                </Form.Item>
                <Form.Item
                  label="Công ty"
                  name="working_company"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ""
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
                    },
                  ]}
                >
                  <Input placeholder="Tên công ty" />
                </Form.Item>
                <Form.Item
                  label="Địa chỉ công ty"
                  name="company_address"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ""
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
                    },
                  ]}
                >
                  <Input placeholder="Địa chỉ công ty" />
                </Form.Item>
                <Form.Item
                  label="Chức vụ"
                  name="company_position"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value.trim() === ""
                        ) {
                          return Promise.resolve();
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
                    },
                  ]}
                >
                  <Input placeholder="Chức vụ trong công ty" />
                </Form.Item>
                <Form.Item
                  label="Mức lương"
                  name="company_salary"
                  rules={[
                    {
                      required: false,
                      validator: (_, value) => {
                        if (
                          value === null ||
                          value === undefined ||
                          value === ""
                        ) {
                          return Promise.resolve();
                        }
                        const salary = value.toString();
                        if (Validater.isNumber(salary)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error("Phải là số"));
                      },
                    },
                    // {
                    //   whitespace: true,
                    //   message: "Trường không được chứa khoảng trắng",
                    // },
                  ]}
                >
                  <Input placeholder="5000000" />
                </Form.Item>

                <div></div>
              </div>
              <Spacer y={0.1} />
            </Card.Body>
          </Card>
        </Grid>
    
          <Grid
            sm={4.5}
            direction={"column"}
            css={{ rowGap: 20, position: "relative" }}
          >
            <Card
              variant="bordered"
              css={{ backgroundColor: "transparent", border: "none" }}
            >
              <Card.Header css={{ margin: "12px 0 0 0" }}>
                <Text
                  p
                  b
                  size={17}
                  css={{
                    width: "100%",
                    textAlign: "center",
                    margin: "0",
                  }}
                >
                  Ảnh đại diện
                </Text>
              </Card.Header>
              <Card.Body
                css={{
                  width: "100%",
                  textAlign: "center",
                }}
              >
                <div className={classes.contantLogo}>
                  <div className={classes.logo}>
                    <Image
                      className={classes.avatarMini}
                      width={250}
                      src="https://cdna.artstation.com/p/assets/images/images/048/859/290/large/xu-weili-4d6e20d94309f4e40f1a252e5f8711e.jpg?1651098275"
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
                <Upload>
                  <Button
                    css={{
                      fontSize: "12px",
                      height: "28px",
                      margin: "16px 0 0 0",
                    }}
                    auto
                    flat
                    icon={<UploadOutlined />}
                  >
                    Upload
                  </Button>
                </Upload>

                {/* </div> */}
              </Card.Body>
            </Card>
            <Card variant="bordered">
              <Card.Header css={{ margin: "0px 0 0 0" }}>
                <Text
                  b
                  p
                  size={15}
                  css={{
                    width: "100%",
                    textAlign: "center",
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Thông tin quan trọng
                </Text>
              </Card.Header>
              <Card.Body
                css={{
                  width: "100%",
                  //   textAlign: "center",
                }}
              >
                <Form.Item
                  name="status"
                  label="Thay đổi tình trạng"
                  labelWrap={true}
                  style={{
                    // margin: "auto",
                    width: "100%",
                    textAlign: "left",
                  }}
                >
                  <Select
                    placeholder="Tình trạng học viên"
                    dropdownStyle={{ zIndex: 9999 }}
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
                    <Select.Option key={103} value={4}>
                      ClassQueue
                    </Select.Option>
                    <Select.Option key={104} value={5}>
                      Transfer
                    </Select.Option>
                    <Select.Option key={105} value={6}>
                      Upgrade
                    </Select.Option>
                    <Select.Option key={106} value={7}>
                      Finished
                    </Select.Option>
                  </Select>
                </Form.Item>
                <Form.Item
                  name="course_code"
                  label="Mã khóa học"
                  labelWrap={true}
                  style={{
                    // margin: "auto",
                    width: "100%",
                    textAlign: "left",
                  }}
                  rules={[
                    {
                      required: true,
                      message: "Hãy chọn khóa học",
                    },
                  ]}
                >
                  <Select
                    dropdownStyle={{ zIndex: 9999 }}
                    onChange={getListCourse}
                  >
                    {listCourses.map((e, index) => (
                      <Select.Option key={index} value={e.code}>
                        {e.data} {e.code}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
              </Card.Body>
            </Card>
            <Card variant="bordered">
              <Card.Header css={{ margin: "0px 0 0 0" }}>
                <Text
                  b
                  p
                  size={15}
                  css={{
                    width: "100%",
                    textAlign: "center",
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Thông tin CMND/CCCD
                </Text>
              </Card.Header>
              <Card.Body
                css={{
                  width: "100%",
                  //   textAlign: "center",
                }}
              >
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
                          new Error("Số CMND/CCCD không hợp lệ")
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="CMND/CCCD" />
                </Form.Item>
                <Form.Item
                  label="Ngày cấp"
                  name="citizen_identity_card_published_date"
                  rules={[
                    {
                      required: true,
                      message: "Hãy nhập ngày cấp",
                    },
                  ]}
                >
                  <DatePicker format={"DD/MM/YYYY"} />
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
                            "Trường này không được để trống"
                          );
                        }
                        if (
                          Validater.isContaintSpecialCharacterForName(
                            value.trim()
                          )
                        ) {
                          return Promise.reject(
                            "Trường này không được chứa ký tự đặc biệt"
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error("Trường phải từ 1 đến 255 ký tự")
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: "Trường không được chứa khoảng trắng",
                    },
                  ]}
                >
                  <Input placeholder="Nơi cấp" />
                </Form.Item>
              </Card.Body>
            </Card>
            <Card variant="bordered">
              <Card.Header css={{ margin: "0px 0 0 0" }}>
                <Text
                  b
                  p
                  size={15}
                  css={{
                    width: "100%",
                    textAlign: "center",
                    //   margin: '0',
                    //   padding: '0',
                  }}
                >
                  Thông tin bổ sung
                </Text>
              </Card.Header>
              <Card.Body
                css={{
                  width: "100%",
                  //   textAlign: "center",
                }}
              >
                <Form.Item
                  label="Facebook"
                  name="facebook_url"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  // rules={[
                  //   {
                  //     // required: true,

                  //     message: "Hãy nhập đường dẫn facebook",
                  //   },
                  // ]}
                >
                  <Input placeholder="Đường dẫn tới facebook" />
                </Form.Item>

                <Form.Item
                  label="CV"
                  name="portfolio_url"
                  style={{
                    // margin: "auto",
                    width: "100%",
                    // textAlign: "left",
                  }}
                  // rules={[
                  //   {
                  //     // required: true,
                  //     message: "Hãy nhập đường dẫn tới cv",
                  //   },
                  // ]}
                >
                  <Input placeholder="Đường dẫn tới cv" />
                </Form.Item>
              </Card.Body>
            </Card>
            <Form.Item
              style={{
                display: "inline-block",
                textAlign: "right",
                width: "100%",
              }}
            >
              {!isCreatingOrUpdating && messageFailed !== undefined && (
                <Text
                  size={14}
                  css={{
                    background: "#fff",
                    color: "red",
                  }}
                >
                  {messageFailed}
                </Text>
              )}
            </Form.Item>

            <div className={classes.buttonCreate}>
              <Button
                flat
                auto
                css={{
                  width: "150px",
                  position: "absolute",
                  right: "10px",
                  bottom: "10px",
                }}
                type="primary"
                htmlType="submit"
                // disabled={isCreatingOrUpdating}
              >
             Cập nhật
              </Button>
              <Button
                flat
                auto
                color="error"
                css={{
                  width: "150px",
                  position: "absolute",
                  right: "180px",
                  bottom: "10px",
                }}
                onPress={() => {
                  navigate("/sro/manage/student/");
                }}
                // disabled={isCreatingOrUpdating}
              >
                Hủy
              </Button>
            </div>
          </Grid>
        
      </Grid.Container>
    </Form>
  );
};
export default StudentUpdate;
