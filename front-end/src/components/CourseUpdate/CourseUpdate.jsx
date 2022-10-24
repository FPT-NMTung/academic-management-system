import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button, Spin, message } from 'antd';
import { Fragment } from 'react';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { CourseFamilyApis, CourseApis } from '../../apis/ListApi';
import classes from './CourseUpdate.module.css';
import { Validater } from '../../validater/Validater';
const CourseUpdate = ({ data, onUpdateSuccess }) => {
  const [isUpdating, setIsUpdating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [IsLoading, setIsLoading] = useState(true);
  const [form] = Form.useForm();
  const [listCourse, setlistCourse] = useState([]);
  const [listCourseFamily, setlistCourseFamily] = useState([]);
  const getlistCourseFamily = () => {
    FetchApi(CourseFamilyApis.getAllCourseFamily).then((res) => {
      setlistCourseFamily(res.data);
    });
  };
  const getData = () => {
    setIsLoading(true);
    const apiCourse = CourseApis.getAllCourse;
    console.log(apiCourse);

    FetchApi(apiCourse).then((res) => {
      setlistCourse(res.data);
    });
  };

  useEffect(() => {
    setIsLoading(false);
    getlistCourseFamily();
  }, []);

  const handleSubmitForm = (e) => {
    setIsUpdating(true);

    const body = {
      code: e.codecourse,
      course_family_code: e.course_family_code,
      name: e.coursename.trim(),
      semester_count: e.semester_count,
      is_active: true,
    };

    FetchApi(CourseApis.updateCourse, body, null, [`${e.codecourse}`])
      .then((res) => {
        message.success('Cập nhật khóa học thành công');
        onUpdateSuccess();
      })
      .catch((err) => {
        setIsUpdating(false);
        setIsFailed(true);
      });
  };

  return (
    <Fragment>
      {IsLoading && (
        <div className={classes.loading}>
          <Spin />
        </div>
      )}
      {!IsLoading && (
        <Form
          labelCol={{ span: 7 }}
          wrapperCol={{ span: 16 }}
          layout="horizontal"
          onFinish={handleSubmitForm}
          form={form}
          initialValues={{
            coursename: data?.coursename,
            codecourse: data?.codecourse,
            course_family_code: data?.course_family_code,
            semester_count: data?.semester_count,
          }}
          labelWrap
        >
          <Form.Item
            name={'coursename'}
            label={'Tên khóa học'}
            rules={[
              {
                required: true,
                validator: (_, value) => {
                  if (value === null || value === undefined) {
                    return Promise.reject('Trường này không được để trống');
                  }
                  if (
                    Validater.isContaintSpecialCharacterForName(value.trim())
                  ) {
                    return Promise.reject(
                      'Trường này không được chứa ký tự đặc biệt'
                    );
                  }
                  if (value.trim().length < 2 || value.trim().length > 255) {
                    return Promise.reject(
                      new Error('Trường phải từ 2 đến 255 ký tự')
                    );
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <Input />
          </Form.Item>

          <Fragment>
            <Form.Item
              name={'course_family_code'}
              label={'Mã chương trình học'}
              rules={[
                {
                  required: true,
                  message: 'Hãy chọn mã chương trình học',
                },
              ]}
            >
              <Select
                showSearch
                disabled={listCourseFamily.length === 0}
                placeholder="Chọn mã chương trình học"
                optionFilterProp="children"
                onChange={getlistCourseFamily}
                dropdownStyle={{ zIndex: 9999 }}
              >
                {listCourseFamily.map((e, index) => (
                  <Select.Option key={index} value={e.code}>
                    {e.data} {e.code}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={'codecourse'}
              label={'Mã khóa học'}
              rules={[
                {
                  required: true,
                  message: 'Hãy nhập mã khóa học',
                },
              ]}
            >
              <Input disabled />
            </Form.Item>
            <Form.Item
              name={'semester_count'}
              label={'Học kỳ'}
              rules={[
                {
                  // message: 'Hãy nhập học kỳ',
                  required: true,
                  validator: (_, value) => {
                    const semester = value.toString();
                    // check regex phone number viet nam
                    if (Validater.isNumber(semester)) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error('Học kỳ không hợp lệ'));
                  },
                },
              ]}
            >
              <Input />
            </Form.Item>
          </Fragment>

          <Form.Item wrapperCol={{ offset: 7, span: 99 }}>
            <Button type="primary" htmlType="submit" loading={isUpdating}>
              Cập nhật
            </Button>
            <Button
              style={{ marginLeft: 10 }}
              type="primary"
              htmlType="button"
              danger
              disabled
            >
              Xoá
            </Button>
          </Form.Item>
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
          Cập nhật thất bại, vui lòng thử lại
        </Text>
      )}
    </Fragment>
  );
};

export default CourseUpdate;
