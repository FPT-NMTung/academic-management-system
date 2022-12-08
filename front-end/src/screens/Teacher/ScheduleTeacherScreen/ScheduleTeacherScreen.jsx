import classes from "./ScheduleTeacherScreen.module.css";
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
import { UserTeacherApis } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
import { useEffect, useState } from "react";
import { Fragment } from "react";
import moment from "moment";
import "moment/locale/vi";
import toast from "react-hot-toast";

const ScheduleTeacherScreen = () => {
  const [listTeachSession, setListTeachSession] = useState([]);
  const [value, setValue] = useState(moment());
  const [selectedValue, setSelectedValue] = useState(moment());
  const [detailTeachingDate, setDetailTeachingDate] = useState([]);
  const [isLoading, setisLoading] = useState(true);

  const getData = () => {
    setisLoading(true);
    FetchApi(UserTeacherApis.getAllTeachSession)
      .then((res) => {
        setListTeachSession(res.data);
        setisLoading(false);
        console.log(res.data);
      })
      .catch((err) => {
        toast.error("Lỗi lấy lịch dậy");
      });
  };

  const getDetail = () => {
    setDetailTeachingDate("");

    const body = {
      teach_date: moment.utc(selectedValue).local().format(),
    };
    console.log(body);
    FetchApi(UserTeacherApis.getDetailTeachSession, body, null, null)
      .then((res) => {
        const data = res.data === null ? "" : res.data;
        setDetailTeachingDate(data);
        console.log(detailTeachingDate);
      })
      .catch((err) => {
        toast.error("Lỗi lấy chi tiết ngày học");
      });
  };

  useEffect(() => {
    getDetail();
  }, [selectedValue]);
  useEffect(() => {
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
                              : //   : listExamDate.find(
                              //       (item) =>
                              //         value.format("DD/MM/YYYY") ===
                              //         moment(item.learning_date).format(
                              //           "DD/MM/YYYY"
                              //         )
                              //     )
                              //   ? "#7828c8"
                              listTeachSession.find(
                                  (item) =>
                                    value.format("DD/MM/YYYY") ===
                                    moment(item).format("DD/MM/YYYY")
                                )
                              ? "#fdd8e5"
                              : value.day() === 0
                              ? "#F1F1F1"
                              : // : value.format('DD/MM/YYYY') === moment(dataUser[0].end_date).format('DD/MM/YYYY')
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
              <Grid.Container css={{ padding: "12px 14px" }} gap={0.5}>
                <Grid xs={12} alignItems="center">
                  <Badge
                    css={{ backgroundColor: "#cee4fe" }}
                    variant="default"
                  />
                  <Spacer x={0.5} />
                  <Text css={{ ml: "$2" }}>Ngày đang chọn</Text>
                </Grid>
                <Grid xs={12} alignItems="center">
                  <Badge
                    css={{ backgroundColor: "#fdd8e5" }}
                    variant="default"
                  />
                  <Spacer x={0.5} />
                  <Text css={{ ml: "$2" }}>Ngày có lịch dạy</Text>
                </Grid>
              </Grid.Container>
            </Card>
          </Grid>
          <Grid xs={6}>
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
                  Lịch dạy của ngày {selectedValue.format("DD/MM/YYYY")}
                </Text>
              </Card.Header>
              <Grid.Container gap={2}>
                <Grid xs={12} direction={"column"}>
                {detailTeachingDate.length === 0 && (
                  <Card
                  variant="bordered"
                  css={{
                    minHeight: "140px",
                    borderStyle: "dashed",
                  }}
                  
                >
                <Text
                          p
                          size={14}
                          css={{
                            textAlign: "center",
                            marginTop: "20px",
                          }}
                          color={"lightGray"}
                          i
                        >
                          Không có lịch
                        </Text>
              
                </Card>
  )}
                  {detailTeachingDate.length > 0 && detailTeachingDate.map((item, index) => (
              <Card
                variant="bordered"
                css={{
                  minHeight: "140px",
                  borderStyle: "dashed",
                  marginBottom: "24px",
                }}
                key={index}
              >

                {detailTeachingDate.length > 0 && (
                <Card.Header
                          css={{
                            display: "flex",
                            justifyContent: "space-around",
                          }}
                        >
                          <div
                            style={{ display: "block", textAlign: "center" }}
                          >
                            <Fragment>
                            <Text
                              p
                              b
                              size={14}
                              css={{ display: "block", width: "100%" }}
                            >
                              {" "} 
                              {item.session_title}
                            </Text>

                            <Text
                              p
                              b
                              size={14}
                              css={{ display: "block", width: "100%" }}
                            >
                              Thời gian:{" "} 
                              <Badge color="success">
                                {moment(item?.start_time, "HH:mm:ss").format( "HH:mm")}
                        - {moment(item?.end_time, "HH:mm:ss").format( "HH:mm")} </Badge>
                        
                            </Text>
                            </Fragment>
                          </div>
                        </Card.Header>
                )}
                
                {detailTeachingDate.length > 0 && (
               
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
                              <Table.Column width={200}>Môn học</Table.Column>
                              <Table.Column width={100}>Nội dung</Table.Column>
                              <Table.Column width={100}>Lớp</Table.Column>
                              <Table.Column width={150}>Phòng</Table.Column>
                            </Table.Header>
                            <Table.Body>
                              <Table.Row key="1">
                                <Table.Cell>{item.module.name}</Table.Cell>
                                <Table.Cell>{item.session_type.value}</Table.Cell>
                                <Table.Cell>{item.class.name}</Table.Cell>
                                <Table.Cell>{item.room.name}</Table.Cell>
                              </Table.Row>
                            </Table.Body>
                          </Table> 
                
                )}
              </Card>
            ))}
                </Grid>
              </Grid.Container>

            </Card>

          </Grid>
        </Grid.Container>
      )}
    </Fragment>
  );
};

export default ScheduleTeacherScreen;
