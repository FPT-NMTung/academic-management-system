import {
  Grid,
  Spacer,
  Text,
  Badge,
  Card,
  Button,
  Dropdown,
  Modal,
  Table,
  Loading,
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
  Menu,
} from "antd";
import { useState, useEffect } from "react";
import { AiFillPhone } from "react-icons/ai";
import { MdDelete, MdPersonAdd, MdSave } from "react-icons/md";
import { HiHome } from "react-icons/hi";
import FetchApi from "../../../../apis/FetchApi";
import { ManageStudentApis, ModulesApis } from "../../../../apis/ListApi";
import ManImage from "../../../../images/3d-fluency-businessman-1.png";
import WomanImage from "../../../../images/3d-fluency-businesswoman-1.png";
import { FaCloudDownloadAlt, FaSyncAlt } from "react-icons/fa";
import { RiEyeFill, RiPencilFill } from "react-icons/ri";
import { Fragment } from "react";
import toast from "react-hot-toast";
import { MdMenuBook } from "react-icons/md";
import { ImLibrary } from "react-icons/im";

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
  const [listModuleSemester, setListModuleSemester] = useState([]);
  const [listGrade, setListGrade] = useState([]);
  const [listGradeFinal, setListGradeFinal] = useState([]);
  const [informationModule, setInformationModule] = useState(undefined);
  const [averagePracticeGrade, setAveragePracticeGrade] = useState(undefined);
  const { id } = useParams();
  const navigate = useNavigate();

  const getModuleSemester = () => {
    // setIsLoading(true);
    FetchApi(
      ManageStudentApis.getStudentSemesterWithClassAndModuleBySro,
      null,
      null,
      [`${id}`]
    )
      .then((res) => {
        const data = res.data === null ? "" : res.data;
        setListModuleSemester(data);
        // setIsLoading(false);
      })
      .catch((err) => {
        toast.error("Lỗi khi tải dữ liệu");
      });
  };

  const onSelectTree = async (moduleid, classid) => {
    setListGrade([]);

    const res1 = await FetchApi(ModulesApis.getModuleByID, null, null, [
      String(moduleid),
    ]);
    const info = res1.data;
    setInformationModule(info);

    const res2 = await FetchApi(
      ManageStudentApis.getGradeStudentBySro,
      null,
      null,
      [String(classid), String(moduleid), String(id)]
    );

    const listGradePractice = res2.data.filter(
      (item) => item.grade_category_id !== 6 && item.grade_category_id !== 8
    );

    const listGradeTheory = res2.data.filter(
      (item) => item.grade_category_id === 6 || item.grade_category_id === 8
    );

    setListGrade(listGradePractice);
    setListGradeFinal(listGradeTheory);

    console.clear();

    let avgPracticeGrade = 0;
    const hasResitPractice =
      listGradePractice.find((item) => item.grade_category_id === 7) !==
      undefined;

    const clone = [...listGradePractice].filter((item) => {
      if (hasResitPractice) {
        return item.grade_category_id !== 5;
      } else {
        return item.grade_category_id !== 7;
      }
    });

    for (let i = 0; i < clone.length; i++) {
      const gradeItem = clone[i];
      console.log(gradeItem.grade_item.grade);

      if (
        gradeItem.grade_category_id === 5 ||
        gradeItem.grade_category_id === 7
      ) {
        // console.log(true, gradeItem);
        avgPracticeGrade +=
          clone[i].grade_item.grade *
          (10 / info?.max_practical_grade) *
          (gradeItem.total_weight / clone[i].quantity_grade_item);
      } else {
        console.log(false, gradeItem);
        if (gradeItem.grade_item.grade === null) {
          avgPracticeGrade = 0;
        } else {
          avgPracticeGrade +=
            clone[i].grade_item.grade *
            (gradeItem.total_weight / clone[i].quantity_grade_item);
        }
      }
    }
    setAveragePracticeGrade(avgPracticeGrade / 100);
  };

  const getInformationModule = (moduleId) => {};

  const getDataStudent = () => {
    FetchApi(ManageStudentApis.detailStudent, null, null, [`${id}`])
      .then((res) => {
        setDataStudent(res.data);
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
    getModuleSemester();
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
                  // onChange={() => {
                  //   getModuleSemester();
                  // }}

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

                  <items className="" tab="Điểm số học viên" key="222222">
                    <Grid.Container gap={1} justify={"space-between"}>
                      <Grid xs={4}>
                        <Card
                          variant="bordered"
                          css={{
                            height: "fit-content",
                          }}
                        >
                          <Card.Header>
                            <Text
                              p
                              b
                              size={14}
                              css={{
                                width: "100%",
                                textAlign: "center",
                                marginBottom: "12px",
                              }}
                            >
                              Chọn môn học
                            </Text>
                          </Card.Header>
                          <Card.Divider />
                          <Card.Body>
                            {isLoading ? (
                              <Loading />
                            ) : (
                              <div style={{ color: "black !important" }}>
                                <Menu
                                  mode="inline"
                                  // defaultOpenKeys={["1"]}
                                  style={{ width: "100%" }}
                                >
                                  {listModuleSemester.map((item, index) => (
                                    <Fragment key={index}>
                                      <Menu.SubMenu
                                        style={{ color: "black!important" }}
                                        title={item.name}
                                        key={item.id}
                                        rootStyle={{ width: "100%" }}
                                        icon={<ImLibrary />}
                                      >
                                        {item.modules.map((modules, index) => (
                                          <Menu.Item
                                            key={modules.id + modules.class.id}
                                            rootStyle={{ width: "100%" }}
                                            onClick={() => {
                                              onSelectTree(
                                                modules.id,
                                                modules.class.id
                                              );
                                              getInformationModule(modules.id);
                                            }}
                                            icon={<MdMenuBook />}
                                          >
                                            <span>
                                              {modules.name +
                                                " ( " +
                                                modules.class.name +
                                                " )"}
                                            </span>
                                          </Menu.Item>
                                        ))}
                                      </Menu.SubMenu>
                                    </Fragment>
                                  ))}
                                </Menu>
                              </div>
                            )}
                          </Card.Body>
                        </Card>
                      </Grid>
                      <Grid xs={8}>
                        <Card variant="bordered">
                          <Card.Header>
                            <Text
                              p
                              b
                              size={14}
                              css={{
                                width: "100%",
                                textAlign: "center",
                              }}
                            >
                              Bảng điểm
                            </Text>
                          </Card.Header>
                          <Spacer y={0.6} />
                          <Card.Divider />
                          {
                            <Card.Body>
                              {informationModule?.max_practical_grade && (
                                <Text
                                  p
                                  i
                                  size={12}
                                  css={{
                                    paddingLeft: "10px",
                                  }}
                                >
                                  * Điểm tối đa của Practical exam:{" "}
                                  {informationModule?.max_practical_grade}
                                </Text>
                              )}
                              {informationModule?.max_theory_grade && (
                                <Text
                                  p
                                  i
                                  size={12}
                                  css={{
                                    paddingLeft: "10px",
                                  }}
                                >
                                  * Điểm tối đa của Theory exam:{" "}
                                  {informationModule?.max_theory_grade}
                                </Text>
                              )}
                              {listGrade.length > 0 && (
                                <Fragment>
                                  {/* {" "}
                  <Text
                    b
                    
                    size={18}
                    css={{
                      paddingLeft: "10px",
                      marginTop: "10px",
                    }}
                  >
                    {" "}
                    <Badge color="success">Thực hành</Badge> 
                  </Text> */}
                                  <Table
                                    aria-label=""
                                    css={{
                                      height: "auto",
                                      minWidth: "100%",
                                    }}
                                    lined
                                    headerLined
                                    shadow={false}
                                  >
                                    <Table.Header>
                                      <Table.Column width={200}>
                                        Loại điểm thành phần
                                      </Table.Column>
                                      <Table.Column width={100}>
                                        Trọng số
                                      </Table.Column>
                                      <Table.Column width={50}>
                                        Điểm
                                      </Table.Column>
                                    </Table.Header>
                                    <Table.Body>
                                      {listGrade.map((item, index) => (
                                        <Table.Row key={index}>
                                          <Table.Cell>
                                            {item.grade_item.name}
                                          </Table.Cell>
                                          <Table.Cell>
                                            {item.total_weight ? (
                                              <Badge color="warning">
                                                {Math.round(
                                                  (item.total_weight /
                                                    item.quantity_grade_item) *
                                                    100
                                                ) / 100}
                                                %
                                              </Badge>
                                            ) : (
                                              ""
                                            )}
                                          </Table.Cell>
                                          <Table.Cell b>
                                            {item.grade_item.grade !== null
                                              ? Math.round(
                                                  item.grade_item?.grade * 100
                                                ) / 100
                                              : " "}
                                          </Table.Cell>
                                        </Table.Row>
                                      ))}
                                    </Table.Body>
                                  </Table>
                                </Fragment>
                              )}
                              {listGrade.length > 0 &&
                                averagePracticeGrade !== 0 && (
                                  <div>
                                    <Text
                                      b
                                      i
                                      size={18}
                                      css={{
                                        paddingLeft: "10px",
                                      }}
                                    >
                                      Tổng điểm thực hành:{" "}
                                      <Badge color="success">
                                        {Math.round(
                                          averagePracticeGrade * 100
                                        ) / 100}{" "}
                                        {informationModule?.max_practical_grade ===
                                        null
                                          ? ""
                                          : "/ 10"}
                                      </Badge>
                                    </Text>
                                    <Text p i size={12}>
                                      (Đã quy về hệ số 10)
                                    </Text>
                                  </div>
                                )}
                              {listGradeFinal.length > 0 && (
                                <Card
                                  variant="bordered"
                                  css={{
                                    minHeight: "140px",
                                    borderStyle: "dashed",
                                    marginTop: "12px",
                                    borderColor: "#17c964",
                                  }}
                                >
                                  <Table
                                    aria-label=""
                                    css={{
                                      height: "auto",
                                      minWidth: "100%",
                                    }}
                                    lined
                                    headerLined
                                    shadow={false}
                                  >
                                    <Table.Header>
                                      <Table.Column width={200}>
                                        Điểm cuối kỳ lý thuyết
                                      </Table.Column>
                                      <Table.Column width={50}>
                                        Điểm
                                      </Table.Column>
                                    </Table.Header>
                                    <Table.Body>
                                      {listGradeFinal.map((item, index) => (
                                        <Table.Row key={index}>
                                          <Table.Cell>
                                            {item.grade_item.name}
                                          </Table.Cell>
                                          {/* <Table.Cell>
                            {item.total_weight !== null ? (
                              <Badge color="warning">
                                {Math.round(
                                  (item.total_weight /
                                    item.quantity_grade_item) *
                                    10
                                ) / 10}
                                %
                              </Badge>
                            ) : (
                              ''
                            )}
                          </Table.Cell> */}
                                          <Table.Cell b>
                                            {item.grade_item.grade
                                              ? Math.round(
                                                  item.grade_item?.grade * 100
                                                ) / 100
                                              : " "}
                                          </Table.Cell>
                                        </Table.Row>
                                      ))}
                                    </Table.Body>
                                  </Table>
                                </Card>
                              )}
                            </Card.Body>
                          }
                        </Card>
                      </Grid>
                    </Grid.Container>
                  </items>
                  <items tab="Thông tin khác " key="333333">
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
                          })}
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
                      {dataStudent.current_class !== null &&
                        dataStudent.is_draft == false && (
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
