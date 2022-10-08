import classes from './Module.module.css';
import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table, Modal } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ModulesApis } from '../../../apis/ListApi';
import  ModuleUpdate  from '../../../components/ModuleUpdate/ModuleUpdate';
import  ModuleCreate  from '../../../components/ModuleCreate/ModuleCreate';
import { MdEdit } from 'react-icons/md';
const Module = () => {
    const [listModules, setlistModules] = useState([]);
    const [selectedModuleId, setselectedModuleId] = useState(null);
    const [IsLoading, setIsLoading] = useState(false);
    const [isCreate, setIsCreate] = useState(false);
    const getData = () => {
        setIsLoading(true);
        const apiModule = ModulesApis.getAllModules;

        FetchApi(apiModule).then((res) => {
            const data = res.data;
            const mergeModuleRes = data.map((e, index) => {
                return {
                    id: e.id,
                    key: index,
                    modulename: e.module_name,
                    ...e,
                    ...e,
                    centername: e.center.province.name,
                    coursecode: e.course_module_semester.course_code,
                    hours: e.hours,
                    days: e.days,
                    module_type: e.module_type,
                    exam_type: e.exam_type,
                    max_theory_grade: e.max_theory_grade,
                    max_practical_grade: e.max_practical_grade,
                    semester_name_portal: e.semester_name_portal,
                    module_exam_name_portal: e.module_exam_name_portal,
                    semester: e.course_module_semester.semester_id,

                };
            });


            setlistModules(mergeModuleRes);
            // setIsLoading(false);
        });
    };
    const handleAddSuccess = () => {
        setIsCreate(false);
        getData();
    }
    const handleUpdateSuccess = () => {
        setselectedModuleId(null);
        getData();
    };
    useEffect(() => {
        getData();

    }, []);
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

                                >
                                    <Form.Item

                                        name={'modulename'}


                                        rules={[
                                            {

                                                message: 'Hãy điền tên môn học',
                                                // min: 0, max: 10
                                            },
                                        ]}
                                    >
                                        <Input placeholder={'Môn học'} />
                                    </Form.Item>
                                    <Form.Item
                                        name={'course_code'}

                                        rules={[
                                            {

                                                message: 'Hãy điền mã khóa học',
                                            },
                                        ]}
                                    >
                                        <Input placeholder={'Mã khóa học'} />
                                    </Form.Item>
                                    <Form.Item
                                        name={'module_type'}

                                        rules={[
                                            {

                                                message: 'Hãy điền hình thức học',
                                            },
                                        ]}
                                    >

                                        <Input placeholder={'Hình thức học'} />
                                    </Form.Item>
                                    <Form.Item

                                        name={'exam_type'}

                                        rules={[
                                            {

                                                message: 'Hãy điền hình thức thi',
                                            },
                                        ]}
                                    >

                                        <Input placeholder={'Hình thức thi'} />
                                    </Form.Item>


                                </Form>
                                <Button
                                    type="primary"
                                // onClick={() => {
                                //     setIsCreate(!isCreate);
                                // }}
                                >
                                    Tìm kiếm
                                </Button>
                                <Button
                                    title=''
                                    dataIndex="action"
                                    key="action"
                                    type="primary"
                                    onClick={() => {
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
                            dataSource={listModules}
                        // tableLayout="auto "
                        >
                            <Table.Column title="" dataIndex="id" key="id" />
                            <Table.Column title="STT" dataIndex="key" key="name" />
                            <Table.Column title="Môn học" dataIndex="modulename" key="modulename" />
                            <Table.Column title="Cơ sở" dataIndex="centername" key="centername" />
                            <Table.Column title="Mã Khóa Học" width='150px' dataIndex="coursecode" key="coursecode" />
                            <Table.Column title="Thời lượng học/ tiếng" dataIndex="hours" key="hours" />
                            <Table.Column title="Số buổi" dataIndex="days" key="days" />
                            <Table.Column title="Hình thức học" dataIndex="module_type" key="module_type" />
                            <Table.Column title="Hình thức thi" dataIndex="exam_type" key="exam_type" />
                            <Table.Column title="Điểm tối đa thi lý thuyết" dataIndex="max_theory_grade" key="max_theory_grade" />
                            <Table.Column title="Điểm tối đa thi thực hành" dataIndex="max_practical_grade" key="max_practical_grade" />
                            <Table.Column title="Tên kỳ học ( Ấn Độ )" dataIndex="semester_name_portal" key="semester_name_portal" />
                            <Table.Column title="Tên kỳ thi ( Ấn Độ )" dataIndex="module_exam_name_portal" key="module_exam_name_portal" />
                            <Table.Column title="Học kỳ" dataIndex="semester" key="semester" />
                            <Table.Column
                                title="Hành động"
                                dataIndex="action"
                                key="action"
                                render={(_, data) => {
                                    return (
                                        <MdEdit className={classes.editIcon} onClick={() => {
                                            // setIsModalOpen(!isModalOpen);

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
            <Modal
                closeButton
                aria-labelledby="modal-title"
                open={isCreate===true}
                onClose={() => {
                    setIsCreate(!isCreate);

                }}
                blur
                width="1200px"
            >
                <Modal.Header>
                    <Text size={16} b>
                        Cập nhật thông tin môn học
                    </Text>
                </Modal.Header>
                <Modal.Body>
                   <ModuleCreate onCreateSuccess={handleAddSuccess}/>
   
                </Modal.Body>
            </Modal>
        </div>
    )
}
export default Module;