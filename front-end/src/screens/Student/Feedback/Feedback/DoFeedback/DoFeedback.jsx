import classes from "../../../../../components/ModuleCreate/ModuleCreate.module.css";
import {
  Radio,
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
import { Descriptions, Skeleton, Form, Select, Upload, Input } from "antd";
import { ManageGpa } from "../../../../../apis/ListApi";
import FetchApi from "../../../../../apis/FetchApi";

const DoFeedback = () => {
  const [form] = Form.useForm();

  const [listQnA, setListQnA] = useState([]);
  const [listForm, setListForm] = useState([]);
  const navigate = useNavigate();
  const [isLoading, setisLoading] = useState(true);
  const { id } = useParams();
  const getForm = () => {
    setisLoading(true);
    FetchApi(ManageGpa.getForm)
      .then((res) => {
        setListForm(res.data);
        setisLoading(false);
      })
      .catch((err) => {
        navigate("/404");
      });
  };
  const getQandA = () => {
    setisLoading(true);
    FetchApi(ManageGpa.getQandA, null, null, [String(id)])
      .then((res) => {
        setListQnA(res.data);
        setisLoading(false);
      })
      .catch((err) => {
        navigate("/404");
      });
  };

  useEffect(() => {
    getForm();
    getQandA();
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
          // onFinish={handleSubmitForm}
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
                      name="class_name"
                      style={{
                        width: "100%",
                        display: "block",
                      }}
                      rules={[
                        {
                          required: true,
                          message: "Vui lòng chọn đáp án",
                        },
                      ]}
                    >
                      <Radio.Group label="" defaultValue="0" size="xs">
                        <Radio value="1">Always punctual (Luôn đúng giờ)</Radio>
                        <Radio value="2">
                          Mostly punctual (Phần lớn đúng giờ)
                        </Radio>
                        <Radio value="3">
                          Rarely all punctual (Ít khi đúng giờ)
                        </Radio>
                        <Radio value="4">
                          Not at all punctual (Không bao giờ đúng giờ)
                        </Radio>
                      </Radio.Group>

                      <Spacer y={0.5} />
                      <Card.Divider />
                    </Form.Item>
                  </Fragment>
                </Card>
              ))}
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
