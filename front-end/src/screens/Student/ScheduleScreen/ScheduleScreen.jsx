import classes from "./ScheduleScreen.module.css";
import CalendarStudent from "../../../components/CalendarStudent/CalendarStudent";
import {
  Card,
  Grid,
  Text,
  Badge,
  Spacer,
  Button,
  Loading,
  Table,
} from "@nextui-org/react";
import {
  Calendar,
  Select,
  Row,
  Col,
  Form,
  Input,
  Divider,
  Timeline,
} from "antd";
import TimelineStudent from "../../../components/TimelineStudent/TimelineStudent";
import { useNavigate, useParams } from "react-router-dom";
import { MdNoteAlt } from "react-icons/md";
import { ManageGpa, UserStudentApis } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
import { useEffect, useState } from "react";
import { Fragment } from "react";
import moment from "moment";
import "moment/locale/vi";
import toast from "react-hot-toast";

const Schedule = () => {
  const navigate = useNavigate();
  const [listLearningDate, setListLearningDate] = useState([]);
  const [dataUser, setDataUser] = useState({});
  const [listForm, setListForm] = useState([]);
  const [value, setValue] = useState(moment());
  const [selectedValue, setSelectedValue] = useState(moment());
  const [detailLearningDate, setDetailLearningDate] = useState(undefined);
  const [isLoading, setisLoading] = useState(true);
  const getForm = () => {
    setisLoading(true);
    FetchApi(ManageGpa.getForm)
      .then((res) => {
        setListForm(res.data);
        setisLoading(false);
      })
      .catch((err) => {
        toast.error("Đã xảy ra lỗi");
      });
  };
  const getData = () => {
    setisLoading(true);
    FetchApi(UserStudentApis.getScheduleStudent)
      .then((res) => {
        setDataUser(res.data);
        // console.log("sssss" +dataUser[0].start_date);
        setListLearningDate(res.data[0].sessions);
        setDetailLearningDate(res.data[0].sessions);
        // console.log(dataUser[0]);
        setisLoading(false);
      })
      .catch((err) => {
        toast.error("Đã xảy ra lỗi");
      });
  };

  useEffect(() => {
    // getForm();
    getData();
  }, []);

  return (
    <Fragment>
      {isLoading ? (
        <div className={classes.loading}>
          <Loading />
        </div>
      ) : (
        <Grid.Container gap={2} justify={"center"}>
          {/* <Grid xs={12} md={4}>
          <Card
            css={{
              width: "100%",
              height: "fit-content",
            }}
          >
            <Card.Body>
              <div className={classes.calendar}>
                <CalendarStudent />
                <Divider>
                  <Text p={true}>Chú thích</Text>
                </Divider>
                <div className={classes.content}>
                  <Grid.Container gap={0.5}>
                    <Grid xs={12} alignItems="center">
                      <Badge color="primary" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày đang chọn</Text>
                    </Grid>
                    <Grid xs={12} alignItems="center">
                      <Badge color="success" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày có lịch học</Text>
                    </Grid>
                    <Grid xs={12} alignItems="center">
                      <Badge color="warning" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày có lịch thi</Text>
                    </Grid>
                  </Grid.Container>
                </div>
              </div>
            </Card.Body>
          </Card>
        </Grid> */}
          <Grid xs={3}>
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
                  }}
                >
                  Lịch học
                </Text>
              </Card.Header>
              <Card.Body
                css={{
                  padding: "5px 10px",
                }}
              >
                <Calendar
                  fullscreen={false}
                  value={value}
                  onSelect={(value) => {
                    setValue(value);
                    setSelectedValue(value);
                  }}
                  headerRender={({ value, onChange }) => {
                    const start = 0;
                    const end = 12;
                    const monthOptions = [];

                    const months = [
                      "Tháng 1",
                      "Tháng 2",
                      "Tháng 3",
                      "Tháng 4",
                      "Tháng 5",
                      "Tháng 6",
                      "Tháng 7",
                      "Tháng 8",
                      "Tháng 9",
                      "Tháng 10",
                      "Tháng 11",
                      "Tháng 12",
                    ];

                    for (let i = start; i < end; i++) {
                      monthOptions.push(
                        <Select.Option key={i} value={i} className="month-item">
                          {months[i]}
                        </Select.Option>
                      );
                    }

                    const year = value.year();
                    const month = value.month();
                    const options = [];
                    for (let i = year - 10; i < year + 10; i += 1) {
                      options.push(
                        <Select.Option key={i} value={i} className="year-item">
                          {i}
                        </Select.Option>
                      );
                    }
                    return (
                      <div style={{ padding: 8 }}>
                        <Row gutter={8}>
                          <Col>
                            <Select
                              dropdownMatchSelectWidth={false}
                              value={year}
                              onChange={(newYear) => {
                                const now = value.clone().year(newYear);
                                onChange(now);
                              }}
                            >
                              {options}
                            </Select>
                          </Col>
                          <Col>
                            <Select
                              dropdownMatchSelectWidth={false}
                              value={month}
                              onChange={(newMonth) => {
                                const now = value.clone().month(newMonth);
                                onChange(now);
                              }}
                            >
                              {monthOptions}
                            </Select>
                          </Col>
                        </Row>
                      </div>
                    );
                  }}
                  mode="month"
                  dateFullCellRender={(value) => {
                    return (
                      <Card
                        variant={value.day() === 0 ? "flat" : "bordered"}
                        disableRipple={true}
                        css={{
                          fontSize: "12px",
                          width: "40px",
                          height: "40px",
                          margin: "1px auto",
                          display: "flex",
                          justifyContent: "center",
                          alignItems: "center",
                          color:
                            selectedValue.format("DD/MM/YYYY") ===
                            value.format("DD/MM/YYYY")
                              ? "#0072F5"
                              : selectedValue.format("MM/YYYY") ===
                                value.format("MM/YYYY")
                              ? "black"
                              : "lightgray",
                          fontWeight:
                            selectedValue.format("MM/YYYY") ===
                            value.format("MM/YYYY")
                              ? "500"
                              : "200",
                          backgroundColor:
                            value.format("DD/MM/YYYY") ===
                            selectedValue.format("DD/MM/YYYY")
                              ? "#CEE4FE"
                              : listLearningDate.find(
                                  (item) =>
                                    value.format("DD/MM/YYYY") ===
                                    moment(item.learning_date).format(
                                      "DD/MM/YYYY"
                                    )
                                ) ||
                                value.format("DD/MM/YYYY") ===
                                  moment(dataUser[0].start_date).format(
                                    "DD/MM/YYYY"
                                  ) ||
                                value.format("DD/MM/YYYY") ===
                                  moment(dataUser[0].end_date).format(
                                    "DD/MM/YYYY"
                                  )
                              ? "#fdd8e5"
                              : value.day() === 0
                              ? "#F1F1F1"
                              : // : value.format('DD/MM/YYYY') === moment(dataUser[0].start_date).format('DD/MM/YYYY')
                                // ? '#fdd8e5'
                                // : value.format('DD/MM/YYYY') === moment(dataUser[0].end_date).format('DD/MM/YYYY')
                                // ? '#fdd8e5'
                                "#fff",
                          borderRadius: "10em",
                          borderColor:
                            value.format("DD/MM/YYYY") ===
                            moment().format("DD/MM/YYYY")
                              ? "#0072F5"
                              : "",
                        }}
                        isPressable={true}
                      >
                        {value.date()}
                      </Card>
                    );
                  }}
                />
              </Card.Body>
              <Grid.Container css={{padding:"12px 14px"}} gap={0.5}>
                    <Grid xs={12} alignItems="center">
                      <Badge css={{backgroundColor:"#cee4fe"}}  variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày đang chọn</Text>
                    </Grid>
                    <Grid xs={12} alignItems="center">
                      <Badge css={{backgroundColor:"#fdd8e5"}}  variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày có lịch học</Text>
                    </Grid>
                    {/* <Grid xs={12} alignItems="center">
                      <Badge color="warning" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày có lịch thi</Text>
                    </Grid> */}
                  </Grid.Container>
            </Card>
          </Grid>
          <Grid xs={5}>
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
                  }}
                >
                  Lịch học của ngày {selectedValue.format("DD/MM/YYYY")}
                </Text>
              </Card.Header>
              <Grid.Container gap={2}>
                <Grid xs={12}>
                  <Card
                    variant="bordered"
                    css={{
                      minHeight: "140px",
                      borderStyle: "dashed",
                    }}
                  >
                    <Card.Header>
                      <Text p b size={14}>
                        Thời gian: <strong>8h30 - 10h30</strong>
                      </Text>
                    </Card.Header>
                    <Card.Body
                      css={{
                        padding: "5px 10px",
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
                          <Table.Column>Môn học</Table.Column>
                          <Table.Column width={100}>Phòng học</Table.Column>
                          <Table.Column width={200}>Giảng viên</Table.Column>
                          <Table.Column width={150}>Điểm danh</Table.Column>
                        </Table.Header>
                        <Table.Body>
                          <Table.Row key="1">
                            <Table.Cell>Phát triển ứng dụng web</Table.Cell>
                            <Table.Cell>Phòng 1</Table.Cell>
                            <Table.Cell>Nguyễn Văn A</Table.Cell>
                            <Table.Cell>
                              <Badge color="success">Đã điểm danh</Badge>
                            </Table.Cell>
                          </Table.Row>
                        </Table.Body>
                      </Table>
                    </Card.Body>
                  </Card>
                </Grid>
              </Grid.Container>
              {/* <Card.Body
              // isPressable={true}
              >
                <Timeline mode="center">
                  <Timeline.Item color="none">
                    <Text p b size={14}>
                      Thời gian: <strong> 8h30 - 10h30 </strong>
                    </Text>
                    <Card
                      variant="bordered"
                      css={{
                        borderStyle: "dashed",
                        margin: "0 20px",
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
                          <Table.Column>Môn học</Table.Column>
                          <Table.Column width={100}>Phòng học</Table.Column>
                          <Table.Column width={200}>Giảng viên</Table.Column>
                          <Table.Column width={150}>Điểm danh</Table.Column>
                        </Table.Header>
                        <Table.Body>
                          <Table.Row key="1">
                            <Table.Cell>Phát triển ứng dụng web</Table.Cell>
                            <Table.Cell>Phòng 1</Table.Cell>
                            <Table.Cell>Nguyễn Văn A</Table.Cell>
                            <Table.Cell>
                              <Badge color="success">Đã điểm danh</Badge>
                            </Table.Cell>
                          </Table.Row>
                        </Table.Body>
                      </Table>
                    </Card>
                  </Timeline.Item>
                </Timeline>
              </Card.Body> */}
            </Card>
          </Grid>
        </Grid.Container>
      )}
    </Fragment>
  );
};

export default Schedule;
