import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button, Spin, Divider, InputNumber, message  } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { ModulesApis, CenterApis, CourseApis } from '../../apis/ListApi';
import classes from './ModuleUpdate.module.css';
import { useParams, useNavigate } from 'react-router-dom';
import ModuleGradeType from '../ModuleGradeType/ModuleGradeType';
const TYPE_MODULE = {
  'Lý thuyết': 1,
  'Thực hành': 2,
  'Thực hành và Lý thuyết': 3,
};

const ModuleUpdate = ({ onUpdateSuccess }) => {
  const [isLoading, setisLoading] = useState(true);
  const [isUpdating, setIsUpdating] = useState(false);
  const [form] = Form.useForm();
  const [listCenters, setListCenters] = useState([]);
  const [isFailed, setIsFailed] = useState(false);
  const [typeExam, setTypeExam] = useState(4);

  const { id } = useParams();
  const navigate = useNavigate();

  const handleSubmitForm = () => {
    setIsUpdating(true);
    const data = form.getFieldsValue();

    const body = {
      center_id: data.center_id,
      semester_name_portal: data.portal_semester_name,
      module_type: data.module_type,
      module_name: data.module_name,
      module_exam_name_portal: data.portal_exam_name,
      max_theory_grade: data.max_theory_grade,
      max_practical_grade: data.max_practical_grade,
      hours: data.hours,
      days: data.days,
      exam_type: data.exam_type,
    };

    console.log(123);

    FetchApi(ModulesApis.updateModule, body, null, [`${id}`])
      .then((res) => {
        message.success('Cập nhật thành công');
        setIsUpdating(false);
      })
      .catch((err) => {
        setIsUpdating(false);
        setIsFailed(true);
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

  const getModulebyid = () => {
    setisLoading(true);
    FetchApi(ModulesApis.getModuleByID, null, null, [`${id}`])
      .then((res) => {
        const data = res.data;
        form.setFieldsValue({
          center_id: data.center.id,
          portal_semester_name: data.semester_name_portal,
          module_type: data.module_type,
          module_name: data.module_name,
          portal_exam_name: data.module_exam_name_portal,
          max_theory_grade: data.max_theory_grade,
          max_practical_grade: data.max_practical_grade,
          hours: data.hours,
          days: data.days,
          exam_type: data.exam_type,
        });
        setTypeExam(data.exam_type);
        setisLoading(false);
      })
      .catch(() => {
        navigate('/404');
      });
  };

  useEffect(() => {
    getModulebyid();
    getListCenter();
  }, []);

  return (
    <Grid.Container justify="center" gap={2}>
      <Grid xs={6}>
        <Card
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={15}
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              Thông tin tổng quan môn học
            </Text>
          </Card.Header>
          <Card.Body>
            {isLoading && (
              <div className={classes.loading}>
                <Spin />
              </div>
            )}
            {!isLoading && (
              <Form
                labelCol={{
                  span: 7,
                }}
                wrapperCol={{
                  span: 16,
                }}
                layout="horizontal"
                labelAlign="right"
                onFinish={handleSubmitForm}
                form={form}
                initialValues={{
                  exam_type: typeExam,
                }}
              >
                <div className={classes.layout}>
                  <Form.Item
                    name={'module_name'}
                    label="Môn học"
                    rules={[
                      {
                        required: true,
                        validator: (_, value) => {
                          if (value === null || value === undefined) {
                            return Promise.reject(
                              'Trường này không được để trống'
                            );
                          }
                          if (value.trim().length >= 2) {
                            return Promise.resolve();
                          }
                          return Promise.reject(
                            new Error('Trường phải có ít nhất 2 ký tự')
                          );
                        },
                      },
                      {
                        whitespace: true,
                        message: 'Trường không được chứa khoảng trắng',
                      },
                    ]}
                  >
                    <Input placeholder="Nhập tên môn học" />
                  </Form.Item>
                  <Form.Item
                    name={'center_id'}
                    label="Cơ sở"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng chọn cơ sở',
                      },
                    ]}
                  >
                    <Select
                      loading={listCenters.length === 0}
                      placeholder="Chọn cơ sở"
                    >
                      {listCenters.map((e) => {
                        return (
                          <Select.Option value={e.key}>{e.name}</Select.Option>
                        );
                      })}
                    </Select>
                  </Form.Item>
                </div>
                <Divider orientation="left">
                  <Text
                    p
                    b
                    size={15}
                    css={{
                      width: '100%',
                      textAlign: 'center',
                    }}
                  >
                    Thông tin thời gian
                  </Text>
                </Divider>
                <div className={classes.layout}>
                  <Form.Item
                    name={'hours'}
                    label="Số giờ học"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng nhập số giờ học',
                      },
                    ]}
                  >
                    <InputNumber
                      placeholder="0"
                      min={1}
                      max={200}
                    ></InputNumber>
                  </Form.Item>
                  <div></div>
                  <Form.Item
                    name={'days'}
                    label="Số buổi học"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng nhập số buổi học',
                      },
                    ]}
                  >
                    <InputNumber placeholder="0" min={1} max={50}></InputNumber>
                  </Form.Item>
                </div>
                <Divider orientation="left">
                  <Text
                    p
                    b
                    size={15}
                    css={{
                      width: '100%',
                      textAlign: 'center',
                    }}
                  >
                    Thông tin điểm thi tối đa
                  </Text>
                </Divider>
                <div className={classes.layout}>
                  <Form.Item
                    name={'module_type'}
                    label="Hình thức học"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng chọn hình thức học',
                      },
                    ]}
                  >
                    <Select placeholder="Chọn hình thức học">
                      <Select.Option value={1}>Lý thuyết</Select.Option>
                      <Select.Option value={2}>Thực hành</Select.Option>
                      <Select.Option value={3}>
                        Lý thuyết và Thực hành
                      </Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item
                    name={'exam_type'}
                    label="Hình thức thi"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng chọn hình thức thi',
                      },
                    ]}
                  >
                    <Select
                      placeholder="Chọn hình thức thi"
                      onChange={(value) => {
                        setTypeExam(value);
                      }}
                    >
                      <Select.Option value={1}>Lý thuyết</Select.Option>
                      <Select.Option value={2}>Thực hành</Select.Option>
                      <Select.Option value={3}>
                        Lý thuyết và Thực hành
                      </Select.Option>
                      <Select.Option value={4}>Không thi</Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item
                    name={'max_theory_grade'}
                    label="Lý thuyết"
                    rules={[
                      {
                        required: typeExam === 1 || typeExam === 3,
                        message: 'Vui lòng nhập điểm tối đa lý thuyết',
                      },
                    ]}
                  >
                    <InputNumber
                      disabled={typeExam === 2 || typeExam === 4}
                      placeholder="0"
                      min={1}
                      max={100}
                    ></InputNumber>
                  </Form.Item>
                  <Form.Item
                    name={'max_practical_grade'}
                    label="Thực hành"
                    rules={[
                      {
                        required: typeExam === 2 || typeExam === 3,
                        message: 'Vui lòng nhập điểm tối đa thực hành',
                      },
                    ]}
                  >
                    <InputNumber
                      disabled={typeExam === 1 || typeExam === 4}
                      placeholder="0"
                      min={1}
                      max={100}
                    ></InputNumber>
                  </Form.Item>
                </div>
                <Divider orientation="left">
                  <Text
                    p
                    b
                    size={15}
                    css={{
                      width: '100%',
                      textAlign: 'center',
                    }}
                  >
                    Thông tin liên quan đến Aptech Ấn độ
                  </Text>
                </Divider>
                <div className={classes.layout}>
                  <Form.Item
                    name={'portal_semester_name'}
                    label="Semester"
                    rules={[
                      {
                        required: true,
                        validator: (_, value) => {
                          if (value === null || value === undefined) {
                            return Promise.reject(
                              'Trường này không được để trống'
                            );
                          }
                          if (value.trim().length >= 2) {
                            return Promise.resolve();
                          }
                          return Promise.reject(
                            new Error('Trường phải có ít nhất 2 ký tự')
                          );
                        },
                      },
                      {
                        whitespace: true,
                        message: 'Trường không được chứa khoảng trắng',
                      },
                    ]}
                  >
                    <Input placeholder="Nhập tên semester"></Input>
                  </Form.Item>
                  <Form.Item
                    name={'portal_exam_name'}
                    label="Exam name"
                    rules={[
                      {
                        required: true,
                        validator: (_, value) => {
                          if (value === null || value === undefined) {
                            return Promise.reject(
                              'Trường này không được để trống'
                            );
                          }
                          if (value.trim().length >= 2) {
                            return Promise.resolve();
                          }
                          return Promise.reject(
                            new Error('Trường phải có ít nhất 2 ký tự')
                          );
                        },
                      },
                      {
                        whitespace: true,
                        message: 'Trường không được chứa khoảng trắng',
                      },
                    ]}
                  >
                    <Input placeholder="Nhập tên exam"></Input>
                  </Form.Item>
                </div>
                <div
                  style={{
                    display: 'flex',
                    justifyContent: 'center',
                  }}
                >
                  <Form.Item>
                    <Button type="primary" htmlType="submit" loading={isUpdating}>
                      Cập nhật
                    </Button>
                  </Form.Item>
                </div>
              </Form>
            )}
            {!isUpdating && isFailed && (
              <Text
                size={14}
                css={{
                  color: 'red',
                  textAlign: 'center',
                }}
              >
                Cập nhật môn học thất bại, vui lòng thử lại
              </Text>
            )}
          </Card.Body>
        </Card>
      </Grid>
      <Grid xs={4}>
        <Card
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={15}
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              Thông tin điểm thành phần
            </Text>
          </Card.Header>
          <Card.Body>
            <ModuleGradeType />
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};
export default ModuleUpdate;
