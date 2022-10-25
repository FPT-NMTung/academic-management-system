import { Card, Grid, Text } from '@nextui-org/react';
import { Form, DatePicker, Select, Input, Button, Radio, message } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { CourseFamilyApis } from '../../apis/ListApi';
import { Validater } from '../../validater/Validater';
import { ErrorCodeApi } from '../../apis/ErrorCodeApi';

const CouseFamilyCreate = ({ onCreateSuccess }) => {
  const [isCreating, setIsCreating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [form] = Form.useForm();
  const [errorValue, setErrorValue] = useState(undefined);

  const handleSubmitForm = (e) => {
    setIsCreating(true);
    FetchApi(
      CourseFamilyApis.createCourseFamily,
      {
        name: e.coursefamilyname,
        code: e.coursefamilycode,
        published_year: e.year.year(),
        is_active: true,
      },
      null,
      null
    )
      .then(() => {
        message.success('Tạo chương trình học thành công');
        onCreateSuccess();
      })
      .catch((err) => {
        setErrorValue(ErrorCodeApi[err.type_error]);
        setIsCreating(false);
        setIsFailed(true);
      });
  };
  return (
    <Grid xs={4}>
      <Card
        css={{
          width: '100%',
          height: 'fit-content',
        }}
      >
        <Card.Header>
          <Text size={14}>
            Tạo thêm chương trình học mới: <b></b>
          </Text>
        </Card.Header>
        <Card.Divider />
        <Card.Body>
          <Form
            // labelCol={{ span: 6 }}
            // wrapperCol={{ span: 14 }}
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
              <Button type="primary" htmlType="submit" loading={isCreating}>
                Tạo mới
              </Button>
            </Form.Item>
          </Form>
          {!isCreating && isFailed && (
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
