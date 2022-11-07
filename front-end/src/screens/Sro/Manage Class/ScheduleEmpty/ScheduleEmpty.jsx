import { Text } from '@nextui-org/react';
import classes from './ScheduleEmpty.module.css';

const ScheduleEmpty = () => {
  return (
    <div className={classes.main}>
      <Text i p size={14}>
        Vui lòng chọn một môn học để xem chi tiết lịch học
      </Text>
    </div>
  );
};

export default ScheduleEmpty;
