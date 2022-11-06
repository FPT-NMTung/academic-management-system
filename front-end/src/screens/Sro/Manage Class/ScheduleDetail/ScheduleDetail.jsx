import {
  Button,
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
            <Descriptions.Item label="Thời gian học trong tuần"></Descriptions.Item>
            <Descriptions.Item span={2} label="Giáo viên"></Descriptions.Item>
            <Descriptions.Item label="Ngày bắt đầu"></Descriptions.Item>
            <Descriptions.Item
              span={2}
              label="Giờ học bắt đầu"
            ></Descriptions.Item>
            <Descriptions.Item label="Ngày kết thúc"></Descriptions.Item>
            <Descriptions.Item
              span={2}
              label="Giờ học kết thúc"
            ></Descriptions.Item>
            <Descriptions.Item
              span={3}
              label="Số lượng buổi học"
            ></Descriptions.Item>
            <Descriptions.Item label="Ngày tạo"></Descriptions.Item>
            <Descriptions.Item label="Ngày cập nhật"></Descriptions.Item>
          </Descriptions>
          <Divider />
          <Grid.Container gap={2}>
            <Grid
              xs={2}
              css={{
                backgroundColor: '#f5f5f5',
              }}
            >
              zsdasd
            </Grid>
          </Grid.Container>
        </div>
      )}
    </div>
  );
};

export default ScheduleDetail;
