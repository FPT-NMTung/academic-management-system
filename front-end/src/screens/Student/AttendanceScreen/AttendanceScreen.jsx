import classes from "./AttendanceScreen.module.css";
import {
  Button,
  Card,
  Grid,
  Loading,
  Modal,
  Table,
  Text,
  Spacer,
  Badge,
} from "@nextui-org/react";
import {
  Calendar,
  Select,
  Row,
  Col,
  Form,
  Input,
  Tree,
  Menu,
  Space,
} from "antd";
import { Fragment, useState } from "react";
import moment from "moment";
import "moment/locale/vi";
import { useEffect } from "react";
import toast from "react-hot-toast";
import { MdDelete } from "react-icons/md";
import { UserStudentApis } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
import { MdMenuBook } from "react-icons/md";
import { ImLibrary } from "react-icons/im";
import Item from "antd/lib/list/Item";
const AttendanceScreen = () => {
  const [listModuleSemester, setListModuleSemester] = useState([]);
  const [listAttendance, setListAttendance] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  // const [isLoadingGrade, setIsLoadingGrade] = useState(true);

  const getModuleSemester = () => {
    setIsLoading(true);
    FetchApi(UserStudentApis.getModuleSemester) // get list module semester
      .then((res) => {
        // res.data.map((item) => {
        //   setListModuleSemester(item);
        // });
        setListModuleSemester(res.data);
        // console.log(listModuleSemester.map((item) => item.name));
        // console.log(listModuleSemester.modules);
        setIsLoading(false);
      })
      .catch((err) => {
        toast.error("Lỗi khi tải dữ liệu");
      });
  };
  const onSelectTree = (moduleid, classid) => {
    setListAttendance([]);
    // setIsLoadingGrade(false);
    console.log("selected module " + moduleid + "classid " + classid);
    // console.log(FetchApi(UserStudentApis.getGradesbyclass, null, null, [String(classid),String(moduleid)]));
    FetchApi(UserStudentApis.getAttendance, null, null, [
      String(classid),
      String(moduleid),
    ])
      .then((res) => {
        setListAttendance(res.data);
        console.log(listAttendance);
        // console.log(listGrade.map((item) => item.name));
      })
      .catch((err) => {
        // toast.error("Lỗi khi tải điểm");
      });
  };
  const renderAttendanceStatus = (id) => {
    if (id === 1) {
      return <Badge color="default">Chưa điểm danh</Badge>;
    } else if (id === 2) {
      return <Badge color="error">Vắng mặt</Badge>;
    } else if (id === 3) {
      return <Badge color="success">Đã điểm danh</Badge>;
    } else {
      return <Badge color="default">Chưa điểm danh</Badge>;
    }
  };
  useEffect(() => {
    getModuleSemester();
  }, []);

  return (
    <Grid.Container gap={2} justify={"center"}>
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
                  defaultOpenKeys={["1"]}
                  style={{ width: "100%" }}
                >
                  {listModuleSemester.map((item, index) => (
                    <Fragment key={index}>
                      <Menu.SubMenu
                        style={{ color: "black!important" }}
                        title={item.name}
                        key={index + 1}
                        rootStyle={{ width: "100%" }}
                        icon={<ImLibrary />}
                      >
                        {item.modules.map((modules, index) => (
                          <Menu.Item
                            // title={modules.name + " ( " + modules.class.name + " )"}
                            key={index + 2}
                            rootStyle={{ width: "100%" }}
                            onClick={() =>
                              onSelectTree(modules.id, modules.class.id)
                            }
                            icon={<MdMenuBook />}
                          >
                            <span>
                              {modules.name + " ( " + modules.class.name + " )"}
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
              Xem điểm danh
            </Text>
          </Card.Header>
          <Spacer y={0.6} />
          <Card.Divider />
          <Card.Body>
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
                <Table.Column width={50}>STT</Table.Column>
                <Table.Column css={{ textAlign: "center" }} width={130}>
                  Ngày
                </Table.Column>
                <Table.Column css={{ textAlign: "center" }} width={30}>
                  Slot
                </Table.Column>
                <Table.Column width={100}>Phòng</Table.Column>
                <Table.Column width={100}>Giáo viên</Table.Column>
                <Table.Column>Lớp học</Table.Column>
                <Table.Column css={{ textAlign: "center" }} width={100}>
                  Trạng thái
                </Table.Column>
                <Table.Column>Chú thích</Table.Column>
              </Table.Header>
              <Table.Body>
                {listAttendance.map((item, index) => (
                  <Table.Row key={item.id}>
                    <Table.Cell>{index + 1}</Table.Cell>
                    <Table.Cell>
                      <Badge color="primary">
                        {moment(item.learning_date).format("ddd DD/MM/YYYY")}
                      </Badge>
                    </Table.Cell>
                    <Table.Cell>
                      <Badge color="secondary">
                        {moment(item?.start_time, "HH:mm:ss").format("H:mm")}-{" "}
                        {moment(item?.end_time, "HH:mm:ss").format("H:mm")}
                      </Badge>
                    </Table.Cell>
                    <Table.Cell>{item.room.name}</Table.Cell>
                    <Table.Cell>
                      {item.teacher.first_name} {item.teacher.last_name}
                    </Table.Cell>
                    <Table.Cell>{item.class.name}</Table.Cell>
                    <Table.Cell>
                      {renderAttendanceStatus(item.attendance_status?.id)}
                    </Table.Cell>
                    <Table.Cell css={{ color: "#f31260" }}>
                      {item?.note}{" "}
                    </Table.Cell>
                  </Table.Row>
                ))}
              </Table.Body>
            </Table>
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default AttendanceScreen;
