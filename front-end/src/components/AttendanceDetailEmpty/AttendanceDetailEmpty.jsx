import { Card, Text } from '@nextui-org/react';

const AttendanceDetailEmpty = () => {
  return (
    <Card
      variant="bordered"
      css={{
        height: 'fit-content',
      }}
    >
      <Card.Body>
        <Text
          p
          i
          size={14}
          css={{
            textAlign: 'center',
            width: '100%',
            margin: '180px 0',
          }}
        >
          Vui lòng chọn lớp để xem danh sách slot dạy
        </Text>
      </Card.Body>
    </Card>
  );
};

export default AttendanceDetailEmpty;
