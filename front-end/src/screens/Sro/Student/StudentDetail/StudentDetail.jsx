import { Grid, Spacer, Text, Badge, Card, Button } from "@nextui-org/react";
import classes from "./StudentDetail.module.css";
import { useParams, useNavigate } from "react-router-dom";
import { Descriptions, Spin, Tag, Tabs, Divider } from "antd";
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
                  Mã số học viên
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
                      title="Thông tin bổ sung"
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
                        {dataStudent.district.name}, {dataStudent.province.name}
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
                    <Spacer y={1} />
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
                    <Spacer y={0.3}/>
                  </Grid.Container>
                </Card.Header>
              </Card>
            </Grid>

            <Grid sm={8.5} direction="column" css={{ rowGap: 20 }}>
           
         
                  <Tabs
                    defaultActiveKey="1"
                    tabBarStyle={{ fontStyle: "15px" }}
                    type="card"
                    size="large"

                    // closable={false}
                  >
                    <Tabs.TabPane tab="Thông tin học viên" key="1"  >
                      
                      <Card variant="bordered" css={{marginBottom:"20px"}}>
                        <Card.Body>
                          
                  <Button
                    css={{ position: "absolute", top: 10, right: 12 }}
                    flat
                    auto
                    type="primary"
                    onPress={() => {
                      navigate(`/sro/manage/student/${id}/update`);
                    }}
                  >
                    Chỉnh sửa thông tin
                  </Button>
                          <Descriptions
                            bordered
                            title="Thông tin cơ bản"
                            column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                            // extra={

                            // }
                          >
                            <Descriptions.Item label="Họ và tên đệm" labelStyle={{width:"190px"}}>
                              {dataStudent.first_name}
                            </Descriptions.Item>
                            <Descriptions.Item label="Tên">
                              {dataStudent.last_name}
                            </Descriptions.Item>
                            <Descriptions.Item label="Mã số học viên">
                              {dataStudent.enroll_number}
                            </Descriptions.Item>
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
                              {new Date(
                                dataStudent.birthday
                              ).toLocaleDateString("vi-VN")}
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

                      <Card variant="bordered" css={{marginBottom:"20px"}}>
                        <Card.Body>
                          <Descriptions
                            bordered
                            title="Thông tin phụ huynh"
                            column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                            // extra={

                            // }
                          >
                            <Descriptions.Item label="Họ và tên phụ huynh" labelStyle={{width:"190px"}}>
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
                      <Card variant="bordered" css={{marginBottom:"20px"}}>
                        <Card.Body>
                          <Descriptions
                            bordered
                            title="Thông tin liên quan đến học viện"
                            column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                            // extra={

                            // }
                          >
                        <Descriptions.Item label="Học bổng" labelStyle={{width:"190px"}}>
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
              <Card variant="bordered" css={{marginBottom:"20px"}}>
                        <Card.Body>
                          <Descriptions
                            bordered
                            title="Học vấn và công việc"
                            column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                            // extra={

                            // }
                          >
                        <Descriptions.Item label="Trường cấp 3" labelStyle={{width:"190px"}}>
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
                          {dataStudent.company_salary}
                        </Descriptions.Item>
                      </Descriptions>
                      </Card.Body>
              </Card>
                    </Tabs.TabPane>

                    <Tabs.TabPane tab="Thông tin học tập" key="2">
                      Content of Tab Pane 2
                    </Tabs.TabPane>
                    <Tabs.TabPane tab="Thông tin khác " key="3">
                      Content of Tab Pane 2
                    </Tabs.TabPane>
                  </Tabs>


          
            </Grid>
          </Grid.Container>
        )}
      </Grid>
    </Grid.Container>
  );
};
export default StudentDetail;
