import { Card, Grid, Text, Modal } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { CourseFamilyApis } from '../../../apis/ListApi';
import classes from './CourseFamily.module.css';
import { MdEdit } from 'react-icons/md';
import CouseFamilyCreate from '../../../components/CourseFamilyCreate/CourseFamilyCreate';
import CourseFamilyUpdate from '../../../components/CourseFamilyUpdate/CourseFamilyUpdate';



const CourseFamily = () => {
    const [listCourseFamily, setlistCourseFamily] = useState([]);
    const [selectedCourseFamilyCode, setselectedCourseFamilyCode] = useState(null);

    const [IsLoading, setIsLoading] = useState(true);
    const [isCreate, setIsCreate] = useState(false);
    const [messageFailed, setMessageFailed] = useState(undefined);
    


    const getData = () => {
        setIsLoading(true);
        const apiCourseFamily = CourseFamilyApis.getAllCourseFamily;
        // console.log(apiCourseFamily);
        FetchApi(apiCourseFamily).then((res) => {
            const data = res.data;
           
            data.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));

            const mergeAllCourseFamily = data.map((e, index) => {
                return {

                    key: index,
                    namecoursefamily: `${e.name}`,
                    codefamily: `${e.code}`,
                    codefamilyyear: `${e.published_year}`,
                    activatecourse: e.is_active,

                };

            });
            setIsLoading(false);
            setlistCourseFamily(mergeAllCourseFamily);
        });
       
    }
    const handleAddSuccess = () => {
        setIsCreate(false);
        getData();
    }
    const handleUpdateSuccess = () => {
        setselectedCourseFamilyCode(null);
       
        getData();
    };
    useEffect(() => {

        getData();
       
    }, []);

    

    return (
    
        <div>
            <Grid.Container gap={2} justify="center">
                <Grid xs={5}>
                    <Card
                        css={{
                            width: '100%',
                            height: 'fit-content',
                        }}
                    >
                        <Card.Header>
                            <div className={classes.headerTable}>
                                <Text size={14}>Danh sách chương trình học</Text>
                                <Button
                                    type="primary"
                                    onClick={() => {
                                        setIsCreate(!isCreate);
                                    }}
                                >
                                    Thêm chương trình học
                                </Button>
                            </div>
                        </Card.Header>
                        <Card.Divider />
                        <Table
                            loading={IsLoading}
                       
                            rowClassName={(record, index) => {
                                if (record.activatecourse === false) {
                                    return record.classes = classes.rowDisable;
                                }

                            }}

                         
                            sortDirections={['descend', 'ascend']}
                            pagination={{ position: ['bottomCenter'] }}
                            dataSource={listCourseFamily}


                        // rowClassName={record => activatecourse === 'false' && "disabled-row"}
                        >
                            <Table.Column 
                                title="Tên" dataIndex="namecoursefamily" key="name" />
                            <Table.Column 
                                title="Mã chương trình học" dataIndex="codefamily" key="address" />
                            <Table.Column 
                                sorter={(a, b) => a.codefamilyyear - b.codefamilyyear}
                                title="Năm áp dụng"
                               
                                dataIndex="codefamilyyear"
                                key="year" />
                            <Table.Column
                                title=""
                                dataIndex="action"
                                key="action"
                                render={(_, data) => {
                                    return (
                                        <MdEdit
                                            className={classes.editIcon}
                                            onClick={() => {
                                                setselectedCourseFamilyCode(data.codefamily);
                                                
                                                
                                            }} />
                                    );
                                }}
                            />
                        </Table>
                    </Card>
                </Grid>
                {isCreate && (
                    <CouseFamilyCreate onCreateSuccess={handleAddSuccess} />
                )}
            </Grid.Container>
            <Modal
                closeButton
                aria-labelledby="modal-title"
                open={selectedCourseFamilyCode !== null}
                onClose={() => {
                    setselectedCourseFamilyCode(null);
                   
                }}
            blur
            width="500px"
            >
            <Modal.Header>
                <Text size={16} b>
                    Cập nhật thông tin chương trình học
                </Text>
            </Modal.Header>
            <Modal.Body>
                <CourseFamilyUpdate data={listCourseFamily.find((e) => e.codefamily === `${selectedCourseFamilyCode}`)} onUpdateSuccess={handleUpdateSuccess} />
            </Modal.Body>
        </Modal>
        </div >
    )
}
export default CourseFamily;