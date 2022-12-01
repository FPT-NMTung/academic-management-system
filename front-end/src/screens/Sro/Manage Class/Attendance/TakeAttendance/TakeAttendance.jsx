import { Button, Modal, Spacer, Switch as SwitchNextUI, Text } from '@nextui-org/react';
import { Form, Input, Table, Switch } from 'antd';
import { useEffect } from 'react';
import { useState } from 'react';
import toast from 'react-hot-toast';
import { Link } from 'react-router-dom';
import FetchApi from '../../../../../apis/FetchApi';
import { ManageAttendanceApis } from '../../../../../apis/ListApi';
import { Validater } from '../../../../../validater/Validater';

import classes from './TakeAttendance.module.css';

const TakeAttendance = ({ session, scheduleId, onClose }) => {
  const [isLoading, setIsLoading] = useState(true);
  const [listAttendance, setListAttendance] = useState([]);
  const [listStatus, setListStatus] = useState([]);

  const [form] = Form.useForm();

  const getListAttendance = () => {
    setIsLoading(true);
    FetchApi(ManageAttendanceApis.getAttendanceBySlotId, null, null, [
      String(scheduleId),
      String(session.id),
    ])
      .then((res) => {
        const data = res.data[0].attendances.map((item, index) => {
          return {
            image: item.student.avatar,
            information: {
              name: item.student.first_name + ' ' + item.student.last_name,
              user_id: item.student.user_id,
              enroll_number: item.student.enroll_number,
              email_organization: item.student.email_organization,
            },
            status: item.attendance_status,
            note: item.note,
            key: item.student.user_id,
          };
        });

        setListAttendance(data);

        const dataForm = {};
        const dataStatus = [];
        data.forEach((item) => {
          dataStatus.push({
            user_id: item.information.user_id,
            status:
              item.status === null ? false : item.status.id == 3 ? true : false,
          });
          dataForm[`note-${item.key}`] = item.note;
        });

        setListStatus(dataStatus);
        setIsLoading(false);

        form.setFieldsValue(dataForm);
      })
      .catch((err) => {
        toast.error('Lỗi lấy thoong tin điểm danh');
      });
  };

  useEffect(() => {
    getListAttendance();
  }, []);

  const handleChangeStatus = (record, status) => {
    const index = listStatus.findIndex(
      (item) => item.user_id === record.information.user_id
    );
    const data = [...listStatus];
    data[index].status = !status;
    setListStatus(data);
  };

  const handleChangeAllStatus = (status) => {
    const data = [...listStatus];
    data.forEach((item) => {
      item.status = status;
    });
    setListStatus(data);
  };

  const onSubmitForm = (values) => {
    // convert object to array
    const data = Object.keys(values).map((key) => {
      return {
        user_id: Number.parseInt(key.split('-')[1]),
        note: values[key],
      };
    });

    const dataSubmit = listStatus.map((item) => {
      const find = data.find((item2) => item2.user_id === item.user_id).note;
      return {
        student_id: item.user_id,
        attendance_status_id: item.status ? 3 : 2,
        note: find === '' ? null : find,
      };
    });    

    var api = localStorage.getItem('role') === 'teacher' ? ManageAttendanceApis.submitAttendanceTeacher : ManageAttendanceApis.submitAttendanceSro;

    toast.promise(
      FetchApi(api, dataSubmit, null, [
        String(scheduleId),
        String(session.id),
      ]),
      {
        loading: 'Đang lưu ... ',
        success: (res) => {
          return 'Lưu thành công';
        },
        error: (err) => {
          return 'Lưu thất bại';
        },
      }
    );
  };

  return (
    <Modal
      blur
      open={session !== undefined}
      closeButton={true}
      onClose={onClose}
      width={1000}
    >
      <Modal.Header>
        <Text p b size={14}>
          Điểm danh
        </Text>
      </Modal.Header>
      <Modal.Body>
        <div className={classes.buttonChangeAttendance}>
          <Button
            flat
            auto
            size={'sm'}
            color={'success'}
            onPress={() => handleChangeAllStatus(true)}
          >
            Tất cả có mặt
          </Button>
          <Button
            flat
            auto
            size={'sm'}
            color={'error'}
            onPress={() => handleChangeAllStatus(false)}
          >
            Tất cả vắng mặt
          </Button>
        </div>
        <Form form={form} onFinish={onSubmitForm}>
          {/* {isLoading && ( */}
          <Table
            loading={isLoading}
            columns={[
              {
                title: 'Hình ảnh',
                dataIndex: 'image',
                key: 'image',
                render: (image) => (
                  <img className={classes.imageStudent} src={image} />
                ),
                width: 200,
              },
              {
                title: 'Thông tin',
                dataIndex: 'information',
                key: 'information',
                render: (information) => (
                  <div className={classes.information}>
                    <Link
                      target={'_blank'}
                      to={`/sro/manage/student/${information.user_id}`}
                    >
                      <Text p b size={14}>
                        {information.name}
                      </Text>
                    </Link>
                    <Text p size={14}>
                      {information.email_organization}
                    </Text>
                    <Text p size={14}>
                      {information.enroll_number}
                    </Text>
                  </div>
                ),
              },
              {
                title: 'Trạng thái',
                dataIndex: 'status',
                width: 100,
                key: 'status',
                render: (status, record) => {
                  const temp = listStatus.find(
                    (item) => item.user_id === record.information.user_id
                  );
                  const check = temp !== undefined ? temp.status : false;
                  return (
                    // <Switch
                    //   onChange={() => handleChangeStatus(record, check)}
                    //   checked={check}
                    // />
                    <SwitchNextUI
                      onChange={() => handleChangeStatus(record, check)}
                      checked={check}
                      color={'success'}
                    />
                  );
                },
              },
              {
                title: 'Ghi chú',
                dataIndex: 'note',
                key: 'note',
                render: (note, record) => (
                  <Form.Item
                    name={`note-${record.information.user_id}`}
                    rules={[
                      {
                        required: false,
                        validator: (_, value) => {
                          if (
                            value === null ||
                            value === undefined ||
                            value.trim() === ''
                          ) {
                            return Promise.resolve();
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
                      {
                        whitespace: true,
                        message: 'Trường không được chứa khoảng trắng',
                      },
                    ]}
                  >
                    <Input />
                  </Form.Item>
                ),
              },
            ]}
            dataSource={listAttendance}
            pagination={false}
            scroll={{ y: 520 }}
          />
          {/* )} */}
          <Form.Item>
            <Spacer y={1} />
            <Button auto color={'success'} type="primary" htmlType="submit">
              Lưu
            </Button>
          </Form.Item>
        </Form>
      </Modal.Body>
    </Modal>
  );
};

export default TakeAttendance;
