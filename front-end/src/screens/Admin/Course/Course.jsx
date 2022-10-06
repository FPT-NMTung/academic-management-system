import classes from './Course.module.css';
import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { CourseApis } from '../../../apis/ListApi';
import CourseCreate from '../../../components/CourseCreate/CourseCreate';
import { MdEdit } from 'react-icons/md';
const Course = () => {
    const [listCourse, setlistCourse] = useState([]);
    const [selectedCourseCode, setselectedCourseCode] = useState(null);
    const [isCreate, setIsCreate] = useState(false);
    const [IsLoading, setIsLoading] = useState(false);

    
    const getData = () => {
        setIsLoading(true);
        const apiCourse = CourseApis.getAllCourse;
        console.log(apiCourse);
        FetchApi(apiCourse).then((res) => {
            const data = res;
            console.log(data);
            data.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
            const mergeAllCourse = data.map((e, index) => {
                return {

                    key: index,
                    coursename: `${e.name}`,
                    codecoursefamily: `${e.course_family_code}`,
                    codecourse: `${e.code}`,
                    semester: `${e.semester_count}`,
                    activatecourse: e.is_active,
                    ...e,

                };

            });

            setlistCourse(mergeAllCourse);
            setIsLoading(false);
        });
       
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
                        >
                            <Table.Column title="Tên Khóa Học" 
                             sorter={(a, b) => a.coursename - b.coursename}
                            dataIndex="coursename" key="coursename" />
                            <Table.Column title="Mã chương trình học" dataIndex="codecoursefamily" key="codecoursefamily" />
                            <Table.Column title="Mã Khóa Học" dataIndex="codecourse" key="codecourse" />
                            <Table.Column title="Học kỳ" dataIndex="semester" key="semester" />
                            <Table.Column
                                title=""
                                dataIndex="action"
                                key="action"
                                render={(_, data) => {
                                    return (
                                        <MdEdit className={classes.editIcon} onClick={() => { }} />
                                    );
                                }}
                            />
                        </Table>
                    </Card>
                </Grid>
                {isCreate && (
                    <CourseCreate />
                )}
            </Grid.Container>
        </div>
    )
}
export default Course;