import { Grid, Spacer, Text, Badge, Card } from '@nextui-org/react';
import classes from './TeacherDetail.module.css';
import { useParams, useNavigate } from 'react-router-dom';
import { Descriptions, Spin, Button } from 'antd';
import { useState, useEffect } from 'react';
import { AiFillPhone } from 'react-icons/ai';
import { MdEmail } from 'react-icons/md';
import { HiOfficeBuilding } from 'react-icons/hi';
import FetchApi from '../../../../apis/FetchApi';
import { ManageTeacherApis } from '../../../../apis/ListApi';

const TeacherDetail = () => {
  const [dataUser, setDataUser] = useState({});
  const [isLoading, setIsLoading] = useState(true);

  const { id } = useParams();
  const navigate = useNavigate();

  const getDataUser = () => {
    FetchApi(ManageTeacherApis.detailTeacher, null, null, [`${id}`])
      .then((res) => {
        setDataUser(res.data);
        setIsLoading(false);
      })
      .catch(() => {
        navigate('/404');
      });
  };

  useEffect(() => {
    getDataUser();
  }, []);

  return (
    <Grid.Container justify="center">
      <Grid sm={8}>
        {isLoading && (
          <div className={classes.loading}>
            <Spin />
          </div>
        )}
        {!isLoading && (
          <Grid.Container
            gap={2}
            css={{
              position: 'relative',
            }}
          >
            <Grid
              sm={3.5}
              css={{
                display: 'flex',
                flexDirection: 'column',
                width: '100%',
                height: 'fit-content',
              }}
            >
              <div className={classes.contantLogo}>
                <div className={classes.logo}>
                  {dataUser.avatar && (
                    <img className={classes.avatar} src={dataUser.avatar} />
                  )}
                </div>
                <Spacer y={0.7} />
                <Text h3 size={20} b>
                  {dataUser.first_name} {dataUser.last_name}
                </Text>
              </div>
              <Spacer y={1} />
              <div className={classes.iconInformation}>
                <AiFillPhone />
                <Text p size={15}>
                  {dataUser.mobile_phone}
                </Text>
              </div>
              <div className={classes.iconInformation}>
                <MdEmail />
                <Text p size={15}>
                  {dataUser.email}
                </Text>
              </div>
              <div className={classes.iconInformation}>
                <HiOfficeBuilding />
                <Text p size={15}>
                  {dataUser.email_organization}
                </Text>
              </div>
            </Grid>
            <Grid sm={8.5} direction="column" css={{ rowGap: 20 }}>
              <Card>
                <Card.Body>
                  <Descriptions
                    title="Thông tin cơ bản"
                    bordered
                    column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                    extra={
                      <Button
                        type="primary"
                        onClick={() => {
                          navigate(`/admin/account/teacher/${id}/update`);
                        }}
                      >
                        Chỉnh sửa thông tin
                      </Button>
                    }
                  >
                    <Descriptions.Item label="Họ và tên đệm">
                      {dataUser.first_name}
                    </Descriptions.Item>
                    <Descriptions.Item label="Tên">
                      {dataUser.last_name}
                    </Descriptions.Item>
                    <Descriptions.Item label="Số điện thoại">
                      {dataUser.mobile_phone}
                    </Descriptions.Item>
                    <Descriptions.Item label="Giới tính">
                      {dataUser.gender.id === 1 ? 'Nam' : 'Nữ'}
                    </Descriptions.Item>
                    <Descriptions.Item label="Địa chỉ" span={2}>
                      {dataUser.ward.prefix} {dataUser.ward.name},{' '}
                      {dataUser.district.prefix} {dataUser.district.name},{' '}
                      {dataUser.province.name}
                    </Descriptions.Item>
                    <Descriptions.Item label="Trạng thái">
                      <Badge color={'success'} variant={'flat'}>
                        Active
                      </Badge>
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày sinh">
                      {new Date(dataUser.birthday).toLocaleDateString('vi-VN')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Email cá nhân" span={2}>
                      {dataUser.email}
                    </Descriptions.Item>
                    <Descriptions.Item label="Email tổ chức" span={2}>
                      {dataUser.email_organization}
                    </Descriptions.Item>
                  </Descriptions>
                </Card.Body>
              </Card>
              <Card>
                <Card.Body>
                  <Descriptions
                    title="Thông tin bổ sung"
                    bordered
                    column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                  >
                    <Descriptions.Item label="Biệt danh">
                      {dataUser.nickname ? (
                        dataUser.nickname
                      ) : (
                        <i style={{ color: 'lightgray' }}>Không có</i>
                      )}
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày bắt đầu dạy">
                      {new Date(dataUser.start_working_date).toLocaleDateString(
                        'vi-VN'
                      )}
                    </Descriptions.Item>
                    <Descriptions.Item label="Nơi công tác" span={2}>
                      {dataUser.company_address ? (
                        dataUser.company_address
                      ) : (
                        <i style={{ color: 'lightgray' }}>Không có</i>
                      )}
                    </Descriptions.Item>
                    <Descriptions.Item label="Loại hợp đồng">
                      {dataUser.teacher_type.value}
                    </Descriptions.Item>
                    <Descriptions.Item label="Mã số thuế">
                      {dataUser.tax_code}
                    </Descriptions.Item>
                    <Descriptions.Item label="Thời gian dạy">
                      {dataUser.working_time.value}
                    </Descriptions.Item>
                    <Descriptions.Item label="Mức lương">
                      {dataUser.salary
                        ? String(dataUser.salary).replace(
                            /\B(?=(\d{3})+(?!\d))/g,
                            ','
                          )
                        : 0}{' '}
                      VNĐ
                    </Descriptions.Item>
                  </Descriptions>
                </Card.Body>
              </Card>
              <Card>
                <Card.Body>
                  <Descriptions
                    title="Thông tin CMND/CCCD"
                    bordered
                    column={{ md: 2, lg: 2, xl: 2, xxl: 2 }}
                  >
                    <Descriptions.Item label="Số thẻ">
                      {dataUser.citizen_identity_card_no}
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày cấp">
                      {new Date(
                        dataUser.citizen_identity_card_published_date
                      ).toLocaleDateString('vi-VN')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Nơi cấp">
                      {dataUser.citizen_identity_card_published_place}
                    </Descriptions.Item>
                  </Descriptions>
                </Card.Body>
              </Card>
            </Grid>
          </Grid.Container>
        )}
      </Grid>
    </Grid.Container>
  );
};

export default TeacherDetail;
