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
      start_date: moment.utc(data.start_date).local().format(),
      completion_date: moment.utc(data.completion_date).local().format(),
      graduation_date: moment.utc(data.graduation_date).local().format(),
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
          return 'Th??nh c??ng !';
        },

        error: (err) => {
          setisCreatingOrUpdating(false);
          setMessageFailed(ErrorCodeApi[err.type_error]);
          if (err?.type_error) {
            return ErrorCodeApi[err.type_error];
          }
          return 'Th???t b???i !';
        },
      }

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
                    {!modeUpdate && 'T???o l???p h???c'}
                    {modeUpdate && 'C???p nh???t th??ng tin l???p h???c'}
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
                      label="T??n"
                      name="class_name"
                      rules={[
                        {
                          required: true,
                          validator: (_, value) => {
                            if (value === null || value === undefined) {
                              return Promise.reject(
                                'Tr?????ng ph???i t??? 1 ?????n 255 k?? t???'
                              );
                            }
                            if (
                              Validater.isContaintSpecialCharacterForName(
                                value.trim()
                              )
                            ) {
                              return Promise.reject(
                                'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                              );
                            }
                            if (
                              value.trim().length < 1 ||
                              value.trim().length > 255
                            ) {
                              return Promise.reject(
                                new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
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
                      <Input placeholder="Nh???p t??n l???p h???c" />
                    </Form.Item>

                    <Form.Item
                      label="Gi??? h???c"
                      name="class_hour"
                      rules={[
                        {
                          message: 'Vui l??ng ch???n gi??? h???c',
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
                      label="Ng??y nh???p h???c"
                      name="start_date"
                      rules={[
                        {
                          message: 'Vui l??ng ch???n ng??y nh???p h???c',
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
                        placeholder="Ng??y nh???p h???c"
                      />
                    </Form.Item>

                    <Form.Item
                      label="Ch????ng tr??nh h???c"
                      name="course_family_code"
                      rules={[
                        {
                          message: 'Vui l??ng ch???n ch????ng tr??nh h???c',
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
                        placeholder="Ch???n ch????ng tr??nh h???c"
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
                      label="Ng??y ho??n th??nh"
                      name="completion_date"
                      rules={[
                        {
                          message: 'Vui l??ng ch???n ng??y ho??n th??nh',
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
                        placeholder="Ng??y ho??n th??nh d??? ki???n"
                      />
                    </Form.Item>
                    <Form.Item
                      label="Ng??y t???t nghi???p"
                      name="graduation_date"
                      rules={[
                        {
                          message: 'Vui l??ng ch???n ng??y t???t nghi???p',
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
                        placeholder="Ng??y t???t nghi???p d??? ki???n"
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
                      label="Tr???ng th??i l???p"
                      name="class_status_id"
                      rules={[
                        {
                          message: 'Vui l??ng ch???n tr???ng th??i l???p',
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
                        placeholder="Tr???ng th??i l???p h???c"
                        style={{ width: '100%' }}
                        dropdownStyle={{ zIndex: 9999 }}
                        filterOption={(input, option) =>
                          option.children
                            .toLowerCase()
                            .includes(input.toLowerCase())
                        }
                      >
                        <Select.Option key="100" value={1}>
                          ???? l??n l???ch
                        </Select.Option>
                        <Select.Option key="101" value={2}>
                          ??ang h???c
                        </Select.Option>
                        <Select.Option key="102" value={3}>
                          ???? ho??n th??nh
                        </Select.Option>
                        <Select.Option key="103" value={4}>
                          Hu???
                        </Select.Option>
                        <Select.Option key="104" value={5}>
                          Ch??a l??n l???ch
                        </Select.Option>
                        <Select.Option key="105" value={6}>
                          ???? gh??p
                        </Select.Option>
                      </Select>
                    </Form.Item>
                    <Form.Item
                      label="Ng??y h???c"
                      name="class_days_id"
                      rules={[
                        {
                          message: 'Vui l??ng ch???n c??c ng??y h???c trong tu???n',
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
                        placeholder="Ng??y h???c trong tu???n"
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
                      {/* {!isCreatingOrUpdating && messageFailed !== undefined && (
                        <Text
                          size={14}
                          css={{
                            background: '#fff',
                            color: 'red',
                          }}
                        >
                          {messageFailed}
                        </Text>
                      )} */}
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
                        {!modeUpdate && 'T???o m???i'}
                        {modeUpdate && 'C???p nh???t '}
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
