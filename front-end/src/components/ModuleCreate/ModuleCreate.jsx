import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button, Spin, Table, Tooltip, Space, Typography } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { ModulesApis, CenterApis, CourseApis, SemesterApis } from '../../apis/ListApi';
import classes from './ModuleCreate.module.css';
import { Fragment } from 'react';
import { MdEdit } from 'react-icons/md';
import ColumnGroup from 'antd/lib/table/ColumnGroup';


const ModuleCreate = ({ onCreateSuccess }) => {

    const [isCreating, setIsCreating] = useState(false);
    const [isFailed, setIsFailed] = useState(false);
    const [isLoading, setisLoading] = useState(true);
    const [listCenters, setListCenters] = useState([]);
    const [listCourses, setListCourses] = useState([]);
    const [listSemesterid, setListSemesterid] = useState([]);
    const [exam_type, setExam_type] = useState(4);


    const [form] = Form.useForm();

    const getListCenter = () => {

        FetchApi(CenterApis.getAllCenter).then((res) => {
            const data = res.data.map((e) => {
                return {
                    key: e.id,
                    ...e,
                };
            });
            setListCenters(data);
            setisLoading(false);
        });
    };
    const getListCourse = () => {

        FetchApi(CourseApis.getAllCourse).then((res) => {
            const data = res.data.map((e) => {
                return {
                    coursename: e.code,
                };
            });
            setListCourses(data);
            setisLoading(false);


        });
        setListSemesterid([]);
    };
    const GetListSemesterid = () => {
        const coursecode = form.getFieldValue('course_code').trim();

        FetchApi(CourseApis.getCourseByCode, null, null, [`${coursecode}`]).then((res) => {
            const data = res.data.semester_count

            setListSemesterid(data);


        });
        setisLoading(false);
    };

    const handleChangeExamType = () => {
        const examtype = form.getFieldValue('exam_type');
        setExam_type(examtype);

    };

    useEffect(() => {
        getListCenter();
        getListCourse();


    }, []);

    const handleSubmitForm = (e) => {
        setIsCreating(true);
        FetchApi(
            ModulesApis.createModules,
            {
                center_id: e.center_id,
                semester_name_portal: e.semester_name_portal.trim(),
                module_name: e.module_name.trim(),
                module_exam_name_portal: e.module_exam_name_portal.trim(),
                module_type: e.module_type,
                max_theory_grade: e.max_theory_grade,
                max_practical_grade: e.max_practical_grade,
                hours: e.hours,
                days: e.days,
                exam_type: e.exam_type,
                course_code: e.course_code,
                semester_id: e.semesterid,

            },
            null,
            null
        )
            .then(() => {
                alert('Tạo môn học thành công');
                setIsCreating(false);
                setIsFailed(false);
                onCreateSuccess();
            })
            .catch(() => {
                setIsCreating(false);
                setIsFailed(true);
            });
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
                    initialValues={{
                        exam_type: 4,
                        max_theory_grade: null,
                        max_practical_grade: null,
                    }}

                    onFinish={handleSubmitForm}
                    form={form}
                >

                    <Form.Item
                        name={'module_name'}
                        label={'Môn học'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy chọn môn học',
                            },
                        ]}
                    >

                        <Input placeholder='Tên môn học' style={{
                            width: 300,
                        }} />
                    </Form.Item>
                    <Form.Item
                        name={'center_id'}
                        label={'Cơ sở'}
                        rules={[
                            {
                                required: true,
                                message: 'Hãy chọn cơ sở',
                            },
                        ]}
                    >

                        <Select
                            placeholder="Chọn cơ sở"
                            style={{ width: '100%' }}
                            dropdownStyle={{ zIndex: 9999 }}
                            loading={isLoading}
                        >
                            {listCenters.map((e) => (
                                <Select.Option key={e.key} value={e.id}>
                                    {e.name}
                                </Select.Option>
                            ))}
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
                            name="course_code"
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
                            <Select
                                placeholder="Chọn mã khóa học"
                                style={{ width: '100%' }}
                                dropdownStyle={{ zIndex: 9999 }}
                                loading={isLoading}
                                onChange={GetListSemesterid}

                            >
                                {listCourses.map((e, index) => (
                                    <Select.Option key={index} value={e.coursename}>
                                        {e.coursename}
                                    </Select.Option>
                                ))}
                            </Select>
                        </Form.Item>
                        <Form.Item
                            name="semesterid"
                            rules={[
                                {
                                    required: true,
                                    message: 'Hãy chọc học kỳ',
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
                                style={{ width: '100%' }}
                                dropdownStyle={{ zIndex: 9999 }}
                                // disabled={listSemesterid.length === 0}
                                placeholder="Chọn học kỳ"
                                optionFilterProp="children"
                            // onChange={getListDistrict}
                            >
                                <Select.Option key="1" value="1">Học kỳ 1</Select.Option>
                                <Select.Option key="2" value="2">Học kỳ 2</Select.Option>
                                <Select.Option key="3" value="3">Học kỳ 3</Select.Option>
                                <Select.Option key="4" value="4">Học kỳ 4</Select.Option>






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
                            name="hours"
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
                            name="days"
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
                            name="module_type"
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
                            <Select
                                style={{ width: '100%' }}
                                dropdownStyle={{ zIndex: 9999 }}
                                placeholder="Hình thức học">
                                <Select.Option key="1" value="1">Lý thuyết</Select.Option>
                                <Select.Option key="2" value="2">Thực hành</Select.Option>
                                <Select.Option key="3" value="3">Lý thuyết và Thực hành</Select.Option>
                            </Select>
                        </Form.Item>
                        <Form.Item
                            name="exam_type"

                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                                margin: '0 8px',
                            }}
                        >
                            <Select
                                onChange={handleChangeExamType}
                                style={{ width: '100%' }}
                                dropdownStyle={{ zIndex: 9999 }}
                                placeholder="Hình thức thi">
                                <Select.Option key="4" value={1}>Lý thuyết</Select.Option>
                                <Select.Option key="5" value={2}>Thực hành</Select.Option>
                                <Select.Option key="6" value={3}>Lý thuyết và Thực hành</Select.Option>
                                <Select.Option key="7" value={4}>Không thi</Select.Option>
                                {/* <Select.Option key="4" value="Lý thuyết">Lý thuyết</Select.Option>
                                <Select.Option key="5" value="Thực hành">Thực hành</Select.Option>
                                <Select.Option key="6" value="Lý thuyết và thực hành">Lý thuyết và Thực hành</Select.Option>
                                <Select.Option key="7" value="Không thi">Không thi</Select.Option> */}
                            </Select>
                        </Form.Item>
                    </Form.Item>
                    <Form.Item
                        label="Điểm thi tối đa"
                        style={{
                            marginBottom: 0,
                        }}
                    >
                        <Form.Item
                            name="max_theory_grade"
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                            }}
                        >
                            <Input
                                disabled={exam_type !== 1 && exam_type !== 3}
                                placeholder='Lý thuyết'>

                            </Input>
                        </Form.Item>
                        <Form.Item
                            name="max_practical_grade"

                            rules={[
                                {

                                },
                            ]}
                            style={{
                                display: 'inline-block',
                                width: 'calc(50% - 8px)',
                                margin: '0 8px',
                            }}
                        >
                            <Input
                                disabled={exam_type !== 2 && exam_type !== 3}
                                placeholder='Thực hành' />

                        </Form.Item>
                    </Form.Item>
                    <Form.Item
                        label="Ấn Độ"
                        style={{
                            marginBottom: 0,
                        }}
                    >
                        <Form.Item
                            name="semester_name_portal"
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
                            <Input placeholder='Tên kỳ học'></Input>
                        </Form.Item>
                        <Form.Item
                            name="module_exam_name_portal"
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
                            <Input placeholder='Tên khóa học'></Input>
                        </Form.Item>
                    </Form.Item>
                    <Card.Divider />
                    <Form.Item
                        wrapperCol={{ offset: 19, span: 10 }}
                        style={{

                            margin: '10px 0 0 0',
                        }}
                    >
                        <Button type="primary" htmlType="submit" loading={isCreating} >
                            Tạo mới
                        </Button>

                    </Form.Item>









                </Form>
            )}
            {!isCreating && isFailed && (
                form.resetFields(['max_theory_grade', 'max_practical_grade']),


                <Text
                    size={14}
                    css={{
                        color: 'red',
                        textAlign: 'center',
                    }}
                >
                    Tạo môn học thất bại, vui lòng thử lại
                </Text>
            )}
        </Fragment>
    );
};

export default ModuleCreate;
