import {
  Badge,
  Button,
  Card,
  Dropdown,
  Grid,
  Loading,
  Modal,
  Spacer,
  Table,
  Text,
  Tooltip,
} from '@nextui-org/react';
import classes from './DetailClass.module.css';
import { FaCloudDownloadAlt, FaCloudUploadAlt } from 'react-icons/fa';
import { IoArrowBackCircle } from 'react-icons/io5';
import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import FetchApi from '../../../../apis/FetchApi';
import { ManageClassApis, ManageStudentApis } from '../../../../apis/ListApi';
import { Descriptions, Skeleton, Form, Select, Upload, Input } from 'antd';
import toast from 'react-hot-toast';
import { Fragment } from 'react';
import { FcFile } from 'react-icons/fc';
import { ErrorCodeApi } from '../../../../apis/ErrorCodeApi';
import { RiEyeFill } from 'react-icons/ri';
import { MdDelete, MdPersonAdd, MdSave } from 'react-icons/md';
import { TiWarning } from 'react-icons/ti';
import { AiFillSchedule } from 'react-icons/ai';
import { TiFlowMerge } from 'react-icons/ti';

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
  const [isOpenModalMerge, setIsOpenModalMerge] = useState(false);
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const [currentClass, setCurrentClass] = useState(null);
  const [listClassToMerge, setListClassToMerge] = useState([]);
  const [messageFailed, setMessageFailed] = useState(undefined);

  const { id } = useParams();

  // const getCurrentClass = () => {
  //   // setIsLoading(true);
  //   FetchApi(ManageStudentApis.getCurrentClasss, null, null, [`${id}`])
  //     .then((res) => {
  //       setCurrentClass(res.data.name);
  //       // setIsLoading(false);
  //     })
  //     .catch(() => {
  //       navigate("/404");
  //     });
  // };
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
    } else if (id === 5) {
      return (
        <Badge variant="flat" color="default">
          Chưa lên lịch
        </Badge>
      );
    } else {
      return (
        <Badge variant="flat" color="success">
          Đã ghép
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
  const getAllClassToMerge = () => {
    FetchApi(ManageClassApis.getAvailableClassToMerge, null, null, [String(id)])
      .then((res) => {
        setListClassToMerge(res.data);
        console.log(res.data);
      })
      .catch(() => {
        navigate('/404');
      });
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
      return true;
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
  };
  const handleOpenModalMerge = () => {
    setIsOpenModalMerge(true);
    getAllClassToMerge();
    // getCurrentClass();
  };
  const handleSelectOption = (key) => {
    switch (key) {
      case 'add':
        navigate(`/sro/manage-class/${id}/add`);
        break;
      case 'import':
        handleOpenModal();
        break;
      case 'download':
        handleDownload();
        break;
      case 'clear':
        handleClear();
        break;
      case 'save':
        handleSave();
        break;
      case 'schedule':
        navigate(`/sro/manage-class/${id}/schedule`);
        break;
      case 'merge':
        handleOpenModalMerge();
        break;
      default:
        break;
    }
  };
  const handleSubmitFormMergeClass = (e) => {
    form.resetFields();
    console.log(dataClass.id);
    console.log(e.merge_class);
    const body = {
      first_class_id: dataClass.id,
      second_class_id: e.merge_class,
    };

    toast.promise(FetchApi(ManageClassApis.mergeClass, body, null, null), {
      loading: 'Đang cập nhật...',
      success: (res) => {
        setIsOpenModalMerge(false);
        getAllStudent();
        getData();

        return 'Cập nhật thành công';
      },
      error: (err) => {
        setMessageFailed(ErrorCodeApi[err.type_error]);
        if (err?.type_error) {
          return 'Thất bại ! ' + ErrorCodeApi[err.type_error];
        }
        return 'Cập nhật thất bại';
      },
    });
  };

  const hasAcitveStudent = () => {
    if (listStudent === undefined || listStudent === null) {
      return false;
    }

    if (listStudent.length === 0) {
      return false;
    }

    if (listStudent.find((item) => item.is_draft === false)) {
      return true;
    }

    return false;
  }

  return (
    <Fragment>
      <Modal
        open={isOpenModalMerge}
        width="500px"
        blur
        onClose={() => {
          setIsOpenModalMerge(false);
        }}
      >
        <Modal.Header>
          <Text size={16} b>
            Ghép lớp
          </Text>
        </Modal.Header>
        <Modal.Body>
          <Form
            labelCol={{ span: 7 }}
            wrapperCol={{ span: 12 }}
            layout="horizontal"
            onFinish={handleSubmitFormMergeClass}
            form={form}
          >
            <Form.Item name={'current_class'} label={'Lớp hiện tại'}>
              <Input disabled={true} placeholder={dataClass?.name}></Input>
            </Form.Item>
            <Form.Item
              name={'merge_class'}
              label={'Lớp ghép tới'}
              rules={[
                {
                  required: true,
                  message: 'Hãy chọn lớp ghép đến',
                },
              ]}
            >
              <Select
                placeholder="Chọn lớp ghép tới"
                dropdownStyle={{ zIndex: 9999 }}
              >
                {listClassToMerge.map((e) => (
                  <Select.Option key={e.id} value={e.id}>
                    {e.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item wrapperCol={{ offset: 9, span: 99 }}>
              <div
                style={{
                  display: 'flex',
                  gap: '10px',
                }}
              >
                <Button
                  flat
                  auto
                  css={{
                    width: '120px',
                  }}
                  type="primary"
                  htmlType="submit"
                >
                  Cập nhật
                </Button>
              </div>
            </Form.Item>
          </Form>
        </Modal.Body>
      </Modal>
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
              <Grid.Container>
                <Grid xs={4} justify="flex-start">
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
                </Grid>
                <Grid xs={4} justify="center"></Grid>
                <Grid xs={4} justify="flex-end">
                  <Dropdown>
                    <Dropdown.Button flat color="secondary">
                      Chức năng
                    </Dropdown.Button>
                    <Dropdown.Menu
                      onAction={handleSelectOption}
                      color="secondary"
                      aria-label="Actions"
                      css={{ $$dropdownMenuWidth: '340px' }}
                    >
                      <Dropdown.Section title="Cơ bản">
                        <Dropdown.Item
                          key="add"
                          description="Thêm học viên thủ công"
                          icon={<MdPersonAdd />}
                          color={'success'}
                        >
                          Thêm học viên
                        </Dropdown.Item>
                        {listStudent?.length === 0 && (
                          <Dropdown.Item
                            key="import"
                            description="Tải lên danh sách học viên"
                            icon={<FaCloudUploadAlt />}
                            color={'success'}
                          >
                            Import
                          </Dropdown.Item>
                        )}
                        {listStudent?.length === 0 && (
                          <Dropdown.Item
                            key="download"
                            description="Tải xuống file mẫu"
                            icon={<FaCloudDownloadAlt />}
                            color={'primary'}
                          >
                            Download file mẫu
                          </Dropdown.Item>
                        )}
                        {listStudent?.length > 0 && isDraft() && (
                          <Dropdown.Item
                            key="save"
                            description="Lưu danh sách học viên"
                            icon={<MdSave />}
                            color={'warning'}
                          >
                            Lưu
                          </Dropdown.Item>
                        )}
                        {hasAcitveStudent() && (
                          <Dropdown.Item
                            key="schedule"
                            description="Xem tất cả lịch học của lớp"
                            icon={<AiFillSchedule />}
                            color={'success'}
                          >
                            Lịch học
                          </Dropdown.Item>
                        )}
                        {listStudent?.length > 0 && (
                          <Dropdown.Item
                            key="merge"
                            description="Gộp lớp này với lớp khác"
                            icon={<TiFlowMerge />}
                            color={'secondary'}
                          >
                            Gộp lớp học
                          </Dropdown.Item>
                        )}
                        {hasAcitveStudent() && (
                          <Dropdown.Item
                            key="grade"
                            description="Xem điểm của lớp học"
                            icon={<AiFillSchedule />}
                            color={'warning'}
                          >
                            Xem điểm
                          </Dropdown.Item>
                        )}
                      </Dropdown.Section>
                      {listStudent?.length > 0 && isDraft() && (
                        <Dropdown.Section title="Nguy hiểm">
                          <Dropdown.Item
                            key="clear"
                            color={'error'}
                            description="Xóa danh sách học viên nháp"
                            icon={<MdDelete />}
                          >
                            Xoá nháp
                          </Dropdown.Item>
                        </Dropdown.Section>
                      )}
                    </Dropdown.Menu>
                  </Dropdown>
                </Grid>
              </Grid.Container>
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
                  <Table.Column>Email tổ chức</Table.Column>
                  <Table.Column>Giới tính</Table.Column>
                  <Table.Column>
                    <div
                      style={{
                        display: 'flex',
                        justifyContent: 'center',
                      }}
                    >
                      Trạng thái
                    </div>
                  </Table.Column>
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
                        {item.is_draft && (
                          <div
                            style={{
                              display: 'flex',
                              justifyContent: 'center',
                            }}
                          >
                            <Tooltip content={'Bản nháp'} color="warning">
                              <TiWarning size={24} color={'#f1c40f'} />
                            </Tooltip>
                          </div>
                        )}
                      </Table.Cell>
                      <Table.Cell>
                        <RiEyeFill
                          size={20}
                          color="5EA2EF"
                          style={{ cursor: 'pointer' }}
                          onClick={() => {
                            navigate(`/sro/manage/student/${item.user_id}`);
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
          </Card>
        </Grid>
      </Grid.Container>
    </Fragment>
  );
};

export default DetailClass;
