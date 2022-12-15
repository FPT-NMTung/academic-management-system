import {
  Grid,
  Card,
  Text,
  Tooltip,
  Button,
  Table,
  Badge,
  Loading,
  Switch,
} from "@nextui-org/react";
import { Form, Input, message, DatePicker, Progress, Space } from "antd";
import FetchApi from "../../../apis/FetchApi";
import { useNavigate } from "react-router-dom";
import { ManageTeacherApis } from "../../../apis/ListApi";
import { useState } from "react";
import { useEffect } from "react";
import ColumnGroup from "antd/lib/table/ColumnGroup";
import { Fragment } from "react";
import classes from "../../Admin/Sro/SroScreen.module.css";
import { RiEyeFill } from "react-icons/ri";
import moment from "moment";
import { toast } from "react-hot-toast";
import { ErrorCodeApi } from "../../../apis/ErrorCodeApi";

const ManageTeacher = () => {
  const [dataSource, setDataSource] = useState([]);
  const [isGetData, setIsGetData] = useState(true);
  const [passRate, setPassRate] = useState(0);
  const [passRateAll, setPassRateAll] = useState(0)
  const [messageFailed, setMessageFailed] = useState(undefined);

  const [form1] = Form.useForm();
  const [form2] = Form.useForm();
  const navigate = useNavigate();

  const getListTeacher = (param) => {
    setIsGetData(true);
    FetchApi(ManageTeacherApis.searchTeacherBySro, null, param, null)
      .then((res) => {
        setDataSource(
          res.data.map((item, index) => {
            return {
              ...item,
              key: item.id,
              index: index + 1,
            };
          })
        );
        console.log(res.data);
        setIsGetData(false);
      })
      .catch((err) => {
        setIsGetData(false);
        if (err?.type_error) {
          return toast.error(ErrorCodeApi[err.type_error]);
        }
        toast.error("Lỗi lấy danh sách giáo viên");
      });
  };
  const getPassRateAllTeacherInAllTime = () => {
    FetchApi(ManageTeacherApis.getPassRateAllTeacherInAllTime, null, null, null)
      .then((res) => {
        const data = res.data === null ? "" : res.data;
        let passStudent = 0;
        let totalStudent = 0;

        for (let i = 0; i < data.length; i++) {
          passStudent += data[i].number_of_passed_students;
          totalStudent += data[i].number_of_all_students;
        }
        if (totalStudent === 0) {
          setPassRate(0);
        } else {
          setPassRate(passStudent / totalStudent);
          setPassRateAll(passStudent / totalStudent);
        }
      })
      .catch((err) => {
        if (err?.type_error) {
          return toast.error(ErrorCodeApi[err.type_error]);
        }
        toast.error("Lỗi lấy tỷ lệ học viên qua môn");
      });
  };

  const handleSubmitForm = () => {
    const data = form1.getFieldsValue();
    const param = {
      firstName: data.first_name.trim(),
      lastName: data.last_name.trim(),
      nickname: data.nickname.trim(),
      email: data.email.trim(),
      mobilePhone: data.mobile_phone.trim(),
      emailOrganization: data.email_organization.trim(),
    };

    getListTeacher(param);
  };
  const handleSubmitForm2 = () => {
    // setPassRate(0);
    const data2 = form2.getFieldsValue();
    const body = {
      from_date: moment.utc(data2.periodtime[0]).local().format(),
      to_date: moment.utc(data2.periodtime[1]).local().format(),
    };
    console.log(body);
    FetchApi(
      ManageTeacherApis.getPassRateAllTeacherOnPeriodTime,
      body,
      null,
      null
    )
      .then((res) => {
        const data = res.data === null ? "" : res.data;
        let passStudent = 0;
        let totalStudent = 0;

        for (let i = 0; i < data.length; i++) {
          passStudent += data[i].number_of_passed_students;
          totalStudent += data[i].number_of_all_students;
        }
        if (totalStudent === 0) {
          setPassRate(0);
        } else {
          setPassRate(passStudent / totalStudent);
        }
      })
      .catch((err) => {
        if (err?.type_error) {
          return toast.error(ErrorCodeApi[err.type_error]);
        }
        toast.error("Lỗi lấy tỷ lệ học viên qua môn trong khoảng thời gian này");
      });
  };

  const handleClearInput = () => {
    form1.resetFields();
    getListTeacher();
  };

  useEffect(() => {
    getListTeacher();
    getPassRateAllTeacherInAllTime();
  }, []);

  const renderGender = (id) => {
    if (id === 1) {
      return (
        <Badge variant="flat" color="success">
          Nam
        </Badge>
      );
    } else if (id === 2) {
      return (
        <Badge variant="flat" color="error">
          Nữ
        </Badge>
      );
    } else if (id === 3) {
      return (
        <Badge variant="flat" color="default">
          Khác
        </Badge>
      );
    } else {
      return (
        <Badge variant="flat" color="default">
          Không xác định
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
              form={form1}
              initialValues={{
                first_name: "",
                last_name: "",
                mobile_phone: "",
                nickname: "",
                email: "",
                email_organization: "",
              }}
              onFinish={handleSubmitForm}
            >
              <Form.Item
                name="first_name"
                style={{ width: "calc(15% - 16px)" }}
              >
                <Input placeholder="Họ" />
              </Form.Item>
              <Form.Item name="last_name" style={{ width: "calc(15% - 16px)" }}>
                <Input placeholder="Tên" />
              </Form.Item>
              <Form.Item name="nickname" style={{ width: "calc(10% - 16px)" }}>
                <Input placeholder="Biệt danh" />
              </Form.Item>
              <Form.Item
                name="mobile_phone"
                style={{ width: "calc(15% - 16px)" }}
              >
                <Input placeholder="Số điện thoại" />
              </Form.Item>
              <Form.Item name="email" style={{ width: "calc(15% - 16px)" }}>
                <Input placeholder="Email cá nhân" />
              </Form.Item>
              <Form.Item
                name="email_organization"
                style={{ width: "calc(15% - 16px)" }}
              >
                <Input placeholder="Email tổ chức" />
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
        <Card
          variant="bordered"
          css={{
            minHeight: "300px",
          }}
        >
          <Card.Header>
            <Grid.Container>
              <Grid sm={4}>
                <div style={{ height: "80px" }}>
                  <Text
                    b
                    size={12}
                    css={{
                      width: "100%",
                      textAlign: "left",
                      marginBottom: "2px",
                    }}
                  >
                    Tỷ lệ học viên qua môn từ:
                  </Text>
                  <Form
                    name="passrate"
                    layout="inline"
                    form={form2}
                    onFinish={handleSubmitForm2}
                  >
                    <Form.Item
                      name="periodtime"
                      rules={[
                        { required: true, message: "Vui lòng chọn thời gian" },
                      ]}
                    >
                      {/* <div style={{ width: 280 }}> */}
                      <DatePicker.RangePicker format={"DD-MM-YYYY"} />
                      {/* </div> */}
                    </Form.Item>
                    <Form.Item>
                      <Button
                        // disabled={form2.getFieldsValue().periodtime.length === 0}
                        auto
                        flat
                        type="primary"
                        htmlType="submit"
                      >
                        Xem
                      </Button>
                    </Form.Item>
                  </Form>
                  <div style={{ width: 300 }}>
                    <Progress
                      percent={Math.round(passRate * 1000) / 10}
                      size="small"
                      status={
                        Math.round(passRate * 1000) / 10 > 50
                          ? "success"
                          : "default"
                      }
                    />
                  </div>
                </div>
              </Grid>
              <Grid sm={4}>
                <Text
                  b
                  size={14}
                  p
                  css={{
                    width: "100%",
                    textAlign: "center",
                  }}
                >
                  Danh sách giáo viên
                </Text>
              </Grid>
              <Grid sm={4}></Grid>
            </Grid.Container>
          </Card.Header>
          {isGetData && <Loading />}
          {!isGetData && (
            <Table aria-label="">
              <Table.Header>
                <Table.Column width={60}>STT</Table.Column>
                <Table.Column width={270}>Họ và tên</Table.Column>
                <Table.Column width={150}>Biệt danh</Table.Column>
                <Table.Column width={150}>Cơ sở</Table.Column>
                <Table.Column width={250}>Email cá nhân</Table.Column>
                <Table.Column width={250}>Email tổ chức</Table.Column>
                <Table.Column>Số điện thoại</Table.Column>
                <Table.Column>Giới tính</Table.Column>
                <Table.Column width={20}></Table.Column>
              </Table.Header>
              <Table.Body>
                {dataSource.map((data, index) => (
                  <Table.Row key={data.user_id}>
                    <Table.Cell>{index + 1}</Table.Cell>
                    <Table.Cell>
                      <b>
                        {data.first_name} {data.last_name}
                      </b>
                    </Table.Cell>
                    <Table.Cell>{data.nickname}</Table.Cell>
                    <Table.Cell>{data.center_name}</Table.Cell>
                    <Table.Cell>{data.email}</Table.Cell>
                    <Table.Cell>{data.email_organization}</Table.Cell>
                    <Table.Cell>{data.mobile_phone}</Table.Cell>
                    <Table.Cell>{renderGender(data.gender.id)}</Table.Cell>
                    <Table.Cell>
                      <RiEyeFill
                        size={20}
                        color="5EA2EF"
                        style={{ cursor: "pointer" }}
                        onClick={() => {
                          navigate(`/sro/manage/teacher/${data.user_id}`);
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

export default ManageTeacher;
