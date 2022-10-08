import classes from './Module.module.css';
import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table, Modal } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ModulesApis } from '../../../apis/ListApi';
import ModuleUpdate from '../../../components/ModuleUpdate/ModuleUpdate';
import ModuleCreate from '../../../components/ModuleCreate/ModuleCreate';
import { MdEdit } from 'react-icons/md';
import { Fragment } from 'react';
import ColumnGroup from 'antd/lib/table/ColumnGroup';
import { Navigate, useNavigate } from 'react-router-dom';
const Module = () => {
    const [listModules, setlistModules] = useState([]);
    const [selectedModuleId, setselectedModuleId] = useState(null);
    const [IsLoading, setIsLoading] = useState(false);
    const [isCreate, setIsCreate] = useState(false);
    const navigate = useNavigate();
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

            <Grid.Container gap={2}>
                <Grid sm={12}>
                    <Card>
                        <Card.Body>
                            <Form layout="inline"
                            >
                                <Form.Item
                                    name="modulename"
                                    style={{ width: 'calc(17% - 16px)' }}
                                >
                                    <Input placeholder="Môn học" />
                                </Form.Item>
                                <Form.Item name="course_code" style={{ width: 'calc(17% - 16px)' }}>
                                    <Input placeholder="Mã khóa học" />
                                </Form.Item>
                                <Form.Item
                                    name="module_type"
                                    style={{ width: 'calc(17% - 16px)' }}
                                >
                                    <Input placeholder="Hình thức học" />
                                </Form.Item>
                                <Form.Item name="exam_type" style={{ width: 'calc(17% - 16px)' }}>
                                    <Input placeholder="Hình thức thi" />
                                </Form.Item>
                             
                                <Form.Item style={{ width: 'calc(9% - 16px)' }}>
                                    <Button
                                        type="primary"
                                        htmlType="submit"
                                        style={{ width: '100%' }}
                                    >
                                        Tìm kiếm
                                    </Button>
                                </Form.Item>
                                <Form.Item style={{ width: '6%', marginRight: 0 }}>
                                    <Button
                                        type="default"
                                        style={{
                                            width: '100%',
                                        }}
                                    >
                                        Huỷ
                                    </Button>
                                </Form.Item>
                            </Form>
                        </Card.Body>
                    </Card>
                </Grid>
                <Grid sm={12}>
                    <Card>
                        <Card.Header>
                            <Grid.Container>
                                <Grid sm={1}></Grid>
                                <Grid sm={10}>
                                    <Text
                                        b
                                        size={14}
                                        p
                                        css={{
                                            width: '100%',
                                            textAlign: 'center',
                                        }}
                                    >
                                        Danh sách môn học
                                    </Text>
                                </Grid>
                                <Grid sm={1}>
                                    <Button
                                        type="primary"
                                        style={{
                                            width: '100%',
                                        }}
                                        onClick={() => {
                                            // navigate('/admin/account/sro/create');
                                        }}
                                    >
                                        + Tạo mới
                                    </Button>
                                </Grid>
                            </Grid.Container>
                        </Card.Header>
                        <Table
                            // loading={IsLoading}
                            bordered
                            size="middle"
                            dataSource={listModules}

                            // columns={designColumns}
                            scroll={{
                                x: 1500,
                            }}
                            pagination={{
                                size: 'default',
                                position: ['bottomCenter'],
                            }}
                        >
                            <Table.Column
                                title="STT"
                                dataIndex="key"
                                key="key"
                                width={50}
                                fixed={'left'}
                            />
                            <ColumnGroup title="Môn học">
                                <Table.Column
                                    title="Tên môn học"
                                    dataIndex="modulename"
                                    key="modulename"
                                    width={320}
                                    fixed={'left'}
                                />
                                <Table.Column
                                    title="Mã khóa học"
                                    dataIndex="coursecode"
                                    key="coursecode"
                                    width={200}
                                    fixed={'left'}
                                />
                            </ColumnGroup>
                            <ColumnGroup title="Thời lượng học">
                                <Table.Column
                                    title="Số Tiếng"
                                    dataIndex="hours"
                                    key="hours"
                                    width={80}
                                />
                                <Table.Column
                                    title="Số buổi"
                                    dataIndex="days"
                                    key="days"
                                    width={80}
                                />
                            </ColumnGroup>
                            <ColumnGroup title="Hình thức môn học">
                                <Table.Column
                                    title="Hình thức học"
                                    dataIndex="module_type"
                                    key="module_type"
                                    width={150}
                                />
                                <Table.Column
                                    title="Hình thức thi"
                                    dataIndex="exam_type"
                                    key="exam_type"
                                    width={150}
                                />
                            </ColumnGroup>
                            <ColumnGroup title="Điểm tối đa thi">
                                <Table.Column
                                    title="Lý thuyểt"
                                    dataIndex="max_theory_grade"
                                    key="max_theory_grade"
                                    width={90}
                                />
                                <Table.Column
                                    title="Thực hành"
                                    dataIndex="max_practical_grade"
                                    key="max_practical_grade"
                                    width={90}
                                />
                            </ColumnGroup>
                            <ColumnGroup title="Tên theo Ấn Độ">
                                <Table.Column
                                    title="Kỳ học"
                                    dataIndex="semester_name_portal"
                                    key="semester_name_portal"
                                    width={350}
                                />
                                <Table.Column
                                    title="Kỳ thi"
                                    dataIndex="module_exam_name_portal"
                                    key="module_exam_name_portal"
                                    width={350}
                                />
                            </ColumnGroup>
                            <Table.Column
                                title="Học kỳ"
                                dataIndex="semester"
                                key="semester"
                                width={90}
                            />
                               <Table.Column
                                title="Cơ sở"
                                dataIndex="centername"
                                key="centername"
                                width={90}
                            />
                    
                     
                     
                            <Table.Column
                                width={50}
                                title=""
                                dataIndex=""
                                key="action"
                                fixed={'right'}
                                render={(_, data) => {
                                    return (
                                        <div className={classes.logoEdit}>
                                            <MdEdit
                                                color="0a579f"
                                                style={{ cursor: 'pointer' }}
                                                onClick={() => {
                                                    // navigate(`/admin/account/sro/${data.user_id}`);
                                                }}
                                            />
                                        </div>
                                    );
                                }}
                            />
                        </Table>
                    </Card>
                </Grid>
            </Grid.Container>
            <Modal
                closeButton
                aria-labelledby="modal-title"
                // open={isCreate===true}
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
                    <ModuleCreate onCreateSuccess={handleAddSuccess} />

                </Modal.Body>
            </Modal>
        </div>
    )
}
export default Module;