import {
  Grid,
  Card,
  Text,
  Tooltip,
  Button,
  Table,
  Badge,
  Loading,
} from "@nextui-org/react";
import { Form, Input, message, Select } from "antd";
import FetchApi from "../../../apis/FetchApi";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { useEffect } from "react";
import ColumnGroup from "antd/lib/table/ColumnGroup";
import { Fragment } from "react";
import classes from "./ManageClass.module.css";
import { RiEyeFill, RiPencilFill } from "react-icons/ri";
import { ManageClassApis, CenterApis } from "../../../apis/ListApi";

const ManageClass = () => {
  const [dataSource, setDataSource] = useState([]);
  const [isGetData, setIsGetData] = useState(true);
  const [listCenters, setListCenters] = useState([]);

  const [form] = Form.useForm();
  const navigate = useNavigate();
  const getData = () => {
    const param = {
      className: form.getFieldValue("class_name"),
      courseFamilyCode: form.getFieldValue("course_family_code"),
      sroName: form.getFieldValue("sro_name"),
    };

    const classStatusName = form.getFieldValue("class_status_name");
    // const center_Id = form.getFieldValue("center_id");
    const class_days = form.getFieldValue("class_days");

    if (classStatusName !== null) {
      param.classStatusId = classStatusName;
    }
    // if (center_Id !== null) {
    //   param.centerId = center_Id;
    // }
    if (class_days !== null) {
      param.classDaysId = class_days;
    }

    FetchApi(ManageClassApis.searchClass, null, param, null).then((res) => {
      const data = res.data;
      const mergeModuleRes = data
        .sort((a, b) => -(new Date(b.created_at) - new Date(a.created_at)))
        .map((e, index) => {
          return {
            key: e.id,
            center_id: e.center_id,
            ...e,
            center_name: e.center.name,
            class_name: e.name,
            course_family_code: e.course_family_code,
            sro_name: e.sro_first_name + " " + e.sro_last_name,
            class_status_id: e.class_status_id,
            class_days_id: e.class_days_id,
            created_at: e.created_at,
            updated_at: e.updated_at,
            start_date: e.start_date,
            completion_date: e.completion_date,
            graduation_date: e.graduation_date,
            class_hour_start: e.class_hour_start,
            class_hour_end: e.class_hour_end,
            // class_days: e.class_days.Value,
            class_days: e.class_days.value,
          };
        });

      setDataSource(mergeModuleRes);
      setIsGetData(false);
    });
  };
  const getListCenter = () => {
    FetchApi(CenterApis.getAllCenter).then((res) => {
      const data = res.data.map((e) => {
        return {
          key: e.id,
          ...e,
        };
      });
      setListCenters(data);
    });
  };
  const handleSubmitForm = () => {
    getData();
  };

  const handleClearInput = () => {
    form.resetFields();
    getData();
  };

  useEffect(() => {
    getListCenter();
    getData();
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
    } else if (id === 4) {
      return (
        <Badge variant="flat" color="default">
          Hủy
        </Badge>
      );
    } else {
      return (
        <Badge variant="flat" color="primary">
          Chưa lên lịch
        </Badge>
      );
    }
  };
  const renderDays = (id) => {
    if (id === 1) {
      return (
        <Badge variant="bordered" color="warning">
          2/4/6
        </Badge>
      );
    } else {
      return (
        <Badge variant="bordered" color="success">
          3/5/7
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
                class_status_name: null,
                class_days: null,
                course_family_code: "",
                // center_id: null,
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
                  <Select.Option key="103" value="5">
                    Chưa lên lịch
                  </Select.Option>
                  <Select.Option key="104" value="4">
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
                name="course_family_code"
                style={{ width: "calc(20% - 16px)" }}
              >
                <Input placeholder="Mã Chương Trình Học" />
              </Form.Item>
              {/* <Form.Item name="center_id" style={{ width: "calc(12% - 16px)" }}>
                <Select
                  placeholder="Cơ Sở"
                  style={{ width: "100%" }}
                  // disabled={listCenters.length === 0}
                  dropdownStyle={{ zIndex: 9999 }}
                >
                  {listCenters.map((e) => (
                    <Select.Option key={e.key} value={e.id}>
                      {e.name}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item> */}
              <Form.Item name="sro_name" style={{ width: "calc(20% - 16px)" }}>
                <Input placeholder="Người phụ trách" />
              </Form.Item>
              <Form.Item
                name="class_days"
                style={{ width: "calc(15% - 16px)" }}
              >
                <Select
                  placeholder="Ngày học"
                  style={{ width: "100%" }}
                  dropdownStyle={{ zIndex: 9999 }}
                >
                  <Select.Option key="105" value="1">
                    2-4-6
                  </Select.Option>
                  <Select.Option key="106" value="2">
                    3-5-7
                  </Select.Option>
                </Select>
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
      <Grid sm={10}>
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
                    navigate("/sro/manage-class/create");
                  }}
                >
                  + Tạo mới
                </Button>
              </Grid>
            </Grid.Container>
          </Card.Header>
          {isGetData && (
            <div className={classes.loading}>
              <Loading color="error" type="points-opacity" size="xl" />
            </div>
          )}
          {!isGetData && (
            <Table aria-label="" disabledKeys={["4"]}>
              <Table.Header>
                <Table.Column width={60}>STT</Table.Column>
                <Table.Column width={200}>Tên lớp học</Table.Column>
                <Table.Column width={100}>Cơ Sở</Table.Column>
                <Table.Column width={200}>Mã Chương Trình Học</Table.Column>
                <Table.Column width={200}>Người Phụ Trách</Table.Column>
                <Table.Column width={150}>Ngày Học</Table.Column>
                <Table.Column width={200}>Trạng Thái Lớp Học</Table.Column>
                <Table.Column width={200}>Chỉnh Sửa</Table.Column>
                <Table.Column width={200}>Xem Chi Tiết</Table.Column>
              </Table.Header>
              <Table.Body>
                {dataSource.map((data, index) => (
                  <Table.Row key={data.key}>
                    <Table.Cell>{index + 1}</Table.Cell>
                    <Table.Cell>
                      <b>{data.class_name}</b>
                    </Table.Cell>
                    <Table.Cell>{data.center_name}</Table.Cell>
                    <Table.Cell>{data.course_family_code}</Table.Cell>
                    <Table.Cell>{data.sro_name}</Table.Cell>
                    <Table.Cell css={{ textAlign: "start" }}>
                      {renderDays(data.class_days_id)}
                    </Table.Cell>
                    <Table.Cell css={{ textAlign: "start" }}>
                      {renderStatus(data.class_status_id)}
                    </Table.Cell>
                    <Table.Cell css={{ textAlign: "center" }}>
                      <RiPencilFill
                        size={20}
                        color="f42a70"
                        style={{ cursor: "pointer" }}
                        onClick={() => {
                          navigate(`/sro/manage-class/${data.key}/update/`);
                        }}
                      />
                    </Table.Cell>
                    <Table.Cell css={{ textAlign: "center" }}>
                      <RiEyeFill
                        size={20}
                        color="5EA2EF"
                        style={{ cursor: "pointer" }}
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
                color="error"
              />
            </Table>
          )}
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default ManageClass;
