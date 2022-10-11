import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button, Spin } from 'antd';
import { Fragment } from 'react';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { CourseFamilyApis } from '../../apis/ListApi';
import classes from './CourseFamilyUpdate.module.css';
import { Validater } from '../../validater/Validater';

const CourseFamilyUpdate = ({ data, onUpdateSuccess }) => {
  // console.log(data);
  const [isUpdating, setIsUpdating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [IsLoading, setIsLoading] = useState(true);
  const [form] = Form.useForm();
  const [listCourseFamily, setlistCourseFamily] = useState([]);

  const getData = () => {
    setIsLoading(true);
    const apiCourseFamily = CourseFamilyApis.getAllCourseFamily;
    console.log(apiCourseFamily);

    FetchApi(apiCourseFamily).then((res) => {
      setlistCourseFamily(res.data);




    });

  }
  useEffect(() => {
    setIsLoading(false);
    // getData();


  }, []);
  const handleSubmitForm = (e) => {
    setIsUpdating(true);

    const body = {
      name: e.coursefamilyname.trim(),
      code: e.codefamily,
      published_year: e.codefamilyyear,
      is_active: true,

    };

    FetchApi(CourseFamilyApis.updateCourseFamily, body, null, [`${data.codefamily}`])
      .then((res) => {
        onUpdateSuccess();
      })
      .catch((err) => {
        setIsUpdating(false);
        setIsFailed(true);
      });
  };
  return (
    <Fragment>
      {(IsLoading) && (
        <div className={classes.loading}>
          <Spin />
        </div>
      )}
      {!IsLoading && (
        <Form
          labelWrap
          labelCol={{ span: 7 }}
          wrapperCol={{ span: 16 }}
          layout="horizontal"
          onFinish={handleSubmitForm}
          form={form}
          initialValues={{
            coursefamilyname: data?.namecoursefamily,
            codefamily: data?.codefamily,
            codefamilyyear: data?.codefamilyyear,


          }}
        >
          <Form.Item
            name={'coursefamilyname'}
            label={'Tên'}
            rules={[
              {
                required: true,
                message: 'Hãy nhập tên',
              },
            ]}
          >
            <Input />
          </Form.Item>

          <Fragment>
            <Form.Item
              name={'codefamily'}
              label={'Mã chương trình học'}

              rules={[
                {
                  required: true,
                  message: 'Hãy nhập mã chương trình học',
                },
              ]}
            >
              <Input disabled />
            </Form.Item>
            <Form.Item
              name={'codefamilyyear'}
              label={'Năm áp dụng'}
              rules={[
                {
                  
                  required: true,
                  validator: (_, value) => {
                    // check regex phone number viet nam
                    if (Validater.isNumber(value)) {
                      return Promise.resolve();
                    }
                    return Promise.reject(
                      new Error('Số năm không hợp lệ')
                    );
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

export default CourseFamilyUpdate;
