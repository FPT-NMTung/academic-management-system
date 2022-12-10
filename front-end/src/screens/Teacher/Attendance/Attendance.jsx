import { Card, Text, Grid } from '@nextui-org/react';
import { useState } from 'react';
import { Fragment } from 'react';
import { useEffect } from 'react';
import { NavLink, Outlet } from 'react-router-dom';
import FetchApi from '../../../apis/FetchApi';
import { ManageAttendanceApis } from '../../../apis/ListApi';

const Attendance = () => {
  const [listData, setListData] = useState([]);

  useEffect(() => {
    FetchApi(ManageAttendanceApis.getTeachingClass, null, null, null)
      .then((res) => {
        setListData(res.data);
      })
      .catch((err) => {});
  }, []);

  return (
      <Grid.Container gap={2}>
        <Grid xs={3}>
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
                Danh sách lớp
              </Text>
            </Card.Header>
            <Card.Body>
              {listData.map((item) => {
                return (
                  <NavLink
                    to={`/teacher/class/${item.class.id}/module/${item.module.id}/schedule/${item.schedule_id}`}
                    css={{
                      textDecoration: 'none',
                      color: 'black',
                    }}
                  >
                    <Card
                      variant="bordered"
                      css={{
                        height: 'fit-content',
                        margin: '5px 0',
                      }}
                      isPressable
                    >
                      <Card.Body
                        css={{
                          padding: '10px 20px',
                        }}
                      >
                        <Text p size={14}>
                          Lớp: <b>{item.class.name}</b>
                        </Text>
                        <Text p size={14}>
                          Môn học: <b>{item.module.name}</b>
                        </Text>
                      </Card.Body>
                    </Card>
                  </NavLink>
                );
              })}
            </Card.Body>
          </Card>
        </Grid>
        <Grid xs={9}>
          <Outlet context={[listData]} />
        </Grid>
      </Grid.Container>
  );
};

export default Attendance;
