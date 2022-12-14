import {
  Badge,
  Button,
  Card,
  Divider,
  Grid,
  Loading,
  Modal,
  Spacer,
  Text,
  Dropdown,
} from '@nextui-org/react';
import {
  DatePicker,
  Descriptions,
  Form,
  Input,
  InputNumber,
  Select,
  TimePicker,
} from 'antd';
import { useState } from 'react';
import { Fragment } from 'react';
import { useEffect } from 'react';
import toast from 'react-hot-toast';
import { useNavigate, useOutletContext, useParams } from 'react-router-dom';
import FetchApi from '../../../../apis/FetchApi';
import { ManageScheduleApis, ModulesApis } from '../../../../apis/ListApi';
import ScheduleCreate from '../../../../components/ScheduleCreate/ScheduleCreate';
import ScheduleUpdate from '../../../../components/ScheduleUpdate/ScheduleUpdate';
import TakeAttendance from '../Attendance/TakeAttendance/TakeAttendance';
import ViewAllAttendance from '../Attendance/ViewAllAttendance/ViewAllAttendance';
import ViewGrade from '../ViewGrade/ViewGrade';
import classes from './ScheduleDetail.module.css';

import { MdDelete } from 'react-icons/md';
import { BsFillPersonCheckFill } from 'react-icons/bs';
import { MdEditCalendar } from 'react-icons/md';
import { RiBookmark3Fill } from 'react-icons/ri';

const ScheduleDetail = () => {
  const [dataSchedule, setDataSchedule] = useState(undefined);
  const [gettingData, setGettingData] = useState(true);
  const [dataModule, setDataModule] = useState(undefined);
  const [createMode, setCreateMode] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [viewGrade, setViewGrade] = useState(false);
  const [viewAttendance, setViewAttendance] = useState(false);

  const [selectSession, setSelectSession] = useState(undefined);

  const { id, moduleId } = useParams();
  const [handleUpdateListCourse, dataClass] = useOutletContext();
  const navigate = useNavigate();

  const getData = () => {
    setDataSchedule(undefined);
    setGettingData(true);
    FetchApi(ManageScheduleApis.getScheduleByClassIdAndModuleId, null, null, [
      String(id),
      String(moduleId),
    ])
      .then((res) => {
        setGettingData(false);
        setDataSchedule(res.data);
      })
      .catch((err) => {
        setGettingData(false);
        if (err?.type_error === 'class-schedule-error-0026') {
          navigate('/404');
        }
      })
      .finally(() => {
        getDataModule();
      });
  };

  const getDataModule = () => {
    setDataModule(undefined);
    FetchApi(ModulesApis.getModuleByID, null, null, [String(moduleId)])
      .then((res) => {
        setDataModule(res.data);
      })
      .catch((err) => {
        navigate('/404');
      });
  };

  const handleSuccess = () => {
    setCreateMode(false);
    setEditMode(false);
    getData();
  };

  const handleDeleteSchedule = () => {
    toast.promise(
      FetchApi(ManageScheduleApis.deleteSchedule, null, null, [
        String(id),
        String(dataSchedule?.id),
      ]),
      {
        loading: '??ang x??a l???ch h???c...',
        success: () => {
          getData();
          handleUpdateListCourse();
          return 'X??a l???ch h???c th??nh c??ng!';
        },
        error: (err) => {
          return 'X??a l???ch h???c th???t b???i!';
        },
      }
    );
  };

  useEffect(() => {
    getData();
    setCreateMode(false);
    setEditMode(false);
    setViewGrade(false);
  }, [id, moduleId]);

  const handleCloseModalAttendance = () => {
    setSelectSession(undefined);
  };

  const handleCloseViewAttendance = () => {
    setViewAttendance(false);
  };

  const handleSelectOption = (key) => {
    switch (key) {
      case 'attendance':
        setViewAttendance(true);
        break;
      case 'edit':
        setEditMode(true);
        break;
      case 'delete':
        handleDeleteSchedule();
        break;
      case 'grade':
        setViewGrade(true);
        break;
      default:
        break;
    }
  };

  const canViewGrade = () => {
    return new Date(Date.now()) >= new Date(dataSchedule?.start_date);
  };

  return (
    <Fragment>
      {selectSession && dataSchedule && (
        <TakeAttendance
          session={selectSession}
          scheduleId={dataSchedule.id}
          onClose={handleCloseModalAttendance}
        />
      )}
      {viewAttendance && dataSchedule && (
        <ViewAllAttendance
          open={viewAttendance}
          scheduleId={dataSchedule.id}
          onClose={handleCloseViewAttendance}
        />
      )}
      <Card variant={'bordered'}>
        <Card.Header>
          <Grid.Container justify="space-between">
            <Grid xs={4}>
              {(editMode || viewGrade) && (
                <Button
                  flat
                  auto
                  color={'error'}
                  size={'sm'}
                  onPress={() => {
                    setEditMode(false);
                    setViewGrade(false);
                  }}
                >
                  Tr??? v???
                </Button>
              )}
            </Grid>
            <Grid xs={4}>
              <Text
                p
                b
                size={14}
                css={{
                  width: '100%',
                  textAlign: 'center',
                }}
              >
                {createMode
                  ? 'T???o l???ch h???c'
                  : editMode
                  ? 'Ch???nh s???a l???ch h???c'
                  : viewGrade
                  ? 'Xem ??i???m'
                  : 'Th??ng tin l???ch h???c'}
              </Text>
            </Grid>
            <Grid xs={4} justify={'flex-end'}></Grid>
          </Grid.Container>
        </Card.Header>
        <Card.Body>
          {!createMode && !editMode && !viewGrade && (
            <Fragment>
              {(gettingData || dataModule === undefined) && (
                <div className={classes.loading}>
                  <Loading size="sm">Vui l??ng ?????i ...</Loading>
                </div>
              )}
              {!gettingData &&
                dataSchedule === undefined &&
                dataModule !== undefined && (
                  <div className={classes.loading}>
                    <Text p size={14}>
                      B???n ??ang ch???n m??n h???c <b>"{dataModule?.module_name}"</b>
                    </Text>
                    <Text p size={14}>
                      Ch??a c?? l???ch h???c n??o ???????c t???o cho m??n h???c n??y, b???n c?? th???
                      t???o l???ch h???c b???ng c??ch nh???n n??t b??n d?????i.
                    </Text>
                    <Spacer y={1} />
                    <Button
                      flat
                      auto
                      color="success"
                      onPress={() => {
                        setCreateMode(true);
                      }}
                    >
                      T???o l???ch h???c
                    </Button>
                  </div>
                )}
              {!gettingData &&
                dataSchedule !== undefined &&
                dataModule !== undefined && (
                  <div>
                    <div
                      style={{
                        padding: 10,
                      }}
                    >
                      <Descriptions>
                        <Descriptions.Item span={3} label="M??n h???c">
                          <b>{dataSchedule.module_name}</b>
                        </Descriptions.Item>
                        <Descriptions.Item label="Th???i gian h???c trong tu???n">
                          <b>
                            {dataSchedule.class_days.id === 1
                              ? 'Th??? 2, 4, 6'
                              : 'Th??? 3, 5, 7'}
                          </b>
                        </Descriptions.Item>
                        <Descriptions.Item label="Gi??o vi??n">
                          <b>{`${dataSchedule.teacher.first_name} ${dataSchedule.teacher.last_name}`}</b>
                        </Descriptions.Item>
                        <Descriptions.Item label="Email gi??o vi??n">
                          <b>{`${dataSchedule.teacher.email_organization}`}</b>
                        </Descriptions.Item>
                        <Descriptions.Item label="Ng??y b???t ?????u">
                          <b>
                            {new Date(
                              dataSchedule.start_date
                            ).toLocaleDateString('vi-VN')}
                          </b>
                        </Descriptions.Item>
                        <Descriptions.Item span={2} label="Gi??? h???c b???t ?????u">
                          <b>{dataSchedule.class_hour_start}</b>
                        </Descriptions.Item>
                        <Descriptions.Item label="Ng??y k???t th??c">
                          <b>
                            {new Date(dataSchedule.end_date).toLocaleDateString(
                              'vi-VN'
                            )}
                          </b>
                        </Descriptions.Item>
                        <Descriptions.Item span={2} label="Gi??? h???c k???t th??c">
                          <b>{dataSchedule.class_hour_end}</b>
                        </Descriptions.Item>
                        <Descriptions.Item span={3} label="S??? l?????ng bu???i h???c">
                          <b>{dataSchedule.duration}</b>
                        </Descriptions.Item>
                        <Descriptions.Item label="Ng??y t???o l???ch">
                          <b>
                            {new Date(
                              dataSchedule.created_at
                            ).toLocaleDateString('vi-VN')}
                          </b>
                        </Descriptions.Item>
                        <Descriptions.Item span={2} label="Ng??y c???p nh???t">
                          <b>
                            {new Date(
                              dataSchedule.updated_at
                            ).toLocaleDateString('vi-VN')}
                          </b>
                        </Descriptions.Item>
                        <Descriptions.Item span={2} label="Ghi ch??">
                          <i>{dataSchedule.note}</i>
                        </Descriptions.Item>
                        <Descriptions.Item>
                          <div className={classes.buttonGroupEdit}>
                            {dataClass?.class_status.id !== 6 && (
                              <Dropdown>
                                <Dropdown.Button flat color="secondary">
                                  Ch???c n??ng
                                </Dropdown.Button>
                                <Dropdown.Menu
                                  onAction={handleSelectOption}
                                  color="secondary"
                                  aria-label="Actions"
                                  css={{ $$dropdownMenuWidth: '340px' }}
                                >
                                  <Dropdown.Section title="C?? b???n">
                                    <Dropdown.Item
                                      key="attendance"
                                      description="Xem danh s??ch ??i???m danh"
                                      color={'success'}
                                      icon={<BsFillPersonCheckFill />}
                                    >
                                      ??i???m danh
                                    </Dropdown.Item>
                                    <Dropdown.Item
                                      key="edit"
                                      description="Ch???nh s???a l???ch h???c"
                                      color={'secondary'}
                                      icon={<MdEditCalendar />}
                                    >
                                      Ch???nh s???a
                                    </Dropdown.Item>
                                    {canViewGrade() && (
                                      <Dropdown.Item
                                        key="grade"
                                        description="Xem ??i???m c???a h???c sinh"
                                        color={'warning'}
                                        icon={<RiBookmark3Fill />}
                                      >
                                        Xem ??i???m
                                      </Dropdown.Item>
                                    )}
                                  </Dropdown.Section>
                                  <Dropdown.Section title="Nguy hi???m">
                                    <Dropdown.Item
                                      key="delete"
                                      description="X??a l???ch h???c"
                                      color={'error'}
                                      icon={<MdDelete />}
                                    >
                                      X??a l???ch h???c
                                    </Dropdown.Item>
                                  </Dropdown.Section>
                                </Dropdown.Menu>
                              </Dropdown>
                            )}
                          </div>
                        </Descriptions.Item>
                      </Descriptions>
                    </div>
                    <Spacer y={1} />
                    <Divider />
                    <Spacer y={1} />
                    <Grid.Container gap={2}>
                      {dataSchedule.sessions
                        .sort(
                          (a, b) =>
                            new Date(a.learning_date) -
                            new Date(b.learning_date)
                        )
                        .map((data, index) => {
                          return (
                            <Grid key={index} xs={2}>
                              <Card
                                variant={
                                  canViewGrade() &&
                                  dataClass?.class_status.id !== 6
                                    ? 'bordered'
                                    : 'flat'
                                }
                                isPressable={
                                  canViewGrade() &&
                                  dataClass?.class_status.id !== 6
                                }
                                onPress={() => {
                                  setSelectSession(data);
                                }}
                                css={{
                                  height: 'fit-content',
                                  border:
                                    new Date(
                                      data.learning_date
                                    ).toLocaleDateString('vi-VN') ===
                                      new Date(Date.now()).toLocaleDateString(
                                        'vi-VN'
                                      ) && '2px solid red',
                                  cursor:
                                    canViewGrade() &&
                                    dataClass?.class_status.id !== 6
                                      ? 'pointer'
                                      : 'no-drop',
                                }}
                              >
                                <Card.Body
                                  css={{
                                    padding: '6px 12px',
                                    backgroundColor:
                                      data.session_type === 3 ||
                                      data.session_type === 4
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
                                      {new Date(
                                        data.learning_date
                                      ).toLocaleDateString('vi-VN')}
                                    </Text>
                                  </div>
                                  <Spacer y={0.6} />
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
                                    Ph??ng <b>{data.room.name}</b>
                                  </Text>
                                </Card.Body>
                              </Card>
                            </Grid>
                          );
                        })}
                    </Grid.Container>
                  </div>
                )}
            </Fragment>
          )}
          {createMode && dataModule !== undefined && (
            <ScheduleCreate dataModule={dataModule} onSuccess={handleSuccess} />
          )}
          {editMode && dataModule !== undefined && (
            <ScheduleUpdate
              dataModule={dataModule}
              dataSchedule={dataSchedule}
              onSuccess={handleSuccess}
            />
          )}
          {viewGrade && dataModule !== undefined && (
            <ViewGrade
              role={'sro'}
              dataModule={dataModule}
              dataSchedule={dataSchedule}
              onSuccess={handleSuccess}
            />
          )}
        </Card.Body>
      </Card>
    </Fragment>
  );
};

export default ScheduleDetail;
