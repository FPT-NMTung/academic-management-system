import classes from './Module.module.css';
import { Card, Grid, Text, Modal } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ModulesApis, CourseModuleSemesterApis } from '../../../apis/ListApi';
import ModuleUpdate from '../../../components/ModuleUpdate/ModuleUpdate';
import ModuleCreate from '../../../components/ModuleCreate/ModuleCreate';
import { MdEdit } from 'react-icons/md';
import { Fragment } from 'react';
import ColumnGroup from 'antd/lib/table/ColumnGroup';
import { useNavigate } from 'react-router-dom';

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
            -(new Date(a.module.center_id) - new Date(b.module.center_id))
        )
        .map((e, index) => {
          return {
            id: e.module.id,
            key: e.module.id,
            index: index + 1,
            center_id: e.module.center_id,
            modulename: e.module.module_name,
            centername: e.module.center.name,
            coursecode: e.course_code,
            hours: e.module.hours,
            days: e.module.days,
            module_type: TYPE_MODULE[e.module.module_type],
            exam_type: TYPE_EXAM[e.module.exam_type],
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

  useEffect(() => {
    getData();
  }, []);

  return (
    <div>
      <Grid.Container gap={2}>
        <Grid sm={12}>
          <Card>
            <Card.Body>
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
                    style={{ width: '100%' }}
                    dropdownStyle={{ zIndex: 9999 }}
                    placeholder="Hình thức học"
                  >
                    <Select.Option key="1" value="1">Lý thuyết</Select.Option>
                    <Select.Option key="2" value="2">Thực hành</Select.Option>
                    <Select.Option key="3" value="3">Lý thuyết và Thực hành</Select.Option>
                  </Select>
                </Form.Item>
                <Form.Item
                  name="exam_type"
                  style={{ width: 'calc(17% - 16px)' }}
                >
                  <Select
                    style={{ width: '100%' }}
                    dropdownStyle={{ zIndex: 9999 }}
                    placeholder="Hình thức thi">
                    <Select.Option key="4" value="1">Lý thuyết</Select.Option>
                    <Select.Option key="5" value="2">Thực hành</Select.Option>
                    <Select.Option key="6" value="3">Lý thuyết và Thực hành</Select.Option>
                    <Select.Option key="7" value="4">Không thi</Select.Option>
                  </Select>
                </Form.Item>
                <Form.Item
                  name="semester_id"
                  style={{ width: 'calc(17% - 16px)' }}
                >
                  <Select
                    style={{ width: '100%' }}
                    dropdownStyle={{ zIndex: 9999 }}
                    placeholder="Chọn học kỳ"
                    optionFilterProp="children"
                  >
                    <Select.Option key="1" value="1">Học kỳ 1</Select.Option>
                    <Select.Option key="2" value="2">Học kỳ 2</Select.Option>
                    <Select.Option key="3" value="3">Học kỳ 3</Select.Option>
                    <Select.Option key="4" value="4">Học kỳ 4</Select.Option>

                  </Select>
                </Form.Item>

                <Form.Item style={{ width: 'calc(9% - 16px)' }}>
                  <Button
                    type="primary"
                    htmlType="submit"
                    style={{ width: '100%' }}
                  >
                    Tìm kiếm
                  </Button>
                </Form.Item>
                <Form.Item style={{ width: '6%', marginRight: 0 }}>
                  <Button
                    type="default"
                    style={{
                      width: '100%',
                    }}
                    onClick={handleClearInput}
                  >
                    Huỷ
                  </Button>
                </Form.Item>
              </Form>
            </Card.Body>
          </Card>
        </Grid>
        <Grid sm={12}>
          <Card>
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
            <Table
              loading={IsLoading}
              bordered
              size="middle"
              dataSource={listModules}
              // columns={designColumns}
              scroll={{
                x: 1500,
              }}
              pagination={{
                size: 'default',
                position: ['bottomCenter'],
              }}
            >
              <Table.Column
                title="STT"
                dataIndex="index"
                key="index"
                width={50}
                fixed={'left'}
              />
              <ColumnGroup title="Môn học">
                <Table.Column
                  title="Tên môn học"
                  dataIndex="modulename"
                  key="modulename"
                  width={320}
                  fixed={'left'}
                />
                <Table.Column
                  title="Mã khóa học"
                  dataIndex="coursecode"
                  key="coursecode"
                  width={200}
                  fixed={'left'}
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
                fixed={'right'}
                render={(_, data) => {
                  return (
                    <div className={classes.logoEdit}>
                      <MdEdit
                        color="0a579f"
                        style={{ cursor: 'pointer' }}
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
            </Table>
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
        <Card.Divider />
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
        <Card.Divider />
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
