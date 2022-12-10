import {
  Grid,
  Spacer,
  Text,
  Badge,
  Card,
  Button,
  Dropdown,
  Modal,
} from "@nextui-org/react";
import classes from "./StudentDetail.module.css";
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
import { ManageStudentApis } from "../../../../apis/ListApi";
import ManImage from "../../../../images/3d-fluency-businessman-1.png";
import WomanImage from "../../../../images/3d-fluency-businesswoman-1.png";
import { FaCloudDownloadAlt, FaSyncAlt } from "react-icons/fa";
import { RiEyeFill, RiPencilFill } from "react-icons/ri";
import { Fragment } from "react";
import toast from "react-hot-toast";

const gender = {
  1: "Nam",
  2: "Nữ",
  3: "Khác",
  4: "Không xác định",
};
const StudentDetail = () => {
  const [dataStudent, setDataStudent] = useState({});
  const [listAvailableClassToChange, setListAvailableClassToChange] = useState(
    []
  );
  const [isLoading, setIsLoading] = useState(true);
  const [isOpenModal, setIsOpenModal] = useState(false);
  const [currentClass, setCurrentClass] = useState(null);
  const [form] = Form.useForm();

  const { id } = useParams();
  const navigate = useNavigate();

  const getDataStudent = () => {
    FetchApi(ManageStudentApis.detailStudent, null, null, [`${id}`])
      .then((res) => {
        setDataStudent(res.data);
        // console.log(res.data);
        // console.log(res.data.old_class[0].class_name);
        setIsLoading(false);
      })
      .catch(() => {
        navigate("/404");
      });
  };
  const getCurrentClass = () => {
    setIsLoading(true);
    FetchApi(ManageStudentApis.getCurrentClasss, null, null, [`${id}`])
      .then((res) => {
        setCurrentClass(res.data.name);
        setIsLoading(false);
      })
      .catch(() => {
        navigate("/404");
      });
  };
  const getAvailableClassToChange = () => {
    FetchApi(ManageStudentApis.listAvailableClassToChange, null, null, [
      `${id}`,
    ])
      .then((res) => {
        setListAvailableClassToChange(res.data);
        // console.log(res.data);
      })
      .catch(() => {
        navigate("/404");
      });
  };

  useEffect(() => {
    getDataStudent();
  }, []);
  const renderStatusStudent = (id) => {
    if (id === 1) {
      return (
        <Badge variant="flat" color="primary">
          Studying
        </Badge>
      );
    } else if (id === 2) {
      return (
        <Badge variant="flat" color="warning">
          Delay
        </Badge>
      );
    } else if (id === 3) {
      return (
        <Badge variant="flat" color="default">
          Dropout
        </Badge>
      );
    } else if (id === 4) {
      return (
        <Badge variant="flat" color="secondary">
          ClassQueue
        </Badge>
      );
    } else if (id === 5) {
      return (
        <Badge variant="flat" color="default">
          Transfer
        </Badge>
      );
    } else if (id === 6) {
      return (
        <Badge variant="flat" color="error">
          Upgrade
        </Badge>
      );
    } else {
      return (
        <Badge variant="flat" color="success">
          Finished
        </Badge>
      );
    }
  };
  const handleOpenModal = () => {
    setIsOpenModal(true);
    getCurrentClass();
    getAvailableClassToChange();
  };
  const handleSelectOption = (key) => {
    switch (key) {
      case "edit":
        navigate(`/sro/manage/student/${id}/update`);
        break;
      case "change":
        handleOpenModal();
        break;
      default:
        break;
    }
  };
  const handleSubmitForm = (e) => {
    form.resetFields();
    const body = {
      current_class_id: dataStudent.current_class.class_id,
      new_class_id: e.new_class,
    };

    toast.promise(
      FetchApi(ManageStudentApis.changeClass, body, null, [`${id}`]),
      {
        loading: "Đang cập nhật...",
        success: (res) => {
          setIsOpenModal(false);
          getDataStudent();

          return "Cập nhật thành công";
        },
        error: (err) => {
          return "Cập nhật thất bại";
        },
      }
    );
  };
  return (
    <Fragment>
      <Modal
        open={isOpenModal}
        width="500px"
        blur
        onClose={() => {
          setIsOpenModal(false);
        }}
      >
        <Modal.Header>
          <Text size={16} b>
            Cập nhật chuyển lớp
          </Text>
        </Modal.Header>

        <Modal.Body>
          <Form
            labelCol={{ span: 7 }}
            wrapperCol={{ span: 12 }}
            layout="horizontal"
            onFinish={handleSubmitForm}
            form={form}
          >
            <Form.Item name={"current_class"} label={"Lớp hiện tại"}>
              <Input disabled={true} placeholder={currentClass}></Input>
            </Form.Item>
            <Form.Item
              name={"new_class"}
              label={"Lớp chuyển tới"}
              rules={[
                {
                  required: true,
                  message: "Hãy chọn lớp chuyển tới",
                },
              ]}
            >
              <Select
                placeholder="Chọn lớp chuyển tới"
                dropdownStyle={{ zIndex: 9999 }}
              >
                {listAvailableClassToChange.map((e) => (
                  <Select.Option key={e.id} value={e.id}>
                    {e.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item wrapperCol={{ offset: 9, span: 99 }}>
              <div
                style={{
                  display: "flex",
                  gap: "10px",
                }}
              >
                <Button
                  flat
                  auto
                  css={{
                    width: "120px",
                  }}
                  type="primary"
                  htmlType="submit"
                >
                  Cập nhật
                </Button>
              </div>
            </Form.Item>
          </Form>
        </Modal.Body>
      </Modal>

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
                  // marginTop: "1rem",
                }}
              >
                <Card variant="bordered" css={{ marginBottom: "20px" }}>
                  <Card.Body>
                    <div className={classes.contantLogo}>
                      <div className={classes.logo}>
                        {dataStudent.avatar && (
                          <img
                            className={classes.avatar}
                            src={dataStudent.avatar}
                          />
                        )}

                        {!dataStudent.avatar && (
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
                        )}
                      </div>
                      <Spacer y={0.7} />
                      <Text h3 size={20} b>
                        {dataStudent.first_name} {dataStudent.last_name}
                      </Text>
                      <Text h6 className={classes.statusStudent}>
                        {renderStatusStudent(dataStudent.status)}
                      </Text>
                    </div>
                    <Spacer y={1} />
                    <div className={classes.iconInformation}>
                      <Text size={14} b>
                        Mã số học viên
                      </Text>
                      <Text size={14}>{dataStudent.enroll_number}</Text>
                    </div>
                    <Card.Divider />
                    <div className={classes.iconInformation}>
                      <Text size={14} b>
                        Lớp học
                      </Text>
                      {dataStudent.current_class === null && (
                        <Text p size={14}>
                        Hiện Không học lớp nào
                        </Text>
                      )}
                      {dataStudent.current_class !== null && (
                        <Text p size={14}>
                          {dataStudent.current_class.class_name}
                        </Text>
                      )}
                    </div>
                    <Card.Divider />

                    <div className={classes.iconInformation}>
                      <Text size={14} b>
                        Mã khóa học
                      </Text>
                      <Text p size={14}>
                        {dataStudent.course_code}
                      </Text>
                    </div>

                    <Card.Divider />
                    <Spacer y={2.5} />
                  </Card.Body>
                </Card>

                <Card variant="bordered" css={{ marginBottom: "20px" }}>
                  <Card.Header>
                    <Grid.Container alignItems="center">
                      {/* <Grid sm={6}>
                      <Text p size={14} b>
                        Thông tin cơ bản
                      </Text>
                    </Grid>
                    <Grid
                      sm={6}
                      css={{
                        display: "flex",
                        justifyContent: "flex-end",
                      }}
                    >
                      <MdEmail
                        size={16}
                        style={{
                          cursor: "pointer",
                        }}
                        // onClick={() => setIsEditSkill(!isEditSkill)}
                      />
                    </Grid> */}

                      <Descriptions
                        title="Thông tin cơ bản"
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
                          <Text size={14} b>
                            Giới tính:
                          </Text>
                          {gender[dataStudent.gender.id]}
                        </Descriptions.Item>
                        <Descriptions.Item
                          contentStyle={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: "space-between",
                          }}
                        >
                          <Text size={14} b>
                            SĐT:
                          </Text>
                          {dataStudent.mobile_phone}
                        </Descriptions.Item>

                        <Descriptions.Item
                          contentStyle={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: "space-between",
                          }}
                        >
                          <Text size={14} b>
                            Email tổ chức:
                          </Text>
                          {dataStudent.email_organization}
                        </Descriptions.Item>

                        {/* <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                      >
                        <Text size={14} b>
                          CCCD/CMT:
                        </Text>
                        {dataStudent.citizen_identity_card_no}
                      </Descriptions.Item> */}
                        {/* <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                      >
                        <Text size={14} b>
                          Ngày cấp:
                        </Text>

                        {new Date(
                          dataStudent.citizen_identity_card_published_date
                        ).toLocaleDateString("vi-VN")}
                      </Descriptions.Item> */}
                        {/* <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                      >
                        <Text size={14} b>
                          Tên phụ huynh:
                        </Text>
                        {dataStudent.parental_name}
                      </Descriptions.Item> */}
                        {/* <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                      >
                        <Text size={14} b>
                          Mối quan hệ:
                        </Text>
                        {dataStudent.parental_relationship}
                      </Descriptions.Item> */}

                        <Descriptions.Item
                          contentStyle={{
                            display: "flex",
                            // flex: 1,
                            flexDirection: "row",
                            justifyContent: "space-between",
                          }}
                        >
                          <Text
                            size={14}
                            // css={{ display: "inline", width: "100%" }}
                            b
                          >
                            Địa chỉ
                          </Text>{" "}
                          {dataStudent.ward.prefix} {dataStudent.ward.name},{" "}
                          {dataStudent.district.prefix}{" "}
                          {dataStudent.district.name},{" "}
                          {dataStudent.province.name}
                        </Descriptions.Item>
                        <Descriptions.Item
                          contentStyle={{
                            display: "flex",
                            // flex: 1,
                            flexDirection: "row",
                            justifyContent: "space-between",
                          }}
                        >
                          <Text size={14} b>
                            Facebook:
                          </Text>
                          <Text size={14} b></Text>
                          <a href={dataStudent.facebook_url}>Facebook_URL</a>
                        </Descriptions.Item>
                        <Descriptions.Item
                          contentStyle={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: "space-between",
                          }}
                        >
                          <Text
                            size={14}
                            // css={{ display: "block", width: "100%" }}
                            b
                          >
                            Portfolio:
                          </Text>
                          <Text size={14} b></Text>
                          <a href={dataStudent.portfolio_url}>Portfolio_URL</a>
                        </Descriptions.Item>
                      </Descriptions>
                      <Spacer y={0.6} />
                    </Grid.Container>
                  </Card.Header>
                </Card>

                <Card variant="bordered">
                  <Card.Header>
                    <Grid.Container alignItems="baseline">
                      <Descriptions
                        title="Thông tin CMT/CCCD"
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
                          <Text size={14} b>
                            Số CCCD/CMT:
                          </Text>
                          {dataStudent.citizen_identity_card_no}
                        </Descriptions.Item>
                        <Descriptions.Item
                          contentStyle={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: "space-between",
                          }}
                        >
                          <Text size={14} b>
                            Ngày cấp:
                          </Text>

                          {new Date(
                            dataStudent.citizen_identity_card_published_date
                          ).toLocaleDateString("vi-VN")}
                        </Descriptions.Item>
                        <Descriptions.Item
                          contentStyle={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: "space-between",
                          }}
                        >
                          <Text size={14} b>
                            Nơi cấp:
                          </Text>
                          {dataStudent.citizen_identity_card_published_place}
                        </Descriptions.Item>

                        {/* <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                      >
                        <Text size={14} b>
                          CCCD/CMT:
                        </Text>
                        {dataStudent.citizen_identity_card_no}
                      </Descriptions.Item> */}

                        {/* <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                      >
                        <Text size={14} b>
                          Tên phụ huynh:
                        </Text>
                        {dataStudent.parental_name}
                      </Descriptions.Item> */}
                        {/* <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                      >
                        <Text size={14} b>
                          Mối quan hệ:
                        </Text>
                        {dataStudent.parental_relationship}
                      </Descriptions.Item> */}
                      </Descriptions>
                      <Spacer y={0.3} />
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

                  // closable={false}
                >
                  <items tab="Chi tiết học viên" key="1">
                    <Card variant="bordered" css={{ marginBottom: "20px" }}>
                      <Card.Body>
                        {/* <Button
                        css={{ position: "absolute", top: 10, right: 12 }}
                        flat
                        auto
                        type="primary"
                        onPress={() => {
                          navigate(`/sro/manage/student/${id}/update`);
                        }}
                      >
                        Chỉnh sửa thông tin
                      </Button> */}
                        <Descriptions
                          bordered
                          title="Thông tin bổ sung"
                          column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                          // extra={

                          // }
                        >
                          {/* <Descriptions.Item label="Họ và tên đệm" labelStyle={{width:"190px"}}>
                              {dataStudent.first_name}
                            </Descriptions.Item>
                            <Descriptions.Item label="Tên">
                              {dataStudent.last_name}
                            </Descriptions.Item>
                            <Descriptions.Item label="Mã số học viên">
                              {dataStudent.enroll_number}
                            </Descriptions.Item> */}
                          <Descriptions.Item label="Số điện thoại">
                            {dataStudent.mobile_phone}
                          </Descriptions.Item>
                          <Descriptions.Item label="Giới tính">
                            {gender[dataStudent.gender.id]}
                          </Descriptions.Item>
                          <Descriptions.Item
                            label="Địa chỉ thường trú"
                            span={2}
                          >
                            {dataStudent.ward.prefix} {dataStudent.ward.name},{" "}
                            {dataStudent.district.prefix}{" "}
                            {dataStudent.district.name},{" "}
                            {dataStudent.province.name}
                          </Descriptions.Item>
                          <Descriptions.Item label="Địa chỉ cụ thể" span={2}>
                            {dataStudent.contact_address},{" "}
                            {dataStudent.ward.prefix} {dataStudent.ward.name},{" "}
                            {dataStudent.district.prefix}{" "}
                            {dataStudent.district.name},{" "}
                            {dataStudent.province.name}
                          </Descriptions.Item>
                          <Descriptions.Item label="Trạng thái">
                            {renderStatusStudent(dataStudent.status)}
                          </Descriptions.Item>
                          <Descriptions.Item label="Ngày sinh">
                            {new Date(dataStudent.birthday).toLocaleDateString(
                              "vi-VN"
                            )}
                          </Descriptions.Item>
                          <Descriptions.Item label="Email cá nhân" span={1}>
                            {dataStudent.email}
                          </Descriptions.Item>
                          <Descriptions.Item label="Email tổ chức" span={1}>
                            {dataStudent.email_organization}
                          </Descriptions.Item>
                        </Descriptions>
                      </Card.Body>
                    </Card>

                    <Card variant="bordered" css={{ marginBottom: "20px" }}>
                      <Card.Body>
                        <Descriptions
                          bordered
                          title="Thông tin phụ huynh"
                          column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                          // extra={

                          // }
                        >
                          <Descriptions.Item
                            label="Họ và tên phụ huynh"
                            labelStyle={{ width: "190px" }}
                          >
                            {dataStudent.parental_name}
                          </Descriptions.Item>
                          <Descriptions.Item label="Số điện thoại">
                            {dataStudent.parental_phone}
                          </Descriptions.Item>
                          <Descriptions.Item label="Quan hệ">
                            {dataStudent.parental_relationship}
                          </Descriptions.Item>
                        </Descriptions>
                      </Card.Body>
                    </Card>
                    <Card variant="bordered" css={{ marginBottom: "20px" }}>
                      <Card.Body>
                        <Descriptions
                          bordered
                          title="Thông tin liên quan đến học viện"
                          column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                          // extra={

                          // }
                        >
                          <Descriptions.Item
                            label="Học bổng"
                            labelStyle={{ width: "190px" }}
                          >
                            {dataStudent.promotion}
                          </Descriptions.Item>
                          <Descriptions.Item label="Kế hoạch phí">
                            {dataStudent.fee_plan}
                          </Descriptions.Item>
                          <Descriptions.Item label="Hồ Sơ">
                            <a href={dataStudent.application_document}>
                              Link hồ sơ
                            </a>
                          </Descriptions.Item>
                          <Descriptions.Item label="Ngày nộp">
                            {new Date(
                              dataStudent.application_date
                            ).toLocaleDateString("vi-VN")}
                          </Descriptions.Item>
                        </Descriptions>
                      </Card.Body>
                    </Card>
                    <Card variant="bordered" css={{ marginBottom: "20px" }}>
                      <Card.Body>
                        <Descriptions
                          bordered
                          title="Học vấn và công việc"
                          column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                          // extra={

                          // }
                        >
                          <Descriptions.Item
                            label="Trường cấp 3"
                            labelStyle={{ width: "190px" }}
                          >
                            {dataStudent.high_school}
                          </Descriptions.Item>
                          <Descriptions.Item label="Trường đại học">
                            {dataStudent.university}
                          </Descriptions.Item>
                          <Descriptions.Item label="Công ty">
                            {dataStudent.working_company}
                          </Descriptions.Item>
                          <Descriptions.Item label="Địa chỉ công ty">
                            {dataStudent.company_address}
                          </Descriptions.Item>
                          <Descriptions.Item label="Chức vụ">
                            {dataStudent.company_position}
                          </Descriptions.Item>
                          <Descriptions.Item label="Mức lương">
                            {dataStudent.company_salary
                              ? String(dataStudent.company_salary).replace(
                                  /\B(?=(\d{3})+(?!\d))/g,
                                  ","
                                )
                              : 0}{" "}
                            VNĐ
                          </Descriptions.Item>
                        </Descriptions>
                      </Card.Body>
                    </Card>
                  </items>

                  <items className="" tab="Thông tin học tập" key="2">
                    Thông tin học tập
                  </items>
                  <items tab="Thông tin khác " key="3">
                  <Card variant="bordered" css={{ marginBottom: "20px" }}>
                      <Card.Body>

                        <Descriptions
                          bordered
                          title="Các lớp đã học"
                          column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                          // extra={

                          // }
                        >
                          {dataStudent.old_class.map((item, index) => {
                            return (
                              <Descriptions.Item
                              label={dataStudent.course_code}
                                // labelStyle={{ width: "100%" }}
                                // style={{ width: "100%" }}
                              >
                                 {item.class_name}
                               
                              </Descriptions.Item>
                            );
                            
                          }
                          )}
                          {/* // <Descriptions.Item >
                          //   {dataStudent.old_class[0].class_name}
                          // </Descriptions.Item> */}
                          
                        </Descriptions>
                      </Card.Body>
                    </Card>
                  </items>
                </Tabs>
              </Grid>
              <Grid xs={4} css={{ position: "absolute", right: 24, top: 24 }}>
                <Dropdown>
                  <Dropdown.Button flat color="secondary">
                    Chức năng
                  </Dropdown.Button>
                  <Dropdown.Menu
                    onAction={handleSelectOption}
                    color="secondary"
                    aria-label="Actions"
                    css={{ $$dropdownMenuWidth: "340px" }}
                  >
                    <Dropdown.Section title="Cơ bản">
                      <Dropdown.Item
                        key="edit"
                        description="Chỉnh sửa thông tin học viên"
                        icon={<RiPencilFill />}
                        color={"primary"}
                      >
                        Chỉnh sửa
                      </Dropdown.Item>
                      {dataStudent.current_class !== null && dataStudent.is_draft == false  && (
                        <Dropdown.Item
                          key="change"
                          description="Chuyển lớp học viên"
                          icon={<FaSyncAlt />}
                          color={"warning"}
                        >
                          Chuyển lớp
                        </Dropdown.Item>
                      )}
                    </Dropdown.Section>

                    {/* <Dropdown.Section title="Nguy hiểm">
                          <Dropdown.Item
                            key="clear"
                            color={'error'}
                            description="Xóa học viên"
                            icon={<MdDelete />}
                          >
                            Xoá
                          </Dropdown.Item>
                        </Dropdown.Section>
              */}
                  </Dropdown.Menu>
                </Dropdown>
              </Grid>
            </Grid.Container>
          )}
        </Grid>
      </Grid.Container>
    </Fragment>
  );
};
export default StudentDetail;
