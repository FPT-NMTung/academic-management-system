import { Card, Grid, Button, Text } from '@nextui-org/react';
import {
  Form,
  Select,
  Input,
  Spin,
  Table,
  Tooltip,
  Space,
  Typography,
  message,
  DatePicker,
  TimePicker,
} from 'antd';
import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import FetchApi from '../../../../apis/FetchApi';
import { CourseFamilyApis, ManageClassApis } from '../../../../apis/ListApi';
import classes from '../../../../components/ModuleCreate/ModuleCreate.module.css';
import { Fragment } from 'react';
import { MdEdit } from 'react-icons/md';
import ColumnGroup from 'antd/lib/table/ColumnGroup';
import { ErrorCodeApi } from '../../../../apis/ErrorCodeApi';
import { Validater } from '../../../../validater/Validater';
import moment from 'moment';
import toast from 'react-hot-toast';

const ClassCreate = ({ modeUpdate }) => {
  const [isCreatingOrUpdating, setisCreatingOrUpdating] = useState(false);
  // const [isFailed, setIsFailed] = useState(false);
  const [isLoading, setisLoading] = useState(true);
  const [listCourseFamily, setlistCourseFamily] = useState([]);
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const { id } = useParams();
  const [messageFailed, setMessageFailed] = useState(undefined);

  const getListCourseFamily = () => {
    setisLoading(true);
    const apiCourseFamily = CourseFamilyApis.getAllCourseFamily;
    FetchApi(apiCourseFamily).then((res) => {
      const data = res.data;

      data.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));

      const mergeAllCourseFamily = data.map((e, index) => {
        return {
          key: index,
          course_family_code: e.code,
        };
      });
      setisLoading(false);
      setlistCourseFamily(mergeAllCourseFamily);
    });
  };
  const handleSubmitForm = () => {
    const data = form.getFieldsValue();
    setisCreatingOrUpdating(true);

    const body = {
      class_days_id: data.class_days_id,
      class_status_id: data.class_status_id,
      name: data.class_name.trim(),
      course_family_code: data.course_family_code,
      start_date: data.start_date.add(7, 'hours').toDate(),
      completion_date: data.completion_date.add(7, 'hours').toDate(),
      graduation_date: data.graduation_date.add(7, 'hours').toDate(),
      class_hour_start: data.class_hour[0].format('HH:mm:ss'),
      class_hour_end: data.class_hour[1].format('HH:mm:ss'),
    };
    const api = modeUpdate
      ? ManageClassApis.updateClass
      : ManageClassApis.createClass;
    const params = modeUpdate ? [`${id}`] : null;
    toast.promise(
      FetchApi(api, body, null, params),
      {
        loading: '...',
        success: (res) => {
          setisCreatingOrUpdating(false);
          navigate(`/sro/manage-class`);
          return 'Thành công !';
        },

        error: (err) => {
          setisCreatingOrUpdating(false);
          setMessageFailed(ErrorCodeApi[err.type_error]);
          // if (err?.type_error) {
          //   return ErrorCodeApi[err.type_error];
          // }
          return 'Thất bại !';
        },
      }
      // .then((res) => {
      //   if (modeUpdate) {
      //     message.success("Cập nhật lớp học thành công");
      //   } else {
      //     message.success("Tạo lớp học thành công");
      //   }

      //   navigate(`/sro/manage-class/`);
      // })
      // .catch((err) => {
      //   setisCreatingOrUpdating(false);
      //   setMessageFailed(ErrorCodeApi[err.type_error]);
      // });
    );
  };
  const handleChangeClassStatus = () => {
    if (!modeUpdate) {
      form.setFieldsValue({
        class_status_id: 5,
      });
    }
  };
  const getInformationClass = () => {
    setisLoading(true);

    FetchApi(ManageClassApis.getInformationClass, null, null, [`${id}`]).then(
      (res) => {
        const data = res.data;

        form.setFieldsValue({
          class_name: data.name,
          class_hour: [
            moment(data.class_hour_start, 'HH:mm:ss'),
            moment(data.class_hour_end, 'HH:mm:ss'),
          ],
          start_date: moment(data.start_date),
          course_family_code: data.course_family_code,
          completion_date: moment(data.completion_date),
          graduation_date: moment(data.graduation_date),
          class_status_id: data.class_status.id,
          class_days_id: data.class_days.id,
        });

        setisLoading(false);
      }
    );
  };
  useEffect(() => {
    getListCourseFamily();
    handleChangeClassStatus();
    if (modeUpdate) {
      getInformationClass();
    }
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
          wrapperCol={{
            span: 24,
          }}
          layout="horizontal"
          labelAlign="right"
          labelWrap
          initialValues={{}}
          onFinish={handleSubmitForm}
          form={form}
        >
          <Grid.Container gap={1} justify="center">
            <Grid xs={6} direction={'column'}>
              <Card
                style={{ padding: '20px 10px 20px 10px', marginTop: '100px' }}
              >
                <Card.Header>
                  <Text
                    b
                    size={16}
                    p
                    css={{
                      width: '100%',
                      textAlign: 'center',
                    }}
                  >
                    {!modeUpdate && 'Tạo Lớp Học'}
                    {modeUpdate && 'Cập Nhật Thông Tin Lớp Học'}
                  </Text>
                </Card.Header>
                <Card.Body>
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
                      label="Tên"
                      name="class_name"
                      rules={[
                        {
                          required: true,
                          validator: (_, value) => {
                            if (value === null || value === undefined) {
                              return Promise.reject(
                                'Trường này không được để trống'
                              );
                            }
                            if (
                              Validater.isContaintSpecialCharacterForName(
                                value.trim()
                              )
                            ) {
                              return Promise.reject(
                                'Trường này không được chứa ký tự đặc biệt'
                              );
                            }
                            if (
                              value.trim().length < 1 ||
                              value.trim().length > 255
                            ) {
                              return Promise.reject(
                                new Error('Trường phải từ 1 đến 255 ký tự')
                              );
                            }
                            return Promise.resolve();
                          },
                        },
                      ]}
                      style={{
                        display: 'inline-block',
                        width: 'calc(50% - 8px)',
                      }}
                    >
                      <Input placeholder="Nhập tên lớp học" />
                    </Form.Item>

                    <Form.Item
                      label="Giờ học"
                      name="class_hour"
                      rules={[
                        {
                          message: 'Vui lòng chọn giờ học',
                          required: true,
                        },
                      ]}
                      style={{
                        display: 'inline-block',
                        width: 'calc(50% - 8px)',
                        margin: '0 8px',
                      }}
                    >
                      <TimePicker.RangePicker format={'HH:mm'} />
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
                      <DatePicker
                        format={'DD/MM/YYYY'}
                        style={{
                          width: 'calc(100%)',
                        }}
                        placeholder="Ngày nhập học"
                      />
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
                      <Select
                        showSearch
                        style={{ width: '100%' }}
                        dropdownStyle={{ zIndex: 9999 }}
                        placeholder="Chọn chương trình học"
                        filterOption={(input, option) =>
                          option.children
                            .toLowerCase()
                            .includes(input.toLowerCase())
                        }
                      >
                        {listCourseFamily.map((e) => (
                          <Select.Option
                            key={e.key}
                            value={e.course_family_code}
                          >
                            {e.course_family_code}
                          </Select.Option>
                        ))}
                      </Select>
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
                      <DatePicker
                        format={'DD/MM/YYYY'}
                        style={{
                          width: 'calc(100%)',
                        }}
                        placeholder="Ngày hoàn thành dự kiến"
                      />
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
                      <DatePicker
                        format={'DD/MM/YYYY'}
                        style={{
                          width: 'calc(100%)',
                        }}
                        placeholder="Ngày tốt nghiệp dự kiến"
                      />
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
                      rules={[
                        {
                          message: 'Vui lòng chọn trạng thái lớp',
                          required: modeUpdate,
                        },
                      ]}
                      style={{
                        display: 'inline-block',
                        width: 'calc(50% - 8px)',
                      }}
                    >
                      <Select
                        showSearch
                        disabled={!modeUpdate}
                        placeholder="Trạng thái lớp học"
                        style={{ width: '100%' }}
                        dropdownStyle={{ zIndex: 9999 }}
                        filterOption={(input, option) =>
                          option.children
                            .toLowerCase()
                            .includes(input.toLowerCase())
                        }
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
                        <Select.Option key="103" value={5}>
                          Chưa lên lịch
                        </Select.Option>
                        <Select.Option key="104" value={4}>
                          Huỷ
                        </Select.Option>
                      </Select>
                    </Form.Item>
                    <Form.Item
                      label="Ngày Học"
                      name="class_days_id"
                      rules={[
                        {
                          message: 'Vui lòng chọn các ngày học trong tuần',
                          required: true,
                        },
                      ]}
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
                        filterOption={(input, option) =>
                          option.children
                            .toLowerCase()
                            .includes(input.toLowerCase())
                        }
                      >
                        <Select.Option key="105" value={1}>
                          2-4-6
                        </Select.Option>
                        <Select.Option key="106" value={2}>
                          3-5-7
                        </Select.Option>
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
                      {!isCreatingOrUpdating && messageFailed !== undefined && (
                        <Text
                          size={14}
                          css={{
                            background: '#fff',
                            color: 'red',
                          }}
                        >
                          {messageFailed}
                        </Text>
                      )}
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
                        loading={isCreatingOrUpdating}
                      >
                        {!modeUpdate && 'Tạo mới'}
                        {modeUpdate && 'Cập nhật '}
                      </Button>
                    </Form.Item>
                  </Form.Item>
                </Card.Body>
              </Card>
            </Grid>
          </Grid.Container>
        </Form>
      )}
    </Fragment>
  );
};
export default ClassCreate;
