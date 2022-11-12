import { Card, Grid, Button, Text,Loading } from '@nextui-org/react';
import { Form, Select, Input, Spin, message } from 'antd';
import { Fragment } from 'react';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { CourseFamilyApis, CourseApis } from '../../apis/ListApi';
import classes from './CourseUpdate.module.css';
import { Validater } from '../../validater/Validater';
import toast from 'react-hot-toast';
import { ErrorCodeApi } from "../../apis/ErrorCodeApi";

const CourseUpdate = ({ data, onUpdateSuccess }) => {
  const [isUpdating, setIsUpdating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [IsLoading, setIsLoading] = useState(true);
  const [form] = Form.useForm();
  const [listCourse, setlistCourse] = useState([]);
  const [listCourseFamily, setlistCourseFamily] = useState([]);
  const [messageFailed, setMessageFailed] = useState(undefined);
  const [canDelete, setCanDelete] = useState(undefined);
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
  const checkCanDelete = () => {
    console.log(CourseApis.checkCanDeleteCourse);
    FetchApi(CourseApis.checkCanDeleteCourse, null, null, [
      String(data.codecourse),
    ])
      .then((res) => {
        if (res.data.can_delete === true) {
          setCanDelete(true);
        } else {
          setCanDelete(false);
        }
      })
      .catch((err) => {
        toast.error('Lỗi kiểm tra khả năng xóa');
      });
  };
  const handleDelete = () => {
    toast.promise(
      FetchApi(CourseApis.deleteCourse, null, null, [
        String(data.codecourse),
      ]),
      {
        loading: 'Đang xóa',
        success: (res) => {
          onUpdateSuccess();
          return 'Xóa thành công';
        },
        error: (err) => {
          return 'Xóa thất bại';
        },
      }
    );
  };

  useEffect(() => {
    setIsLoading(false);
    getlistCourseFamily();
    checkCanDelete();
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
    toast.promise(
    FetchApi(CourseApis.updateCourse, body, null, [`${e.codecourse}`]),
    {
      loading: "Đang cập nhật...",
      success: (res) => {
        onUpdateSuccess();
        return "Cập nhật thành công !";
      },
      error: (err) => {
        setIsUpdating(false);
        setIsFailed(true);
        setMessageFailed(ErrorCodeApi[err.type_error]);
        if (err?.type_error) {
          return ErrorCodeApi[err.type_error];
        }
        return "Cập nhật thất bại !";
      },
    }
      // .then((res) => {
      //   message.success('Cập nhật khóa học thành công');
      //   onUpdateSuccess();
      // })
      // .catch((err) => {
      //   setIsUpdating(false);
      //   setIsFailed(true);
      // }
      );
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
                  if (value.trim().length < 1 || value.trim().length > 255) {
                    return Promise.reject(
                      new Error('Trường phải từ 1 đến 255 ký tự')
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
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
                }
              >
                {listCourseFamily.map((e, index) => (
                  <Select.Option key={index} value={e.code}>
                    {e.code}
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

          <Form.Item wrapperCol={{ offset: 7, span: 99 }} >
          <div
              style={{
                display: 'flex',
                gap: '10px',
              }}
            >
          <Button
              flat
              auto
              css={{
                width: "120px",
                
              }}
              type="primary"
              htmlType="submit"
              disabled={isUpdating}
            >
              Cập nhật
            </Button>
            <Button
                flat
                auto
                css={{
                  width: '80px',
                }}
                color={'error'}
                disabled={!canDelete}
                onPress={handleDelete}
              >
                {canDelete === undefined && <Loading size="xs" />}
                {canDelete !== undefined && 'Xoá'}
              </Button>{' '}
              </div>
          </Form.Item>
        </Form>
      )}
    
    </Fragment>
  );
};

export default CourseUpdate;
