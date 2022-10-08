import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button, Spin } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { ModulesApis } from '../../apis/ListApi';
import classes from './ModuleCreate.module.css';
import { Fragment } from 'react';

const ModuleCreate = ({onCreateSuccess}) => {

    const [isUpdating, setIsUpdating] = useState(false);
    const [isCreating, setIsCreating] = useState(false);
    const [isFailed, setIsFailed] = useState(false);
    const [isLoading, setisLoading] = useState(true);

    const [form] = Form.useForm();


    useEffect(() => {
        setisLoading(false);
    }, []);

      const handleSubmitForm = (e) => {
        // setIsCreating(true);
        // FetchApi(
        //   CenterApis.createCenter,
        //   {
        //     name: e.name.trim(),
        //     province_id: e.province,
        //     district_id: e.district,
        //     ward_id: e.ward,
        //   },
        //   null,
        //   null
        // )
        //   .then(() => {
        //     onCreateSuccess();
        //   })
        //   .catch(() => {
        //     setIsCreating(false);
        //     setIsFailed(true);
        //   });
      };

    return (
        <Fragment>
            {(isLoading) && (
                <div className={classes.loading}>
                    <Spin />
                </div>
            )}
            {!isLoading && (
                <Form
                    labelCol={{ span: 7 }}
                    wrapperCol={{ span: 16 }}
                    layout="horizontal"
                    // onFinish={handleSubmitForm}
                    form={form}
                // initialValues={{
                //   name: data?.name,
                //   province: data?.province.id,
                //   district: data?.district.id,
                //   ward: data?.ward.id,
                // }}
                >
                    <Form.Item
                        name={'name'}
                        label={'Tên cơ sở'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy nhập tên cơ sở',
                            },
                        ]}
                    >
                        <Input />
                    </Form.Item>

                    <Fragment>
                        <Form.Item
                            name={'province'}
                            label={'Tỉnh/Thành phố'}
                            rules={[
                                {
                                    required: true,
                                    message: 'Hãy chọn tỉnh/thành phố',
                                },
                            ]}
                        >

                        </Form.Item>
                        <Form.Item
                            name={'district'}
                            label={'Quận/Huyện'}
                            rules={[
                                {
                                    required: true,
                                    message: 'Hãy chọn quận/huyện',
                                },
                            ]}
                        >

                        </Form.Item>
                        <Form.Item
                            name={'ward'}
                            label={'Phường/Xã'}
                            rules={[
                                {
                                    required: true,
                                    message: 'Hãy chọn phường/xã',
                                },
                            ]}
                        >

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

export default ModuleCreate;
