import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { CourseFamilyApis } from '../../../apis/ListApi';
import classes from './CourseFamily.module.css';
import { MdEdit } from 'react-icons/md';
import CouseFamilyCreate from '../../../components/CourseFamilyCreate/CourseFamilyCreate';


const CourseFamily = () => {
    const [listCourseFamily, setlistCourseFamily] = useState([]);
    const [selectedCourseFamilyId, setselectedCourseFamilyId] = useState(null);
    const [IsLoading, setIsLoading] = useState(false);
    const [isCreate, setIsCreate] = useState(false);

    
    const getData = () => {
        setIsLoading(true);
        const apiCourseFamily = CourseFamilyApis.getAllCourseFamily;
        console.log(apiCourseFamily);
        FetchApi(apiCourseFamily).then((res) => {
            const data = res.data;
            
            data.sort((a,b) => new Date(a.created_at).getDate - new Date(b.created_at).getDate);
          
            const mergeAllCourseFamily = data.map((e, index) => {
                return {
                    
                    key: index,
                    namecoursefamily: `${e.name}`,
                    codefamily: `${e.code}`,
                    codefamiyyear: `${e.published_year}`,
                   
                    activatecourse : e.is_active, 
                 
                };
            
            });
           
            setlistCourseFamily(mergeAllCourseFamily);
        });
    
    }
    const handleSuccess = () => {
        setIsCreate(false);
        getData();
    }
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
                            rowClassName = {  (record, index) => {
                                if (record.activatecourse === false) {
                                    return record.classes = classes.rowDisable;
                                }

                            }}

                            
                            sortDirections={['descend']}
                            pagination={{ position: ['bottomCenter'] }}
                            dataSource={listCourseFamily} 
                           
                      
                            // rowClassName={record => activatecourse === 'false' && "disabled-row"}
                        >
                            <Table.Column  title="Tên" dataIndex="namecoursefamily" key="name" />
                            <Table.Column  title="Mã chương trình học" dataIndex="codefamily" key="address" />
                            <Table.Column sorter = { (a,b) => a.codefamiyyear - b.codefamiyyear}  title="Năm áp dụng" dataIndex="codefamiyyear" key="year" />
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
                    <CouseFamilyCreate onCreateSuccess={handleSuccess} />
                )}
            </Grid.Container>
        </div>
    )
}
export default CourseFamily;