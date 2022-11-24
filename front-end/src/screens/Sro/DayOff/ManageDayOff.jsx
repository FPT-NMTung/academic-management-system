import {
  Button,
  Card,
  Grid,
  Loading,
  Modal,
  Table,
  Text,
} from '@nextui-org/react';
import { Calendar, Select, Row, Col, Form, Input } from 'antd';
import { Fragment, useState } from 'react';
import moment from 'moment';
import 'moment/locale/vi';
import { useEffect } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ManageDayOffApis, ManageTeacherApis } from '../../../apis/ListApi';
import toast from 'react-hot-toast';
import classes from './ManageDayOff.module.css';
import { MdDelete } from 'react-icons/md';
import { ErrorCodeApi } from '../../../apis/ErrorCodeApi';

const ManageDayOff = () => {
  const [listDayOff, setListDayOff] = useState([]);
  const [value, setValue] = useState(moment());
  const [selectedValue, setSelectedValue] = useState(moment());
  const [detailDayOff, setDetailDayOff] = useState(undefined);
  const [listTeacher, setListTeacher] = useState([]);
  const [isOpenCreateDayOff, setIsOpenCreateDayOff] = useState(false);

  const getListDayOff = () => {
    FetchApi(ManageDayOffApis.getDayOff, null, null, null)
      .then((res) => {
        setListDayOff(res.data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy danh sách ngày nghỉ');
      });
  };

  const getListTeacher = () => {
    FetchApi(ManageTeacherApis.getListTeacherBySro, null, null, null)
      .then((res) => {
        setListTeacher(
          res.data.map((item) => {
            return {
              name: item.first_name + ' ' + item.last_name,
              id: item.user_id,
            };
          })
        );
      })
      .catch((err) => {
        toast.error('Lỗi lấy danh sách giáo viên');
      });
  };

  const getDetail = (isShowReload) => {
    if (isShowReload) {
      setDetailDayOff(undefined);
    }

    const body = {
      date: moment.utc(selectedValue).local().format(),
    };

    FetchApi(ManageDayOffApis.getDetail, body, null, null)
      .then((res) => {
        const dayOff = {
          morning: [],
          afternoon: [],
          evening: [],
        };
        const { data } = res;

        data.forEach((item) => {
          if (item.working_time_id === 1) {
            dayOff.morning.push(item);
          } else if (item.working_time_id === 2) {
            dayOff.afternoon.push(item);
          } else {
            dayOff.evening.push(item);
          }
        });

        setDetailDayOff(dayOff);
      })
      .catch((err) => {
        toast.error('Lỗi lấy chi tiết ngày nghỉ');
      });
  };

  useEffect(() => {
    getDetail(true);
  }, [selectedValue]);

  useEffect(() => {
    getListDayOff();
    getListTeacher();
  }, []);

  const handleFormSubmit = (e) => {
    const body = {
      title: e.title,
      date: moment.utc(selectedValue).local().format(),
      teacher_id: e.teacher_id,
      working_time_ids: e.wk_time,
    };

    toast.promise(FetchApi(ManageDayOffApis.createDayOff, body, null, null), {
      loading: 'Đang tạo ngày nghỉ',
      success: (res) => {
        setIsOpenCreateDayOff(false);
        getListDayOff();
        getDetail(false);
        return 'Tạo ngày nghỉ thành công';
      },
      error: (err) => {
        if (err?.type_error) {
          return ErrorCodeApi[err.type_error];
        }
        return 'Lỗi tạo ngày nghỉ';
      },
    });
  };

  const handleDeleteDayOff = (id) => {
    toast.promise(
      FetchApi(ManageDayOffApis.deleteDayOff, null, null, [String(id)]),
      {
        loading: 'Đang xóa ngày nghỉ',
        success: (res) => {
          getListDayOff();
          getDetail(false);
          return 'Xóa ngày nghỉ thành công';
        },
        error: (err) => {
          if (err?.type_error) {
            return ErrorCodeApi[err.type_error];
          }
          return 'Lỗi xóa ngày nghỉ';
        },
      }
    );
  };

  return (
    <Fragment>
      <Modal
        open={isOpenCreateDayOff}
        width="500px"
        blur
        closeButton
        onClose={() => {
          setIsOpenCreateDayOff(false);
        }}
      >
        <Modal.Header>
          <Text p b size={14}>
            Thêm ngày nghỉ
          </Text>
        </Modal.Header>
        <Modal.Body>
          <Text p i color="error">
            Lưu ý: Ngày nghỉ được thêm có thể ảnh hưởng đến các lịch học. Nếu
            thêm nhầm ngày nghỉ, khi xoá sẽ không khôi phục lại được lịch học,
            bạn sẽ phải cập nhật lịch học thủ công.
          </Text>
          <Form
            labelCol={{ span: 6 }}
            wrapperCol={{ span: 16 }}
            onFinish={handleFormSubmit}
          >
            <Form.Item
              name={'title'}
              label="Lý do"
              rules={[
                {
                  required: true,
                  validator: (_, value) => {
                    if (value === null || value === undefined) {
                      return Promise.reject('Trường phải từ 1 đến 255 ký tự');
                    }
                    if (value.trim().length < 1 || value.trim().length > 255) {
                      return Promise.reject(
                        new Error('Trường phải từ 1 đến 255 ký tự')
                      );
                    }
                    return Promise.resolve();
                  },
                },
                {
                  whitespace: true,
                  message: 'Trường không được chứa khoảng trắng',
                },
              ]}
            >
              <Input />
            </Form.Item>
            <Form.Item name={'teacher_id'} label="Giáo viên">
              <Select dropdownStyle={{ zIndex: 9999 }}>
                {listTeacher.map((item) => {
                  return (
                    <Select.Option key={item.id} value={item.id}>
                      {item.name}
                    </Select.Option>
                  );
                })}
              </Select>
            </Form.Item>
            <Form.Item
              name={'wk_time'}
              label="Ca học"
              rules={[
                {
                  required: true,
                  message: 'Trường này không được để trống',
                },
              ]}
            >
              <Select dropdownStyle={{ zIndex: 9999 }} mode="multiple">
                <Select.Option value={1}>Sáng</Select.Option>
                <Select.Option value={2}>Chiều</Select.Option>
                <Select.Option value={3}>Tối</Select.Option>
              </Select>
            </Form.Item>
            <div className={classes.buttonSubmit}>
              <Button flat auto type="primary" htmlType="submit">
                Thêm ngày nghỉ
              </Button>
            </div>
          </Form>
        </Modal.Body>
      </Modal>
      <Grid.Container gap={2} justify={'center'}>
        <Grid xs={3}>
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
                size={14}
                css={{
                  width: '100%',
                  textAlign: 'center',
                }}
              >
                Lịch nghỉ
              </Text>
            </Card.Header>
            <Card.Body
              css={{
                padding: '5px 10px',
              }}
            >
              <Calendar
                fullscreen={false}
                value={value}
                onSelect={(value) => {
                  setValue(value);
                  setSelectedValue(value);
                }}
                headerRender={({ value, onChange }) => {
                  const start = 0;
                  const end = 12;
                  const monthOptions = [];

                  const months = [
                    'Tháng 1',
                    'Tháng 2',
                    'Tháng 3',
                    'Tháng 4',
                    'Tháng 5',
                    'Tháng 6',
                    'Tháng 7',
                    'Tháng 8',
                    'Tháng 9',
                    'Tháng 10',
                    'Tháng 11',
                    'Tháng 12',
                  ];

                  for (let i = start; i < end; i++) {
                    monthOptions.push(
                      <Select.Option key={i} value={i} className="month-item">
                        {months[i]}
                      </Select.Option>
                    );
                  }

                  const year = value.year();
                  const month = value.month();
                  const options = [];
                  for (let i = year - 10; i < year + 10; i += 1) {
                    options.push(
                      <Select.Option key={i} value={i} className="year-item">
                        {i}
                      </Select.Option>
                    );
                  }
                  return (
                    <div style={{ padding: 8 }}>
                      <Row gutter={8}>
                        <Col>
                          <Select
                            dropdownMatchSelectWidth={false}
                            value={year}
                            onChange={(newYear) => {
                              const now = value.clone().year(newYear);
                              onChange(now);
                            }}
                          >
                            {options}
                          </Select>
                        </Col>
                        <Col>
                          <Select
                            dropdownMatchSelectWidth={false}
                            value={month}
                            onChange={(newMonth) => {
                              const now = value.clone().month(newMonth);
                              onChange(now);
                            }}
                          >
                            {monthOptions}
                          </Select>
                        </Col>
                      </Row>
                    </div>
                  );
                }}
                mode="month"
                dateFullCellRender={(value) => {
                  return (
                    <Card
                      variant={value.day() === 0 ? 'flat' : 'bordered'}
                      disableRipple={true}
                      css={{
                        fontSize: '12px',
                        width: '40px',
                        height: '40px',
                        margin: '1px auto',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        color:
                          selectedValue.format('DD/MM/YYYY') ===
                          value.format('DD/MM/YYYY')
                            ? '#0072F5'
                            : selectedValue.format('MM/YYYY') ===
                              value.format('MM/YYYY')
                            ? 'black'
                            : 'lightgray',
                        fontWeight:
                          selectedValue.format('MM/YYYY') ===
                          value.format('MM/YYYY')
                            ? '500'
                            : '200',
                        backgroundColor:
                          value.format('DD/MM/YYYY') ===
                          selectedValue.format('DD/MM/YYYY')
                            ? '#CEE4FE'
                            : listDayOff.find(
                                (item) =>
                                  value.format('DD/MM/YYYY') ===
                                  moment(item).format('DD/MM/YYYY')
                              )
                            ? '#fdd8e5'
                            : value.day() === 0
                            ? '#F1F1F1'
                            : '#fff',
                        borderRadius: '10em',
                        borderColor:
                          value.format('DD/MM/YYYY') ===
                          moment().format('DD/MM/YYYY')
                            ? '#0072F5'
                            : '',
                      }}
                      isPressable={true}
                    >
                      {value.date()}
                    </Card>
                  );
                }}
              />
            </Card.Body>
          </Card>
        </Grid>
        <Grid xs={5}>
          <Card
            variant="bordered"
            css={{
              height: 'fit-content',
            }}
          >
            <Card.Header>
              <Grid.Container>
                <Grid xs={8}>
                  <Text
                    p
                    b
                    size={14}
                    css={{
                      width: '100%',
                    }}
                  >
                    Lịch nghỉ của ngày {selectedValue.format('DD/MM/YYYY')}
                  </Text>
                </Grid>
                <Grid xs={4} justify={'flex-end'}>
                  <Button
                    size={'sm'}
                    flat
                    auto
                    onPress={() => {
                      setIsOpenCreateDayOff(true);
                    }}
                  >
                    + Thêm lịch nghỉ
                  </Button>
                </Grid>
              </Grid.Container>
            </Card.Header>
            <Card.Body
              css={{
                paddingTop: '0px',
              }}
            >
              {!detailDayOff && (
                <div className={classes.loadingDiv}>
                  <Loading size="sm" />
                </div>
              )}
              {detailDayOff && (
                <Grid.Container gap={2}>
                  <Grid xs={12}>
                    <Card
                      variant="bordered"
                      css={{
                        minHeight: '140px',
                        borderStyle: 'dashed',
                      }}
                    >
                      <Card.Header>
                        <Text p b size={14}>
                          Buổi sáng
                        </Text>
                      </Card.Header>
                      <Card.Body
                        css={{
                          padding: '5px 10px',
                        }}
                      >
                        {detailDayOff?.morning.length === 0 && (
                          <Text
                            p
                            size={14}
                            css={{
                              textAlign: 'center',
                            }}
                            color={'lightGray'}
                            i
                          >
                            Không có lịch nghỉ
                          </Text>
                        )}
                        {detailDayOff?.morning[0]?.teacher_id === null && (
                          <Fragment>
                            <Text
                              p
                              size={14}
                              css={{
                                textAlign: 'center',
                              }}
                            >
                              <b>Lịch nghỉ chung của cơ sở: </b>
                              {detailDayOff?.morning[0]?.title}
                            </Text>
                            <div>
                              <Button
                                color={'error'}
                                size={'sm'}
                                auto
                                flat
                                css={{
                                  margin: '10px auto 0',
                                }}
                                onPress={() => {
                                  handleDeleteDayOff(
                                    detailDayOff?.morning[0]?.id
                                  );
                                }}
                              >
                                Xoá
                              </Button>
                            </div>
                          </Fragment>
                        )}
                        {detailDayOff !== undefined &&
                          detailDayOff.morning[0] !== undefined &&
                          detailDayOff.morning[0].teacher_id !== null && (
                            <Table
                              css={{ padding: 0 }}
                              lined
                              headerLined
                              shadow={false}
                            >
                              <Table.Header>
                                <Table.Column>Giáo viên</Table.Column>
                                <Table.Column>Lý do</Table.Column>
                                <Table.Column
                                  css={{
                                    width: '10px',
                                  }}
                                >
                                  Xoá
                                </Table.Column>
                              </Table.Header>
                              <Table.Body>
                                {detailDayOff?.morning.map((item) => (
                                  <Table.Row>
                                    <Table.Cell>
                                      {item.teacher_first_name}{' '}
                                      {item.teacher_last_name}
                                    </Table.Cell>
                                    <Table.Cell>{item.title}</Table.Cell>
                                    <Table.Cell
                                      css={{
                                        display: 'flex',
                                        justifyContent: 'center',
                                      }}
                                    >
                                      <MdDelete
                                        className={classes.buttonDelete}
                                        size={16}
                                        color={'#F31260'}
                                        onClick={() => {
                                          handleDeleteDayOff(item.id);
                                        }}
                                      />
                                    </Table.Cell>
                                  </Table.Row>
                                ))}
                              </Table.Body>
                            </Table>
                          )}
                      </Card.Body>
                    </Card>
                  </Grid>
                  <Grid xs={12}>
                    <Card
                      variant="bordered"
                      css={{
                        minHeight: '140px',
                        borderStyle: 'dashed',
                      }}
                    >
                      <Card.Header>
                        <Text p b size={14}>
                          Buổi chiều
                        </Text>
                      </Card.Header>
                      <Card.Body
                        css={{
                          padding: '5px 10px',
                        }}
                      >
                        {detailDayOff?.afternoon.length === 0 && (
                          <Text
                            p
                            size={14}
                            css={{
                              textAlign: 'center',
                            }}
                            color={'lightGray'}
                            i
                          >
                            Không có lịch nghỉ
                          </Text>
                        )}
                        {detailDayOff?.afternoon[0]?.teacher_id === null && (
                          <Fragment>
                            <Text
                              p
                              size={14}
                              css={{
                                textAlign: 'center',
                              }}
                            >
                              <b>Lịch nghỉ chung của cơ sở: </b>
                              {detailDayOff?.afternoon[0]?.title}
                            </Text>
                            <div>
                              <Button
                                color={'error'}
                                size={'sm'}
                                auto
                                flat
                                css={{
                                  margin: '10px auto 0',
                                }}
                                onPress={() => {
                                  handleDeleteDayOff(
                                    detailDayOff?.afternoon[0]?.id
                                  );
                                }}
                              >
                                Xoá
                              </Button>
                            </div>
                          </Fragment>
                        )}
                        {detailDayOff !== undefined &&
                          detailDayOff.afternoon[0] !== undefined &&
                          detailDayOff.afternoon[0].teacher_id !== null && (
                            <Table
                              css={{ padding: 0 }}
                              lined
                              headerLined
                              shadow={false}
                            >
                              <Table.Header>
                                <Table.Column>Giáo viên</Table.Column>
                                <Table.Column>Lý do</Table.Column>
                                <Table.Column
                                  css={{
                                    width: '10px',
                                  }}
                                >
                                  Xoá
                                </Table.Column>
                              </Table.Header>
                              <Table.Body>
                                {detailDayOff?.afternoon.map((item) => (
                                  <Table.Row>
                                    <Table.Cell>
                                      {item.teacher_first_name}{' '}
                                      {item.teacher_last_name}
                                    </Table.Cell>
                                    <Table.Cell>{item.title}</Table.Cell>
                                    <Table.Cell
                                      css={{
                                        display: 'flex',
                                        justifyContent: 'center',
                                      }}
                                    >
                                      <MdDelete
                                        className={classes.buttonDelete}
                                        size={16}
                                        color={'#F31260'}
                                        onClick={() => {
                                          handleDeleteDayOff(item.id);
                                        }}
                                      />
                                    </Table.Cell>
                                  </Table.Row>
                                ))}
                              </Table.Body>
                            </Table>
                          )}
                      </Card.Body>
                    </Card>
                  </Grid>
                  <Grid xs={12}>
                    <Card
                      variant="bordered"
                      css={{
                        minHeight: '140px',
                        borderStyle: 'dashed',
                      }}
                    >
                      <Card.Header>
                        <Text p b size={14}>
                          Buổi tối
                        </Text>
                      </Card.Header>
                      <Card.Body
                        css={{
                          padding: '5px 10px',
                        }}
                      >
                        {detailDayOff?.evening.length === 0 && (
                          <Text
                            p
                            size={14}
                            css={{
                              textAlign: 'center',
                            }}
                            color={'lightGray'}
                            i
                          >
                            Không có lịch nghỉ
                          </Text>
                        )}
                        {detailDayOff?.evening[0]?.teacher_id === null && (
                          <Fragment>
                            <Text
                              p
                              size={14}
                              css={{
                                textAlign: 'center',
                              }}
                            >
                              <b>Lịch nghỉ chung của cơ sở: </b>
                              {detailDayOff?.evening[0]?.title}
                            </Text>
                            <div>
                              <Button
                                color={'error'}
                                size={'sm'}
                                auto
                                flat
                                css={{
                                  margin: '10px auto 0',
                                }}
                                onPress={() => {
                                  handleDeleteDayOff(
                                    detailDayOff?.evening[0]?.id
                                  );
                                }}
                              >
                                Xoá
                              </Button>
                            </div>
                          </Fragment>
                        )}
                        {detailDayOff !== undefined &&
                          detailDayOff.evening[0] !== undefined &&
                          detailDayOff.evening[0].teacher_id !== null && (
                            <Table
                              css={{ padding: 0 }}
                              lined
                              headerLined
                              shadow={false}
                            >
                              <Table.Header>
                                <Table.Column>Giáo viên</Table.Column>
                                <Table.Column>Lý do</Table.Column>
                                <Table.Column
                                  css={{
                                    width: '10px',
                                  }}
                                >
                                  Xoá
                                </Table.Column>
                              </Table.Header>
                              <Table.Body>
                                {detailDayOff?.evening.map((item) => (
                                  <Table.Row>
                                    <Table.Cell>
                                      {item.teacher_first_name}{' '}
                                      {item.teacher_last_name}
                                    </Table.Cell>
                                    <Table.Cell>{item.title}</Table.Cell>
                                    <Table.Cell
                                      css={{
                                        display: 'flex',
                                        justifyContent: 'center',
                                      }}
                                    >
                                      <MdDelete
                                        className={classes.buttonDelete}
                                        size={16}
                                        color={'#F31260'}
                                        onClick={() => {
                                          handleDeleteDayOff(item.id);
                                        }}
                                      />
                                    </Table.Cell>
                                  </Table.Row>
                                ))}
                              </Table.Body>
                            </Table>
                          )}
                      </Card.Body>
                    </Card>
                  </Grid>
                </Grid.Container>
              )}
            </Card.Body>
          </Card>
        </Grid>
      </Grid.Container>
    </Fragment>
  );
};

export default ManageDayOff;
