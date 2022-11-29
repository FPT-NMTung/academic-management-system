import { Button, Checkbox, Divider, Spacer } from '@nextui-org/react';
import {
  DatePicker,
  Descriptions,
  Form,
  Input,
  InputNumber,
  Select,
  TimePicker,
} from 'antd';
import { useState } from 'react';
import { useEffect } from 'react';
import { Fragment } from 'react';
import toast from 'react-hot-toast';
import { useOutletContext, useParams } from 'react-router-dom';
import FetchApi from '../../apis/FetchApi';
import {
  ManageClassApis,
  ManageScheduleApis,
  ManageTeacherApis,
  ModulesApis,
  RoomApis,
} from '../../apis/ListApi';
import moment from 'moment';
import { ErrorCodeApi } from '../../apis/ErrorCodeApi';
import classes from './ScheduleUpdate.module.css';

const ScheduleUpdate = ({ dataModule, dataSchedule, onSuccess }) => {
  const [listTeacher, setListTeacher] = useState([]);
  const [listRoom, setListRoom] = useState([]);
  const [dataModuleDetail, setDataModuleDetail] = useState(undefined);
  const [duration, setDuration] = useState(0);
  const [listSessionPractice, setListSessionPractice] = useState([]);

  const { id, moduleId } = useParams();
  const [form] = Form.useForm();
  const [handleUpdateListCourse] = useOutletContext();

  const getListTeacher = () => {
    FetchApi(ManageTeacherApis.getListTeacherBySro, null, null, null)
      .then((res) => {
        setListTeacher(res.data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy danh sách giáo viên');
      });
  };

  const getListRoom = () => {
    FetchApi(RoomApis.getRoomsBySro, null, null, null)
      .then((res) => {
        setListRoom(res.data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy danh sách phòng học');
      });
  };

  const getInformationModule = () => {
    FetchApi(ModulesApis.getModuleByID, null, null, [String(moduleId)])
      .then((res) => {
        console.log(res.data);
        setDataModuleDetail(res.data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy thông tin môn học');
      });
  };

  const fillDataToForm = () => {
    console.log(form);
    console.log(dataSchedule);

    form.setFieldsValue({
      teacher_id: dataSchedule.teacher.id,
      class_days_id: dataSchedule.class_days.id,
      start_date: moment(dataSchedule.start_date, 'YYYY-MM-DD'),
      working_time_id: dataSchedule.working_time_id,
      time: [
        moment(dataSchedule.class_hour_start, 'HH:mm:ss'),
        moment(dataSchedule.class_hour_end, 'HH:mm:ss'),
      ],
      theory_room_id: dataSchedule.theory_room_id,
      lab_room_id: dataSchedule.lab_room_id,
      exam_room_id: dataSchedule.exam_room_id,
      duration: dataSchedule.duration,
      note: dataSchedule.note,
    });

    setDuration(dataSchedule.duration);
    const sessionPractice = dataSchedule.sessions.sort((session1, session2) => {
      return new Date(session1.learning_date) - new Date(session2.learning_date);
    })
    .map((session, index) => {
      return session.session_type === 2 ? index + 1 : null;
    })
    .filter((session) => session !== null);
    setListSessionPractice(sessionPractice);
  };

  useEffect(() => {
    getListTeacher();
    getListRoom();
    getInformationModule();
    fillDataToForm();
  }, []);

  const handleChangeSession = (status, value) => {
    if (status) {
      setListSessionPractice([...listSessionPractice, value]);
    } else {
      setListSessionPractice(
        listSessionPractice.filter((item) => item !== value)
      );
    }
  };

  const renderSession = () => {
    const array = [];
    for (let index = 0; index < duration; index++) {
      array.push(index + 1);
    }
    console.log(array);
    return array;
  };

  const handleSubmitForm = (e) => {
    const body = {
      module_id: Number.parseInt(moduleId),
      teacher_id: e.teacher_id,
      class_days_id: e.class_days_id,
      working_time_id: e.working_time_id,
      theory_room_id: e.theory_room_id,
      lab_room_id: e.lab_room_id,
      exam_room_id: e.exam_room_id,
      duration: e.duration,
      practice_session: [...listSessionPractice],
      start_date: moment.utc(e.start_date).local().format(),
      class_hour_start: moment(e.time[0]).format('HH:mm:ss'),
      class_hour_end: moment(e.time[1]).format('HH:mm:ss'),
      note: e.note,
    };

    toast.promise(
      FetchApi(ManageScheduleApis.updateSchedule, body, null, [String(dataSchedule.id)]),
      {
        loading: 'Đang cập nhật lịch học',
        success: (res) => {
          onSuccess();
          handleUpdateListCourse();
          return 'Cập nhật lịch học thành công';
        },
        error: (err) => {
          if (err?.type_error) {
            return ErrorCodeApi[err.type_error];
          }
          return 'Cập nhật lịch học thất bại';
        },
      }
    );
  };

  return (
    <Fragment>
      <Descriptions>
        <Descriptions.Item span={3} label="Môn học">
          <b>{dataModule.module_name}</b>
        </Descriptions.Item>
      </Descriptions>
      <Divider />
      <Spacer y={1} />
      <Form
        onFinish={handleSubmitForm}
        form={form}
        labelCol={{ span: 8 }}
        wrapperCol={{ span: 16 }}
      >
        <div className={classes.formCreate}>
          <Form.Item
            rules={[{ required: true, message: 'Vui lòng chọn thời gian học' }]}
            name={'class_days_id'}
            label="Thời gian học"
          >
            <Select placeholder="Chọn thời gian học">
              <Select.Option value={1}>Thứ 2, 4, 6</Select.Option>
              <Select.Option value={2}>Thứ 3, 5, 7</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item
            rules={[{ required: true, message: 'Vui lòng chọn giáo viên' }]}
            name={'teacher_id'}
            label="Giáo viên"
          >
            <Select placeholder="Chọn giáo viên">
              {listTeacher.map((item) => (
                <Select.Option value={item.user_id}>
                  {`${item.first_name} ${item.last_name}`}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item
            rules={[{ required: true, message: 'Vui lòng chọn ngày' }]}
            name={'start_date'}
            label="Ngày bắt đầu"
          >
            <DatePicker placeholder="Chọn ngày bắt đầu" format="DD/MM/YYYY" />
          </Form.Item>
          <Form.Item
            rules={[{ required: true, message: 'Vui lòng chọn khung giờ' }]}
            name={'working_time_id'}
            label="Khung giờ dạy"
          >
            <Select placeholder="Chọn thời gian học trong tuần">
              <Select.Option value={1}>Sáng (8h00 - 12h00)</Select.Option>
              <Select.Option value={2}>Chiều (13h00 - 17h00)</Select.Option>
              <Select.Option value={3}>Tối (18h00 - 22h00)</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item
            rules={[{ required: true, message: 'Vui lòng chọn giờ học' }]}
            name={'time'}
            label="Giờ học"
          >
            <TimePicker.RangePicker format={'HH:mm'} />
          </Form.Item>
          <Form.Item
            rules={[{ required: true, message: 'Vui lòng chọn số lượng' }]}
            name={'duration'}
            label="Số lượng buổi học"
          >
            <InputNumber
              onChange={(value) => {
                setDuration(value);
              }}
              min={1}
              max={50}
              placeholder="Nhập số"
            />
          </Form.Item>
          {dataModuleDetail !== undefined && (
            <Fragment>
              {(dataModuleDetail.module_type === 1 ||
                dataModuleDetail.module_type === 3) && (
                <Fragment>
                  <Form.Item
                    rules={[
                      { required: true, message: 'Vui lòng chọn phòng học' },
                    ]}
                    name={'theory_room_id'}
                    label="Phòng học lý thuyết"
                  >
                    <Select placeholder="Chọn phòng học">
                      {listRoom
                        .filter(
                          (item) => item.is_active && item.room_type.id === 1
                        )
                        .map((item) => (
                          <Select.Option value={item.id}>
                            {item.name}
                          </Select.Option>
                        ))}
                    </Select>
                  </Form.Item>
                </Fragment>
              )}
              {(dataModuleDetail.module_type === 2 ||
                dataModuleDetail.module_type === 3) && (
                <Fragment>
                  <Form.Item
                    rules={[
                      { required: true, message: 'Vui lòng chọn phòng học' },
                    ]}
                    name={'lab_room_id'}
                    label="Phòng học thực hành"
                  >
                    <Select placeholder="Chọn phòng học">
                      {listRoom
                        .filter(
                          (item) => item.is_active && item.room_type.id === 2
                        )
                        .map((item) => (
                          <Select.Option value={item.id}>
                            {item.name}
                          </Select.Option>
                        ))}
                    </Select>
                  </Form.Item>
                </Fragment>
              )}
            </Fragment>
          )}
          <Form.Item className={classes.note} name={'note'} label="Ghi chú">
            <Input.TextArea rows={4} placeholder="Nhập ghi chú" />
          </Form.Item>
          {dataModuleDetail?.exam_type !== 4 && (
            <Form.Item
              rules={[{ required: true, message: 'Vui lòng chọn phòng thi' }]}
              name={'exam_room_id'}
              label="Phòng thi"
            >
              <Select placeholder="Chọn phòng thi">
                {listRoom
                  .filter((item) => item.is_active)
                  .map((item) => (
                    <Select.Option value={item.id}>{item.name}</Select.Option>
                  ))}
              </Select>
            </Form.Item>
          )}
          {dataModuleDetail?.module_type === 3 && (
            <div className={classes.sessions}>
              {renderSession().map((value) => (
                <Checkbox
                  size="xs"
                  value={value}
                  isSelected={listSessionPractice.includes(value)}
                  onChange={(status) => handleChangeSession(status, value)}
                >
                  {value}
                </Checkbox>
              ))}
            </div>
          )}
        </div>
        <div>
          <Button
            css={{
              margin: '0 auto',
            }}
            flat
            auto
            color="success"
            type="primary"
            htmlType="submit"
          >
            Cập nhật lịch học
          </Button>
        </div>
      </Form>
    </Fragment>
  );
};

export default ScheduleUpdate;
