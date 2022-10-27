import { Grid, Spacer, Text, Badge, Card, Button } from "@nextui-org/react";
import classes from "./StudentDetail.module.css";
import { useParams, useNavigate } from "react-router-dom";
import { Descriptions, Spin, Tag } from "antd";
import { useState, useEffect } from "react";
import { AiFillPhone } from "react-icons/ai";
import { MdEmail } from "react-icons/md";
import { HiOfficeBuilding } from "react-icons/hi";
import FetchApi from "../../../../apis/FetchApi";
import { ManageTeacherApis } from "../../../../apis/ListApi";
import ManImage from "../../../../images/3d-fluency-businessman-1.png";
import WomanImage from "../../../../images/3d-fluency-businesswoman-1.png";
import { RiSettingsFill } from "react-icons/ri";
import { IoArrowBackCircle } from "react-icons/io5";

const StudentDetai = () => {
  const [dataStudent, setDataStudent] = useState({});
  const [isLoading, setIsLoading] = useState(false);

  const { id } = useParams();
  const navigate = useNavigate();

  const getDataStudent = () => {};
  return (
    <Grid.Container justify="center">
      <Grid sm={8}>
        <Card variant="bordered" css={{}}>
          <Card.Body
            css={{
              padding: 10,
              backgroundColor: "transparent !important",
            }}
          >
            <div className={classes.header}>
              <div className={classes.groupButton}>
                <Button
                  flat
                  auto
                  icon={<IoArrowBackCircle size={20} />}
                  color={"error"}
                  onPress={() => {
                    navigate("/sro/manage/student");
                  }}
                >
                  Trở về
                </Button>
              </div>
              <div className={classes.groupButton}>
              <Button
                flat
                auto
                type="primary"
                // onPress={() => {
                //   navigate(`/admin/account/teacher/${id}/update`);
                // }}
              >
                Chỉnh sửa thông tin
              </Button>
              </div>
            </div>
          </Card.Body>
        </Card>
      </Grid>
      <Grid sm={8}>
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
                  {/* {dataStudent.avatar && (
                        <img className={classes.avatar} src={dataStudent.avatar} />
                      )} */}
                  <img
                    className={classes.logo}
                    src="https://media-cldnry.s-nbcnews.com/image/upload/t_fit-1500w,f_auto,q_auto:best/rockcms/2022-09/justin-bieber-te-220906-50b634.jpg"
                  ></img>
                  {/* {!dataStudent.avatar && (
                        <img
                          className={classes.avatarMini}
                          src={
                            dataStudent.gender.id === 1
                              ? ManImage
                              : dataStudent.gender.id === 2
                              ? WomanImage
                              : ''
                          }
                        />
                      )} */}
                </div>
                <Spacer y={0.7} />
                <Text h3 size={20} b>
                  Lê Nhữ Bắc
                  {/* {dataStudent.first_name} {dataStudent.last_name} */}
                </Text>
              </div>
              <Spacer y={1} />
              <div className={classes.iconInformation}>
                <AiFillPhone />
                <Text p size={15}>
                  {/* {dataStudent.mobile_phone} */}
                  0375661741
                </Text>
              </div>
              <div className={classes.iconInformation}>
                <MdEmail />
                <Text p size={15}>
                  {/* {dataStudent.email} */}
                  baclnhe141006@fpt.edu.vn
                </Text>
              </div>
              <div className={classes.iconInformation}>
                <HiOfficeBuilding />
                <Text p size={15}>
                  {/* {dataStudent.email_organization} */}
                  baclnhe141006@fpt.edu.vn
                </Text>
              </div>
              <Spacer y={1} />
              <Card variant="bordered">
                <Card.Header>
                  <Grid.Container alignItems="center">
                    <Grid sm={6}>
                      <Text p size={14} b>
                        Kỹ năng
                      </Text>
                    </Grid>
                    <Grid
                      sm={6}
                      css={{
                        display: "flex",
                        justifyContent: "flex-end",
                      }}
                    >
                      <RiSettingsFill
                        size={16}
                        style={{
                          cursor: "pointer",
                        }}
                        // onClick={() => setIsEditSkill(!isEditSkill)}
                      />
                    </Grid>
                  </Grid.Container>
                </Card.Header>
                <Card.Body
                  css={{
                    marginTop: "0rem",
                  }}
                >
                  <div>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                    <Tag>magenta</Tag>
                  </div>
                </Card.Body>
              </Card>
            </Grid>
            <Grid sm={8.5} direction="column" css={{ rowGap: 20 }}>
              <Card variant="bordered">
                <Card.Body>
                  <Descriptions
                    title="Thông tin cơ bản"
                    bordered
                    column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
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
                  >
                    <Descriptions.Item label="Họ và tên đệm">
                      Nguyễn Trần Vân Nam
                      {/* {dataStudent.first_name} */}
                    </Descriptions.Item>
                    <Descriptions.Item label="Tên">
                      {/* {dataStudent.last_name} */}
                      Nam
                    </Descriptions.Item>
                    <Descriptions.Item label="Số điện thoại">
                      {/* {dataStudent.mobile_phone} */}
                      0375661741
                    </Descriptions.Item>
                    <Descriptions.Item label="Giới tính">
                      {/* {dataStudent.gender.id} */}
                      Nam
                    </Descriptions.Item>
                    <Descriptions.Item label="Địa chỉ" span={2}>
                      {/* {dataStudent.ward.prefix} {dataStudent.ward.name},{' '}
                          {dataStudent.district.prefix} {dataStudent.district.name},{' '}
                          {dataStudent.province.name} */}
                      Trung Sơn, Sầm Sơn, Thanh Hóa
                    </Descriptions.Item>
                    <Descriptions.Item label="Trạng thái">
                      {dataStudent.is_active && (
                        <Badge color={"success"} variant={"flat"}>
                          Đang hoạt động
                        </Badge>
                      )}
                      {!dataStudent.is_active && (
                        <Badge color={"error"} variant={"flat"}>
                          Dừng hoạt động
                        </Badge>
                      )}
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày sinh">
                      {/* {new Date(dataStudent.birthday).toLocaleDateString('vi-VN')} */}
                      12/12/2000
                    </Descriptions.Item>
                    <Descriptions.Item label="Email cá nhân" span={2}>
                      {/* {dataStudent.email} */}
                      baclnhe141006@gmail.com
                    </Descriptions.Item>
                    <Descriptions.Item label="Email tổ chức" span={2}>
                      {/* {dataStudent.email_organization} */}
                      baclnhe141006@fpt.edu.vn
                    </Descriptions.Item>
                  </Descriptions>
                </Card.Body>
              </Card>
              <Card variant="bordered">
                <Card.Body>
                  <Descriptions
                    title="Thông tin bổ sung"
                    bordered
                    column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                  >
                    <Descriptions.Item label="Biệt danh">
                      {/* {dataStudent.nickname ? (
                            dataStudent.nickname
                          ) : (
                            <i style={{ color: 'lightgray' }}>Không có</i>
                          )} */}
                      Bắc
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày bắt đầu dạy">
                      {/* {new Date(dataStudent.start_working_date).toLocaleDateString(
                            'vi-VN'
                          )} */}
                      Bắc
                    </Descriptions.Item>
                    <Descriptions.Item label="Nơi công tác" span={2}>
                      {/* {dataStudent.company_address ? (
                            dataStudent.company_address
                          ) : (
                            <i style={{ color: 'lightgray' }}>Không có</i>
                          )} */}
                      FPT University
                    </Descriptions.Item>
                    <Descriptions.Item label="Loại hợp đồng">
                      {/* {dataStudent.teacher_type.value} */}
                      CCCCCC
                    </Descriptions.Item>
                    <Descriptions.Item label="Mã số thuế">
                      {/* {dataStudent.tax_code} */}
                      203923233
                    </Descriptions.Item>
                    <Descriptions.Item label="Thời gian dạy">
                      {/* {dataStudent.working_time.value} */}1 năm
                    </Descriptions.Item>
                    <Descriptions.Item label="Mức lương">
                      {/* {dataStudent.salary
                            ? String(dataStudent.salary).replace(
                                /\B(?=(\d{3})+(?!\d))/g,
                                ','
                              )
                            : 0}{' '} */}
                      233232300 VNĐ
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
                      {/* {dataStudent.citizen_identity_card_no} */}
                      09302323223
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày cấp">
                      {/* {new Date(
                            dataStudent.citizen_identity_card_published_date
                          ).toLocaleDateString('vi-VN')} */}
                      23/04/1999
                    </Descriptions.Item>
                    <Descriptions.Item label="Nơi cấp">
                      {/* {dataStudent.citizen_identity_card_published_place} */}
                      Thanh Hóa
                    </Descriptions.Item>
                  </Descriptions>
                </Card.Body>
              </Card>
            </Grid>
          </Grid.Container>
        )}
      </Grid>
    </Grid.Container>
  );
};
export default StudentDetai;
