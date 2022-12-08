import { Card, Grid, Spacer, Text, Button, Loading } from '@nextui-org/react';
import { Form, Select, Input, Spin, Divider, InputNumber, message } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../apis/FetchApi';
import {
  ModulesApis,
  CenterApis,
  GradeModuleSemesterApis,
} from '../../apis/ListApi';
import classes from './ModuleUpdate.module.css';
import { useParams, useNavigate } from 'react-router-dom';
import ModuleGradeType from '../ModuleGradeType/ModuleGradeType';
import { Validater } from '../../validater/Validater';
import { IoArrowBackCircle } from "react-icons/io5";
import toast from 'react-hot-toast';
import { Fragment } from "react";

const TYPE_MODULE = {
  'Lý thuyết': 1,
  'Thực hành': 2,
  'Thực hành và Lý thuyết': 3,
};

const ModuleUpdate = () => {
  const [isLoading, setisLoading] = useState(true);
  const [isUpdating, setIsUpdating] = useState(false);
  const [form] = Form.useForm();
  const [listCenters, setListCenters] = useState([]);
  const [listGrade, setListGrade] = useState([]);
  const [isFailed, setIsFailed] = useState(false);
  const [typeExam, setTypeExam] = useState(4);
  const [canDelete, setCanDelete] = useState(undefined);
  const [typeExamSubmit, setTypeExamSubmit] = useState(4);

  const { id } = useParams();
  const navigate = useNavigate();

  const handleSubmitForm = () => {
    setIsUpdating(true);
    const data = form.getFieldsValue();

    const body = {
      center_id: data.center_id,
      semester_name_portal: data.portal_semester_name,
      module_type: data.module_type,
      module_name: data.module_name,
      module_exam_name_portal: data.portal_exam_name,
      max_theory_grade: data.max_theory_grade,
      max_practical_grade: data.max_practical_grade,
      hours: data.hours,
      days: data.days,
      exam_type: data.exam_type,
    };
    toast.promise(FetchApi(ModulesApis.updateModule, body, null, [`${id}`]), {
      loading: 'Đang cập nhật...',
      success: (res) => {
        setIsUpdating(false);
        setTypeExamSubmit(data.exam_type);
        getListGrade();
        return 'Cập nhật thành công';
      },
      error: (err) => {
        setIsUpdating(false);
        setIsFailed(true);
        return 'Cập nhật thất bại';
      },
    });
  };

  const getListCenter = () => {
    FetchApi(CenterApis.getAllCenter).then((res) => {
      const data = res.data.map((e) => {
        return {
          key: e.id,
          ...e,
        };
      });
      setListCenters(data);
    });
  };

  const getModulebyid = () => {
    setisLoading(true);
    FetchApi(ModulesApis.getModuleByID, null, null, [`${id}`])
      .then((res) => {
        const data = res.data;
        form.setFieldsValue({
          center_id: data.center.id,
          portal_semester_name: data.semester_name_portal,
          module_type: data.module_type,
          module_name: data.module_name,
          portal_exam_name: data.module_exam_name_portal,
          max_theory_grade: data.max_theory_grade,
          max_practical_grade: data.max_practical_grade,
          hours: data.hours,
          days: data.days,
          exam_type: data.exam_type,
        });
        setTypeExam(data.exam_type);
        setTypeExamSubmit(data.exam_type);
        setisLoading(false);
      })
      .catch(() => {
        navigate('/404');
      });
  };

  const handleTypeExamChange = (value) => {
    setTypeExam(value);

    console.log(value === 1 || value === 4);
    console.log(value === 2 || value === 4);

    if (value === 1 || value === 4) {
      form.setFieldsValue({
        max_practical_grade: null,
      });
    }

    if (value === 2 || value === 4) {
      form.setFieldsValue({
        max_theory_grade: null,
      });
    }
  };

  const getListGrade = () => {
    FetchApi(GradeModuleSemesterApis.getListGradeByModuleId, null, null, [
      String(id),
    ])
      .then((res) => {
        setListGrade(
          res.data
            .filter(
              (item) =>
                item.grade_category.id !== 7 && item.grade_category.id !== 8
            )
            .map((item) => {
              return {
                key: item.grade_category.id,
                grade_id: item.grade_category.id,
                grade_name: item.grade_category.name,
                quantity: item.quantity_grade_item,
                weight: item.total_weight,
              };
            })
        );
      })
      .catch(() => {});
  };

  const handleAddRow = (data) => {
    let temp = listGrade.filter((item) => item.grade_id !== data.grade_id);

    temp.push(data);

    setListGrade(temp.sort((a, b) => a.grade_id - b.grade_id));
  };

  const handleDeleteRow = (id) => {
    const temp = listGrade.filter((item) => {
      return item.grade_id !== id;
    });

    setListGrade(temp.sort((a, b) => a.grade_id - b.grade_id));
  };

  const handleSave = () => {
    const data = listGrade.map((item) => {
      return {
        grade_category_id: item.grade_id,
        total_weight: item.weight,
        quantity_grade_item: item.quantity,
      };
    }).filter((item) => item.grade_category_id !== 6 && item.grade_category_id !== 8);

    const body = {
      grade_category_details: data,
    };

    toast.promise(
      FetchApi(GradeModuleSemesterApis.updateGradeModule, body, null, [
        String(id),
      ]),
      {
        loading: 'Đang cập nhật...',
        success: (res) => {
          return 'Cập nhật thành công';
        },
        error: (err) => {
          return 'Cập nhật thất bại';
        },
      }
    );
  };
  const checkCanDelete = () => {
    console.log(ModulesApis.checkCanDeleteModule);
    FetchApi(ModulesApis.checkCanDeleteModule, null, null, [String([`${id}`])])
      .then((res) => {
        if (res.data.can_delete === true) {
          setCanDelete(true);
        } else {
          setCanDelete(false);
        }
      })
      .catch((err) => {
        toast.error('Lỗi kiểm tra khả năng xóa');
      });
  };
  const handleDelete = () => {
    toast.promise(
      FetchApi(ModulesApis.deleteCourse, null, null, [String([`${id}`])]),
      {
        loading: 'Đang xóa',
        success: (res) => {
          navigate('/admin/manage-course/module');
          return 'Xóa thành công';
        },
        error: (err) => {
          return 'Xóa thất bại';
        },
      }
    );
  };
  useEffect(() => {
    getModulebyid();
    getListCenter();
    getListGrade();
    checkCanDelete();
  }, []);

  return (


    <Grid.Container justify="center" gap={2}>



    <Button
    css={{
      position: 'absolute',
      left: '35px',
      top:'110px',
    }}
    flat
    auto
    icon={<IoArrowBackCircle size={20} />}
    color={"error"}
    onPress={() => {
      navigate("/admin/manage-course/module");
    }}
  >
    Trở về
  </Button>


    
         
      <Grid sm={6}>
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
              size={15}
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              Thông tin tổng quan môn học
            </Text>
          </Card.Header>
          <Card.Body>
            {isLoading && (
              <div className={classes.loading}>
                <Spin />
              </div>
            )}
            {!isLoading && (
              <Form
                labelCol={{
                  span: 7,
                }}
                wrapperCol={{
                  span: 16,
                }}
                layout="horizontal"
                labelAlign="right"
                onFinish={handleSubmitForm}
                form={form}
                initialValues={{
                  exam_type: typeExam,
                }}
              >
                <div className={classes.layout}>
                  <Form.Item
                    name={'module_name'}
                    label="Môn học"
                    rules={[
                      {
                        required: true,
                        validator: (_, value) => {
                          if (value === null || value === undefined) {
                            return Promise.reject(
                              'Trường phải từ 1 đến 255 ký tự'
                            );
                          }
                          if (
                            Validater.isContaintSpecialCharacterForNameModule(
                              value.trim()
                            )
                          ) {
                            return Promise.reject(
                              'Trường này không được chứa ký tự đặc biệt'
                            );
                          }
                          if (
                            value.trim().length < 1 ||
                            value.trim().length > 255
                          ) {
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
                    <Input placeholder="Nhập tên môn học" />
                  </Form.Item>
                  <Form.Item
                    name={'center_id'}
                    label="Cơ sở"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng chọn cơ sở',
                      },
                    ]}
                  >
                    <Select
                      showSearch
                      loading={listCenters.length === 0}
                      placeholder="Chọn cơ sở"
                      filterOption={(input, option) =>
                        option.children
                          .toLowerCase()
                          .includes(input.toLowerCase())
                      }
                    >
                      {listCenters
                        .filter((e) => e.is_active)
                        .map((e, index) => {
                          return (
                            <Select.Option key={index} value={e.key}>
                              {e.name}
                            </Select.Option>
                          );
                        })}
                    </Select>
                  </Form.Item>
                </div>
                <Divider orientation="left">
                  <Text
                    p
                    b
                    size={15}
                    css={{
                      width: '100%',
                      textAlign: 'center',
                    }}
                  >
                    Thông tin thời gian
                  </Text>
                </Divider>
                <div className={classes.layout}>
                  <Form.Item
                    name={'hours'}
                    label="Số giờ học"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng nhập số giờ học',
                      },
                    ]}
                  >
                    <InputNumber
                      placeholder="0"
                      min={1}
                      max={200}
                    ></InputNumber>
                  </Form.Item>
                  <div></div>
                  <Form.Item
                    name={'days'}
                    label="Số buổi học"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng nhập số buổi học',
                      },
                    ]}
                  >
                    <InputNumber placeholder="0" min={1} max={50}></InputNumber>
                  </Form.Item>
                </div>
                <Divider orientation="left">
                  <Text
                    p
                    b
                    size={15}
                    css={{
                      width: '100%',
                      textAlign: 'center',
                    }}
                  >
                    Thông tin điểm thi tối đa
                  </Text>
                </Divider>
                <div className={classes.layout}>
                  <Form.Item
                    name={'module_type'}
                    label="Hình thức học"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng chọn hình thức học',
                      },
                    ]}
                  >
                    <Select
                      showSearch
                      filterOption={(input, option) =>
                        option.children
                          .toLowerCase()
                          .includes(input.toLowerCase())
                      }
                      placeholder="Chọn hình thức học"
                    >
                      <Select.Option value={1}>Lý thuyết</Select.Option>
                      <Select.Option value={2}>Thực hành</Select.Option>
                      <Select.Option value={3}>
                        Lý thuyết và Thực hành
                      </Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item
                    name={'exam_type'}
                    label="Hình thức thi"
                    rules={[
                      {
                        required: true,
                        message: 'Vui lòng chọn hình thức thi',
                      },
                    ]}
                  >
                    <Select
                      showSearch
                      placeholder="Chọn hình thức thi"
                      onChange={(value) => {
                        handleTypeExamChange(value);
                      }}
                      filterOption={(input, option) =>
                        option.children
                          .toLowerCase()
                          .includes(input.toLowerCase())
                      }
                    >
                      <Select.Option value={1}>Lý thuyết</Select.Option>
                      <Select.Option value={2}>Thực hành</Select.Option>
                      <Select.Option value={3}>
                        Lý thuyết và Thực hành
                      </Select.Option>
                      <Select.Option value={4}>Không thi</Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item
                    name={'max_theory_grade'}
                    label="Lý thuyết"
                    rules={[
                      {
                        required: typeExam === 1 || typeExam === 3,
                        message: 'Vui lòng nhập điểm tối đa lý thuyết',
                      },
                    ]}
                  >
                    <InputNumber
                      disabled={typeExam === 2 || typeExam === 4}
                      placeholder="0"
                      min={1}
                      max={100}
                    ></InputNumber>
                  </Form.Item>
                  <Form.Item
                    name={'max_practical_grade'}
                    label="Thực hành"
                    rules={[
                      {
                        required: typeExam === 2 || typeExam === 3,
                        message: 'Vui lòng nhập điểm tối đa thực hành',
                      },
                    ]}
                  >
                    <InputNumber
                      disabled={typeExam === 1 || typeExam === 4}
                      placeholder="0"
                      min={1}
                      max={100}
                    ></InputNumber>
                  </Form.Item>
                </div>
                <Divider orientation="left">
                  <Text
                    p
                    b
                    size={15}
                    css={{
                      width: '100%',
                      textAlign: 'center',
                    }}
                  >
                    Thông tin liên quan đến Aptech Ấn độ
                  </Text>
                </Divider>
                <div className={classes.layout}>
                  <Form.Item
                    name={'portal_semester_name'}
                    label="Semester"
                    rules={[
                      {
                        required: true,
                        validator: (_, value) => {
                          if (value === null || value === undefined) {
                            return Promise.reject(
                              'Trường phải từ 1 đến 255 ký tự'
                            );
                          }
                          if (
                            Validater.isContaintSpecialCharacterForName(
                              value.trim()
                            )
                          ) {
                            return Promise.reject(
                              'Trường này không được chứa ký tự đặc biệt'
                            );
                          }
                          if (
                            value.trim().length < 1 ||
                            value.trim().length > 255
                          ) {
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
                    <Input placeholder="Nhập tên semester"></Input>
                  </Form.Item>
                  <Form.Item
                    name={'portal_exam_name'}
                    label="Exam name"
                    rules={[
                      {
                        required: true,
                        validator: (_, value) => {
                          if (value === null || value === undefined) {
                            return Promise.reject(
                              'Trường phải từ 1 đến 255 ký tự'
                            );
                          }
                          if (
                            Validater.isContaintSpecialCharacterForNameModule(
                              value.trim()
                            )
                          ) {
                            return Promise.reject(
                              'Trường này không được chứa ký tự đặc biệt'
                            );
                          }
                          if (
                            value.trim().length < 1 ||
                            value.trim().length > 255
                          ) {
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
                    <Input placeholder="Nhập tên exam"></Input>
                  </Form.Item>
                </div>
                <div
                  style={{
                    display: 'flex',
                    justifyContent: 'center',
                  }}
                >
                  <Form.Item>
                    <div
                      style={{
                        display: 'flex',
                        gap: '10px',
                        // textAlign: 'center',
                        // marginRight: '10px',
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
                        disabled={isUpdating}
                      >
                        Cập nhật
                      </Button>
                      <Button
                        flat
                        auto
                        css={{
                          width: '80px',
                        }}
                        color={'error'}
                        disabled={!canDelete}
                        onPress={handleDelete}
                      >
                        {canDelete === undefined && <Loading size="xs" />}
                        {canDelete !== undefined && 'Xoá'}
                      </Button>
                    </div>
                  </Form.Item>
                </div>
              </Form>
            )}
          </Card.Body>
        </Card>
      </Grid>
      <Grid sm={4}>
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
              size={15}
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              Thông tin điểm thành phần
            </Text>
          </Card.Header>
          <Card.Body>
            {(typeExamSubmit === 2 || typeExamSubmit === 3) && (
              <ModuleGradeType
                typeExam={typeExam}
                listGrade={listGrade}
                onAddRow={handleAddRow}
                onDeleteRow={handleDeleteRow}
                onSave={handleSave}
              />
            )}
            {!(typeExamSubmit === 2 || typeExamSubmit === 3) && (
              <div
                style={{
                  textAlign: 'center',
                  padding: '80px 0 100px',
                }}
              >
                <Text p i size={12}>
                  Không áp dụng điểm thành phần cho loại thi mà bạn đang chọn
                </Text>
                <Spacer y={0.5} />
                <Text p i size={12}>
                  Chỉ áp dụng cho loại có thi thực hành.
                </Text>
              </div>
            )}
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};
export default ModuleUpdate;
