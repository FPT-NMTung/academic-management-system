import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../apis/ListApi';
import mergeCourseFamilyres from '../../screens/Admin/Course Family/CourseFamily';


const CourseCreate = () => {


    const [isCreating, setIsCreating] = useState(false);
    const [isFailed, setIsFailed] = useState(false);
    const [form] = Form.useForm();
    const handleSubmitForm = (e) => {

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
                    <Text size={14
                    }

                    >
                        Tạo thêm khóa học mới: <b></b>
                    </Text>
                </Card.Header>
                <Card.Divider />
                <Card.Body>
                    <Form

                        // labelCol={{ span: 6 }}
                        labelCol={{ flex: '200px', span: 6 }}
                        layout="horizontal"
                        labelAlign="left"
                        labelWrap

                        //   onFinish={handleSubmitForm}
                        form={form}
                    >
                        <Form.Item

                            name={'coursefamilyname'}
                            label={'Tên Khóa Học'}

                            rules={[
                                {
                                    required: true,
                                    message: 'Hãy điền tên khóa học',
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
                                // showSearch
                                // disabled={listProvince.length === 0}
                                placeholder="Chọn mã chương trình"
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
                            name={'coursefamilycode'}
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
                       
                            name={'year'}
                            label={'Học Kỳ'}
                            rules={[
                                {
                                    required: true,
                                    message: 'Hãy điền học kỳ',
                                },
                            ]}
                        >

                            <Input />
                        </Form.Item>

                        <Form.Item wrapperCol={{ offset: 6, span: 10 }}>
                            <Button type="primary" htmlType="submit" loading={isCreating}>
                                Tạo mới
                            </Button>
                        </Form.Item>
                    </Form>
                    {isCreating && isFailed && (
                        <Text
                            size={14}
                            css={{
                                color: 'red',
                                textAlign: 'center',
                            }}
                        >
                            Tạo khóa học thất bại, kiểm tra lại thông tin và thử lại
                        </Text>
                    )}
                </Card.Body>
            </Card>
        </Grid>


    )
}
export default CourseCreate;