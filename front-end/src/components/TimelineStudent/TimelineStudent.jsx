import { Card, Text, Badge, Table ,Button } from '@nextui-org/react';
import { Timeline } from 'antd';

const TimelineStudent = () => {
  return (
    <Timeline mode="left">
      <Timeline.Item color="green">
        <Text p>
          Thời gian: <strong>8h30 - 10h30</strong>
        </Text>
        <Card
          variant="bordered"
          css={{
            borderRadius: '6px',
          }}
          isPressable={true}
        >
          <Table
            aria-label=""
            css={{
              height: 'auto',
              minWidth: '100%',
            }}
            lined
            headerLined
            shadow={false}
          >
            <Table.Header>
              <Table.Column>Môn học</Table.Column>
              <Table.Column width={100}>Phòng học</Table.Column>
              <Table.Column width={200}>Giảng viên</Table.Column>
              <Table.Column width={150}>Điểm danh</Table.Column>
            </Table.Header>
            <Table.Body>
              <Table.Row key="1">
                <Table.Cell>Phát triển ứng dụng web</Table.Cell>
                <Table.Cell>Phòng 1</Table.Cell>
                <Table.Cell>Nguyễn Văn A</Table.Cell>
                <Table.Cell>
                  <Badge color="success">Đã điểm danh</Badge>
                </Table.Cell>
              </Table.Row>
            </Table.Body>
          </Table>
        </Card>
      </Timeline.Item>
      <Timeline.Item color="red">
        <Text p>
          Thời gian: <strong>8h30 - 10h30</strong>
        </Text>
        <Card
          variant="bordered"
          css={{
            borderRadius: '6px',
          }}
          isPressable={true}
        >
          <Table
            aria-label=""
            css={{
              height: 'auto',
              minWidth: '100%',
            }}
            lined
            headerLined
            shadow={false}
          >
            <Table.Header>
              <Table.Column>Môn học</Table.Column>
              <Table.Column width={100}>Phòng học</Table.Column>
              <Table.Column width={200}>Giảng viên</Table.Column>
              <Table.Column width={150}>Điểm danh</Table.Column>
            </Table.Header>
            <Table.Body>
              <Table.Row key="1">
                <Table.Cell>Phát triển ứng dụng web</Table.Cell>
                <Table.Cell>Phòng 1</Table.Cell>
                <Table.Cell>Nguyễn Văn A</Table.Cell>
                <Table.Cell>
                  <Badge color="error">Vắng mặt</Badge>
                </Table.Cell>
              </Table.Row>
            </Table.Body>
          </Table>
        </Card>
      </Timeline.Item>
      <Timeline.Item color="gray">
        <Text p>
          Thời gian: <strong>8h30 - 10h30</strong>
        </Text>
        <Card
          variant="bordered"
          css={{
            borderRadius: '6px',
          }}
          isPressable={true}
        >
          <Table
            aria-label=""
            css={{
              height: 'auto',
              minWidth: '100%',
            }}
            lined
            headerLined
            shadow={false}
          >
            <Table.Header>
              <Table.Column>Môn học</Table.Column>
              <Table.Column width={100}>Phòng học</Table.Column>
              <Table.Column width={200}>Giảng viên</Table.Column>
              <Table.Column width={150}>Điểm danh</Table.Column>
            </Table.Header>
            <Table.Body>
              <Table.Row key="1">
                <Table.Cell>Phát triển ứng dụng web</Table.Cell>
                <Table.Cell>Phòng 1</Table.Cell>
                <Table.Cell>Nguyễn Văn A</Table.Cell>
                <Table.Cell>
                  <Badge color="default">Chưa điểm danh</Badge>
                </Table.Cell>
              </Table.Row>
            </Table.Body>
          </Table>
        </Card>
      </Timeline.Item>
    </Timeline>
    
  );
};

export default TimelineStudent;
