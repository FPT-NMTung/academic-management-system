import {
  Grid,
  Card,
  Text,
  Tooltip,
  Button,
  Table,
  Badge,
} from "@nextui-org/react";
import { Form, Input, message } from "antd";
import FetchApi from "../../../apis/FetchApi";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { useEffect } from "react";
import ColumnGroup from "antd/lib/table/ColumnGroup";
import { Fragment } from "react";
import classes from "./ManageClass.module.css";
import { RiEyeFill } from "react-icons/ri";

const ManageClass = () => {
  const [dataSource, setDataSource] = useState([]);
  const [isGetData, setIsGetData] = useState(true);

  const [form] = Form.useForm();
  const navigate = useNavigate();

  const handleSubmitForm = () => {};

  const handleClearInput = () => {
    form.resetFields();
  };

  useEffect(() => {}, []);

  const renderStatus = (id) => {
    if (id === 1) {
      return (
        <Badge variant="flat" color="success">
          Đã lên lịch
        </Badge>
      );
    } else if (id === 2) {
      return (
        <Badge variant="flat" color="error">
          Đang học
        </Badge>
      );
    } else if (id === 3) {
      return (
        <Badge variant="flat" color="default">
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
    <Grid.Container gap={2}>
      <Grid sm={12}>
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
                class_status_name: "",
                course_code: "",
                center_id: "",
                sro_name: "",
                class_name: "",
              }}
              onFinish={handleSubmitForm}
            >
              <Form.Item
                name="class_status_name"
                style={{ width: "calc(15% - 16px)" }}
              >
                <Input placeholder="Trạng thái" />
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
                <Input placeholder="Mã khóa học" />
              </Form.Item>
              <Form.Item name="center_id" style={{ width: "calc(15% - 16px)" }}>
                <Input placeholder="Cơ Sở" />
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
      <Grid sm={12}>
        <Card variant="bordered">
          <Card.Header>
            <Grid.Container>
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
              <Table.Column width={320}>Tên lớp học</Table.Column>
              <Table.Column width={160}>Cơ Sở</Table.Column>
              <Table.Column width={250}>Mã Khóa Học</Table.Column>
              <Table.Column width={250}>Người phụ trách</Table.Column>
              <Table.Column>Trạng thái</Table.Column>
              <Table.Column width={20}></Table.Column>
            </Table.Header>
            <Table.Body>
              {dataSource.map((data, index) => (
                <Table.Row key={data.user_id}>
                  <Table.Cell>{index + 1}</Table.Cell>
                  <Table.Cell>
                    <b>{data.class_name}</b>
                  </Table.Cell>
                  <Table.Cell>{data.center_id}</Table.Cell>
                  <Table.Cell>{data.course_code}</Table.Cell>
                  <Table.Cell>{data.sro_name}</Table.Cell>
                  <Table.Cell>
                    {renderStatus(data.class_status_name.id)}
                  </Table.Cell>
                  <Table.Cell>
                    <RiEyeFill
                      size={20}
                      color="5EA2EF"
                      style={{ cursor: "pointer" }}
                    />
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
            <Table.Pagination shadow noMargin align="center" rowsPerPage={10} />
          </Table>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default ManageClass;
