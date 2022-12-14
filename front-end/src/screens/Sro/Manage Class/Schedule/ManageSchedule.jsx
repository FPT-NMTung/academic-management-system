import { Badge, Card, Grid, Spacer, Text } from '@nextui-org/react';
import { Descriptions, Skeleton, Timeline } from 'antd';
import { useState } from 'react';
import { useEffect } from 'react';
import { NavLink, Outlet, useNavigate, useParams } from 'react-router-dom';
import FetchApi from '../../../../apis/FetchApi';
import { ManageClassApis } from '../../../../apis/ListApi';
import classes from './ManageSchedule.module.css';

const renderStatus = (id) => {
  if (id === 1) {
    return (
      <Badge variant="flat" color="secondary">
        Đã lên lịch
      </Badge>
    );
  } else if (id === 2) {
    return (
      <Badge variant="flat" color="warning">
        Đang học
      </Badge>
    );
  } else if (id === 3) {
    return (
      <Badge variant="flat" color="success">
        Đã hoàn thành
      </Badge>
    );
  } else if (id === 4) {
    return (
      <Badge variant="flat" color="default">
        Hủy
      </Badge>
    );
  }
  else if (id === 6) {
    return (
      <Badge variant="flat" color="success">
        Đã ghép
      </Badge>
    );
  } else {
    return (
      <Badge variant="flat" color="default">
        Chưa lên lịch
      </Badge>
    );
  }
};

const ManageSchedule = () => {
  const [dataClass, setDataClass] = useState(undefined);
  const [listModule, setListModule] = useState(undefined);

  const { id, moduleId } = useParams();
  const navigate = useNavigate();

  const getInformationClass = () => {
    FetchApi(ManageClassApis.getInformationClass, null, null, [String(id)])
      .then((res) => {
        setDataClass(res.data);
        getListModule(res.data.class_status.id);
      })
      .catch((err) => {
        navigate('/404');
      });
  };

  const getListModule = (status) => {
    FetchApi(ManageClassApis.getAllModulesOfClass, null, null, [String(id)])
      .then((res) => {
        const listHasSchedule = res.data
          .filter((item) => {
            return item.status === true;
          })
          .sort((a, b) => {
            return (
              new Date(a.schedule_start_time) - new Date(b.schedule_start_time)
            );
          });
        const listNotSchedule = res.data
          .filter((item) => {
            return item.status === false;
          })
          .sort((a, b) => {
            return (
              new Date(a.module.created_at) - new Date(b.module.created_at)
            );
          });

        if (status === 6) {
          setListModule([...listHasSchedule]);
        } else {
          setListModule([...listHasSchedule, ...listNotSchedule]);
        }
      })
      .catch((err) => {
        navigate('/404');
      });
  };

  const handleUpdateListCourse = () => {
    getListModule(dataClass.class_status.id);
  };

  useEffect(() => {
    getInformationClass();
  }, []);

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
          <Card.Body>
            {!dataClass && <Skeleton />}
            {dataClass && (
              <Descriptions layout="horizontal" column={{ lg: 1 }}>
                <Descriptions.Item label="Tên lớp">
                  <b>{dataClass?.name}</b>
                </Descriptions.Item>
                <Descriptions.Item label="Người quản lý (SRO)">
                  <b>
                    {dataClass?.sro_first_name} {dataClass?.sro_last_name}
                  </b>
                </Descriptions.Item>
                <Descriptions.Item label="Ngày tạo">
                  <b>
                    {new Date(dataClass?.created_at).toLocaleDateString(
                      'vi-VN'
                    )}
                  </b>
                </Descriptions.Item>
                <Descriptions.Item label="Tình trạng lớp">
                  <b>{renderStatus(dataClass?.class_status?.id)}</b>
                </Descriptions.Item>
              </Descriptions>
            )}
          </Card.Body>
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
                {listModule?.map((item, index) => {
                  return (
                    <Timeline.Item
                      color={item.status ? 'green' : 'lightGray'}
                      key={index}
                    >
                      <NavLink
                        replace={true}
                        to={`module/${String(item.module.id)}`}
                      >
                        <Text
                          p
                          b={moduleId === String(item.module.id)}
                          color={item.status ? 'green' : 'black'}
                          size={14}
                        >
                          {item.module.module_name}
                        </Text>
                      </NavLink>
                    </Timeline.Item>
                  );
                })}
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
        <Outlet context={[handleUpdateListCourse, dataClass]} />
      </Grid>
    </Grid.Container>
  );
};

export default ManageSchedule;
