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
import classes from './ScheduleDetail.module.css';

const ScheduleDetail = () => {
  const [dataSchedule, setDataSchedule] = useState(undefined);
  const [gettingData, setGettingData] = useState(true);
  const [dataModule, setDataModule] = useState(undefined);
  const [createMode, setCreateMode] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [viewAttendance, setViewAttendance] = useState(false);

  const [selectSession, setSelectSession] = useState(undefined);

  const { id, moduleId } = useParams();
  const [handleUpdateListCourse] = useOutletContext();
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
        loading: 'Đang xóa lịch học...',
        success: () => {
          getData();
          handleUpdateListCourse();
          return 'Xóa lịch học thành công!';
        },
        error: (err) => {
          return 'Xóa lịch học thất bại!';
        },
      }
    );
  };

  useEffect(() => {
    getData();
    setCreateMode(false);
    setEditMode(false);
  }, [id, moduleId]);

  const handleCloseModalAttendance = () => {
    setSelectSession(undefined);
  };

  const handleCloseViewAttendance = () => {
    setViewAttendance(false);
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
              {editMode && (
                <Button
                  flat
                  auto
                  color={'error'}
                  size={'sm'}
                  onPress={() => {
                    setEditMode(false);
                  }}
                >
                  Trở về
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
                  ? 'Tạo lịch học'
                  : editMode
                  ? 'Chỉnh sửa lịch học'
                  : 'Thông tin lịch học'}
              </Text>
            </Grid>
            <Grid xs={4}></Grid>
          </Grid.Container>
        </Card.Header>
        <Card.Body>
          {!createMode && !editMode && (
            <Fragment>
              {(gettingData || dataModule === undefined) && (
                <div className={classes.loading}>
                  <Loading size="sm">Vui lòng đợi ...</Loading>
                </div>
              )}
              {!gettingData &&
                dataSchedule === undefined &&
                dataModule !== undefined && (
                  <div className={classes.loading}>
                    <Text p size={14}>
                      Bạn đang chọn môn học <b>"{dataModule?.module_name}"</b>
                    </Text>
                    <Text p size={14}>
                      Chưa có lịch học nào được tạo cho môn học này, bạn có thể
                      tạo lịch học bằng cách nhấn nút bên dưới.
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
                      Tạo lịch học
                    </Button>
                  </div>
                )}
              {!gettingData &&
                dataSchedule !== undefined &&
                dataModule !== undefined && (
                  <div>
                    <Descriptions>
                      <Descriptions.Item span={3} label="Môn học">
                        <b>{dataSchedule.module_name}</b>
                      </Descriptions.Item>
                      <Descriptions.Item label="Thời gian học trong tuần">
                        <b>
                          {dataSchedule.class_days.id === 1
                            ? 'Thứ 2, 4, 6'
                            : 'Thứ 3, 5, 7'}
                        </b>
                      </Descriptions.Item>
                      <Descriptions.Item label="Giáo viên">
                        <b>{`${dataSchedule.teacher.first_name} ${dataSchedule.teacher.last_name}`}</b>
                      </Descriptions.Item>
                      <Descriptions.Item label="Email giáo viên">
                        <b>{`${dataSchedule.teacher.email_organization}`}</b>
                      </Descriptions.Item>
                      <Descriptions.Item label="Ngày bắt đầu">
                        <b>
                          {new Date(dataSchedule.start_date).toLocaleDateString(
                            'vi-VN'
                          )}
                        </b>
                      </Descriptions.Item>
                      <Descriptions.Item span={2} label="Giờ học bắt đầu">
                        <b>{dataSchedule.class_hour_start}</b>
                      </Descriptions.Item>
                      <Descriptions.Item label="Ngày kết thúc">
                        <b>
                          {new Date(dataSchedule.end_date).toLocaleDateString(
                            'vi-VN'
                          )}
                        </b>
                      </Descriptions.Item>
                      <Descriptions.Item span={2} label="Giờ học kết thúc">
                        <b>{dataSchedule.class_hour_end}</b>
                      </Descriptions.Item>
                      <Descriptions.Item span={3} label="Số lượng buổi học">
                        <b>{dataSchedule.duration}</b>
                      </Descriptions.Item>
                      <Descriptions.Item label="Ngày tạo lịch">
                        <b>
                          {new Date(dataSchedule.created_at).toLocaleDateString(
                            'vi-VN'
                          )}
                        </b>
                      </Descriptions.Item>
                      <Descriptions.Item span={2} label="Ngày cập nhật">
                        <b>
                          {new Date(dataSchedule.updated_at).toLocaleDateString(
                            'vi-VN'
                          )}
                        </b>
                      </Descriptions.Item>
                      <Descriptions.Item span={2} label="Ghi chú">
                        <i>{dataSchedule.note}</i>
                      </Descriptions.Item>
                      <Descriptions.Item>
                        <div className={classes.buttonGroupEdit}>
                          <Button
                            auto
                            flat
                            size={'sm'}
                            color="warning"
                            onPress={() => {
                              setViewAttendance(true);
                            }}
                          >
                            Xem điểm danh
                          </Button>
                          <Button
                            auto
                            flat
                            size={'sm'}
                            color="primary"
                            onPress={() => {
                              setEditMode(true);
                            }}
                          >
                            Chỉnh sửa
                          </Button>
                          <Button
                            auto
                            flat
                            size={'sm'}
                            color="error"
                            onPress={handleDeleteSchedule}
                          >
                            Xoá
                          </Button>
                        </div>
                      </Descriptions.Item>
                    </Descriptions>
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
                                variant="bordered"
                                isPressable
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
                                    Phòng <b>{data.room.name}</b>
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
        </Card.Body>
      </Card>
    </Fragment>
  );
};

export default ScheduleDetail;
