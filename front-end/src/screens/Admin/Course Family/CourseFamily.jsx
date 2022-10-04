import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
// import { AddressApis, CenterApis } from '../../../apis/ListApi';
import classes from './CourseFamily.module.css';
import { MdEdit } from 'react-icons/md';
import CouseFamilyCreate from '../../../components/CourseFamilyCreate/CourseFamilyCreate';


const CourseFamily = () => {
    const [listCourseFamily, setlistCourseFamily] = useState([]);
    const [selectedCourseFamilyId, setselectedCourseFamilyId] = useState(null);
    const [isCreate, setIsCreate] = useState(false);
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
                year : '2021'
            },
            {
                key: 2,
                name: 'ACSS',
                code: 'CD-7023-CPISM',
                year : '2019'
            },
            {
                key: 3,
                name: 'BSSS',
                code: 'AS-7023-CPISM',
                year : '2022'
            },
            {
                key: 4,
                name: 'ASSSS',
                code: 'SS-7023-CPISM',
                year : '2024'
            }

        ]
        // const data = mergeCourseFamily;
       const mergeCourseFamilyres = mergeCourseFamily.map((e) => {
            return {
                key: e.key,
              
                namecoursefamily: e.name,
                codefamily: e.code,
                codefamiyyear : e.year
            };
        });
         setlistCourseFamily (mergeCourseFamilyres);
     
      });
   
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
                            pagination={{ position: ['bottomCenter'] }}
                            dataSource={listCourseFamily}
                        >
                            <Table.Column title="Tên" dataIndex="namecoursefamily" key="name" />
                            <Table.Column title="Mã chương trình học" dataIndex="codefamily" key="address" />
                            <Table.Column title="Năm áp dụng" dataIndex="codefamiyyear" key="year" />
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
          <CouseFamilyCreate/>
        )}
            </Grid.Container>
        </div>
    )
}
export default CourseFamily;