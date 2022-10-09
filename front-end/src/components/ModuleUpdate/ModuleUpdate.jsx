import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button, Spin, Table, Tooltip, Space, Typography } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { ModulesApis, CenterApis, CourseApis, SemesterApis } from '../../apis/ListApi';
import classes from './ModuleUpdate.module.css';
import { Fragment } from 'react';
import { MdEdit } from 'react-icons/md';
import ColumnGroup from 'antd/lib/table/ColumnGroup';


const ModuleUpdate = ({ data, onUpdateSuccess }) => {
   
    const [isLoading, setisLoading] = useState(true);
    const [isUpdating, setIsUpdating] = useState(false);
    const [form] = Form.useForm();
    const [listCenters, setListCenters] = useState([]);
    const [listCourses, setListCourses] = useState([]);
    const [listSemesterid, setListSemesterid] = useState([]);
    const [isFailed, setIsFailed] = useState(false);
    const [exam_type, setExam_type] = useState(4);
    const handleChangeExamType = () => {
        const examtype = form.getFieldValue('exam_type');
        setExam_type(examtype);

    };
    const handleSubmitForm = (e) => {
        setIsUpdating(true);
        const body = {
            center_id: e.center_id,
            semester_name_portal: e.semester_name_portal,
            module_name: e.module_name,
            module_exam_name_portal: e.module_exam_name_portal,
            module_type: e.module_type,
            max_theory_grade: e.max_theory_grade,
            max_practical_grade: e.max_practical_grade,
            hours: e.hours,
            days: e.days,
            exam_type: e.exam_type,
            course_code: e.course_code,
            semester_id: e.semesterid,
          };
      
          FetchApi(CenterApis.updateCenter, body, null, [`${data.id}`])
            .then((res) => {
              onUpdateSuccess();
            })
            .catch((err) => {
              setIsUpdating(false);
              setIsFailed(true);
            });
    };
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
            console.log(data);
         
        });
        setisLoading(false);
    };
    useEffect(() => {
        getListCenter();
        getListCourse();
    

    }, []);
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
                    module_name: data?.modulename,
                    center_id: data?.center_id,
                    course_code: data?.coursecode,
                    semesterid: data?.semester,
                    hours: data?.hours,
                    days: data?.days,
                    module_type: data?.module_type,
                    exam_type: data?.exam_type,
                    max_theory_grade: data?.max_theory_grade,
                    max_practical_grade: data?.max_practical_grade,
                    semester_name_portal: data?.semester_name_portal,
                    module_exam_name_portal: data?.module_exam_name_portal,

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
                            message: 'Hãy điền mã chương trình',
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
                            message: 'Hãy điền mã chương trình',
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
                            <Select.Option key="1" value="1">1</Select.Option>
                            <Select.Option key="2" value="2">2</Select.Option>
                            <Select.Option key="3" value="3">3</Select.Option>
                            <Select.Option key="4" value="4">4</Select.Option>






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
                            <Select.Option key="1" value="Lý thuyết">Lý thuyết</Select.Option>
                            <Select.Option key="2" value="Thực hành">Thực hành</Select.Option>
                            <Select.Option key="3" value="Lý thuyết và Thực hành">Lý thuyết và Thực hành</Select.Option>
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
                        rules={[
                            {
                               
                            },
                        ]}
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
                    <Button type="primary" htmlType="submit" loading={isUpdating}>
                        Cập nhật
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
                Tạo môn học thất bại, vui lòng thử lại
            </Text>
        )}
    </Fragment>

    );
};
export default ModuleUpdate;