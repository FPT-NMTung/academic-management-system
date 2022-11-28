import classes from "./ScheduleScreen.module.css";
import CalendarStudent from "../../../components/CalendarStudent/CalendarStudent";
import { Card, Grid, Text, Badge, Spacer, Button,Loading } from "@nextui-org/react";
import { Divider } from "antd";
import TimelineStudent from "../../../components/TimelineStudent/TimelineStudent";
import { useNavigate, useParams } from "react-router-dom";
import { MdNoteAlt } from "react-icons/md";
import { ManageGpa } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
import { useEffect, useState } from "react";
import { Fragment } from "react";


const Schedule = () => {
  const navigate = useNavigate();
  const [listForm, setListForm] = useState([]);
  const [isLoading, setisLoading] = useState(true);
  const getForm = () => {
    setisLoading(true);
    FetchApi(ManageGpa.getForm)
      .then((res) => {
        setListForm(res.data);
        setisLoading(false);
      })
      .catch((err) => {
        navigate("/404");
      });
  };

useEffect(() => {
    getForm();
  

  }, []);

  return (
<Fragment>
      {isLoading ? (
        <div className={classes.loading}>
        <Loading />
        </div>
      ) : (
        <Grid.Container gap={2}>
        <Grid xs={12} md={1}></Grid>
        <Grid xs={12} md={4}>
          <Card
            css={{
              width: "100%",
              height: "fit-content",
            }}
          >
            <Card.Body>
              <div className={classes.calendar}>
                <CalendarStudent />
                <Divider>
                  <Text p={true}>Chú thích</Text>
                </Divider>
                <div className={classes.content}>
                  <Grid.Container gap={0.5}>
                    <Grid xs={12} alignItems="center">
                      <Badge color="primary" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày đang chọn</Text>
                    </Grid>
                    <Grid xs={12} alignItems="center">
                      <Badge color="success" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày có lịch học</Text>
                    </Grid>
                    <Grid xs={12} alignItems="center">
                      <Badge color="warning" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: "$2" }}>Ngày có lịch thi</Text>
                    </Grid>
                  </Grid.Container>
                </div>
              </div>
            </Card.Body>
          </Card>
        </Grid>
        <Grid xs={12} md={6}>
          <Card>
            <Card.Body>
              <div>
                <TimelineStudent />
              </div>
            </Card.Body>
            {listForm.map((item, index) => (

          <Button
            flat
            auto
            icon={<MdNoteAlt size={20} />}
            color={"error"}
            css={{ width: "100px" , position:"absolute",right:"12px",bottom:"18px"}}
            onPress={() => {
              navigate(`/student/feedback/${item.id}`);
            }}
          >
            Phản hồi về giảng viên
          </Button>
            ))}
          </Card>

        </Grid>
      </Grid.Container>
      )}
        
        </Fragment>

  );
};

export default Schedule;
