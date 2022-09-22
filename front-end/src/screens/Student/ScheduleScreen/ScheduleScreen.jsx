import classes from './ScheduleScreen.module.css';
import CalendarStudent from '../../../components/CalendarStudent/CalendarStudent';
import { Card, Grid, Text, Badge, Spacer } from '@nextui-org/react';
import { Divider } from 'antd';
import TimelineStudent from '../../../components/TimelineStudent/TimelineStudent';

const Schedule = () => {
  return (
    <div className={classes.main}>
      <Grid.Container gap={2}>
        <Grid xs={12} md={1}></Grid>
        <Grid xs={12} md={4}>
          <Card
            css={{
              width: '100%',
              height: 'fit-content',
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
                      <Text css={{ ml: '$2' }}>Ngày đang chọn</Text>
                    </Grid>
                    <Grid xs={12} alignItems="center">
                      <Badge color="success" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: '$2' }}>Ngày có lịch học</Text>
                    </Grid>
                    <Grid xs={12} alignItems="center">
                      <Badge color="warning" variant="default" />
                      <Spacer x={0.5} />
                      <Text css={{ ml: '$2' }}>Ngày có lịch thi</Text>
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
          </Card>
        </Grid>
      </Grid.Container>
    </div>
  );
};

export default Schedule;
