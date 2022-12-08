
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
import { ManageGpa, UserStudentApis } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
import { useEffect, useState } from "react";
import { Fragment } from "react";
import moment from "moment";
import "moment/locale/vi";
import toast from "react-hot-toast";


const ScheduleTeacherScreen = () => {

  const [listSession, setListSession] = useState([]);
  const [value, setValue] = useState(moment());
  const [selectedValue, setSelectedValue] = useState(moment());
  const [detailLearningDate, setDetailLearningDate] = useState([]);
  const [isLoading, setisLoading] = useState(false);

  const getData = () => {
    setisLoading(true);
    FetchApi(UserStudentApis.getAllSession)
      .then((res) => {
        setListSession(res.data);
        // setListExamDate(
        //   res.data.filter(
        //     (item) => item.session_type === 3 || item.session_type === 4
        //   )
        // );
        setisLoading(false);

      })
      .catch((err) => {
        toast.error("Đã xảy ra lỗi");
      });
  };

  const getDetail = () => {

    setDetailLearningDate("");
    // console.log(listSession);
    // console.log(listExamDate.map((item) => item.learning_date));
   
    const body = {
      learning_date: moment.utc(selectedValue).local().format(),
    };
    console.log(body);
    FetchApi(UserStudentApis.getDetailSession, body, null, null)
      .then((res) => {
        const data = res.data === null ? "" : res.data;
        // res.data.map((item) => {
        //   setDetailLearningDate(item);

        // });
        setDetailLearningDate(data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy chi tiết ngày học');
      });
      // console.log( "sss" + detailLearningDate.title);

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
//   useEffect(() => {
   
//     getDetail();
    
//   }, [selectedValue]);
  useEffect(() => {
    // getForm();
    // getData();
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
                            //   : listExamDate.find(
                            //       (item) =>
                            //         value.format("DD/MM/YYYY") ===
                            //         moment(item.learning_date).format(
                            //           "DD/MM/YYYY"
                            //         )
                            //     )
                            //   ? "#7828c8"
                            //   : listSession.find(
                            //       (item) =>
                            //         value.format("DD/MM/YYYY") ===
                            //         moment(item.learning_date).format(
                            //           "DD/MM/YYYY"
                            //         )
                            //     )
                            //   ? "#fdd8e5"
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
                <Grid xs={12}>
                  <Card
                    variant="bordered"
                    css={{
                      minHeight: "140px",
                      borderStyle: "dashed",
                    }}
                  >
                    <Card.Header css={{display:"flex",justifyContent:"space-around"}}>
                      
                        <div style={{display:"block",textAlign:"center"}}>
                          <Text p b size={14} css={{display:"block",width:"100%"}}>
                          Slot 3
                      </Text>

                        <Text p b size={14} css={{display:"block",width:"100%"}}>
                        Thời gian:  <Badge color="primary">
                            19:00 - 20:00
                        </Badge>
                        
                       
                        </Text>
                        
                        
                      </div>

                    
                    </Card.Header>

                    <Card.Body
                      css={{
                        padding: "5px 10px",
                      }}
                    >
                      
                        {/* <Text
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
                            <Table.Column width={200}>Môn học</Table.Column>
                            <Table.Column width={100}>Lớp</Table.Column>
                            <Table.Column width={150}>Phòng</Table.Column>

                          </Table.Header>
                          <Table.Body>
                            <Table.Row key="1">
                                <Table.Cell>
                                    HTML/CSS
                                </Table.Cell>
                                <Table.Cell>
                                    12A1
                                </Table.Cell>
                                <Table.Cell>
                                    101
                                </Table.Cell>

                            </Table.Row>
                          </Table.Body>
                        </Table> 
                  
                    </Card.Body>
                  </Card>
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
