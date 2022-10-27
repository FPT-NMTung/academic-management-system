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
import { useNavigate } from "react-router-dom";
import classes from "./StudentUpdate.module.css";
import toast from "react-hot-toast";
import FetchApi from "../../../../apis/FetchApi";
import { Validater } from "../../../../validater/Validater";
import {
  CenterApis,
  GenderApis,
  AddressApis,
  ManageTeacherApis,
} from "../../../../apis/ListApi";
import ManImage from "../../../../images/3d-fluency-businessman-1.png";
import WomanImage from "../../../../images/3d-fluency-businesswoman-1.png";

const StudentUpdate = (modeUpdate) => {
  const [isGetData, setIsGetData] = useState(false);
  const [listGender, setListGender] = useState([]);
  const [listProvince, setListProvince] = useState([]);
  const [listDistrict, setListDistrict] = useState([]);
  const [listWard, setListWard] = useState([]);

  const [form] = Form.useForm();

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

  const handleSubmitForm = () => {};

  useEffect(() => {
    // getListCenter();
    getListGender();
    getListProvince();
    // getListWorkingTime();
    // getListTeacherType();

    // if (modeUpdate) {
    //   getInformationTeacher();
    // }
  }, []);
  return (
    <Form
      labelCol={{ span: 7 }}
      wrapperCol={{ span: 15 }}
      form={form}
      onFinish={handleSubmitForm}
      // disabled={modeUpdate && isGettingInformationTeacher}
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
                {!modeUpdate && "Tạo học viên mới"}
                {modeUpdate && "Cập nhật thông tin học viên"}
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
                <Form.Item
                  label="Quan hệ"
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
                  <Input placeholder="Là ... của học viên" />
                </Form.Item>
                <div></div>
              </div>

              <Spacer y={0.5} />

            </Card.Body>
          </Card>
        </Grid>
        {modeUpdate && (
          <Grid sm={4.5} direction={"column"} css={{ rowGap: 0,position: "relative", }}>
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
                      src="https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png"
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
                    css={{ fontSize: "12px", height: "28px", margin: "16px 0" }}
                    auto
                    flat
                    icon={<UploadOutlined />}
                  >
                    Upload
                  </Button>
                </Upload>
                <Text p b size={14}>
                  Tình trạng
                </Text>

                <Form.Item
                  name="status"
                  label="Thay đổi tình trạng"
                  labelWrap={true}
                  style={{
                    margin: "auto",
                    width: "100%",
                    textAlign: "left",
                  }}
                >
                  <Select
                    placeholder="Tình trạng học viên"
                    dropdownStyle={{ zIndex: 9999}}
                  >
                    <Select.Option key={100} value="1">
                      Studying
                    </Select.Option>
                    <Select.Option key={101} value="2">
                    Delay
                    </Select.Option>
                    <Select.Option key={102} value="3">
                    Dropout
                    </Select.Option>
                    <Select.Option key={103} value="4">
                    ClassQueue
                    </Select.Option>
                    <Select.Option key={104} value="5">
                    Transfer
                    </Select.Option>
                    <Select.Option key={105} value="6">
                    Upgrade
                    </Select.Option>
                    <Select.Option key={106} value="7">
                    Finished
                    </Select.Option>
                  </Select>
                </Form.Item>

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

            <div className={classes.buttonCreate}>
                <Button
                  flat
                  auto
                  css={{
                    width: "150px",
                    position: "absolute",
                    right: "10px",
                    bottom: "-50px",
                  }}
                  type="primary"
                  htmlType="submit"
                  // disabled={isCreatingOrUpdating}
                >
                  {!modeUpdate && "Tạo mới"}
                  {modeUpdate && "Cập nhật"}
                </Button>
              </div>
          </Grid>
        )}
      </Grid.Container>
    </Form>
  );
};
export default StudentUpdate;
