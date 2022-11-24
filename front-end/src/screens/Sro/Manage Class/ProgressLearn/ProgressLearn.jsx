import { Button, Card, Grid, Text } from '@nextui-org/react';
import { useNavigate } from 'react-router-dom';
import Timeline from 'react-calendar-timeline';
import 'react-calendar-timeline/lib/Timeline.css';
import moment from 'moment';
import { useState } from 'react';
import { useEffect } from 'react';
import FetchApi from '../../../../apis/FetchApi';
import { ManageScheduleApis } from '../../../../apis/ListApi';
import toast from 'react-hot-toast';

const ProgressLearn = () => {
  const [gettingData, setGettingData] = useState(false);
  const [groups, setGroups] = useState([]);
  const [schedule, setSchedule] = useState([]);

  const navigate = useNavigate();

  const getProgressDate = () => {
    setGettingData(true);

    FetchApi(ManageScheduleApis.progress, null, null, null)
      .then((res) => {
        const data = res.data;
        const temp = data.sort((a, b) => {
          return new Date(a.class.created_at) - new Date(b.class.created_at);
        }).map((item) => {
          return {
            id: item.class.id,
            title: item.class.name,
          };
        });
        const sc = data.map((item) => {
          return item.schedule.map((s) => {
            return {
              id: s.id,
              group: item.class.id,
              title: s.module_name,
              start_time: moment(s.start_date),
              end_time: moment(s.end_date),
            };
          });
        }).flat()
        setSchedule(sc);
        setGroups(temp);
        setGettingData(false);
      })
      .catch((err) => {
        toast.error('Lỗi tải dữ liệu');
      });
  };

  useEffect(() => {
    getProgressDate();
  }, []);

  return (
    <Card variant='bordered'>
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
        {!gettingData && (
          <Timeline
            groups={groups}
            items={schedule}
            defaultTimeStart={moment().add(-6, 'month')}
            defaultTimeEnd={moment().add(6, 'month')}
            minZoom={30 * 24 * 60 * 60 * 1000}
            maxZoom={3 * 365 * 24 * 60 * 60 * 1000}
            canMove={false}
            canResize={false}
          />
        )}
      </Card.Body>
    </Card>
  );
};

export default ProgressLearn;
