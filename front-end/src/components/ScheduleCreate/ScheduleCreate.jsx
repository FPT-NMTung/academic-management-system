import { Button, Divider, Spacer } from '@nextui-org/react';
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
import { useParams } from 'react-router-dom';
import FetchApi from '../../apis/FetchApi';
import { ManageClassApis, ManageTeacherApis, RoomApis } from '../../apis/ListApi';
import classes from './ScheduleCreate.module.css';

const ScheduleCreate = ({ dataModule }) => {
  const [listTeacher, setListTeacher] = useState([]);
  const [listRoom, setListRoom] = useState([]);

  const { id, moduleId } = useParams();
  const [form] = Form.useForm();

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

  const getInformationClass = () => {
    FetchApi(ManageClassApis.getInformationClass, null, null, [String(id)])
      .then((res) => {
        // form.setFieldsValue({
        //   class_name: res.data.class_name,
        //   module_name: dataModule.module_name,
        // });
      })
      .catch((err) => {
        toast.error('Lỗi lấy thông tin lớp học');
      });
  };

  const initValueToFrom = () => {
    form.setFieldsValue({

    });
  }

  useEffect(() => {
    getListTeacher();
    getListRoom();
    getInformationClass();
    initValueToFrom();
  }, []);

  const handleSubmitForm = (e) => {
    console.log(e);
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
        labelCol={{ span: 9 }}
        wrapperCol={{ span: 16 }}
      >
        <div className={classes.formCreate}>
          <Form.Item name={'class_days_id'} label="Thời gian học trong tuần">
            <Select placeholder="Chọn thời gian học">
              <Select.Option value={1}>Thứ 2, 4, 6</Select.Option>
              <Select.Option value={2}>Thứ 3, 5, 7</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item name={'teacher_id'} label="Giáo viên">
            <Select placeholder="Chọn giáo viên">
              {listTeacher.map((item) => (
                <Select.Option value={item.user_id}>
                  {item.first_name} {item.last_name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item name={'start_date'} label="Ngày bắt đầu">
            <DatePicker placeholder="Chọn ngày bắt đầu" format="DD/MM/YYYY" />
          </Form.Item>
          <Form.Item name={'working_time_id'} label="Khung giờ dạy">
            <Select placeholder="Chọn thời gian học trong tuần">
              <Select.Option value={1}>Sáng (8h00 - 12h00)</Select.Option>
              <Select.Option value={2}>Chiều (13h00 - 17h00)</Select.Option>
              <Select.Option value={3}>Tối (18h00 - 22h00)</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item name={'time'} label="Giờ học">
            <TimePicker.RangePicker format={'HH:mm'} />
          </Form.Item>
          <Form.Item label="Số lượng buổi học">
            <InputNumber placeholder="Nhập số" />
          </Form.Item>
          <Form.Item label="Phòng học lý thuyết">
            <Select placeholder="Chọn thời gian học trong tuần">
              {listRoom
                .filter((item) => item.is_active && item.room_type.id === 1)
                .map((item) => (
                  <Select.Option value={item.id}>{item.name}</Select.Option>
                ))}
            </Select>
          </Form.Item>
          <Form.Item label="Phòng học thực hành">
            <Select placeholder="Chọn thời gian học trong tuần">
              {listRoom
                .filter((item) => item.is_active && item.room_type.id === 2)
                .map((item) => (
                  <Select.Option value={item.id}>{item.name}</Select.Option>
                ))}
            </Select>
          </Form.Item>
          <Form.Item label="Ghi chú">
            <Input.TextArea placeholder="Nhập ghi chú" />
          </Form.Item>
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
            Tạo lịch học
          </Button>
        </div>
      </Form>
    </Fragment>
  );
};

export default ScheduleCreate;
