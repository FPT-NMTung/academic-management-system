import {
  Card,
  Grid,
  Text,
  Button,
  Table,
  Badge,
  Loading,
} from "@nextui-org/react";
import { Form, Input, message, Select } from "antd";
import { useEffect, useState } from "react";
import {ManageStudentApis} from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
import { useNavigate } from "react-router-dom";
import { RiEyeFill, RiPencilFill } from "react-icons/ri";

const StudentScreen = () => {
  const [form] = Form.useForm();
  const [dataSource, setDataSource] = useState([]);
  const [IsLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();
  const getData = () => {
    setIsLoading(true);
    const param = {
      studentName: form.getFieldValue("studentName").trim(  ),
      enrollNumber: form.getFieldValue("enrollNumber"),
      className: form.getFieldValue("className"),
      emailOrganization: form.getFieldValue("emailOrganization"),
      email: "",
      courseCode: form.getFieldValue("courseCode"),
    }
FetchApi(ManageStudentApis.searchStudent,null,param,null).then((res) => {
      const data = res.data;
      const mergeModuleRes = data
      .sort((a, b) => -((a.last_name) - (b.last_name)))
      .map((e, index) => {
        return {
          key: e.user_id,
          enrollNumber: `${e.enroll_number}`,
          studentName: `${e.first_name} ${e.last_name}`,
          ...e,
          gender: e.gender.value,
          birthday: `${new Date(e.birthday).toLocaleDateString('vi-VN')}`,
          mobile_phone:e.mobile_phone,
          email_organization:e.email_organization,
          // class_name:e.current_class.class_name,
          class_name:e.current_class?.class_name ? e.current_class.class_name : "",
          citizen_identity_card_no:e.citizen_identity_card_no,
          parental_phone:e.parental_phone,
          status:e.status,
          course_code:e.course_code,
          // className: `${e.class_name}`,
          // email: `${e.email}`,
          // courseCode: `${e.course_code}`,
          // ...e,
        };
      });
      setDataSource(mergeModuleRes);
      setIsLoading(false);
    });
  };


  const handleClearInput = () => {
    form.resetFields();
    getData();
  };
  const handleSubmitForm = () => {
    getData();
  };
  useEffect(() => {
    getData();
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
    }
    else if (id === 5) {
      return (
        <Badge variant="flat" color="default">
          Transfer
        </Badge>
      );
    }else if (id === 6) {
      return (
        <Badge variant="flat" color="error">
          Upgrade
        </Badge>
      );
    }
     else {
      return (
        <Badge variant="flat" color="success">
          Finished
        </Badge>
      );
    }
  };
  return (
    <Grid.Container gap={2} justify="center">
      <Grid sm={12}>
        <Card variant="bordered">
          <Card.Body
            css={{
              padding: 10,
            }}
          >
            <Form
              name="basic"
              layout="inline"
              form={form}
              initialValues={{
                studentName: "",
                enrollNumber: "",
                className: "",
                emailOrganization: "",
                courseCode: "",
              }}
              onFinish={handleSubmitForm}
            >
              <Form.Item
                name="studentName"
                style={{ width: "calc(16% - 16px)" }}
              >
                <Input placeholder="Tên" />
              </Form.Item>
              <Form.Item
                name="enrollNumber"
                style={{ width: "calc(18% - 16px)" }}
              >
                <Input placeholder="Mã Sinh Viên" />
              </Form.Item>
              <Form.Item
                name="className"
                style={{ width: "calc(20% - 16px)" }}
              >
                <Input placeholder="Lớp" />
              </Form.Item>
              <Form.Item name="emailOrganization" style={{ width: "calc(15% - 16px)" }}>
                <Input placeholder="Email tổ chức" />
              </Form.Item>
              <Form.Item
                name="courseCode"
                style={{ width: "calc(16% - 16px)" }}
              >
                <Input placeholder="Mã khóa học" />
              </Form.Item>
              <Form.Item style={{ width: "calc(9% - 16px)" }}>
                <Button
                  flat
                  auto
                  type="primary"
                  htmlType="submit"
                  css={{ width: "100%" }}
                >
                  Tìm kiếm
                </Button>
              </Form.Item>
              <Form.Item style={{ width: "6%", marginRight: 0 }}>
                <Button
                  flat
                  auto
                  color={"error"}
                  css={{
                    width: "100%",
                  }}
                  onPress={handleClearInput}
                >
                  Huỷ
                </Button>
              </Form.Item>
            </Form>
          </Card.Body>
        </Card>
      </Grid>
      <Grid sm={12}>
        <Card variant="bordered">
          <Card.Header>
          <Grid.Container>
          <Grid sm={1}></Grid>
          <Grid sm={10}>
            <Text b p size={14} css={{ width: "100%", textAlign: "center" }}>
              Danh sách học viên
            </Text>
            </Grid>
            <Grid sm={1}> </Grid>
            </Grid.Container>

          </Card.Header>
          {IsLoading && <Loading/>}
            {!IsLoading && (
              <Table aria-label="">
                <Table.Header>
                <Table.Column width={150}>Mã sinh viên</Table.Column>
                  <Table.Column width={170}>Họ và tên</Table.Column>
                  <Table.Column width={80}>Giới tính</Table.Column>
                  <Table.Column width={110}>Ngày sinh</Table.Column>
                  <Table.Column css={{textAlign:"start"}} width={80}>Lớp học</Table.Column>
                  <Table.Column width={110}>Số điện thoại</Table.Column>
                  <Table.Column width={210}>Email tổ chức</Table.Column>                
                  <Table.Column width={140}>CMT/CCCD</Table.Column>
                  <Table.Column width={130}>SĐT Người giám hộ</Table.Column>
                  <Table.Column css={{textAlign:"center"}} width={120}>Mã khóa học</Table.Column>
                  <Table.Column css={{textAlign:"center"}} width={100}>Trạng thái</Table.Column>
                  <Table.Column width={100}>Xem chi tiết</Table.Column>
                </Table.Header>
                <Table.Body>
                  {dataSource.map((data, index) => (
                    <Table.Row key={data.key}>
                    <Table.Cell css={{textAlign:"start"}} >{data.enrollNumber}</Table.Cell>
                      <Table.Cell><b> {data.studentName}</b></Table.Cell>
                      <Table.Cell >{data.gender}</Table.Cell>
                      <Table.Cell>{data.birthday}</Table.Cell>
                      <Table.Cell>{data.class_name}</Table.Cell>                      
                      <Table.Cell>{data.mobile_phone}</Table.Cell>
                      <Table.Cell>{data.email_organization}</Table.Cell>
                      <Table.Cell>{data.citizen_identity_card_no}</Table.Cell>
                      <Table.Cell css={{textAlign:"center"}}>{data.parental_phone}</Table.Cell>
                      <Table.Cell>{data.course_code}</Table.Cell>
                      <Table.Cell css={{textAlign:"center"}}>{renderStatusStudent(data.status)}</Table.Cell>
                      <Table.Cell css={{textAlign:"center"}}>
                        <RiEyeFill
                          size={20}
                 
                          color="5EA2EF"
                          style={{ cursor: "pointer" }}
                          onClick={() => {
                            navigate(
                              `/sro/manage/student/${data.user_id}`
                            );
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
                  rowsPerPage={9}
                />
              </Table>
            )}
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default StudentScreen;
