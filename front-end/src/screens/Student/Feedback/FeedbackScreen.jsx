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
         
      <Grid.Container gap={2}>
    
        <Grid sm={12}>
          <Card variant="bordered"
            css={{
              width: '100%',
              height: 'fit-content',
            }}
          >
     
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
                         
                            navigate(`/student/feedback/${item.id}`);

                        }}/>
                    </Table.Cell>
                   
                  </Table.Row>
             ))}
              </Table.Body>
             
            </Table>
         
          </Card>
        </Grid>
        
      </Grid.Container>
    </div>
  );
};

export default FeedbackScreen;
