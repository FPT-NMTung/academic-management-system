import {
  Badge,
  Button,
  Card,
  Divider,
  Grid,
  Loading,
  Spacer,
  Text,
} from '@nextui-org/react';
import { Descriptions } from 'antd';
import { useState } from 'react';
import { useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import FetchApi from '../../../../apis/FetchApi';
import { ManageScheduleApis, ModulesApis } from '../../../../apis/ListApi';
import classes from './ScheduleDetail.module.css';

const ScheduleDetail = () => {
  const [dataSchedule, setDataSchedule] = useState(undefined);
  const [gettingData, setGettingData] = useState(true);
  const [dataModule, setDataModule] = useState(undefined);

  const { id, moduleId } = useParams();
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

  useEffect(() => {
    getData();
  }, [id, moduleId]);

  return (
    <div>
      {(gettingData || dataModule === undefined) && (
        <div className={classes.loading}>
          <Loading size="sm">Vui lòng đợi ...</Loading>
        </div>
      )}
      {!gettingData && dataSchedule === undefined && dataModule !== undefined && (
        <div className={classes.loading}>
          <Text p size={14}>
            Bạn đang chọn môn học <b>"{dataModule?.module_name}"</b>
          </Text>
          <Text p size={14}>
            Chưa có lịch học nào được tạo cho môn học này, bạn có thể tạo lịch
            học bằng cách nhấn nút bên dưới.
          </Text>
          <Spacer y={1} />
          <Button flat auto color="success">
            Tạo lịch học
          </Button>
        </div>
      )}
      {!gettingData && dataSchedule !== undefined && dataModule !== undefined && (
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
                {new Date(dataSchedule.start_date).toLocaleDateString('vi-VN')}
              </b>
            </Descriptions.Item>
            <Descriptions.Item span={2} label="Giờ học bắt đầu">
              <b>{dataSchedule.class_hour_start}</b>
            </Descriptions.Item>
            <Descriptions.Item label="Ngày kết thúc">
              <b>
                {new Date(dataSchedule.end_date).toLocaleDateString('vi-VN')}
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
                {new Date(dataSchedule.created_at).toLocaleDateString('vi-VN')}
              </b>
            </Descriptions.Item>
            <Descriptions.Item label="Ngày cập nhật">
              <b>
                {new Date(dataSchedule.updated_at).toLocaleDateString('vi-VN')}
              </b>
            </Descriptions.Item>
            <Descriptions.Item>
              <div className={classes.buttonGroupEdit}>
                <Button auto flat color="primary">
                  Chỉnh sửa
                </Button>
                <Button auto flat color="error">
                  Xoá
                </Button>
              </div>
            </Descriptions.Item>
          </Descriptions>
          <Spacer y={1} />
          <Divider />
          <Spacer y={1} />
          <Grid.Container gap={2}>
            {dataSchedule.sessions.map((data, index) => {
              return (
                <Grid key={index} xs={2}>
                  <Card variant="bordered" isPressable>
                    <Card.Body
                      css={{
                        padding: '6px 12px',
                        backgroundColor: (data.session_type === 3 || data.session_type === 4) ? '#7828C8' : '',
                      }}
                    >
                      <div className={classes.infoSlotSession}>
                        <Text p size={12}
                          color={data.session_type === 3 || data.session_type === 4 ? '#fff' : '#000'}
                        >
                          <b>Slot {index + 1}</b>
                        </Text>
                        <Text p size={12}
                          color={data.session_type === 3 || data.session_type === 4 ? '#fff' : '#000'}
                        >
                          {new Date(data.learning_date).toLocaleDateString(
                            'vi-VN'
                          )}
                        </Text>
                      </div>
                      <Spacer y={0.6} />
                      <Text p size={12}
                        color={data.session_type === 3 || data.session_type === 4 ? '#fff' : '#000'}
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
    </div>
  );
};

export default ScheduleDetail;
