import classes from './Module.module.css';
import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table, Modal } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
// import { AddressApis, CenterApis } from '../../../apis/ListApi';

import { MdEdit } from 'react-icons/md';
const Module = () => {
    const [listCourseFamily, setlistCourseFamily] = useState([]);
    const [selectedCourseFamilyId, setselectedCourseFamilyId] = useState(null);
    const [isCreate, setIsCreate] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [loading, setLoading] = useState(false);

    const [isCreating, setIsCreating] = useState(false);
    const [isFailed, setIsFailed] = useState(false);
    const [form] = Form.useForm();
    const showModal = () => {
        setIsModalOpen(true);
    };
    const handleOk = () => {
        setIsModalOpen(false);
    };

    const handleCancel = () => {
        setIsModalOpen(false);
    };
    useEffect(() => {
        // const apiCanter = CenterApis.getAllCenter;
        // FetchApi(apiCanter).then((res) => {
        //   const data = res.data;
        //   const mergeAddressRes = data.map((e) => {
        //     return {
        //       key: e.id,
        //       ...e,
        //       address: `${e.ward.prefix} ${e.ward.name}, ${e.district.prefix} ${e.district.name}, ${e.province.name}`,
        //     };
        //   });
        //   setListCenter(mergeAddressRes);
        // });
        const mergeCourseFamily = [
            {
                key: 1,
                name: 'ASAP',
                code: 'AB-7023-CPISM',
                year: '2021'
            },
            {
                key: 2,
                name: 'ACSS',
                code: 'CD-7023-CPISM',
                year: '2019'
            },
            {
                key: 3,
                name: 'BSSS',
                code: 'AS-7023-CPISM',
                year: '2022'
            },
            {
                key: 4,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            },
            {
                key: 5,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            },
            {
                key: 6,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            },
            ,
            {
                key: 7,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            },
            ,
            {
                key: 8,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            }, {
                key: 9,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            }, {
                key: 10,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            }, {
                key: 11,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            }, {
                key: 12,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year: '2024'
            },

        ]
        // const data = mergeCourseFamily;
        const mergeCourseFamilyres = mergeCourseFamily.map((e) => {
            return {
                key: e.key,

                namecoursefamily: e.name,
                codefamily: e.code,
                codefamiyyear: e.year
            };
        });
        setlistCourseFamily(mergeCourseFamilyres);

    });

    return (
        <div >
            <Grid.Container gap={2} justify="center">
                <Grid xs={12}>
                    <Card
                        css={{
                            width: '100%',
                            height: 'fit-content',

                        }}
                    >
                        <Card.Header>
                            <div className={classes.headerTable}>
                                <Text size={14}>Danh sách môn học</Text>
                                <Form

                                    wrapperCol={{ span: 18 }}
                                    layout="inline"
                                    labelAlign="left"



                                    //   onFinish={handleSubmitForm}
                                    form={form}
                                >
                                    <Form.Item

                                        name={'coursefamilyname'}


                                        rules={[
                                            {

                                                message: 'Hãy điền tên khóa học',
                                                min: 0, max: 10
                                            },
                                        ]}
                                    >
                                        <Input placeholder={'Môn học'} />
                                    </Form.Item>
                                    <Form.Item
                                        name={'coursefamilycode'}

                                        rules={[
                                            {

                                                message: 'Hãy điền mã chương trình',
                                            },
                                        ]}
                                    >
                                        <Input placeholder={'Mã khóa học'} />
                                    </Form.Item>
                                    <Form.Item
                                        name={'Hình thức học'}

                                        rules={[
                                            {

                                                message: 'Hãy điền mã khóa học',
                                            },
                                        ]}
                                    >

                                        <Input placeholder={'Hình thức học'} />
                                    </Form.Item>
                                    <Form.Item

                                        name={'year'}

                                        rules={[
                                            {

                                                message: 'Hãy điền học kỳ',
                                            },
                                        ]}
                                    >

                                        <Input placeholder={'Hình thức thi'} />
                                    </Form.Item>


                                </Form>
                                <Button
                                    type="primary"
                                    onClick={() => {
                                        setIsCreate(!isCreate);
                                    }}
                                >
                                    Tìm kiếm
                                </Button>
                                <Button
                                    type="primary"
                                    onClick={(showModal) => {
                                        setIsModalOpen(!isModalOpen);
                                        setIsCreate(!isCreate);
                                    }}
                                >
                                    Thêm môn học
                                </Button>
                            </div>
                        </Card.Header>

                        <Card.Divider />
                        <Table
                            pagination={{ position: ['bottomCenter'] }}
                            dataSource={listCourseFamily}
                            tableLayout="auto "
                        >
                            <Table.Column title="STT" dataIndex="namecoursefamily" key="name" />
                            <Table.Column title="Môn học" dataIndex="namecoursefamily" key="name" />
                            <Table.Column title="Cơ sở" dataIndex="codefamily" key="address" />
                            <Table.Column title="Mã Khóa Học" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Thời lượng học/ tiếng" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Số buổi" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Hình thức học" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Hình thức thi" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Điểm tối đa thi lý thuyết" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Điểm tối đa thi thực hành" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Tên kỳ học ( Ấn Độ )" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Tên kỳ thi ( Ấn Độ )" dataIndex="codefamiyyear" key="year" />
                            <Table.Column title="Học kỳ" dataIndex="codefamiyyear" key="year" />
                            <Table.Column
                                title="Hành động"
                                dataIndex="action"
                                key="action"
                                render={(_, data) => {
                                    return (
                                        <MdEdit className={classes.editIcon} onClick={(showModal) => {
                                            setIsModalOpen(!isModalOpen);
                                            
                                         }} />
                                    );
                                }}
                            />
                        </Table>

                    </Card>

                </Grid>
                {/* {isCreate && (
                    <ModuleCreate />
                )} */}

            </Grid.Container>

            <Modal title="Thêm môn học"
                getContainer={false}
                zIndex={1000}
                onCancel={handleCancel}
                width={1200}
                open={isModalOpen}
                footer={[
                    <Button key="back" onClick={handleCancel}>
                        Trở lại
                    </Button>,
                    <Button
                        key="submit"
                        type="primary"
                        loading={loading}
                        onClick={handleOk}
                    >
                        Xóa
                    </Button>,
                    <Button
                        key="link"
                        href="https://google.com"
                        type="primary"
                        loading={loading}
                        onClick={handleOk}
                    >
                        Lưu Lại
                    </Button>,
                ]}>

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
                        label={'Môn học'}

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
                        label={'Mã khóa học'}
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
                        label={'Cơ sở'}
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
                        label={'Thời lượng học/ tiếng'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền học kỳ',
                            },
                        ]}
                    >

                        <Input />
                    </Form.Item>
                    <Form.Item

                        name={'year'}
                        label={'Số buổi'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền học kỳ',
                            },
                        ]}
                    >

                        <Input />
                    </Form.Item>
                    <Form.Item

                        name={'year'}
                        label={'Hình thức học'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền học kỳ',
                            },
                        ]}
                    >

                        <Input />
                    </Form.Item>
                    <Form.Item

                        name={'year'}
                        label={'Hình thức thi'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền học kỳ',
                            },
                        ]}
                    >

                        <Input />
                    </Form.Item>
                    <Form.Item

                        name={'year'}
                        label={'Điểm tối đa thi lý thuyết'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền học kỳ',
                            },
                        ]}
                    >

                        <Input />
                    </Form.Item>
                    <Form.Item

                        name={'year'}
                        label={'Điểm tối đa thi thực hành'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền học kỳ',
                            },
                        ]}
                    >

                        <Input />
                    </Form.Item>
                    <Form.Item

                        name={'year'}
                        label={'Tên kỳ học ( Ấn Độ )'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền học kỳ',
                            },
                        ]}
                    >

                        <Input />
                    </Form.Item>
                    <Form.Item

                        name={'year'}
                        label={'Tên khóa học ( Ấn Độ )'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy điền học kỳ',
                            },
                        ]}
                    >

                        <Input />
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
                        Tạo môn học thất bại, kiểm tra lại thông tin và thử lại
                    </Text>
                )}


            </Modal>
        </div>
    )
}
export default Module;