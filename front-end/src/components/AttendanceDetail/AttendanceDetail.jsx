import {
  Badge,
  Button,
  Card,
  Dropdown,
  Grid,
  Loading,
  Spacer,
  Table,
  Text,
  Tooltip,
} from '@nextui-org/react';
import { useState } from 'react';
import { useEffect } from 'react';
import { useNavigate, useOutletContext, useParams } from 'react-router-dom';
import FetchApi from '../../apis/FetchApi';
import { ManageAttendanceApis, ManageClassApis } from '../../apis/ListApi';
import moment from 'moment';
import { TiWarning } from 'react-icons/ti';
import classes from './AttendanceDetail.module.css';
import TakeAttendance from '../../screens/Sro/Manage Class/Attendance/TakeAttendance/TakeAttendance';
import ViewAllAttendance from '../../screens/Sro/Manage Class/Attendance/ViewAllAttendance/ViewAllAttendance';
import { Fragment } from 'react';
import { BsFillPersonCheckFill } from 'react-icons/bs';
import { RiBookmark3Fill, RiEyeFill } from 'react-icons/ri';
import ViewGrade from '../../screens/Sro/Manage Class/ViewGrade/ViewGrade';
import { toast } from 'react-hot-toast';
import { ModulesApis } from '../../apis/ListApi';

const renderGender = (id) => {
  if (id === 1) {
    return (
      <Badge variant="flat" color="success">
        Nam
      </Badge>
    );
  } else if (id === 2) {
    return (
      <Badge variant="flat" color="error">
        Nữ
      </Badge>
    );
  } else if (id === 3) {
    return (
      <Badge variant="flat" color="default">
        Khác
      </Badge>
    );
  } else {
    return (
      <Badge variant="flat" color="default">
        Không xác định
      </Badge>
    );
  }
};

const AttendanceDetail = () => {
  const [listSession, setListSession] = useState(undefined);
  const [listStudent, setListStudent] = useState(undefined);
  const [loading, setLoading] = useState(true);
  const [selectSession, setSelectSession] = useState(undefined);
  const [viewAttendance, setViewAttendance] = useState(false);
  const [module, setModule] = useState(undefined);

  const [viewSlot, setViewSlot] = useState(false);
  const [viewGrade, setViewGrade] = useState(false);

  const { id, moduleId, scheduleId } = useParams();
  const navigate = useNavigate();

  const getListSession = () => {
    setLoading(true);
    FetchApi(ManageAttendanceApis.teacherGetListSessions, null, null, [
      String(scheduleId),
    ])
      .then((res) => {
        setListSession(res.data);
        setLoading(false);
      })
      .catch((err) => {
        toast.error('Lỗi lấy danh sách buổi học');
        setLoading(false);
      });
  };

  const getListStudent = () => {
    FetchApi(ManageClassApis.getStudentByClassId, null, null, [String(id)])
      .then((res) => {
        setListStudent(res.data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy danh sách học viên');
      });
  };

  const getInformationModule = () => {
    FetchApi(ModulesApis.getModuleByID, null, null, [String(moduleId)])
      .then((res) => {
        setModule(res.data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy thông tin môn học');
      });
  };

  useEffect(() => {
    getListSession();
    getListStudent();
    getInformationModule();
  }, [id, moduleId]);

  const handleCloseModalAttendance = () => {
    setSelectSession(undefined);
  };

  const handleCloseViewAttendance = () => {
    setViewAttendance(false);
  };

  const handleSelectOption = (key) => {
    switch (key) {
      case 'slot':
        setViewSlot(true);
        break;
      case 'grade':
        setViewGrade(true);
        break;
      default:
        break;
    }
  };

  return (
    <Fragment>
      {selectSession && (
        <TakeAttendance
          session={selectSession}
          scheduleId={scheduleId}
          onClose={handleCloseModalAttendance}
        />
      )}
      {viewAttendance && (
        <ViewAllAttendance
          open={viewAttendance}
          scheduleId={scheduleId}
          onClose={handleCloseViewAttendance}
        />
      )}
      <Card
        variant="bordered"
        css={{
          height: 'fit-content',
        }}
      >
        <Card.Header>
          <Grid.Container gap={2} alignItems={'center'}>
            <Grid xs={4}>
              {(viewGrade || viewSlot) && (
                <Button
                  color={'error'}
                  size={'sm'}
                  onClick={() => {
                    setViewGrade(false);
                    setViewSlot(false);
                  }}
                  flat
                  auto
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
                  textAlign: 'center',
                  width: '100%',
                }}
              >
                {!viewGrade && !viewSlot && 'Danh sách học viên'}
                {viewGrade && 'Cập nhật điểm'}
                {viewSlot && 'Danh sách lịch học'}
              </Text>
            </Grid>
            <Grid xs={4} justify={'flex-end'}>
              {!(viewGrade || viewSlot) && (
                <Dropdown>
                  <Dropdown.Button color={'secondary'} flat size={'sm'}>
                    Hành động
                  </Dropdown.Button>
                  <Dropdown.Menu
                    aria-label="Static Actions"
                    onAction={handleSelectOption}
                  >
                    <Dropdown.Item
                      key="slot"
                      description="Danh sách lịch học"
                      color={'success'}
                      icon={<BsFillPersonCheckFill />}
                    >
                      Lịch học
                    </Dropdown.Item>
                    <Dropdown.Item
                      key="grade"
                      description="Cập nhật điểm"
                      color={'warning'}
                      icon={<RiBookmark3Fill />}
                    >
                      Điểm
                    </Dropdown.Item>
                  </Dropdown.Menu>
                </Dropdown>
              )}
              {viewSlot && (
                <Button
                  color={'success'}
                  size={'sm'}
                  onClick={() => {
                    setViewAttendance(true);
                  }}
                  flat
                  auto
                >
                  Xem điểm danh
                </Button>
              )}
            </Grid>
          </Grid.Container>
        </Card.Header>
        <Card.Body>
          {loading && <Loading />}
          {!viewGrade &&
            !viewSlot &&
            listStudent !== undefined &&
            listStudent.length !== 0 && (
              <Table>
                <Table.Header>
                  <Table.Column>MSSV</Table.Column>
                  <Table.Column>Họ và tên</Table.Column>
                  <Table.Column>Ngày sinh</Table.Column>
                  <Table.Column>Email tổ chức</Table.Column>
                  <Table.Column>Giới tính</Table.Column>
                  <Table.Column></Table.Column>
                </Table.Header>
                <Table.Body>
                  {listStudent.map((item, index) => (
                    <Table.Row key={index}>
                      <Table.Cell>{item.enroll_number}</Table.Cell>
                      <Table.Cell>
                        {item.first_name + ' ' + item.last_name}
                      </Table.Cell>
                      <Table.Cell>
                        {new Date(item.birthday).toLocaleDateString('vi-VN')}
                      </Table.Cell>
                      <Table.Cell>{item.email_organization}</Table.Cell>
                      <Table.Cell>{renderGender(item.gender.id)}</Table.Cell>
                      <Table.Cell>
                        <RiEyeFill
                          size={20}
                          color="5EA2EF"
                          style={{ cursor: 'pointer' }}
                          onClick={() => {
                            navigate(`/teacher/student/${item.user_id}`);
                          }}
                        />
                      </Table.Cell>
                    </Table.Row>
                  ))}
                </Table.Body>
                <Table.Pagination
                  shadow
                  noMargin
                  align="center"
                  rowsPerPage={10}
                />
              </Table>
            )}
          {!loading && viewGrade && (
            <ViewGrade dataModule={module} role={'teacher'} />
          )}
          {!loading && viewSlot && (
            <Grid.Container gap={2}>
              {listSession
                ?.sort(
                  (a, b) =>
                    new Date(a.learning_date) - new Date(b.learning_date)
                )
                .map((data, index) => {
                  return (
                    <Grid key={index} xs={2}>
                      <Card
                        variant={
                          moment(data.learning_date).toDate() -
                            moment().toDate() <
                          0
                            ? 'bordered'
                            : 'flat'
                        }
                        isPressable={
                          moment(data.learning_date).toDate() -
                            moment().toDate() <
                          0
                        }
                        onPress={() => {
                          setSelectSession(data);
                        }}
                        css={{
                          height: 'fit-content',
                          border:
                            new Date(data.learning_date).toLocaleDateString(
                              'vi-VN'
                            ) ===
                              new Date(Date.now()).toLocaleDateString(
                                'vi-VN'
                              ) && '2px solid red',
                          cursor:
                            moment(data.learning_date).toDate() -
                              moment().toDate() <
                            0
                              ? 'pointer'
                              : 'no-drop',
                        }}
                      >
                        <Card.Body
                          css={{
                            padding: '6px 12px',
                            backgroundColor:
                              data.session_type === 3 || data.session_type === 4
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
                              {new Date(data.learning_date).toLocaleDateString(
                                'vi-VN'
                              )}
                            </Text>
                          </div>
                          <Spacer y={0.6} />
                          <Text
                            p
                            size={12}
                            color={
                              data.session_type === 3 || data.session_type === 4
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
          )}
        </Card.Body>
      </Card>
    </Fragment>
  );
};

export default AttendanceDetail;
