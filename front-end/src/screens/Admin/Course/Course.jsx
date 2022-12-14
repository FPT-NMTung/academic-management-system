import classes from "./Course.module.css";
import {
  Card,
  Grid,
  Text,
  Button,
  Loading,
  Modal,
  Table,
} from "@nextui-org/react";
import { Form, Select, Input, Divider } from "antd";
import { useEffect, useState } from "react";
import FetchApi from "../../../apis/FetchApi";
import { CourseApis } from "../../../apis/ListApi";
import CourseCreate from "../../../components/CourseCreate/CourseCreate";
import CourseUpdate from "../../../components/CourseUpdate/CourseUpdate";
import { MdEdit } from "react-icons/md";
import { FaPen } from "react-icons/fa";
const Course = () => {
  const [listCourse, setlistCourse] = useState([]);
  const [selectedCourseCode, setselectedCourseCode] = useState(null);
  const [isCreate, setIsCreate] = useState(false);
  const [IsLoading, setIsLoading] = useState(true);

  const getData = () => {
    setIsLoading(true);
    const apiCourse = CourseApis.getAllCourse;
    // console.log(apiCourse);
    FetchApi(apiCourse).then((res) => {
      const data = res.data;
      // console.log(data);
      data.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
      const mergeAllCourse = data.map((e, index) => {
        return {
          key: index,
          coursename: `${e.name}`,
          codecoursefamily: `${e.course_family_code}`,
          codecourse: `${e.code}`,
          semester_count: `${e.semester_count}`,
          activatecourse: e.is_active,
          ...e,
        };
      });

      setlistCourse(mergeAllCourse);
      setIsLoading(false);
    });
  };
  const sortCourseName = (a, b) => {
    if (a.coursename < b.coursename) {
      return -1;
    }
    if (a.coursename > b.coursename) {
      return 1;
    }
    return 0;
  };
  const handleAddSuccess = () => {
    setIsCreate(false);
    getData();
  };
  const handleUpdateSuccess = () => {
    setselectedCourseCode(null);

    getData();
  };
  useEffect(() => {
    getData();
  }, []);

  return (
    <div>
      <Grid.Container gap={2} justify="center">
        <Grid xs={7.5}>
          <Card
            variant="bordered"
            css={{
              width: "100%",
              height: "fit-content",
              minHeight: "200px",
            }}
          >
            <Card.Header>
              <div className={classes.headerTable}>
                <Grid.Container justify="space-between">
                  <Grid xs={4}></Grid>
                  <Grid xs={4}>
                    <Text
                      b
                      size={14}
                      p
                      css={{
                        width: "100%",
                        textAlign: "center",
                      }}
                    >
                      Danh s??ch c??c kh??a h???c
                    </Text>
                  </Grid>
                  <Grid xs={4} justify="flex-end">
                    <Button
                      type="primary"
                      onPress={() => {
                        setIsCreate(!isCreate);
                      }}
                      auto
                      flat
                    >
                      + T???o m???i
                    </Button>
                  </Grid>
                </Grid.Container>
              </div>
            </Card.Header>
            {IsLoading && <Loading />}
            {!IsLoading && (
                 <Table aria-label="">
                 <Table.Header>
                   <Table.Column>STT</Table.Column>
                   <Table.Column>T??n Kh??a H???c</Table.Column>
                   <Table.Column>M?? ch????ng tr??nh h???c</Table.Column>
                   <Table.Column>M?? Kh??a H???c</Table.Column>
                   <Table.Column>S??? l?????ng k??? h???c</Table.Column>
                   <Table.Column>Ch???nh s???a</Table.Column>
                 </Table.Header>
                 <Table.Body>
                   {listCourse.map((data, index) => (
                     <Table.Row key={index}>
                       <Table.Cell>{index + 1}</Table.Cell>
                       <Table.Cell>{data.coursename}</Table.Cell>
                       <Table.Cell>{data.codecoursefamily}</Table.Cell>
                       <Table.Cell>{data.codecourse}</Table.Cell>
                       <Table.Cell css={{ textAlign: "center" }}>
                         {data.semester_count}
                       </Table.Cell>
                       <Table.Cell css={{ textAlign: "center" }}>
                         <FaPen
                           size={14}
                           color="5EA2EF"
                           style={{ cursor: "pointer" }}
                           onClick={() => {
                            setselectedCourseCode(data.codecourse);
                           }}
                         />
                       </Table.Cell>
                     </Table.Row>
                   ))}
                 </Table.Body>
                 <Table.Pagination
                   shadow
                   noMargin
                   align="center"
                   rowsPerPage={5}
                   // color="error"
                 />
               </Table>
            // <Table
            //   loading={IsLoading}
            //   pagination={{ position: ["bottomCenter"] }}
            //   dataSource={listCourse}
            //   sortDirections={["descend", "ascend"]}
            //   rowClassName={(record, index) => {
            //     if (record.activatecourse === false) {
            //       return (record.classes = classes.rowDisable);
            //     }
            //   }}
            // >
            //   <Table.Column
            //     title="T??n Kh??a H???c"
            //     dataIndex="coursename"
            //     key="name"
            //     sorter={sortCourseName}
            //   />

            //   <Table.Column
            //     title="M?? ch????ng tr??nh h???c"
            //     dataIndex="codecoursefamily"
            //     key="codecoursefamily"
            //   />
            //   <Table.Column
            //     title="M?? Kh??a H???c"
            //     dataIndex="codecourse"
            //     key="codecourse"
            //   />
            //   <Table.Column
            //     title="S??? l?????ng k??? h???c"
            //     dataIndex="semester_count"
            //     key="semester_count"
            //   />
            //   <Table.Column
            //     title=""
            //     dataIndex="action"
            //     key="action"
            //     render={(_, data) => {
            //       return (
            //         <MdEdit
            //           className={classes.editIcon}
            //           onClick={() => {
            //             setselectedCourseCode(data.codecourse);
            //           }}
            //         />
            //       );
            //     }}
            //   />
            // </Table>
            )}
          </Card>
        </Grid>
        {isCreate && <CourseCreate onCreateSuccess={handleAddSuccess} />}
      </Grid.Container>
      <Modal
        closeButton
        aria-labelledby="modal-title"
        open={selectedCourseCode !== null}
        onClose={() => {
          setselectedCourseCode(null);
        }}
        blur
        width="700px"
      >
        <Modal.Header>
          <Text size={16} b>
            C???p nh???t th??ng tin kh??a h???c
          </Text>
        </Modal.Header>
        <Modal.Body>
          <CourseUpdate
            data={listCourse.find((e) => e.code === `${selectedCourseCode}`)}
            onUpdateSuccess={handleUpdateSuccess}
          />
        </Modal.Body>
      </Modal>
    </div>
  );
};
export default Course;
