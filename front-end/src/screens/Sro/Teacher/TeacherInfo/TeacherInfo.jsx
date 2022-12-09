import {
  Grid,
  Spacer,
  Text,
  Badge,
  Card,
  Button,
  Dropdown,
  Modal,
  Loading,
} from "@nextui-org/react";
import classes from "./TeacherInfo.module.css";
import { useParams, useNavigate } from "react-router-dom";
import {
  Descriptions,
  Spin,
  Select,
  Tag,
  Tabs,
  Divider,
  Form,
  Input,
} from "antd";
import { useState, useEffect } from "react";
import { AiFillPhone } from "react-icons/ai";
import { MdDelete, MdPersonAdd, MdSave } from "react-icons/md";
import { HiHome } from "react-icons/hi";
import FetchApi from "../../../../apis/FetchApi";
import { ManageTeacherApis } from "../../../../apis/ListApi";
import ManImage from "../../../../images/3d-fluency-businessman-1.png";
import WomanImage from "../../../../images/3d-fluency-businesswoman-1.png";
import { FaCloudDownloadAlt, FaSyncAlt } from "react-icons/fa";
import { RiEyeFill, RiPencilFill } from "react-icons/ri";
import { Fragment } from "react";
import toast from "react-hot-toast";
import React from "react";

const gender = {
  1: "Nam",
  2: "Nữ",
  3: "Khác",
  4: "Không xác định",
};
const StudentDetail = () => {
  const [dataUser, setdataUser] = useState({});
  const [listSkill, setListSkill] = useState(undefined);
  const [listModule, setListModule] = useState([]);
  const [listClass, setListClass] = useState([]);
  const [cloneListSkill, setCloneListSkill] = useState(undefined);
  const [isLoading, setIsLoading] = useState(true);
  const [gpa, setGpa] = useState([]);
  const [moduleSelected, setModuleSelected] = React.useState("");
  const [classSelected, setClassSelected] = React.useState("");
  const [gpaByModule, setGpaByModule] = useState([]);
  const [gpaByClass, setGpaByClass] = useState([]);

  const [form] = Form.useForm();

  const { id } = useParams();
  const navigate = useNavigate();

  const getdataUser = () => {
    FetchApi(ManageTeacherApis.detailTeacher, null, null, [`${id}`])
      .then((res) => {
        setdataUser(res.data);
        setIsLoading(false);
      })
      .catch(() => {
        toast.error("Lỗi lấy chi tiết giáo viên");
      });
  };
  const getListSkill = () => {
    FetchApi(ManageTeacherApis.getListSkillOfTeacher, null, null, [`${id}`])
      .then((res) => {
        console.log(res.data[0].skills);
        setListSkill(res.data[0].skills);
      })
      .catch(() => {
        toast.error("Lỗi lấy danh sách kỹ năng");
      });
  };

  const getListModule = () => {
    FetchApi(ManageTeacherApis.getListModulesOfATeacher, null, null, [`${id}`])
      .then((res) => {
        setListModule(res.data);
        console.log("mon hoc " + res.data);
      })
      .catch(() => {
        toast.error("Lỗi lấy danh sách môn học");
      });
  };
  const getListClass = () => {
    const moduleid = form.getFieldValue("subject");
    FetchApi(ManageTeacherApis.getListClassOfATeacherByModule, null, null, [
      `${moduleid}`,
    ])
      .then((res) => {
        setListClass(res.data);
      })
      .catch(() => {
        toast.error("Lỗi lấy danh sách lớp học");
      });
  };
  const getAverageGPA = () => {
    FetchApi(ManageTeacherApis.getAverageGPAOfATeacher, null, null, [`${id}`])
      .then((res) => {
        const data = res.data === null ? "" : res.data;
        setGpa(data);
      })
      .catch(() => {
        toast.error("Lỗi lấy GPA giáo viên");
      });
  };
  const getGPAByModule = () => {
    const moduleid = form.getFieldValue("subject");
    console.log("sssssMon hoc id " + moduleSelected);
    // setGpaByModule("");
    FetchApi(ManageTeacherApis.getAverageGPAOfATeacherByModule, null, null, [
      `${id}`,
      `${moduleid}`,
    ])
      .then((res) => {
        const data = res.data === null ? "" : res.data;
        setGpaByModule(data);
        console.log(gpaByModule);
      })
      .catch(() => {
        toast.error("Lỗi lấy GPA giáo viên theo môn học");
      });
  };
  const getGPAByModuleAndClass = () => {
    const moduleid = form.getFieldValue("subject");
    const classid = form.getFieldValue("class");
    console.log("sssssMon hoc id " + moduleSelected);
    console.log(FetchApi(
      ManageTeacherApis.getAverageGPAOfATeacherByModuleAndClass,
      null,
      null,
      [`${id}`, `${classid}`, `${moduleid}`]
    ));
    FetchApi(
      ManageTeacherApis.getAverageGPAOfATeacherByModuleAndClass,
      null,
      null,
      [`${id}`, `${classid}`, `${moduleid}`]
    )
      .then((res) => {
        const data = res.data === null ? "" : res.data;
        setGpaByClass(data);
        console.log(gpaByClass);
      })
      .catch(() => {
        toast.error("Lỗi lấy GPA giáo viên theo môn học và lớp học");
      });
  };
  useEffect(() => {
    getdataUser();
    getListSkill();
    getAverageGPA();
  }, []);

  return (
    <Fragment>
      <Grid.Container justify="center">
        <Grid sm={12}>
          {isLoading && (
            <div className={classes.loading}>
              <Spin />
            </div>
          )}
          {!isLoading && (
            <Grid.Container
              gap={2}
              css={{
                position: "relative",
              }}
            >
              <Grid
                sm={3.5}
                css={{
                  display: "flex",
                  flexDirection: "column",
                  width: "100%",
                  height: "fit-content",
                }}
              >
                <Card variant="bordered" css={{ marginBottom: "20px" }}>
                  <Card.Body>
                    <div className={classes.contantLogo}>
                      <div className={classes.logo}>
                        {dataUser.avatar && (
                          <img
                            className={classes.avatar}
                            src={dataUser.avatar}
                          />
                        )}

                        {!dataUser.avatar && (
                          <img
                            className={classes.avatarMini}
                            src={
                              dataUser.gender.id === 1
                                ? ManImage
                                : dataUser.gender.id === 2
                                ? WomanImage
                                : ""
                            }
                          />
                        )}
                      </div>
                      <Spacer y={0.7} />
                      <Text h3 size={20} b>
                        {dataUser.first_name} {dataUser.last_name}
                      </Text>
                      <Text h6 className={classes.statusStudent}>
                        {dataUser.is_active && (
                          <Badge color={"success"} variant={"flat"}>
                            Đang hoạt động
                          </Badge>
                        )}
                        {!dataUser.is_active && (
                          <Badge color={"error"} variant={"flat"}>
                            Dừng hoạt động
                          </Badge>
                        )}
                      </Text>
                    </div>
                    <Spacer y={1} />

                    <div className={classes.iconInformation}>
                      <Text size={14} b>
                        Số điện thoại
                      </Text>
                      <Text size={14}>{dataUser.mobile_phone}</Text>
                    </div>
                    <Card.Divider />
                    <div className={classes.iconInformation}>
                      <Text size={14} b>
                        Email cá nhân
                      </Text>

                      {dataUser.email}
                    </div>
                    <Card.Divider />

                    <div className={classes.iconInformation}>
                      <Text size={14} b>
                        Email tổ chức
                      </Text>
                      <Text p size={14}>
                        {dataUser.email_organization}
                      </Text>
                    </div>

                    <Card.Divider />
                    <Spacer y={2.2} />
                  </Card.Body>
                </Card>
                <Card variant="bordered" css={{ marginBottom: "20px" }}>
                  <Card.Header>
                    <Grid.Container alignItems="center">
                      <Descriptions
                        title="Kỹ năng"
                        column={{ md: 1, lg: 1, xl: 1, xxl: 1 }}
                        // size="middle"
                      >
                        <Descriptions.Item
                          contentStyle={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: "space-between",
                          }}
                        >
                          <div>
                            {listSkill?.map((item, index) => (
                              <Tag key={index}>{item.name}</Tag>
                            ))}
                            {listSkill?.length === 0 && (
                              <Text
                                p
                                size={14}
                                css={{
                                  color: "#9e9e9e",
                                  width: "100%",
                                  textAlign: "center",
                                }}
                              >
                                Chưa có kỹ năng
                              </Text>
                            )}
                            {listSkill === undefined && (
                              <div
                                style={{
                                  display: "flex",
                                  justifyContent: "center",
                                }}
                              >
                                <Loading size="xs" color={"error"} />
                              </div>
                            )}
                          </div>
                          <Text size={14} b></Text>
                        </Descriptions.Item>
                      </Descriptions>
                      <Spacer y={0.6} />
                    </Grid.Container>
                  </Card.Header>
                </Card>
              </Grid>
              <Grid sm={8.5} direction="column" css={{ rowGap: 20 }}>
                <Tabs
                  defaultActiveKey="1"
                  tabBarStyle={{
                    display: "flex",
                    fontStyle: "15px",
                    justifycontent: "space-between",
                    border: "solid 1px #e8e8e8",
                    background: "#fff",
                    borderRadius: "15px",
                    padding: "2px 14px",
                  }}
                  type="line"
                  size="large"
                  onChange={() => {
                    getListModule();
                    form.resetFields();
                    setListClass([]);
                    setModuleSelected("");
                    setGpaByModule("");
                    setClassSelected("");
                  }}

                  // closable={false}
                >
                  <items tab="Chi tiết giáo viên" key="1">
                    <Card variant="bordered" css={{ marginBottom: "20px" }}>
                      <Card.Body>
                        <Descriptions
                          bordered
                          title="Thông tin cơ bản"
                          column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                        >
                          <Descriptions.Item label="Họ và tên đệm">
                            {dataUser.first_name}
                          </Descriptions.Item>
                          <Descriptions.Item label="Tên">
                            {dataUser.last_name}
                          </Descriptions.Item>
                          <Descriptions.Item label="Số điện thoại">
                            {dataUser.mobile_phone}
                          </Descriptions.Item>
                          <Descriptions.Item label="Giới tính">
                            {gender[dataUser.gender.id]}
                          </Descriptions.Item>
                          <Descriptions.Item label="Địa chỉ" span={2}>
                            {dataUser.ward.prefix} {dataUser.ward.name},{" "}
                            {dataUser.district.prefix} {dataUser.district.name},{" "}
                            {dataUser.province.name}
                          </Descriptions.Item>
                          <Descriptions.Item label="Ngày sinh">
                            {new Date(dataUser.birthday).toLocaleDateString(
                              "vi-VN"
                            )}
                          </Descriptions.Item>
                          <Descriptions.Item label="Email cá nhân" span={2}>
                            {dataUser.email}
                          </Descriptions.Item>
                          <Descriptions.Item label="Email tổ chức" span={2}>
                            {dataUser.email_organization}
                          </Descriptions.Item>
                        </Descriptions>
                      </Card.Body>
                    </Card>
                    <Card variant="bordered" css={{ marginBottom: "20px" }}>
                      <Card.Body>
                        <Descriptions
                          title="Thông tin bổ sung"
                          bordered
                          column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                        >
                          <Descriptions.Item label="Biệt danh">
                            {dataUser.nickname ? (
                              dataUser.nickname
                            ) : (
                              <i style={{ color: "lightgray" }}>Không có</i>
                            )}
                          </Descriptions.Item>
                          <Descriptions.Item label="Ngày bắt đầu dạy">
                            {new Date(
                              dataUser.start_working_date
                            ).toLocaleDateString("vi-VN")}
                          </Descriptions.Item>
                          <Descriptions.Item label="Nơi công tác" span={2}>
                            {dataUser.company_address ? (
                              dataUser.company_address
                            ) : (
                              <i style={{ color: "lightgray" }}>Không có</i>
                            )}
                          </Descriptions.Item>
                          <Descriptions.Item label="Loại hợp đồng">
                            {dataUser.teacher_type.value}
                          </Descriptions.Item>
                          <Descriptions.Item label="Mã số thuế">
                            {dataUser.tax_code}
                          </Descriptions.Item>
                          <Descriptions.Item label="Thời gian dạy">
                            {dataUser.working_time.value}
                          </Descriptions.Item>
                          <Descriptions.Item label="Mức lương">
                            {dataUser.salary
                              ? String(dataUser.salary).replace(
                                  /\B(?=(\d{3})+(?!\d))/g,
                                  ","
                                )
                              : 0}{" "}
                            VNĐ
                          </Descriptions.Item>
                        </Descriptions>
                      </Card.Body>
                    </Card>
                    <Card variant="bordered">
                      <Card.Body>
                        <Descriptions
                          title="Thông tin CMND/CCCD"
                          bordered
                          column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                        >
                          <Descriptions.Item label="Số thẻ">
                            {dataUser.citizen_identity_card_no}
                          </Descriptions.Item>
                          <Descriptions.Item label="Ngày cấp">
                            {new Date(
                              dataUser.citizen_identity_card_published_date
                            ).toLocaleDateString("vi-VN")}
                          </Descriptions.Item>
                          <Descriptions.Item label="Nơi cấp">
                            {dataUser.citizen_identity_card_published_place}
                          </Descriptions.Item>
                        </Descriptions>
                      </Card.Body>
                    </Card>
                  </items>

                  <items className="" tab="GPA giáo viên" key="2">
                    <Card variant="bordered" css={{ marginBottom: "20px" }}>
                      <Card.Body>
                        <Text
                          b
                          size={18}
                          css={{ textAlign: "center", marginBottom: "20px" }}
                        >
                          GPA Trung bình giáo viên:{"   "}
                          <Badge
                            variant="bordered"
                            color="success"
                            shape="circle"
                            size="md"
                          >
                            {" "}
                            {gpa === ""
                              ? "Chưa có dữ liệu"
                              : Math.round(gpa.average_gpa * 10) / 10}
                            {gpa === "" ? "" : " %"}
                          </Badge>
                        </Text>
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
                              marginBottom: "24px",
                              //   margin: '0',
                              //   padding: '0',
                            }}
                          >
                            Xem GPA trung bình của giáo viên theo môn học và lớp
                          </Text>
                        </Divider>

                        <Form
                          labelCol={{ span: 7 }}
                          wrapperCol={{ span: 15 }}
                          form={form}
                          // disabled={modeUpdate && isGettingInformationStudent}
                        >
                          <div className={classes.layout}>
                            <Form.Item label="Chọn môn học" name="subject">
                              <Select
                                placeholder="Chọn môn học"
                                onChange={() => {
                                  getListClass();
                                  getGPAByModule();
                                }}
                                value={moduleSelected}
                                onSelect={setModuleSelected}
                              >
                                {listModule.map((item, index) => (
                                  <Select.Option value={item.id} key={index}>
                                    {item.module_name}
                                  </Select.Option>
                                ))}
                              </Select>
                            </Form.Item>
                            <Form.Item label="Chọn lớp học" name="class">
                              <Select
                                placeholder="Chọn lớp học theo môn học"
                                disabled={listClass.length === 0}
                                value={classSelected}
                                onSelect={setClassSelected}
                                onChange={() => {
                                  // getGPAByModuleAndClass();
                                }}
                              >
                                {listClass.map((item, index) => (
                                  <Select.Option key={item.id} value={item.id}>
                                    {item.name}
                                  </Select.Option>
                                ))}
                              </Select>
                            </Form.Item>
                          </div>
                        </Form>
                        <Card
                          variant="bordered"
                          css={{
                            minHeight: "140px",
                            borderStyle: "dashed",
                            marginBottom: "24px",
                          }}
                        >
                          <div className={classes.layout}>
                            <Divider
                              orientation="left"
                              style={{ marginTop: 10, marginBottom: 24 }}
                            >
                              <Text
                                b
                                p
                                size={15}
                                css={{
                                  width: "100%",
                                  textAlign: "center",
                                  marginBottom: "24px",
                                  //   margin: '0',
                                  //   padding: '0',
                                }}
                              >
                                GPA trung bình của môn{" "}
                                {listModule.length == 0
                                  ? ""
                                  : listModule.map((item) => {
                                      if (item.id === moduleSelected) {
                                        return item.module_name + " " + "là:  ";
                                      }
                                    })}
                              </Text>
                              {gpaByModule.length == 0 ? (
                                ""
                              ) : (
                                <Badge
                                  variant="bordered"
                                  color="success"
                                  shape="circle"
                                  size="md"
                                >
                                  {" "}
                                  {gpaByModule === ""
                                    ? "Chưa có dữ liệu"
                                    : Math.round(gpaByModule.average_gpa * 10) /
                                      10}
                                  {gpaByModule === "" ? "" : " %"}
                                </Badge>
                              )}
                            </Divider>
                            <Divider
                              orientation="left"
                              style={{ marginTop: 10, marginBottom: 24 }}
                            >
                              <Text
                                b
                                p
                                size={15}
                                css={{
                                  width: "100%",
                                  textAlign: "center",
                                  marginBottom: "24px",
                                  //   margin: '0',
                                  //   padding: '0',
                                }}
                              >
                                GPA trung bình môn {" "}
                                {listModule.length == 0
                                  ? ""
                                  : listModule.map((item) => {
                                      if (item.id === moduleSelected) {
                                        return item.module_name + " ";
                                      }
                                    })}
                                của lớp{" "}
                                {listClass.length == 0
                                  ? ""
                                  : listClass.map((item) => {
                                      if (item.id === classSelected) {
                                        return item.name + " " + "là:  ";
                                      }
                                    })}
                              </Text>
                            </Divider>
                          </div>
                        </Card>
                      </Card.Body>
                    </Card>
                  </items>
                </Tabs>
              </Grid>
            </Grid.Container>
          )}
        </Grid>
      </Grid.Container>
    </Fragment>
  );
};
export default StudentDetail;
