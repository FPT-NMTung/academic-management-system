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

const gender = {
  1: "Nam",
  2: "Nữ",
  3: "Khác",
  4: "Không xác định",
};
const StudentDetail = () => {
  const [dataUser, setdataUser] = useState({});
  const [listSkill, setListSkill] = useState(undefined);
  const [cloneListSkill, setCloneListSkill] = useState(undefined);
  const [isLoading, setIsLoading] = useState(true);

  const { id } = useParams();
  const navigate = useNavigate();

  const getdataUser = () => {
    FetchApi(ManageTeacherApis.detailTeacher, null, null, [`${id}`])
      .then((res) => {
        setdataUser(res.data);
        setIsLoading(false);
      })
      .catch(() => {
        navigate("/404");
      });
  };
  const getListSkill = () => {

    FetchApi(ManageTeacherApis.getListSkillOfTeacher, null, null, [`${id}`])
      .then((res) => {
        console.log(res.data[0].skills);
        setListSkill(res.data[0].skills);
      })
      .catch(() => {
        toast.error('Lỗi lấy danh sách kỹ năng');
      });
  };

  useEffect(() => {
    getdataUser();
    getListSkill();
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
                    <Spacer y={1.3} />
                  </Card.Body>
                  
                </Card>
                <Card variant="bordered" css={{ marginBottom: "20px" }}>
                  <Card.Header>
                    <Grid.Container alignItems="center">
                

                      {/* <Descriptions
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
                          color: '#9e9e9e',
                          width: '100%',
                          textAlign: 'center',
                        }}
                      >
                        Chưa có kỹ năng
                      </Text>
                    )}
                    {listSkill === undefined && (
                      <div
                        style={{
                          display: 'flex',
                          justifyContent: 'center',
                        }}
                      >
                        <Loading size="xs" color={'error'} />
                      </div>
                    )}
                    </div>
                          <Text size={14} b></Text>
                      
                        </Descriptions.Item>
                      </Descriptions> */}
                      <Spacer y={0.6} />
                    </Grid.Container>
                  </Card.Header>
                </Card>
              </Grid>
            </Grid.Container>
          )}
        </Grid>
      </Grid.Container>
    </Fragment>
  );
};
export default StudentDetail;
