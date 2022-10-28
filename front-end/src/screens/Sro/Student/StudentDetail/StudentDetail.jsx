import { Grid, Spacer, Text, Badge, Card, Button } from "@nextui-org/react";
import classes from "./StudentDetail.module.css";
import { useParams, useNavigate } from "react-router-dom";
import { Descriptions, Spin, Tag, Tabs } from "antd";
import { useState, useEffect } from "react";
import { AiFillPhone } from "react-icons/ai";
import { MdEmail } from "react-icons/md";
import { HiHome } from "react-icons/hi";
import FetchApi from "../../../../apis/FetchApi";
import { ManageStudentApis } from "../../../../apis/ListApi";
import ManImage from "../../../../images/3d-fluency-businessman-1.png";
import WomanImage from "../../../../images/3d-fluency-businesswoman-1.png";
import { RiPencilFill } from "react-icons/ri";
const gender = {
  1: "Nam",
  2: "Nữ",
  3: "Khác",
  4: "Không xác định",
};
const StudentDetail = () => {
  const [dataStudent, setDataStudent] = useState({});
  const [isLoading, setIsLoading] = useState(true);

  const { id } = useParams();
  const navigate = useNavigate();

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
  return (
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
              <div className={classes.contantLogo}>
                <div className={classes.logo}>
                  {dataStudent.avatar && (
                    <img className={classes.avatar} src={dataStudent.avatar} />
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
                  Mã số sinh viên
                </Text>
                <Text size={14}>{dataStudent.enroll_number}</Text>
              </div>
              <Card.Divider />
              <div className={classes.iconInformation}>
                <Text size={14} b>
                  Lớp học
                </Text>
                <Text p size={14}>
                  {dataStudent.class_name}
                </Text>
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

              <Spacer y={1} />
              <Card variant="bordered">
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
                      title="Thông tin cá nhân"
                      column={{ md: 1, lg: 1, xl: 1, xxl: 1 }}
                      // size="middle"
                      extra={
                        <Button
                          flat
                          auto
                          type="primary"
                          onPress={() => {
                            navigate(`/sro/manage/student/${id}/update`);
                          }}
                        >
                          <RiPencilFill />
                        </Button>
                      }
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
                          Số điện thoại:
                        </Text>
                        {dataStudent.mobile_phone}
                      </Descriptions.Item>
                      <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                        span={2}
                      >
                        <Text
                          size={14}
                          // css={{ display: "block", width: "100%" }}
                          b
                        >
                          Email cá nhân:
                          <Text size={14}></Text>
                        </Text>
                        {dataStudent.email}
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

                      <Descriptions.Item
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
                          Tên phụ huynh:
                        </Text>
                        {dataStudent.parental_name}
                      </Descriptions.Item>
                      <Descriptions.Item
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
                      </Descriptions.Item>
                      <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flexDirection: "row",
                          justifyContent: "space-between",
                        }}
                      >
                        <Text size={14} b>
                          SĐT phụ huynh:
                        </Text>
                        {dataStudent.parental_phone}
                      </Descriptions.Item>

                      <Descriptions.Item
                        contentStyle={{
                          display: "flex",
                          flex: 1,
                          // flexDirection: "row",
                          justifyContent: "start",
                        }}
                        span={2}
                      >
                        <Text
                          size={14}
                          css={{ display: "block", width: "100%" }}
                          b
                        >
                          Địa chỉ thường trú:
                          <Text size={15}>
                            {" "}
                            {dataStudent.ward.prefix} {dataStudent.ward.name},{" "}
                            {dataStudent.district.prefix}{" "}
                            {dataStudent.district.name},{" "}
                            {dataStudent.province.name}
                          </Text>
                        </Text>
                      </Descriptions.Item>
                    </Descriptions>
                  </Grid.Container>
                </Card.Header>
                <Card.Body
                  css={{
                    marginTop: "0rem",
                  }}
                ></Card.Body>
              </Card>
            </Grid>

            <Grid sm={8.5} direction="column" css={{ rowGap: 10 }}>
              <Card variant="bordered">
                <Card.Body>
                  {/* <Descriptions
                    title="Bảng điểm"
                    bordered
                    column={{ md: 1, lg: 1, xl: 1, xxl: 1 }}
                    // extra={
                    //   <Button
                    //     flat
                    //     auto
                    //     type="primary"
                    //     // onPress={() => {
                    //     //   navigate(`/admin/account/teacher/${id}/update`);
                    //     // }}
                    //   >
                    //     Chỉnh sửa thông tin
                    //   </Button>
                    // }
                  ></Descriptions> */}
                    <Tabs
                      defaultActiveKey="1"
                      tabBarStyle={{ fontStyle: "15px" }}
                      type="card"
                      size="large"
                      // closable={false}
                    >
                      <Tabs.TabPane tab="Học kỳ" key="1">
                        Content of Tab Pane 1
                      </Tabs.TabPane>
                      <Tabs.TabPane tab="Điểm" key="2">
                        Content of Tab Pane 2
                      </Tabs.TabPane>
                      <Tabs.TabPane tab="Chuyển lớp" key="3">
                        Content of Tab Pane 3
                      </Tabs.TabPane>
                    </Tabs>
                </Card.Body>
              </Card>
            </Grid>
          </Grid.Container>
        )}
      </Grid>
    </Grid.Container>
  );
};
export default StudentDetail;
