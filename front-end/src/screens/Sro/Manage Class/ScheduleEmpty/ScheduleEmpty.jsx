import { Card, Text } from '@nextui-org/react';
import classes from './ScheduleEmpty.module.css';

const ScheduleEmpty = () => {
  return (
    <div className={classes.main}>
      <Card
        variant='bordered'
      >
        <Card.Body>
          <Text
            i
            p
            size={14}
            css={{
              textAlign: 'center',
              width: '100%',
              padding: '200px 0',
            }}
          >
            Vui lòng chọn một môn học để xem chi tiết lịch học
          </Text>
        </Card.Body>
      </Card>
    </div>
  );
};

export default ScheduleEmpty;
