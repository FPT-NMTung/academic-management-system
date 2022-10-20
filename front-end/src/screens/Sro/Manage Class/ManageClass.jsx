import {
  Grid,
  Card,
  Text,
  Tooltip,
  Button,
  Table,
  Badge,
} from "@nextui-org/react";
import { Form, Input, message, Select } from "antd";
import FetchApi from "../../../apis/FetchApi";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { useEffect } from "react";
import ColumnGroup from "antd/lib/table/ColumnGroup";
import { Fragment } from "react";
import classes from "./ManageClass.module.css";
import { RiEyeFill,RiRecycleFill } from "react-icons/ri";
import { ModulesApis } from '../../../apis/ListApi';

const ManageClass = () => {
  const [dataSource, setDataSource] = useState([]);
  const [isGetData, setIsGetData] = useState(true);

  const [form] = Form.useForm();
  const navigate = useNavigate();

  const handleSubmitForm = () => {};

  const handleClearInput = () => {
    form.resetFields();
  };

  useEffect(() => {
    setDataSource([
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 1,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 1,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 1,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 2,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 3,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 3,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 3,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 4,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 4,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 4,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 4,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
      {
        course_code: "Mã Khóa Học Ở Đây Này",
        class_days_id: 1,
        class_status_id: 4,
        class_status_name: "Đang học",
        class_name: "Tên Lớp Ở Đây Này 2",
        start_date: "2022-10-09 16:45:24.0000000",
        completion_date: "2022-12-09 16:45:28.0000000",
        graduation_date: "2022-12-15 16:45:32.0000000",
        class_hour_start: "12:30:30",
        class_hour_end: "16:30:30",
        created_at: "2022-10-09 16:45:56.0000000",
        updated_at: "2022-10-09 16:45:57.0000000",
        center_name: "Hà Lội",
        sro_name: "Người Phụ Trách Ở Đây Này",
      },
    ]);
  }, []);

  const renderStatus = (id) => {
    if (id === 1) {
      return (
        <Badge variant="flat" color="secondary">
          Đã lên lịch
        </Badge>
      );
    } else if (id === 2) {
      return (
        <Badge variant="flat" color="warning">
          Đang học
        </Badge>
      );
    } else if (id === 3) {
      return (
        <Badge variant="flat" color="success">
          Đã hoàn thành
        </Badge>
      );
    } else {
      return (
        <Badge variant="flat" color="default">
          Hủy
        </Badge>
      );
    }
  };

  return (
    <Grid.Container gap={2} justify="center">
      <Grid sm={10}>
        <Card variant="bordered">
          <Card.Body
            css={{
              padding: "10px",
            }}
          >
            <Form
              name="basic"
              layout="inline"
              form={form}
              initialValues={{
                sro_name: "",
                class_name: "",
              }}
              onFinish={handleSubmitForm}
            >
              <Form.Item
                name="class_status_name"
                style={{ width: "calc(15% - 16px)" }}
              >
                <Select
                  placeholder="Trạng thái lớp "
                  style={{ width: "100%" }}
                  dropdownStyle={{ zIndex: 9999 }}
                >
                  <Select.Option key="100" value="1">
                    Đã lên lịch
                  </Select.Option>
                  <Select.Option key="101" value="2">
                    Đang học
                  </Select.Option>
                  <Select.Option key="102" value="3">
                    Đã hoàn thành
                  </Select.Option>
                  <Select.Option key="103" value="4">
                    Huỷ
                  </Select.Option>
                </Select>
              </Form.Item>
              <Form.Item
                name="class_name"
                style={{ width: "calc(15% - 16px)" }}
              >
                <Input placeholder="Tên lớp" />
              </Form.Item>
              <Form.Item
                name="course_code"
                style={{ width: "calc(20% - 16px)" }}
              >
                <Input placeholder="Mã Khóa Học" />
              </Form.Item>
              <Form.Item name="center_id" style={{ width: "calc(15% - 16px)" }}>
                <Select
                  placeholder="Cơ Sở"
                  style={{ width: "100%" }}
                  dropdownStyle={{ zIndex: 9999 }}
                ></Select>
              </Form.Item>
              <Form.Item name="sro_name" style={{ width: "calc(20% - 16px)" }}>
                <Input placeholder="Người phụ trách" />
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
      <Grid sm={10} >
        <Card variant="bordered">
          <Card.Header>
            <Grid.Container >
              <Grid sm={1}></Grid>
              <Grid sm={10}>
                <Text
                  b
                  size={14}
                  p
                  css={{
                    width: "100%",
                    textAlign: "center",
                  }}
                >
                  Danh sách lớp học
                </Text>
              </Grid>
           
              <Grid sm={1}>
                <Button
                  flat
                  auto
                  css={{
                    width: "100%",
                  }}
                  type="primary"
                  onPress={() => {
                    navigate("/admin/account/teacher/create");
                  }}
                >
                  + Tạo mới
                </Button>
              </Grid>
            </Grid.Container>
          </Card.Header>
          <Table aria-label="">
            <Table.Header>        
              <Table.Column width={60}>STT</Table.Column>
              <Table.Column width={250}>Tên lớp học</Table.Column>
              <Table.Column width={100}>Cơ Sở</Table.Column>
              <Table.Column width={250}>Mã Khóa Học</Table.Column>
              <Table.Column width={250}>Người Phụ Trách</Table.Column>
              <Table.Column width={250}>Trạng Thái Lớp Học</Table.Column>

              <Table.Column width={100}>Xem Chi Tiết</Table.Column>
            </Table.Header>
            <Table.Body>
              {dataSource.map((data, index) => (
                <Table.Row             
                 key={data.user_id}>
                  <Table.Cell>{index + 1}</Table.Cell>
                  <Table.Cell>
                    <b>{data.class_name}</b>
                  </Table.Cell>
                  <Table.Cell>{data.center_name}</Table.Cell>
                  <Table.Cell>{data.course_code}</Table.Cell>
                  <Table.Cell>{data.sro_name}</Table.Cell>
                  <Table.Cell css={{ textAlign: "start" }}
                  >
                    {renderStatus(data.class_status_id)}
                  </Table.Cell>

                  <Table.Cell css={{ textAlign: "center" }}  >
                    <RiEyeFill 
                  
                      size={20}
                      color="5EA2EF"
                      style={{ cursor: "pointer" }}
                    />
                  </Table.Cell>
                  
                </Table.Row>
              ))}
            </Table.Body>
            <Table.Pagination shadow noMargin align="center" rowsPerPage={7} />
          </Table>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default ManageClass;
