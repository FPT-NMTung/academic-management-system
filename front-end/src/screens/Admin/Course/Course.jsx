import classes from './Course.module.css';
import { Card, Grid, Text, Modal } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { CourseApis } from '../../../apis/ListApi';
import CourseCreate from '../../../components/CourseCreate/CourseCreate';
import CourseUpdate from '../../../components/CourseUpdate/CourseUpdate';
import { MdEdit } from 'react-icons/md';
const Course = () => {
    const [listCourse, setlistCourse] = useState([]);
    const [selectedCourseCode, setselectedCourseCode] = useState(null);
    const [isCreate, setIsCreate] = useState(false);
    const [IsLoading, setIsLoading] = useState(false);


    const getData = () => {
        setIsLoading(true);
        const apiCourse = CourseApis.getAllCourse;
        // console.log(apiCourse);
        FetchApi(apiCourse).then((res) => {
            const data = res.data;
            // console.log(data);
            data.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
            const mergeAllCourse = data.map((e, index) => {
                return {

                    key: index,
                    coursename: `${e.name}`,
                    codecoursefamily: `${e.course_family_code}`,
                    codecourse: `${e.code}`,
                    semester_count: `${e.semester_count}`,
                    activatecourse: e.is_active,
                    ...e,

                };

            });

            setlistCourse(mergeAllCourse);
            setIsLoading(false);
        });

    }
    const sortCourseName = (a, b) => {
        if (a.coursename < b.coursename) {
            return -1;
        }
        if (a.coursename > b.coursename) {
            return 1;
        }
        return 0;
    }
    const handleAddSuccess = () => {
        setIsCreate(false);
        getData();
    }
    const handleUpdateSuccess = () => {
        setselectedCourseCode(null);

        getData();
    };
    useEffect(() => {

        getData();

    }, []);

    return (
        <div >
            <Grid.Container gap={2} justify="center">
                <Grid xs={6}>
                    <Card
                        css={{
                            width: '100%',
                            height: 'fit-content',
                        }}
                    >
                        <Card.Header>
                            <div className={classes.headerTable}>
                                <Text size={14}>Danh sách khóa học</Text>
                                <Button
                                    type="primary"
                                    onClick={() => {
                                        setIsCreate(!isCreate);
                                    }}
                                >
                                    Thêm khoá học
                                </Button>
                            </div>
                        </Card.Header>
                        <Card.Divider />
                        <Table
                            pagination={{ position: ['bottomCenter'] }}
                            dataSource={listCourse}
                            sortDirections={['descend', 'ascend']}
                            rowClassName={(record, index) => {
                                if (record.activatecourse === false) {
                                    return record.classes = classes.rowDisable;
                                }

                            }}
                        >
                            <Table.Column

                                title="Tên Khóa Học"
                                dataIndex="coursename"
                                key="name" 
                                sorter={sortCourseName}
                                />
                        
                            <Table.Column title="Mã chương trình học" dataIndex="codecoursefamily" key="codecoursefamily" />
                            <Table.Column title="Mã Khóa Học" dataIndex="codecourse" key="codecourse" />
                            <Table.Column title="Số lượng kỳ học" dataIndex="semester_count" key="semester_count" />
                            <Table.Column
                                title=""
                                dataIndex="action"
                                key="action"
                                render={(_, data) => {
                                    return (
                                        <MdEdit className={classes.editIcon}
                                            onClick={() => {
                                                setselectedCourseCode(data.codecourse);
                                            }} />
                                    );
                                }}
                            />
                        </Table>
                    </Card>
                </Grid>
                {isCreate && (
                    <CourseCreate onCreateSuccess={handleAddSuccess} />
                )}
            </Grid.Container>
            <Modal
                closeButton
                aria-labelledby="modal-title"
                open={selectedCourseCode !== null}
                onClose={() => {
                    setselectedCourseCode(null);
                }}
                blur

                width="700px"
            >
                <Modal.Header>
                    <Text size={16} b>
                        Cập nhật thông tin khóa học
                    </Text>
                </Modal.Header>
                <Modal.Body>
                    <CourseUpdate data={listCourse.find((e) => e.code === `${selectedCourseCode}`)} onUpdateSuccess={handleUpdateSuccess} />

                </Modal.Body>
            </Modal>
        </div>
    )
}
export default Course;