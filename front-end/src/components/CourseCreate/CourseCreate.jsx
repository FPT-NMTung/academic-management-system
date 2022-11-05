import { Card, Grid, Button, Text } from '@nextui-org/react';
import { Form, Select, Input,message } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../apis/ListApi';
import mergeCourseFamilyres from '../../screens/Admin/Course Family/CourseFamily';
import { CourseFamilyApis, CourseApis } from '../../apis/ListApi';
import { Validater } from '../../validater/Validater';
import { ErrorCodeApi } from '../../apis/ErrorCodeApi';
import toast from "react-hot-toast";
const CourseCreate = ({ onCreateSuccess }) => {
  const [listCourseFamily, setListCourseFamily] = useState([]);

  const [isCreating, setIsCreating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [form] = Form.useForm();
  const [errorValue, setErrorValue] = useState(undefined);

  const getListCourseFamily = () => {
    FetchApi(CourseFamilyApis.getAllCourseFamily).then((res) => {
      setListCourseFamily(res.data);
    });
  };

  const handleSubmitForm = (e) => {
    setIsCreating(true);
    toast.promise
    (FetchApi(
      CourseApis.createCourse,
      {
        code: e.coursecode,
        course_family_code: e.coursefamilycode,
        name: e.coursename,
        semester_count: e.semester,
        is_active: true,
      },
      null,
      null
    ),
    {
      loading: "Đang tạo...",
      success: (res) => {
        onCreateSuccess();
        return "Tạo thành công !";
      },
      error: (err) => {
        setIsCreating(false);
        setIsFailed(true);
        setErrorValue(ErrorCodeApi[err.type_error]);
        if (err?.type_error) {
          return ErrorCodeApi[err.type_error];
        }
        return "Tạo thất bại !";
      },
    }
      );
  };
  useEffect(() => {
    getListCourseFamily();
  }, []);
  return (
    <Grid xs={4}>
      <Card
        css={{
          width: '100%',
          height: 'fit-content',
        }}
        variant="bordered"
      >
        <Card.Header>
        <Text
            b
            p
            size={14}
            css={{
              width: '100%',
              textAlign: 'center',
            }}
          >
            Tạo thêm khóa học mới <b></b>
          </Text>

        </Card.Header>
        <Card.Body>
          <Form
            labelCol={{ flex: '110px', span: 6 }}
            layout="horizontal"
            labelAlign="left"
            labelWrap
            onFinish={handleSubmitForm}
            form={form}
          >
            <Form.Item
              name={'coursename'}
              label={'Tên Khóa Học'}
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
            <Form.Item
              name={'coursefamilycode'}
              label={'Mã chương trình'}
              rules={[
                {
                  required: true,
                  message: 'Hãy điền mã chương trình',
                },
              ]}
            >
              <Select
                showSearch
                disabled={listCourseFamily.length === 0}
                placeholder="Chọn mã chương trình"
                optionFilterProp="children"
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
                }
              >
                {listCourseFamily.map((e, index) => (
                  <Select.Option key={index} value={e.code}>
                    {e.data} {e.code}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={'coursecode'}
              label={'Mã Khóa Học'}
              rules={[
                {
                  required: true,
                  message: 'Hãy điền mã khóa học',
                },
              ]}
            >
              <Input />
            </Form.Item>
            <Form.Item
              name={'semester'}
              label={'Học Kỳ'}
              rules={[
                {
                  // message: 'Hãy nhập học kỳ',
                  required: true,
                  validator: (_, value) => {
                    // check regex phone number viet nam
                    if (Validater.isNumber(value)) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error('Học kỳ không hợp lệ'));
                  },
                },
              ]}
            >
              <Input />
            </Form.Item>
            <Form.Item wrapperCol={{ offset: 6, span: 10 }}>
            <Button
                auto
                flat
                css={{
                  width: '120px',
                }}
                type="primary"
                htmlType="submit"
                disabled={isCreating}
              >
                {'Tạo mới'}
              </Button>
            </Form.Item>
          </Form>
          {!isCreating && errorValue !== undefined && isFailed && (
            <Text
              size={14}
              css={{
                color: 'red',
                textAlign: 'center',
              }}
            >
              {errorValue}, vui lòng thử lại
            </Text>
          )}
        </Card.Body>
      </Card>
    </Grid>
  );
};
export default CourseCreate;
