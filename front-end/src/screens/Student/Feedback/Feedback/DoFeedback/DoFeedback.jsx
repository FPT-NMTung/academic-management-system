import classes from "../../../ScheduleScreen/ScheduleScreen.module.css";
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

  const [listQuestion, setListQuestion] = useState([]);
  const [listAnswer, setListAnswer] = useState([]);
  const navigate = useNavigate();
  const [isLoading, setisLoading] = useState(false);
  const { id } = useParams();
  const GetQuestions = () => {
    FetchApi(ManageGpa.getQuestionByFormID, null, null, [String(id)])
      .then((res) => {
        setListQuestion(res.data);
        console.log(res.data);
      })
      .catch((err) => {
        navigate("/404");
      });
  };
  useEffect(() => {
    GetQuestions();
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
          // labelCol={{ xs: {
          //   span: 24,
          // },
          // sm: {
          //   span: 8,
          // }, }}

          // labelCol={{
          //   span: 1,
          // }}
          // wrapperCol={{
          //   span: 24,
          // }}
          layout="Vertical"
          labelAlign="center"

          // onFinish={handleSubmitForm}
          form={form}
        >
          <Grid.Container gap={2} justify="center">
            <Grid sm={12} direction={"column"}>
              <Card variant="bordered">
                <Card.Header>
                  <Text
                    b
                    size={16}
                    p
                    css={{
                      width: "100%",
                      textAlign: "center",
                    }}
                  >
                    Ý kiến về việc giảng dậy
                  </Text>
                </Card.Header>
                <Card.Body>
                  {listQuestion.map((item, index) => (
                    <Card variant="bordered">
                          <Form.Item
                      label={item.content}
                      name="class_name"
                      style={{
                        width: "100%",
                        display: "block",
                      }}
                    >
                      <Radio.Group
                        css={{ display: "block" }}
                        label=""
                        defaultValue="0"
                        size="sm"
                      >
                        <Radio value="1">Option 1</Radio>
                        <Radio value="2">Option 2</Radio>
                        <Radio value="3">Option 3</Radio>
                        <Radio value="4">Option 4</Radio>
                      </Radio.Group>
                    </Form.Item>
                    </Card>
                  
                  ))}

                  {/* <Form.Item
                  style={{
                    marginBottom: 0,
                  }}
                  rules={[
                    {
                      required: true,
                    },
                  ]}
                >
                  <Form.Item
                    label="Ngày Nhập Học"
                    name="start_date"
                    rules={[
                      {
                        message: 'Vui lòng chọn ngày nhập học',
                        required: true,
                      },
                    ]}
                    style={{
                      display: 'inline-block',
                      width: 'calc(50% - 8px)',
                    }}
                  >
                   
                  </Form.Item>

                  <Form.Item
                    label="Chương trình học"
                    name="course_family_code"
                    rules={[
                      {
                        message: 'Vui lòng chọn chương trình học',
                        required: true,
                      },
                    ]}
                    style={{
                      display: 'inline-block',
                      width: 'calc(50% - 8px)',
                      margin: '0 8px',
                    }}
                  >
                   
                  </Form.Item>
                </Form.Item>
                <Form.Item
                  style={{
                    marginBottom: 0,
                  }}
                  rules={[
                    {
                      required: true,
                    },
                  ]}
                >
                  <Form.Item
                    label="Ngày Hoàn Thành"
                    name="completion_date"
                    rules={[
                      {
                        message: 'Vui lòng chọn ngày hoàn thành',
                        required: true,
                      },
                    ]}
                    style={{
                      display: 'inline-block',
                      width: 'calc(50% - 8px)',
                    }}
                  >
                   
                  </Form.Item>
                  <Form.Item
                    label="Ngày Tốt Nghiệp"
                    name="graduation_date"
                    rules={[
                      {
                        message: 'Vui lòng chọn ngày tốt nghiệp',
                        required: true,
                      },
                    ]}
                    style={{
                      display: 'inline-block',
                      width: 'calc(50% - 8px)',
                      margin: '0 8px',
                    }}
                  >
                   
                  </Form.Item>
                </Form.Item>
                <Form.Item
                  style={{
                    marginBottom: 0,
                  }}
                  rules={[
                    {
                      required: true,
                    },
                  ]}
                >
                  <Form.Item
                    label="Trạng thái lớp"
                    name="class_status_id"
                   
                    style={{
                      display: 'inline-block',
                      width: 'calc(50% - 8px)',
                    }}
                  >
                    <Select
                      showSearch
                    
                      placeholder="Trạng thái lớp học"
                      style={{ width: '100%' }}
                      dropdownStyle={{ zIndex: 9999 }}
                     
                    >
                      <Select.Option key="100" value={1}>
                        Đã lên lịch
                      </Select.Option>
                      <Select.Option key="101" value={2}>
                        Đang học
                      </Select.Option>
                      <Select.Option key="102" value={3}>
                        Đã hoàn thành
                      </Select.Option>
                      <Select.Option key="103" value={4}>
                        Huỷ
                      </Select.Option>
                      <Select.Option key="104" value={5}>
                        Chưa lên lịch
                      </Select.Option>
                      <Select.Option key="105" value={6}>
                        Đã ghép
                      </Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item
                    label="Ngày Học"
                    name="class_days_id"
                   
                    style={{
                      display: 'inline-block',
                      width: 'calc(50% - 8px)',
                      margin: '0 8px',
                    }}
                  >
                    <Select
                      showSearch
                      placeholder="Ngày học trong tuần"
                      style={{ width: '100%' }}
                      dropdownStyle={{ zIndex: 9999 }}
                     
                    >
                      
                    </Select>
                  </Form.Item>
                </Form.Item>

                <Form.Item
                  style={{}}
                  rules={[
                    {
                      required: true,
                    },
                  ]}
                >
                  <Form.Item
                    style={{
                      display: 'inline-block',
                      textAlign: 'center',
                      width: '100%',
                    }}
                  >
               
                  </Form.Item>
                  <Form.Item
                    style={{
                      float: 'right',
                      display: 'inline-block',
                      // width: "calc(50% - 8px)",
                      margin: '0 8px -25px 0',
                    }}
                  >
                    <Button
                      flat
                      auto
                      type="primary"
                      htmlType="submit"
                    //   loading={isCreatingOrUpdating}
                    >
                      Gửi
                    </Button>
                  </Form.Item>
                </Form.Item> */}
                </Card.Body>
              </Card>
            </Grid>
          </Grid.Container>
        </Form>
      )}
    </Fragment>
  );
};

export default DoFeedback;
