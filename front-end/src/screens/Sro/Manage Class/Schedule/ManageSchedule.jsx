import { Card, Grid, Spacer, Text } from '@nextui-org/react';
import { Timeline } from 'antd';
import { Link, NavLink, Outlet } from 'react-router-dom';
import classes from './ManageSchedule.module.css';

const ManageSchedule = () => {
  return (
    <Grid.Container gap={2}>
      <Grid
        xs={3.5}
        css={{
          height: 'fit-content',
        }}
        direction="column"
      >
        <Card variant={'bordered'}>
          <Card.Header>
            <Text
              p
              b
              size={14}
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              Thông tin lớp học
            </Text>
          </Card.Header>
          <Card.Body></Card.Body>
        </Card>
        <Spacer y={1} />
        <Card variant={'bordered'}>
          <Card.Header>
            <Text
              p
              b
              size={14}
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              Danh sách môn học
            </Text>
          </Card.Header>
          <Card.Body>
            <div className={classes.moduleList}>
              <Timeline>
                <Timeline.Item color="green">
                  <NavLink to={'123'}>Môn học 1</NavLink>
                </Timeline.Item>
                <Timeline.Item color="green">
                  <NavLink to={'234'}>Môn học 2</NavLink>
                </Timeline.Item>
                <Timeline.Item color="green">
                  <NavLink to={'345'}>Môn học 3</NavLink>
                </Timeline.Item>
                <Timeline.Item color="green">
                  <NavLink to={'456'}>Môn học 4</NavLink>
                </Timeline.Item>
              </Timeline>
            </div>
          </Card.Body>
        </Card>
      </Grid>
      <Grid
        xs={8.5}
        css={{
          height: 'fit-content',
        }}
      >
        <Card variant={'bordered'}>
          <Card.Body>
            <Outlet />
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default ManageSchedule;
