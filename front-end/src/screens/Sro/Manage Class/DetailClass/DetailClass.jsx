import {
  Badge,
  Button,
  Card,
  Grid,
  Spacer,
  Table,
  Text,
} from '@nextui-org/react';
import classes from './DetailClass.module.css';
import { FaCloudDownloadAlt, FaCloudUploadAlt } from 'react-icons/fa';
import { IoArrowBackCircle } from 'react-icons/io5';
import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import FetchApi from '../../../../apis/FetchApi';
import { ManageClassApis } from '../../../../apis/ListApi';
import { Descriptions, Skeleton } from 'antd';

const DetailClass = () => {
  const [dataClass, setDataClass] = useState(undefined);

  const navigate = useNavigate();
  const { id } = useParams();

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
    } else {
      return (
        <Badge variant="flat" color="primary">
          Chưa lên lịch
        </Badge>
      );
    }
  };

  const getData = () => {
    FetchApi(ManageClassApis.getInformationClass, null, null, [String(id)])
      .then((res) => {
        setDataClass(res.data);
      })
      .catch((err) => {
        navigate('/404');
      });
  };

  useEffect(() => {
    getData();
  }, []);

  return (
    <Grid.Container gap={2}>
      <Grid
        xs={3.5}
        css={{
          display: 'flex',
          flexDirection: 'column',
          gap: 20,
        }}
      >
        <Card
          variant="bordered"
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={16}
              css={{ width: '100%', textAlign: 'center' }}
              color="error"
            >
              Thông tin cơ bản
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
        <Card
          variant="bordered"
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={16}
              css={{ width: '100%', textAlign: 'center' }}
              color="error"
            >
              Thời gian và kế hoạch
            </Text>
          </Card.Header>
          <Card.Body>
            {!dataClass && <Skeleton />}
            {dataClass && (
              <Descriptions layout="horizontal" column={{ lg: 1 }}>
                <Descriptions.Item label="Mã chương trình học">
                  <b>{dataClass?.course_family?.code}</b>
                </Descriptions.Item>
                <Descriptions.Item label="chương trình học">
                  <b>{dataClass?.course_family?.name}</b>
                </Descriptions.Item>
                <Descriptions.Item label="Ngày bắt đầu (dự tính)">
                  <b>
                    {new Date(dataClass?.start_date).toLocaleDateString(
                      'vi-VN'
                    )}
                  </b>
                </Descriptions.Item>
                <Descriptions.Item label="Thời gian học">
                  <b>
                    {dataClass?.class_hour_start?.split(':')[0] +
                      ':' +
                      dataClass?.class_hour_start?.split(':')[1]}
                    {' => '}
                    {dataClass?.class_hour_end?.split(':')[0] +
                      ':' +
                      dataClass?.class_hour_end?.split(':')[1]}
                  </b>
                </Descriptions.Item>
                <Descriptions.Item label="Ngày học trong tuần">
                  <b>
                    {dataClass?.class_days?.id === 1
                      ? 'Thứ 2, 4, 6'
                      : 'Thứ 3, 5, 7'}
                  </b>
                </Descriptions.Item>
                <Descriptions.Item label="Ngày hoàn thành (dự tính)">
                  <b>
                    {new Date(dataClass?.completion_date).toLocaleDateString(
                      'vi-VN'
                    )}
                  </b>
                </Descriptions.Item>
                <Descriptions.Item label="Ngày tốt nghiệp (dự tính)">
                  <b>
                    {new Date(dataClass?.graduation_date).toLocaleDateString(
                      'vi-VN'
                    )}
                  </b>
                </Descriptions.Item>
              </Descriptions>
            )}
          </Card.Body>
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
                  onPress={() => {
                    navigate('/sro/manage-class');
                  }}
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
              Danh sách học viên
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
