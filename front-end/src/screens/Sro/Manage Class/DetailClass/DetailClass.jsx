import { Button, Card, Grid, Spacer, Table, Text } from '@nextui-org/react';
import classes from './DetailClass.module.css';
import { FaCloudDownloadAlt, FaCloudUploadAlt } from 'react-icons/fa';
import { IoArrowBackCircle } from 'react-icons/io5';
import { useNavigate } from 'react-router-dom';

const DetailClass = () => {
  const navigate = useNavigate();

  return (
    <Grid.Container gap={2}>
      <Grid xs={3.5}>
        <Card
          variant="bordered"
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text p b size={14} css={{ width: '100%', textAlign: 'center' }}>
              Thông tin lớp học
            </Text>
          </Card.Header>
          <Card.Body></Card.Body>
        </Card>
      </Grid>
      <Grid xs={8.5} direction={'column'}>
        <Card variant="bordered">
          <Card.Body
            css={{
              padding: 10,
            }}
          >
            <div className={classes.header}>
              <div className={classes.groupButton}>
                <Button
                  flat
                  auto
                  icon={<IoArrowBackCircle size={20} />}
                  color={'error'}
                  onPress={() => {navigate('/sro/class')}}
                >
                  Trở về
                </Button>
              </div>
              <div className={classes.groupButton}>
                <Button flat auto icon={<FaCloudDownloadAlt size={20} />}>
                  Tải xuống Template
                </Button>
                <Button
                  flat
                  auto
                  color={'success'}
                  icon={<FaCloudUploadAlt size={20} />}
                >
                  Import
                </Button>
              </div>
            </div>
          </Card.Body>
        </Card>
        <Spacer y={1} />
        <Card variant="bordered">
          <Card.Header>
            <Text p b size={14} css={{ width: '100%', textAlign: 'center' }}>
              Danh sách học sinh
            </Text>
          </Card.Header>
          <Table>
            <Table.Header>
              <Table.Column>STT</Table.Column>
              <Table.Column>Họ và tên</Table.Column>
              <Table.Column>Ngày sinh</Table.Column>
              <Table.Column>Email</Table.Column>
              <Table.Column>Giới tính</Table.Column>
              <Table.Column></Table.Column>
            </Table.Header>
            <Table.Body></Table.Body>
            <Table.Pagination shadow noMargin align="center" rowsPerPage={10} />
          </Table>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default DetailClass;
