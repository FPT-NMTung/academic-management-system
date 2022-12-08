import {
  Badge,
  Button,
  Card,
  Divider,
  Grid,
  Spacer,
  Text,
} from '@nextui-org/react';
import { Descriptions, Timeline } from 'antd';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import FetchApi from '../../../apis/FetchApi';
import { ManageScheduleApis } from '../../../apis/ListApi';

const WarningSchedule = () => {
  const [dataTeacher, setDataTeacher] = useState([]);
  const [dataRoom, setDataRoom] = useState([]);

  const navigate = useNavigate();

  useEffect(() => {
    FetchApi(ManageScheduleApis.getDuliicateScheduleTeacher, null, null, null)
      .then((res) => {
        setDataTeacher(res.data);
      })
      .catch((err) => {
        console.log(err);
      });
    FetchApi(ManageScheduleApis.getDuliicateScheduleRoom, null, null, null)
      .then((res) => {
        setDataRoom(res.data);
      })
      .catch((err) => {
        console.log(err);
      });
  }, []);

  const renderWorkingTime = (id) => {
    switch (id) {
      case 1:
        return (
          <Badge variant={'flat'} color={'success'}>
            Sáng
          </Badge>
        );
      case 2:
        return (
          <Badge variant={'flat'} color={'warning'}>
            Chiều
          </Badge>
        );
      case 3:
        return (
          <Badge variant={'flat'} color={'secondary'}>
            Tối
          </Badge>
        );
    }
  };

  const renderRoomType = (id) => {
    switch (id) {
      case 1:
        return (
          <Badge variant={'flat'} color={'success'}>
            Lý thuyết
          </Badge>
        );
      case 2:
        return (
          <Badge variant={'flat'} color={'warning'}>
            Thực hành
          </Badge>
        );
    }
  };

  return (
    <Grid.Container gap={2}>
      <Grid xs={1}>
        <Button
          auto
          onPress={() => {
            navigate(-1);
          }}
          flat
          size={'sm'}
          color={'error'}
        >
          Quay lại
        </Button>
      </Grid>
      <Grid xs={5}>
        <Card
          variant="bordered"
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={14}
              css={{
                textAlign: 'center',
                width: '100%',
              }}
            >
              Trùng giáo viên
            </Text>
          </Card.Header>
          <Card.Body>
            <div
              style={{
                display: 'flex',
                flexDirection: 'column',
                gap: '10px',
              }}
            >
              {dataTeacher?.map((item) => {
                return (
                  <Card variant="bordered">
                    <Card.Body
                      css={{
                        padding: '10px 16px',
                      }}
                    >
                      <Descriptions column={1}>
                        <Descriptions.Item label="Giáo viên bị trùng">
                          <Text p size={14}>
                            <b>
                              {item.teacher.first_name +
                                ' ' +
                                item.teacher.last_name}
                            </b>{' '}
                            ({item.teacher.email_organization})
                          </Text>
                        </Descriptions.Item>
                        <Descriptions.Item label="Ngày học">
                          <b>
                            {new Date(item.learning_date).toLocaleDateString(
                              'vi-VN'
                            )}{' '}
                            {renderWorkingTime(item.working_time_id)}
                          </b>
                        </Descriptions.Item>
                      </Descriptions>
                      <Divider />
                      <Spacer y={1} />
                      <Timeline>
                        {item.sessions.map((item) => {
                          return (
                            <Timeline.Item>
                              <Descriptions column={1}>
                                <Descriptions.Item label="Slot">
                                  <b>{item.title}</b>
                                </Descriptions.Item>
                                <Descriptions.Item label="Môn học">
                                  <b>{item.module.name}</b>
                                </Descriptions.Item>
                                <Descriptions.Item label="Lớp học">
                                  <b>{item.class.name}</b>
                                </Descriptions.Item>
                              </Descriptions>
                            </Timeline.Item>
                          );
                        })}
                      </Timeline>
                    </Card.Body>
                  </Card>
                );
              })}
            </div>
          </Card.Body>
        </Card>
      </Grid>
      <Grid xs={5}>
        <Card
          variant="bordered"
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={14}
              css={{
                textAlign: 'center',
                width: '100%',
              }}
            >
              Trùng phòng học
            </Text>
          </Card.Header>
          <Card.Body>
            <div
              style={{
                display: 'flex',
                flexDirection: 'column',
                gap: '10px',
              }}
            >
              {dataRoom?.map((item) => {
                return (
                  <Card variant="bordered">
                    <Card.Body
                      css={{
                        padding: '10px 16px',
                      }}
                    >
                      <Descriptions column={1}>
                        <Descriptions.Item label="Phòng bị trùng">
                          <Text p size={14}>
                            <b>
                              {item.room.name}{' '}
                              {renderRoomType(item.room.room_type.id)}
                            </b>
                          </Text>
                        </Descriptions.Item>
                        <Descriptions.Item label="Ngày học">
                          <b>
                            {new Date(item.learning_date).toLocaleDateString(
                              'vi-VN'
                            )}{' '}
                            {renderWorkingTime(item.working_time_id)}
                          </b>
                        </Descriptions.Item>
                      </Descriptions>
                      <Divider />
                      <Spacer y={1} />
                      <Timeline>
                        {item.sessions.map((item) => {
                          return (
                            <Timeline.Item>
                              <Descriptions column={1}>
                                <Descriptions.Item label="Slot">
                                  <b>{item.title}</b>
                                </Descriptions.Item>
                                <Descriptions.Item label="Môn học">
                                  <b>{item.module.name}</b>
                                </Descriptions.Item>
                                <Descriptions.Item label="Lớp học">
                                  <b>{item.class.name}</b>
                                </Descriptions.Item>
                              </Descriptions>
                            </Timeline.Item>
                          );
                        })}
                      </Timeline>
                    </Card.Body>
                  </Card>
                );
              })}
            </div>
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default WarningSchedule;
