import { Card, Grid, Button,Text } from '@nextui-org/react';
import { Form, DatePicker, Select, Input, Radio, message } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { CourseFamilyApis } from '../../apis/ListApi';
import { Validater } from '../../validater/Validater';
import { ErrorCodeApi } from '../../apis/ErrorCodeApi';
import toast from "react-hot-toast";

const CouseFamilyCreate = ({ onCreateSuccess }) => {
  const [isCreating, setIsCreating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [form] = Form.useForm();
  const [errorValue, setErrorValue] = useState(undefined);

  const handleSubmitForm = (e) => {
    setIsCreating(true);
    toast.promise
    (FetchApi(
      CourseFamilyApis.createCourseFamily,
      {
        name: e.coursefamilyname,
        code: e.coursefamilycode,
        published_year: e.year.year(),
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
            Tạo thêm chương trình học mới <b></b>
          </Text>
        </Card.Header>
        <Card.Body>
          <Form
            layout="horizontal"
            labelCol={{ flex: '110px', span: 6 }}
            labelAlign="left"
            labelWrap
            onFinish={handleSubmitForm}
            form={form}
          >
            <Form.Item
              name={'coursefamilyname'}
              label={'Tên'}
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
                    if (value.trim().length < 2 || value.trim().length > 100) {
                      return Promise.reject(
                        new Error('Trường phải từ 2 đến 100 ký tự')
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
              <Input />
            </Form.Item>
            <Form.Item
              name={'year'}
              label={'Năm áp dụng'}
              rules={[
                {
                  required: true,
                  message: 'Hãy chọn năm áp dụng',
                },
              ]}
            >
              <DatePicker placeholder="Năm áp dụng" picker="year" />
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
export default CouseFamilyCreate;
