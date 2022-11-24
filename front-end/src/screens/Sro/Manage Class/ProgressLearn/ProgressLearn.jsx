import { Button, Card, Grid, Text } from '@nextui-org/react';
import { useNavigate } from 'react-router-dom';
import Timeline from 'react-calendar-timeline';
import 'react-calendar-timeline/lib/Timeline.css';
import moment from 'moment';

const ProgressLearn = () => {
  const navigate = useNavigate();

  return (
    <Card>
      <Card.Header>
        <Grid.Container>
          <Grid xs={4}>
            <Button
              size={'sm'}
              auto
              flat
              color={'error'}
              onPress={() => {
                navigate(-1);
              }}
            >
              Trở về
            </Button>
          </Grid>
          <Grid xs={4} justify={'center'}>
            <Text p b size={14}>
              Tiến độ học của tất cả các lớp
            </Text>
          </Grid>
          <Grid xs={4}></Grid>
        </Grid.Container>
      </Card.Header>
      <Card.Body>
        <Timeline
          groups={[
            {
              id: 1,
              title: 'group 1',
            },
            {
              id: 2,
              title: 'group 2',
            },
          ]}
          items={[
            {
              id: 1,
              group: 1,
              title: 'item 1',
              start_time: moment(),
              end_time: moment().add(1, 'hour'),
            },
            {
              id: 2,
              group: 2,
              title: 'item 2',
              start_time: moment('2022-10-10'),
              end_time: moment('2022-12-12'),
            },
            {
              id: 4,
              group: 2,
              title: 'item 2',
              start_time: moment('2022-12-14'),
              end_time: moment('2023-02-12'),
            },
            {
              id: 3,
              group: 1,
              title: 'item 3',
              start_time: moment().add(2, 'hour'),
              end_time: moment().add(3, 'hour'),
            },
          ]}
          defaultTimeStart={moment().add(-6, 'month')}
          defaultTimeEnd={moment().add(6, 'month')}
          minZoom={30 * 24 * 60 * 60 * 1000}
          maxZoom={3 * 365 * 24 * 60 * 60 * 1000}
        />
      </Card.Body>
    </Card>
  );
};

export default ProgressLearn;
