import {
  Card,
  Grid,
  Text,
  Modal,
  Button,
  Table,
  Loading,
} from "@nextui-org/react";
import { Form, Select, Input, Divider } from "antd";
import { useEffect, useState } from "react";
import FetchApi from "../../../apis/FetchApi";
import { CourseFamilyApis } from "../../../apis/ListApi";
import classes from "./CourseFamily.module.css";
import { MdEdit } from "react-icons/md";
import CouseFamilyCreate from "../../../components/CourseFamilyCreate/CourseFamilyCreate";
import CourseFamilyUpdate from "../../../components/CourseFamilyUpdate/CourseFamilyUpdate";
import { FaPen } from "react-icons/fa";

const CourseFamily = () => {
  const [listCourseFamily, setlistCourseFamily] = useState([]);
  const [selectedCourseFamilyCode, setselectedCourseFamilyCode] =
    useState(null);

  const [IsLoading, setIsLoading] = useState(true);
  const [isCreate, setIsCreate] = useState(false);

  const getData = () => {
    const apiCourseFamily = CourseFamilyApis.getAllCourseFamily;
    // console.log(apiCourseFamily);
    FetchApi(apiCourseFamily).then((res) => {
      const data = res.data;

      data.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));

      const mergeAllCourseFamily = data.map((e, index) => {
        return {
          key: index,
          namecoursefamily: `${e.name}`,
          codefamily: `${e.code}`,
          codefamilyyear: `${e.published_year}`,
          activatecourse: e.is_active,
        };
      });
      setlistCourseFamily(mergeAllCourseFamily);
      setIsLoading(false);
    });
  };
  const handleAddSuccess = () => {
    setIsCreate(false);
    getData();
  };
  const handleUpdateSuccess = () => {
    setselectedCourseFamilyCode(null);

    getData();
  };
  useEffect(() => {
    getData();
  }, []);

  return (
    <div>
      <Grid.Container gap={2} justify="center">
        <Grid xs={6}>
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
                      Danh sách chương trình học
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
                      + Thêm
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
                  <Table.Column>Tên</Table.Column>
                  <Table.Column>Mã chương trình học</Table.Column>
                  <Table.Column>Năm áp dụng</Table.Column>
                  <Table.Column>Chỉnh sửa</Table.Column>
                </Table.Header>
                <Table.Body>
                  {listCourseFamily.map((data, index) => (
                    <Table.Row key={index}>
                      <Table.Cell>{index + 1}</Table.Cell>
                      <Table.Cell>{data.namecoursefamily}</Table.Cell>
                      <Table.Cell>{data.codefamily}</Table.Cell>
                      <Table.Cell css={{ textAlign: "center" }}>
                        {data.codefamilyyear}
                      </Table.Cell>
                      <Table.Cell css={{ textAlign: "center" }}>
                        <FaPen
                          size={14}
                          color="5EA2EF"
                          style={{ cursor: "pointer" }}
                          onClick={() => {
                            setselectedCourseFamilyCode(data.codefamily);
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
            )}
          </Card>
        </Grid>
        {isCreate && <CouseFamilyCreate onCreateSuccess={handleAddSuccess} />}
      </Grid.Container>
      <Modal
        closeButton
        aria-labelledby="modal-title"
        open={selectedCourseFamilyCode !== null}
        onClose={() => {
          setselectedCourseFamilyCode(null);
        }}
        blur
        width="500px"
      >
        <Modal.Header>
          <Text size={16} b>
            Cập nhật thông tin chương trình học
          </Text>
        </Modal.Header>
        <Modal.Body>
          <CourseFamilyUpdate
            data={listCourseFamily.find(
              (e) => e.codefamily === `${selectedCourseFamilyCode}`
            )}
            onUpdateSuccess={handleUpdateSuccess}
          />
        </Modal.Body>
      </Modal>
    </div>
  );
};
export default CourseFamily;
