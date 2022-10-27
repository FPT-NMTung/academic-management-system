import {
  Badge,
  Button,
  Card,
  Grid,
  Loading,
  Modal,
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
import toast from 'react-hot-toast';
import { Fragment } from 'react';
import { Upload } from 'antd';
import { FcFile } from 'react-icons/fc';
import { ErrorCodeApi } from '../../../../apis/ErrorCodeApi';
import { RiEyeFill } from 'react-icons/ri';
import { MdDelete } from 'react-icons/md';

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

const DetailClass = () => {
  const [dataClass, setDataClass] = useState(undefined);
  const [isOpenModal, setIsOpenModal] = useState(false);
  const [listStudent, setListStudent] = useState(undefined);

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

  const getAllStudent = () => {
    setListStudent(undefined);
    FetchApi(ManageClassApis.getStudentByClassId, null, null, [String(id)])
      .then((res) => {
        setListStudent(res.data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy danh sách học viên');
      });
  };

  const handleDownload = () => {
    toast.promise(
      FetchApi(ManageClassApis.downloadTemplate, null, null, null),
      {
        loading: 'Đang tải xuống...',
        success: (blob) => {
          const url = window.URL.createObjectURL(new Blob([blob]));
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', 'template.xlsx');
          document.body.appendChild(link);
          link.click();
          link.parentNode.removeChild(link);

          return 'Tải xuống thành công';
        },
        error: 'Tải xuống thất bại',
      }
    );
  };

  const handleOpenModal = () => {
    setIsOpenModal(true);
  };

  const handleUploadFile = (e) => {
    const a = new FormData();
    a.append('file', e.file);

    setIsOpenModal(false);
    toast.promise(
      FetchApi(ManageClassApis.importStudent, a, undefined, [String(id)]),
      {
        loading: 'Đang tải lên...',
        success: (res) => {
          getAllStudent();
          return 'Tải lên thành công';
        },
        error: (err) => {
          return ErrorCodeApi[err.type_error];
        },
      }
    );
  };

  const handleClear = () => {
    toast.promise(
      FetchApi(ManageClassApis.clearStudent, null, undefined, [String(id)]),
      {
        loading: 'Đang xóa...',
        success: (res) => {
          getAllStudent();
          return 'Xóa thành công';
        },
        error: (err) => {
          return ErrorCodeApi[err.type_error];
        },
      }
    );
  };

  useEffect(() => {
    getData();
    getAllStudent();
  }, []);

  const isDraft = () => {
    if (listStudent === undefined) {
      return false;
    }

    if (listStudent.length === 0) {
      return true;
    }

    if (listStudent.find((item) => item.is_draft)) {
      return true;
    }

    return false;
  };

  const handleSave = () => {
    toast.promise(
      FetchApi(ManageClassApis.saveStudent, null, undefined, [String(id)]),
      {
        loading: 'Đang lưu...',
        success: (res) => {
          getAllStudent();
          return 'Lưu thành công';
        },
        error: (err) => {
          return ErrorCodeApi[err.type_error];
        },
      }
    );
  }

  return (
    <Fragment>
      <Modal
        open={isOpenModal}
        width="500px"
        blur
        onClose={() => {
          setIsOpenModal(false);
        }}
      >
        <Modal.Body
          css={{
            padding: '20px',
          }}
        >
          <Upload.Dragger
            showUploadList={false}
            customRequest={handleUploadFile}
            multiple={false}
          >
            <div className={classes.upload}>
              {
                <Fragment>
                  <FcFile size={50} />
                  <Spacer y={1} />
                  <Text p size={14}>
                    Kéo thả file vào đây hoặc <Text b>chọn file</Text>
                  </Text>
                </Fragment>
              }
            </div>
          </Upload.Dragger>
        </Modal.Body>
      </Modal>
      <Grid.Container gap={2}>
        <Grid
          xs={3}
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
        <Grid xs={9} direction={'column'}>
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
                {isDraft() && (
                  <div className={classes.groupButton}>
                    <Button
                      onPress={handleDownload}
                      flat
                      auto
                      icon={<FaCloudDownloadAlt size={20} />}
                    >
                      Tải xuống Template
                    </Button>
                    {isDraft() && <Button
                      flat
                      auto
                      color={'warning'}
                      icon={<FaCloudUploadAlt size={20} />}
                      onPress={handleSave}
                    >
                      Lưu lại
                    </Button>}
                    {listStudent?.length === 0 && <Button
                      flat
                      auto
                      color={'success'}
                      icon={<FaCloudUploadAlt size={20} />}
                      onPress={handleOpenModal}
                    >
                      Import
                    </Button>}
                    {listStudent?.length !== 0 && <Button
                      flat
                      auto
                      color={'error'}
                      icon={<MdDelete size={20} />}
                      onPress={handleClear}
                    >
                      Clear
                    </Button>}
                  </div>
                )}
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
            {listStudent === undefined && (
              <div
                style={{
                  height: '400px',
                  display: 'flex',
                  justifyContent: 'center',
                  alignItems: 'center',
                }}
              >
                <Loading />
              </div>
            )}
            {listStudent !== undefined && listStudent.length === 0 && (
              <div
                style={{
                  height: '400px',
                  display: 'flex',
                  justifyContent: 'center',
                  alignItems: 'center',
                }}
              >
                <Text
                  i
                  p
                  size={14}
                  css={{ width: '100%', textAlign: 'center' }}
                >
                  Không có dữ liệu học viên trong lớp học này
                </Text>
              </div>
            )}
            {listStudent !== undefined && listStudent.length !== 0 && (
              <Table>
                <Table.Header>
                  <Table.Column>MSSV</Table.Column>
                  <Table.Column>Họ và tên</Table.Column>
                  <Table.Column>Ngày sinh</Table.Column>
                  <Table.Column>Email</Table.Column>
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
                      <Table.Cell>{item.email}</Table.Cell>
                      <Table.Cell>{item.email_organization}</Table.Cell>
                      <Table.Cell>{renderGender(item.gender.id)}</Table.Cell>
                      <Table.Cell>
                        <RiEyeFill
                          size={20}
                          color="5EA2EF"
                          style={{ cursor: 'pointer' }}
                          onClick={() => {}}
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
          </Card>
        </Grid>
      </Grid.Container>
    </Fragment>
  );
};

export default DetailClass;
