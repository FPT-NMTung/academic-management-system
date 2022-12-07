import classes from "../../../../../components/ModuleCreate/ModuleCreate.module.css";
import {
  Card,
  Grid,
  Text,
  Badge,
  Spacer,
  Table,
  Modal,
  Button,
  Loading,
} from "@nextui-org/react";
import { Divider, Spin } from "antd";
import { MdNoteAlt } from "react-icons/md";
import toast from "react-hot-toast";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Fragment } from "react";
import {
  Descriptions,
  Skeleton,
  Form,
  Select,
  Upload,
  Input,
  Radio,
  Space,
} from "antd";
import { ManageGpa } from "../../../../../apis/ListApi";
import FetchApi from "../../../../../apis/FetchApi";
import React from "react";
import { ErrorCodeApi } from "../../../../../apis/ErrorCodeApi";

const DoFeedback = () => {
  const [form] = Form.useForm();

  const [listQnA, setListQnA] = useState([]);
  const [listForm, setListForm] = useState([]);
  const navigate = useNavigate();
  const [isLoading, setisLoading] = useState(true);
  const { id } = useParams();
  const [checked, setChecked] = React.useState("");
  const [messageFailed, setMessageFailed] = useState(undefined);
  const [scheduleInformation, setScheduleInformation] = useState([]);
  const getForm = () => {
    setisLoading(true);
    FetchApi(ManageGpa.getForm)
      .then((res) => {
        setListForm(res.data);
        setisLoading(false);
      })
      .catch((err) => {
        toast.error("Lỗi lấy dữ liệu");
      });
  };
  const getQandA = () => {
    setisLoading(true);
    FetchApi(ManageGpa.getQandA, null, null, ["1"])
      .then((res) => {
        setListQnA(res.data);
        console.log(res.data);
        setisLoading(false);
      })
      .catch((err) => {
        toast.error("Lỗi lấy dữ liệu");
      });
  };
  const getScheduleInformationBySessionId = () => {
    setisLoading(true);
    FetchApi(ManageGpa.getScheduleInformationBySessionId, null, null, [
      String(id),
    ])
      .then((res) => {
        setScheduleInformation(res.data);
        // console.log(res.data);
        setisLoading(false);
      })
      .catch((err) => {
        toast.error("Lỗi lấy thông tin lịch học");
      });
  };
  const handleSubmitForm = () => {
    const data = form.getFieldsValue();
    console.log(data);
    // const body = {
    //   class_id: scheduleInformation.class.id,
    //   teacher_id : scheduleInformation.teacher.id,
    //   module_id : scheduleInformation.module.id,
    //   session_id : id,
    //   comment: data.comment,
    //   answer_id: data.map((item,index) => item.question + (index+1)),
    // };
    const api = ManageGpa.studentTakeGPA;
    // const params = [`${id}`];
    // console.log(params);

    // toast.promise(FetchApi(api, body, null, ["1"]), {
    //   loading: 'Đang xử lý',
    //   success: (res) => {
    //     // setIsCreatingOrUpdating(false);
    //     navigate(`/student/schedule`);
    //     return 'Thành công';
    //   },
    //   error: (err) => {
    //     setMessageFailed(ErrorCodeApi[err.type_error]);
    //     // setIsCreatingOrUpdating(false);
    //     if (err?.type_error) {
    //       return ErrorCodeApi[err.type_error];
    //     }

    //     return 'Thất bại';
    //   },
    // })
  };

  useEffect(() => {
    getForm();
    getQandA();
    getScheduleInformationBySessionId();
    // console.log("Sessionid " +id);
  }, []);
  return (
    <Fragment>
      {isLoading && (
        <div className={classes.loading}>
          <Spin />
        </div>
      )}
      {!isLoading && (
        <Form
          layout="Vertical"
          labelAlign="center"
          onFinish={handleSubmitForm}
          form={form}
        >
          <Grid.Container gap={1} justify="center">
            <Grid sm={8} direction={"column"}>
              <Card
                variant="bordered"
                css={{
                  padding: "10px 20px",
                  borderTop: "solid 12px #cee4fe",
                  marginBottom: "12px",
                }}
              >
                {listForm.map((item, index) => (
                  <Fragment>
                    <Text
                      b
                      size={24}
                      p
                      css={{
                        textAlign: "start",
                        display: "inline-block",
                      }}
                    >
                      {item.title}
                    </Text>
                    <Spacer y={0.5} />
                    <Text
                      size={12}
                      p
                      css={{
                        width: "100%",
                        textAlign: "start",
                      }}
                    >
                      {item.description}
                    </Text>
                    <Spacer y={0.5} />
                    <Card.Divider />
                    <Spacer y={0.4} />
                    <Text
                      size={14}
                      p
                      css={{
                        width: "100%",
                        textAlign: "start",
                        color: "red",
                      }}
                    >
                      *Require
                    </Text>
                  </Fragment>
                ))}
              </Card>

              {listQnA.map((item, index) => (
                <Card
                  variant="bordered"
                  css={{ padding: "20px 20px", marginBottom: "12px" }}
                >
                  <Fragment>
                    <div style={{ dispay: "flex", flex: "1", width: "100%" }}>
                      <Text
                        b
                        size={16}
                        css={{
                          textAlign: "start",
                          // fontStyle: "italic",
                        }}
                      >
                        {item.content}
                        <span style={{ color: "red" }}> *</span>
                      </Text>
                    </div>
                    <Spacer y={1} />

                    <Form.Item
                      name={"question" + (index + 1)} //QUESTION+INDEX
                      style={{
                        width: "100%",
                        display: "block",
                      }}
                      key={"question" + (index + 1)}
                      // rules={[
                      //   {
                      //     required: true,
                      //     message: "Vui lòng chọn đáp án",
                      //   },
                      // ]}
                    >
                      {/* <Radio.Group  size="xs">
                      <Space direction="vertical">
                        {item.answer.map((answer, index) => (
                          <Radio
                          
                            color="error"
                            css={{ marginBottom: "12px" }}
                            value={answer.id}
                            key={answer.id}
                          >
                            {answer.content}
                          </Radio>
                        ))}
                      </Space>
                      </Radio.Group> */}
                      {/* <Text>You've checked: {checked}</Text> */}
                      <Radio.Group size="xs">
                        <Space direction="vertical">
                          <Radio
                            color="error"
                            css={{ marginBottom: "12px" }}
                            value={2}
                            key={121212}
                          >
                            đannád
                          </Radio>
                          <Radio
                            color="error"
                            css={{ marginBottom: "12px" }}
                            value={1}
                            key={122222}
                          >
                            sdsdsđsd
                          </Radio>
                        </Space>
                      </Radio.Group>

                      <Spacer y={0.5} />
                      <Card.Divider />
                    </Form.Item>
                  </Fragment>
                </Card>
              ))}
              <Card
                variant="bordered"
                css={{
                  padding: "10px 20px",
                  marginBottom: "12px",
                }}
              >
                <div style={{ dispay: "flex", flex: "1", width: "100%" }}>
                  <Text
                    b
                    size={16}
                    css={{
                      textAlign: "start",
                      // fontStyle: "italic",
                    }}
                  >
                    Các ý kiến khác:
                    {/* <span style={{ color: "red" }}> *</span> */}
                  </Text>
                </div>
                <Spacer y={1} />

                <Form.Item
                  name="comment" //QUESTION+INDEX
                  style={{
                    width: "100%",
                    display: "block",
                    height: "180px",
                  }}
                >
                  <Input.TextArea
                    style={{
                      width: "100%",
                      display: "block",
                      height: "180px",
                    }}
                    placeholder="Các góp ý khác của bạn về việc giảng dậy của giáo viên..."
                  ></Input.TextArea>

                  <Spacer y={0.5} />
                  <Card.Divider />
                </Form.Item>
              </Card>
              <Spacer y={0.5} />
              <Form.Item>
                <Button
                  auto
                  flat
                  css={{
                    width: "100px",
                  }}
                  type="primary"
                  htmlType="submit"
                >
                  {"Gửi"}
                </Button>
              </Form.Item>
            </Grid>
          </Grid.Container>
        </Form>
      )}
    </Fragment>
  );
};

export default DoFeedback;
