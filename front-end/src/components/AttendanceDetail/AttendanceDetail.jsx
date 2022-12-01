import { Card, Grid, Loading, Spacer, Text } from '@nextui-org/react';
import { useState } from 'react';
import { useEffect } from 'react';
import { useOutletContext, useParams } from 'react-router-dom';
import FetchApi from '../../apis/FetchApi';
import { ManageAttendanceApis } from '../../apis/ListApi';
import moment from 'moment';

import classes from './AttendanceDetail.module.css';
import TakeAttendance from '../../screens/Sro/Manage Class/Attendance/TakeAttendance/TakeAttendance';
import ViewAllAttendance from '../../screens/Sro/Manage Class/Attendance/ViewAllAttendance/ViewAllAttendance';
import { Fragment } from 'react';

const AttendanceDetail = () => {
  const [listSession, setListSession] = useState(undefined);
  const [loading, setLoading] = useState(true);
  const [selectSession, setSelectSession] = useState(undefined);

  const { id } = useParams();
  const [listData] = useOutletContext();

  useEffect(() => {
    setLoading(true);
    FetchApi(ManageAttendanceApis.teacherGetListSessions, null, null, [
      String(id),
    ])
      .then((res) => {
        setListSession(res.data);
        setLoading(false);
      })
      .catch((err) => {});
  }, [id]);

  const handleCloseModalAttendance = () => {
    setSelectSession(undefined);
  };

  const handleCloseViewAttendance = () => {
    // setViewAttendance(false);
  };

  return (
    <Fragment>
      {selectSession && (
        <TakeAttendance
          session={selectSession}
          scheduleId={id}
          onClose={handleCloseModalAttendance}
        />
      )}
      {/* {viewAttendance && (
        <ViewAllAttendance
          open={viewAttendance}
          scheduleId={dataSchedule.id}
          onClose={handleCloseViewAttendance}
        />
      )} */}
      <Card
        variant="bordered"
        css={{
          height: 'fit-content',
        }}
      >
        <Card.Header>
          <Text
            p
            size={14}
            css={{
              textAlign: 'center',
              width: '100%',
            }}
          >
            Danh sách slot dạy của lớp{' '}
            <b>
              {
                listData?.find(
                  (item) => String(item.schedule_id) === String(id)
                )?.class.name
              }
            </b>
          </Text>
        </Card.Header>
        <Card.Body>
          {loading && <Loading />}
          {!loading && (
            <Grid.Container gap={2}>
              {listSession
                ?.sort(
                  (a, b) =>
                    new Date(a.learning_date) - new Date(b.learning_date)
                )
                .map((data, index) => {
                  console.log(
                    moment(data.learning_date).add(1, 'days').toDate() -
                      moment().toDate()
                  );
                  return (
                    <Grid key={index} xs={2}>
                      <Card
                        variant="bordered"
                        isPressable={
                          moment(data.learning_date).add(1, 'days').toDate() -
                            moment().toDate() <
                          0
                        }
                        onPress={() => {
                          setSelectSession(data);
                        }}
                        css={{
                          height: 'fit-content',
                          border:
                            new Date(data.learning_date).toLocaleDateString(
                              'vi-VN'
                            ) ===
                              new Date(Date.now()).toLocaleDateString(
                                'vi-VN'
                              ) && '2px solid red',
                        }}
                      >
                        <Card.Body
                          css={{
                            padding: '6px 12px',
                            backgroundColor:
                              data.session_type === 3 || data.session_type === 4
                                ? '#7828C8'
                                : '',
                          }}
                        >
                          <div className={classes.infoSlotSession}>
                            <Text
                              p
                              size={12}
                              color={
                                data.session_type === 3 ||
                                data.session_type === 4
                                  ? '#fff'
                                  : '#000'
                              }
                            >
                              <b>Slot {index + 1}</b>
                            </Text>
                            <Text
                              p
                              size={12}
                              color={
                                data.session_type === 3 ||
                                data.session_type === 4
                                  ? '#fff'
                                  : '#000'
                              }
                            >
                              {new Date(data.learning_date).toLocaleDateString(
                                'vi-VN'
                              )}
                            </Text>
                          </div>
                          <Spacer y={0.6} />
                          <Text
                            p
                            size={12}
                            color={
                              data.session_type === 3 || data.session_type === 4
                                ? '#fff'
                                : '#000'
                            }
                          >
                            Phòng <b>{data.room.name}</b>
                          </Text>
                        </Card.Body>
                      </Card>
                    </Grid>
                  );
                })}
            </Grid.Container>
          )}
        </Card.Body>
      </Card>
    </Fragment>
  );
};

export default AttendanceDetail;
