import classes from '../ScheduleScreen/ScheduleScreen.module.css';
import { Radio,Card, Grid, Text, Badge, Spacer,Table,Modal,Button,Loading } from '@nextui-org/react';
import { Divider } from 'antd';
import {MdNoteAlt} from 'react-icons/md'
import toast from "react-hot-toast";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Descriptions, Skeleton, Form, Select, Upload,Input } from "antd";
import { ManageGpa } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";


const FeedbackScreen = () => {
    const [isOpenModal, setIsOpenModal] = useState(false);
    const [form] = Form.useForm();
    const [listForm, setListForm] = useState([]);
    const [listQuestion, setListQuestion] = useState([]);
    const [listAnswer, setListAnswer] = useState([]);
    const navigate = useNavigate();
    const getForm = () => {
        FetchApi(ManageGpa.getForm)
          .then((res) => {
            setListForm(res.data);
          })
          .catch((err) => {
            navigate("/404");
          });
      };

    useEffect(() => {
        getForm();
      
 
      }, []);
  return (
    <div className={classes.main}>
         <Modal
        open={isOpenModal}
        width="1000px"
        blur
        onClose={() => {
          setIsOpenModal(false);
        }}
      >
       <Modal.Header>
            <Text b size={16}>Ý kiến về việc giảng dậy</Text>
   
       </Modal.Header>
        <Modal.Body
          css={{
            padding: "20px",
          }}
         
        >
            <Form
       
          layout="horizontal"
          labelAlign="right"
          labelWrap


        >
            <Form.Item>

          <Radio.Group label="Options" defaultValue="0">
      <Radio value="1">Option 1</Radio>
      <Radio value="2" >
        Option 2
      </Radio>
      <Radio value="3">Option 3</Radio>
      <Radio value="4">
        Option 4
      </Radio>
    </Radio.Group>
            </Form.Item>
    </Form>
        </Modal.Body>
      </Modal>
      <Grid.Container gap={2}>
    
        <Grid sm={12}>
          <Card variant="bordered"
            css={{
              width: '100%',
              height: 'fit-content',
            }}
          >
            <Card.Body>
            <Table aria-label=""  css={{
            
                height:"fit-content"
      }}>
              <Table.Header>
                <Table.Column width={150}>Ý kiến về việc giảng dậy</Table.Column>
                <Table.Column width={300}>Mô tả</Table.Column>
                <Table.Column width={150}>Đánh giá</Table.Column>
             
              </Table.Header>
              <Table.Body css={{fixed:"true"}}>
             {listForm.map((item,index)=>(
                  <Table.Row key={index}>
                    <Table.Cell>{item.title}</Table.Cell>
                    <Table.Cell css={{maxWidth:"300px", wordWrap:"break-word"}}>{item.description}</Table.Cell>
                    <Table.Cell>
                        <MdNoteAlt
                        size={20}
                 
                        color="5EA2EF"
                        style={{ cursor: "pointer" }}
                        onClick={() => {
                            setIsOpenModal(true);
                            // getQuestion();
                            navigate(`/student/feedback/${item.id}`);

                        }}/>
                    </Table.Cell>
                   
                  </Table.Row>
             ))}
              </Table.Body>
             
            </Table>
            </Card.Body>
          </Card>
        </Grid>
        
      </Grid.Container>
    </div>
  );
};

export default FeedbackScreen;
