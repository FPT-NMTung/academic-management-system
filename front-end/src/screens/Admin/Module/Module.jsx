import classes from './Module.module.css';
import {
  Card,
  Grid,
  Text,
  Button,
  Loading,
  Modal,
  Badge,
  Table,
} from '@nextui-org/react';
import { Form, Select, Input, Divider } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ModulesApis, CourseModuleSemesterApis } from '../../../apis/ListApi';
import ModuleUpdate from '../../../components/ModuleUpdate/ModuleUpdate';
import ModuleCreate from '../../../components/ModuleCreate/ModuleCreate';
import { Fragment } from 'react';
import ColumnGroup from 'antd/lib/table/ColumnGroup';
import { useNavigate } from 'react-router-dom';
import { FaPen } from 'react-icons/fa';

const TYPE_EXAM = {
  1: 'Lý thuyết',
  2: 'Thực hành',
  3: 'Thực hành và Lý thuyết',
  4: 'Không thi',
};
const TYPE_MODULE = {
  1: 'Lý thuyết',
  2: 'Thực hành',
  3: 'Lý thuyết và Thực hành',
};

const Module = () => {
  const [listModules, setlistModules] = useState([]);
  const [selectedModuleId, setselectedModuleId] = useState(null);
  const [IsLoading, setIsLoading] = useState(true);
  const [isCreate, setisCreate] = useState(false);
  const navigate = useNavigate();
  const [form] = Form.useForm();

  const getData = () => {
    setIsLoading(true);
    const param = {
      moduleName: form.getFieldValue('module_name'),
      courseCode: form.getFieldValue('course_code'),
    };

    const moduletype = form.getFieldValue('module_type');
    const examtype = form.getFieldValue('exam_type');
    const semesterid = form.getFieldValue('semester_id');

    if (moduletype !== null) {
      param.moduleType = moduletype;
    }
    if (examtype !== null) {
      param.examType = examtype;
    }
    if (semesterid !== null) {
      param.semesterId = semesterid;
    }

    FetchApi(ModulesApis.searchModules, null, param, null).then((res) => {
      const data = res.data;
      const mergeModuleRes = data
        .sort(
          (a, b) =>
            -(new Date(a.module.updated_at) - new Date(b.module.updated_at))
        )
        .map((e, index) => {
          return {
            id: e.module.id,
            key: e.course_code + '-' + e.module_id + '-' + e.semester_id,
            index: index + 1,
            center_id: e.module.center_id,
            modulename: e.module.module_name,
            centername: e.module.center.name,
            coursecode: e.course_code,
            hours: e.module.hours,
            days: e.module.days,
            // module_type: TYPE_MODULE[e.module.module_type],
            // exam_type: TYPE_EXAM[e.module.exam_type],
            module_type: e.module.module_type,
            exam_type: e.module.exam_type,
            max_theory_grade: e.module.max_theory_grade,
            max_practical_grade: e.module.max_practical_grade,
            semester_name_portal: e.module.semester_name_portal,
            module_exam_name_portal: e.module.module_exam_name_portal,
            semester: e.semester.name,
            semester_id: e.semester.id,
            created_at: e.module.created_at,
            updated_at: e.module.updated_at,
          };
        });

      setlistModules(mergeModuleRes);
      setIsLoading(false);
    });
  };
  const handleClearInput = () => {
    form.resetFields();
    getData();
  };

  const handleSubmitForm = () => {
    getData();
  };

  const handleAddSuccess = () => {
    setisCreate(false);
    getData();
  };

  const handleUpdateSuccess = () => {
    setselectedModuleId(null);
    getData();
  };
  const renderModuleType = (id) => {
    if (id === 1) {
      return (
        <Badge variant="flat" color="secondary">
          Lý thuyết
        </Badge>
      );
    } else if (id === 2) {
      return (
        <Badge variant="flat" color="primary">
          Thực hành
        </Badge>
      );
    } else {
      return (
        <Badge variant="flat" color="success">
          LT & TH
        </Badge>
      );
    }
  };
  const renderExamType = (id) => {
    if (id === 1) {
      return (
        <Badge variant="flat" color="secondary">
          Lý thuyết
        </Badge>
      );
    } else if (id === 2) {
      return (
        <Badge variant="flat" color="primary">
          Thực hành
        </Badge>
      );
    } else if (id === 3) {
      return (
        <Badge variant="flat" color="success">
          LT & TH
        </Badge>
      );
    } else {
      return (
        <Badge variant="flat" color="default">
          Không thi
        </Badge>
      );
    }
  };

  useEffect(() => {
    getData();
  }, []);

  return (
    <div>
      <Grid.Container gap={2}>
        <Grid sm={12}>
          <Card variant="bordered">
            <Card.Body
              css={{
                padding: '10px',
              }}
            >
              <Form
                layout="inline"
                form={form}
                initialValues={{
                  module_name: '',
                  course_code: '',
                  module_type: null,
                  exam_type: null,
                  semester_id: null,
                }}
                onFinish={handleSubmitForm}
              >
                <Form.Item
                  name="module_name"
                  style={{ width: 'calc(17% - 16px)' }}
                >
                  <Input placeholder="Môn học" />
                </Form.Item>
                <Form.Item
                  name="course_code"
                  style={{ width: 'calc(17% - 16px)' }}
                >
                  <Input placeholder="Mã khóa học" />
                </Form.Item>
                <Form.Item
                  name="module_type"
                  style={{ width: 'calc(17% - 16px)' }}
                >
                  <Select
                    showSearch
                    style={{ width: '100%' }}
                    dropdownStyle={{ zIndex: 9999 }}
                    placeholder="Hình thức học"
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    <Select.Option key="1" value="1">
                      Lý thuyết
                    </Select.Option>
                    <Select.Option key="2" value="2">
                      Thực hành
                    </Select.Option>
                    <Select.Option key="3" value="3">
                      Lý thuyết và Thực hành
                    </Select.Option>
                  </Select>
                </Form.Item>
                <Form.Item
                  name="exam_type"
                  style={{ width: 'calc(17% - 16px)' }}
                >
                  <Select
                    showSearch
                    style={{ width: '100%' }}
                    dropdownStyle={{ zIndex: 9999 }}
                    placeholder="Hình thức thi"
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    <Select.Option key="4" value="1">
                      Lý thuyết
                    </Select.Option>
                    <Select.Option key="5" value="2">
                      Thực hành
                    </Select.Option>
                    <Select.Option key="6" value="3">
                      Lý thuyết và Thực hành
                    </Select.Option>
                    <Select.Option key="7" value="4">
                      Không thi
                    </Select.Option>
                  </Select>
                </Form.Item>
                <Form.Item
                  name="semester_id"
                  style={{ width: 'calc(17% - 16px)' }}
                >
                  <Select
                    showSearch
                    style={{ width: '100%' }}
                    dropdownStyle={{ zIndex: 9999 }}
                    placeholder="Chọn học kỳ"
                    optionFilterProp="children"
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    <Select.Option key="1" value="1">
                      Học kỳ 1
                    </Select.Option>
                    <Select.Option key="2" value="2">
                      Học kỳ 2
                    </Select.Option>
                    <Select.Option key="3" value="3">
                      Học kỳ 3
                    </Select.Option>
                    <Select.Option key="4" value="4">
                      Học kỳ 4
                    </Select.Option>
                  </Select>
                </Form.Item>

                <Form.Item style={{ width: 'calc(9% - 16px)' }}>
                  <Button
                    flat
                    auto
                    type="primary"
                    htmlType="submit"
                    style={{ width: '100%' }}
                  >
                    Tìm kiếm
                  </Button>
                </Form.Item>
                <Form.Item style={{ width: '6%', marginRight: 0 }}>
                  <Button
                    flat
                    auto
                    type="default"
                    style={{
                      width: '100%',
                    }}
                    color="error"
                    onPress={handleClearInput}
                  >
                    Huỷ
                  </Button>
                </Form.Item>
              </Form>
            </Card.Body>
          </Card>
        </Grid>
        <Grid sm={12}>
          <Card
            variant="bordered"
            css={{
              minHeight: '300px',
            }}
          >
            <Card.Header>
              <Grid.Container>
                <Grid sm={1}></Grid>
                <Grid sm={10}>
                  <Text
                    b
                    size={14}
                    p
                    css={{
                      width: '100%',
                      textAlign: 'center',
                    }}
                  >
                    Danh sách môn học
                  </Text>
                </Grid>
                <Grid sm={1}>
                  <Button
                    flat
                    auto
                    type="primary"
                    style={{
                      width: '100%',
                    }}
                    onClick={() => {
                      setisCreate(true);
                    }}
                  >
                    + Tạo mới
                  </Button>
                </Grid>
              </Grid.Container>
            </Card.Header>
            {IsLoading && <Loading />}
            {!IsLoading && (
              <Table aria-label="">
                <Table.Header>
                  <Table.Column width={80}>STT</Table.Column>
                  <Table.Column width={350}>Tên Môn học</Table.Column>
                  <Table.Column width={100}>Mã khoá học</Table.Column>
                  <Table.Column width={110}>Thời lượng học</Table.Column>
                  <Table.Column width={60}>Số buổi</Table.Column>
                  <Table.Column width={90}>Hình thức học</Table.Column>
                  <Table.Column width={80}>Hình thức thi</Table.Column>
                  <Table.Column width={90}>Điểm lý thuyết</Table.Column>
                  <Table.Column width={100}>Điểm thực hành</Table.Column>
                  {/* <Table.Column>Tên Kỳ học</Table.Column> */}
                  {/* <Table.Column>Tên Kỳ thi</Table.Column> */}
                  <Table.Column>Học Kỳ</Table.Column>
                  <Table.Column>Cơ sở</Table.Column>
                  <Table.Column>Chỉnh sửa</Table.Column>
                </Table.Header>
                <Table.Body>
                  {listModules.map((data, index) => (
                    <Table.Row key={data.user_id}>
                      <Table.Cell>{index + 1}</Table.Cell>
                      <Table.Cell>
                        <b>{data.modulename}</b>
                      </Table.Cell>
                      <Table.Cell>{data.coursecode}</Table.Cell>
                      <Table.Cell css={{ textAlign: 'end' }}>
                        {data.hours} Tiếng{' '}
                      </Table.Cell>
                      <Table.Cell css={{ textAlign: 'end' }}>
                        {data.days} buổi
                      </Table.Cell>
                      <Table.Cell>
                        {renderModuleType(data.module_type)}
                      </Table.Cell>
                      <Table.Cell>{renderExamType(data.exam_type)}</Table.Cell>
                      <Table.Cell css={{ textAlign: 'center' }}>
                        {data.max_theory_grade}
                      </Table.Cell>
                      <Table.Cell css={{ textAlign: 'center' }}>
                        {data.max_practical_grade}
                      </Table.Cell>
                      {/* <Table.Cell>{data.semester_name_portal}</Table.Cell> */}
                      {/* <Table.Cell>{data.module_exam_name_portal}</Table.Cell> */}
                      <Table.Cell>Học kỳ {data.semester_id}</Table.Cell>
                      <Table.Cell>{data.centername}</Table.Cell>
                      <Table.Cell css={{ textAlign: 'center' }}>
                        <FaPen
                          size={14}
                          color="5EA2EF"
                          style={{ cursor: 'pointer' }}
                          onClick={() => {
                            navigate(
                              `/admin/manage-course/module/${data.id}/update`
                            );
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
                  rowsPerPage={9}
                />
              </Table>
            )}
            {/* <Table
              loading={IsLoading}
              bordered
              size="middle"
              dataSource={listModules}
              // columns={designColumns}
              scroll={{
                x: 1500,
              }}
              pagination={{
                size: "default",
                position: ["bottomCenter"],
              }}
            >
              <Table.Column
                title="STT"
                dataIndex="index"
                key="index"
                width={50}
                fixed={"left"}
              />
              <ColumnGroup title="Môn học">
                <Table.Column
                  title="Tên môn học"
                  dataIndex="modulename"
                  key="modulename"
                  width={320}
                  fixed={"left"}
                />
                <Table.Column
                  title="Mã khóa học"
                  dataIndex="coursecode"
                  key="coursecode"
                  width={200}
                  fixed={"left"}
                />
              </ColumnGroup>
              <ColumnGroup title="Thời lượng học">
                <Table.Column
                  title="Số Tiếng"
                  dataIndex="hours"
                  key="hours"
                  width={80}
                />
                <Table.Column
                  title="Số buổi"
                  dataIndex="days"
                  key="days"
                  width={80}
                />
              </ColumnGroup>
              <ColumnGroup title="Hình thức môn học">
                <Table.Column
                  title="Hình thức học"
                  dataIndex="module_type"
                  key="module_type"
                  width={200}
                />
                <Table.Column
                  title="Hình thức thi"
                  dataIndex="exam_type"
                  key="exam_type"
                  width={200}
                />
              </ColumnGroup>
              <ColumnGroup title="Điểm tối đa thi">
                <Table.Column
                  title="Lý thuyểt"
                  dataIndex="max_theory_grade"
                  key="max_theory_grade"
                  width={90}
                />
                <Table.Column
                  title="Thực hành"
                  dataIndex="max_practical_grade"
                  key="max_practical_grade"
                  width={90}
                />
              </ColumnGroup>
              <ColumnGroup title="Tên theo Ấn Độ">
                <Table.Column
                  title="Kỳ học"
                  dataIndex="semester_name_portal"
                  key="semester_name_portal"
                  width={430}
                />
                <Table.Column
                  title="Kỳ thi"
                  dataIndex="module_exam_name_portal"
                  key="module_exam_name_portal"
                  width={350}
                />
              </ColumnGroup>
              <Table.Column
                title="Học kỳ"
                dataIndex="semester_id"
                value="semester_id"
                key="semester"
                width={80}
              />
              <Table.Column
                title="Cơ sở"
                dataIndex="center_id"
                key="center_id"
                width={80}
              />

              <Table.Column
                width={50}
                title=""
                dataIndex=""
                key="action"
                fixed={"right"}
                render={(_, data) => {
                  return (
                    <div className={classes.logoEdit}>
                      <MdEdit
                        color="0a579f"
                        style={{ cursor: "pointer" }}
                        onClick={() => {
                          navigate(
                            `/admin/manage-course/module/${data.id}/update`
                          );
                        }}
                      />
                    </div>
                  );
                }}
              />
            </Table> */}
          </Card>
        </Grid>
      </Grid.Container>
      <Modal
        closeButton
        aria-labelledby="modal-title"
        open={isCreate == true}
        onClose={() => {
          setisCreate(false);
        }}
        blur
        width="700px"
      >
        <Modal.Header>
          <Text size={16} b>
            Thêm môn học
          </Text>
        </Modal.Header>
        <Modal.Body>
          <ModuleCreate onCreateSuccess={handleAddSuccess} />
        </Modal.Body>
      </Modal>
      <Modal
        closeButton
        aria-labelledby="modal-title"
        open={selectedModuleId !== null}
        onClose={() => {
          setselectedModuleId(null);
        }}
        blur
        width="700px"
      >
        <Modal.Header>
          <Text size={16} b>
            Chỉnh sửa môn học
          </Text>
        </Modal.Header>
        <Modal.Body>
          <ModuleUpdate
            data={listModules.find((e) => e.id === selectedModuleId)}
            onUpdateSuccess={handleUpdateSuccess}
          />
        </Modal.Body>
      </Modal>
    </div>
  );
};
export default Module;
