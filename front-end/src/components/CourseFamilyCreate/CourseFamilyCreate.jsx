import { Card, Grid, Text } from '@nextui-org/react';
import { Form, Select, Input, Button, Radio } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import { CourseFamilyApis} from '../../apis/ListApi';
import { Validater } from '../../validater/Validater';

const CouseFamilyCreate = ({onCreateSuccess}) => {


    const [isCreating, setIsCreating] = useState(false);
    const [isFailed, setIsFailed] = useState(false);
    const [form] = Form.useForm();

    const handleSubmitForm = (e) => {
        setIsCreating(true);
        FetchApi(
            CourseFamilyApis.createCourseFamily ,
          {
            name: e.coursefamilyname,
            code: e.coursefamilycode,
            published_year: e.year,
            is_active: true,
            
          },
          null,
          null
        )
          .then(() => {
            onCreateSuccess();
          })
          .catch(() => {
            setIsCreating(false);
            setIsFailed(true);
          });
      };
    return (
        <Grid xs={4}>
            <Card
                css={{
                    width: '100%',
                    height: 'fit-content',
                }}
            >
                <Card.Header>
                    <Text size={14
                    }
                
                    >
                        Tạo thêm chương trình học mới: <b></b>
                    </Text>
                </Card.Header>
                <Card.Divider />
                <Card.Body>
                    <Form
                        
                        // labelCol={{ span: 6 }}
                        // wrapperCol={{ span: 14 }}
                        layout="horizontal"                      
                        labelCol={{ flex: '110px' , span: 6 }}                   
                        labelAlign="left"
                        labelWrap
                       
                      onFinish={handleSubmitForm}
                      form={form}
                    >
                        <Form.Item 
                           
                            name={'coursefamilyname'}
                            label={'Tên'}
                            
                            rules={[
                                {
                                    required: true,
                                    message: 'Hãy nhập tên chương trình học',
                                },
                            ]}
                        >
                            <Input />
                        </Form.Item>
                        <Form.Item
                            name={'coursefamilycode'}
                            label={'Mã chương trình'}
                            rules={[
                                {
                                    required: true,
                                    message: 'Hãy điền mã chương trình',
                                },
                            ]}
                        >

                            <Input />
                        </Form.Item>
                        <Form.Item
                            name={'year'}
                            label={'Năm áp dụng'}
                            rules={[
                                {
                                  required: true,
                                  validator: (_, value) => {
                                    // check regex phone number viet nam
                                    if (Validater.isNumber(value)) {
                                      return Promise.resolve();
                                    }
                                    return Promise.reject(
                                      new Error('Số năm không hợp lệ')
                                    );
                                  },
                                },
                              ]}
                        >

                            <Input />
                        </Form.Item>

                        <Form.Item wrapperCol={{ offset: 6, span: 10 }}>
                            <Button 
                             type="primary" htmlType="submit" loading={isCreating}>
                                Tạo mới
                            </Button>

                          
                        </Form.Item>
                        
                    </Form>
                    {!isCreating && isFailed && (
                        <Text
                            size={14}
                            css={{
                                color: 'red',
                                textAlign: 'center',
                            }}
                        >
                            Tạo khóa học thất bại, kiểm tra lại thông tin và thử lại
                        </Text>
                    )}
                </Card.Body>
            </Card>
        </Grid>


    )
}
export default CouseFamilyCreate;