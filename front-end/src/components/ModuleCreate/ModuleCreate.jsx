import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button, Spin, Table, Tooltip, Space, Typography } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { ModulesApis } from '../../apis/ListApi';
import classes from './ModuleCreate.module.css';
import { Fragment } from 'react';
import { MdEdit } from 'react-icons/md';
import ColumnGroup from 'antd/lib/table/ColumnGroup';

const ModuleCreate = ({ onCreateSuccess }) => {

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

                    // labelCol={{ span: 6 }}
                    labelCol={{
                        span: 7,
                    }}
                    wrapperCol={{
                        span: 16,
                    }}
                    layout="horizontal"
                    labelAlign="right"
                    labelWrap


                    //   onFinish={handleSubmitForm}
                    form={form}
                >

                    <Form.Item
                        name={'coursefamilycode'}
                        label={'Môn học'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền mã chương trình',
                            },
                        ]}
                    >

                        <Input placeholder='Tên môn học' style={{
                            width: 300,
                        }} />
                    </Form.Item>
                    <Form.Item
                        name={'coursefamilycode'}
                        label={'Cơ sở'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền mã chương trình',
                            },
                        ]}
                    >

                        <Select style={{
                            width: 300,
                        }}
                            // showSearch
                            // disabled={listProvince.length === 0}
                            placeholder="Chọn cơ sở"
                            optionFilterProp="children"
                        // onChange={getListDistrict}
                        >
                            {/* {listProvince.map((e) => (
                                 <Select.Option key={e.id} value={e.id}>
                                     {e.name}
                                 </Select.Option>
                             ))} */}
                        </Select>
                    </Form.Item>
                    <Form.Item
                        label="Mã khóa học"
                        style={{
                            marginBottom: 0,
                        }}
                        rules={[
                            {
                                required: true,

                            },
                        ]}
                    >
                        <Space></Space>
                        <Form.Item
                            name="year"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                            }}
                        >
                            <Input placeholder="Chọn mã khóa học" />
                        </Form.Item>
                        <Form.Item
                            name="month"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                                margin: '0 8px',
                            }}
                        >

                            <Select
                                // showSearch
                                // disabled={listProvince.length === 0}
                                placeholder="Chọn học kỳ"
                                optionFilterProp="children"
                            // onChange={getListDistrict}
                            >
                                {/* {listProvince.map((e) => (
                                 <Select.Option key={e.id} value={e.id}>
                                     {e.name}
                                 </Select.Option>
                             ))} */}
                            </Select>
                        </Form.Item>
                    </Form.Item>
                    <Form.Item
                        label="Thời lượng học"
                        style={{
                            marginBottom: 0,
                        }}
                        rules={[
                            {
                                required: true,

                            },
                        ]}
                    >
                        <Space></Space>
                        <Form.Item
                            name="year"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                            }}
                        >
                            <Input placeholder="Số tiếng" />
                        </Form.Item>
                        <Form.Item
                            name="month"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                                margin: '0 8px',
                            }}
                        >
                            <Input placeholder="Số buổi" />
                        </Form.Item>
                    </Form.Item>

                    <Form.Item
                        label="Hình thức môn học"
                        style={{
                            marginBottom: 0,
                        }}
                    >
                        <Form.Item
                            name="year"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                            }}
                        >
                            <Select placeholder="Hình thức học"> </Select>
                        </Form.Item>
                        <Form.Item
                            name="month"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                                margin: '0 8px',
                            }}
                        >
                            <Select placeholder="Hình thức thi"> </Select>
                        </Form.Item>
                    </Form.Item>
                    <Form.Item
                        label="Điểm thi tối đa"
                        style={{
                            marginBottom: 0,
                        }}
                    >
                        <Form.Item
                            name="year"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                            }}
                        >
                            <Select placeholder="Lý thuyết"> </Select>
                        </Form.Item>
                        <Form.Item
                            name="month"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                                margin: '0 8px',
                            }}
                        >
                            <Select placeholder="Thực hành"> </Select>
                        </Form.Item>
                    </Form.Item>
                    <Form.Item
                        label="Ấn Độ"
                        style={{
                            marginBottom: 0,
                        }}
                    >
                        <Form.Item
                            name="year"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                            }}
                        >
                            <Select placeholder="Tên kỳ học"> </Select>
                        </Form.Item>
                        <Form.Item
                            name="month"
                            rules={[
                                {
                                    required: true,
                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                                margin: '0 8px',
                            }}
                        >
                            <Select placeholder="Tên khóa học"> </Select>
                        </Form.Item>
                    </Form.Item>
                    <Card.Divider />
                    <Form.Item
                     wrapperCol={{ offset: 19, span: 10 }}
                     style={{
                        
                        margin: '10px 0 0 0',
                    }}
                     >
                         <Button type="primary" htmlType="submit" loading={isCreating}>
                        Tạo mới
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
