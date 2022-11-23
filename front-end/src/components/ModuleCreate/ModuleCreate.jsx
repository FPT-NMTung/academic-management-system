import { Card, Grid, Button, Text } from "@nextui-org/react";
import {
  Form,
  Select,
  Input,
  Spin,
  Table,
  Tooltip,
  Space,
  Typography,
  message,
} from "antd";
import { useEffect, useState } from "react";
import FetchApi from "../../apis/FetchApi";
import {
  ModulesApis,
  CenterApis,
  CourseApis,
  SemesterApis,
} from "../../apis/ListApi";
import classes from "./ModuleCreate.module.css";
import { Fragment } from "react";
import { MdEdit } from "react-icons/md";
import ColumnGroup from "antd/lib/table/ColumnGroup";
import { ErrorCodeApi } from "../../apis/ErrorCodeApi";
import { Validater } from "../../validater/Validater";
import toast from "react-hot-toast";

const ModuleCreate = ({ onCreateSuccess }) => {
  const [isCreating, setIsCreating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [isLoading, setisLoading] = useState(true);
  const [listCenters, setListCenters] = useState([]);
  const [listCourses, setListCourses] = useState([]);
  const [listSemesterid, setListSemesterid] = useState([]);
  const [exam_type, setExam_type] = useState(4);
  const [errorValue, setErrorValue] = useState(undefined);

  const [form] = Form.useForm();

  const getListCenter = () => {
    FetchApi(CenterApis.getAllCenter).then((res) => {
      const data = res.data.map((e) => {
        return {
          key: e.course_code,
          ...e,
        };
      });
      setListCenters(data);
      setisLoading(false);
    });
  };

  const getListCourse = () => {
    FetchApi(CourseApis.getAllCourse).then((res) => {
      const data = res.data.map((e) => {
        return {
          coursename: e.code,
        };
      });
      setListCourses(data);
      setisLoading(false);
    });
    setListSemesterid([]);
  };

  const GetListSemesterid = () => {
    const coursecode = form.getFieldValue("course_code").trim();

    FetchApi(CourseApis.getCourseByCode, null, null, [`${coursecode}`]).then(
      (res) => {
        const data = res.data.semester_count;

        setListSemesterid(data);
      }
    );
    setisLoading(false);
  };

  const handleChangeExamType = () => {
    const examtype = form.getFieldValue("exam_type");
    setExam_type(examtype);
  };

  useEffect(() => {
    getListCenter();
    getListCourse();
  }, []);

  const handleSubmitForm = (e) => {
    setIsCreating(true);
    setErrorValue(undefined);
    toast.promise(
      FetchApi(
        ModulesApis.createModules,
        {
          center_id: e.center_id,
          semester_name_portal: e.semester_name_portal.trim(),
          module_name: e.module_name.trim(),
          module_exam_name_portal: e.module_exam_name_portal.trim(),
          module_type: e.module_type,
          max_theory_grade: e.max_theory_grade,
          max_practical_grade: e.max_practical_grade,
          hours: e.hours,
          days: e.days,
          exam_type: e.exam_type,
          course_code: e.course_code,
          semester_id: e.semesterid,
        },
        null,
        null
      ),
      {
        loading: "Đang tạo...",
        success: (res) => {
          onCreateSuccess();
          setIsCreating(false);
          setIsFailed(false);
          return "Tạo thành công !";
        },
        error: (err) => {
          setIsCreating(false);
          setIsFailed(true);
          setErrorValue(ErrorCodeApi[err.type_error]);
          if (err?.type_error) {
            return ErrorCodeApi[err.type_error];
          }
          return "Tạo thất bại !";
        },
      }
    );
  };

  return (
    <Fragment>
      {isLoading && (
        <div className={classes.loading}>
          <Spin />
        </div>
      )}
      {!isLoading && (
        <Form
          labelCol={{
            span: 6,
          }}
          wrapperCol={{
            span: 18,
          }}
          layout="horizontal"
          labelAlign="right"
          labelWrap
          initialValues={{
            exam_type: 4,
            max_theory_grade: null,
            max_practical_grade: null,
          }}
          onFinish={handleSubmitForm}
          form={form}
        >
          <Form.Item
            name={"module_name"}
            label={"Môn học"}
            rules={[
              {
                required: true,
                validator: (_, value) => {
                  if (value === null || value === undefined) {
                    return Promise.reject("Trường phải từ 1 đến 255 ký tự");
                  }
                  if (
                    Validater.isContaintSpecialCharacterForNameModule(
                      value.trim()
                    )
                  ) {
                    return Promise.reject(
                      "Trường này không được chứa ký tự đặc biệt"
                    );
                  }
                  if (value.trim().length < 1 || value.trim().length > 255) {
                    return Promise.reject(
                      new Error("Trường phải từ 1 đến 255 ký tự")
                    );
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <Input
              placeholder="Tên môn học"
              style={{
                width: 300,
              }}
            />
          </Form.Item>
          <Form.Item
            name={"center_id"}
            label={"Cơ sở"}
            rules={[
              {
                required: true,
                message: "Hãy chọn cơ sở",
              },
            ]}
          >
            <Select
              showSearch
              placeholder="Chọn cơ sở"
              style={{ width: "100%" }}
              dropdownStyle={{ zIndex: 9999 }}
              loading={isLoading}
              filterOption={(input, option) =>
                option.children.toLowerCase().includes(input.toLowerCase())
              }
            >
              {listCenters
                .filter((e) => e.is_active)
                .map((e) => (
                  <Select.Option key={e.key} value={e.id}>
                    {e.name}
                  </Select.Option>
                ))}
            </Select>
          </Form.Item>
          <Form.Item
            label="Mã khóa học"
            style={{
              marginBottom: 0,
            }}
            rules={[
              {
                required: true,
              },
            ]}
          >
            <Space></Space>
            <Form.Item
              name="course_code"
              rules={[
                {
                  required: true,
                  message: "Hãy chọn mã khóa học",
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
              }}
            >
              <Select
                showSearch
                placeholder="Chọn mã khóa học"
                style={{ width: "100%" }}
                dropdownStyle={{ zIndex: 9999 }}
                loading={isLoading}
                mode="multiple"
                maxTagCount={"responsive"}
                onChange={GetListSemesterid}
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
                }
              >
                {listCourses.map((e, index) => (
                  <Select.Option key={index} value={e.coursename}>
                    {e.coursename}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name="semesterid"
              rules={[
                {
                  required: true,
                  message: "Hãy chọn học kỳ",
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
                margin: "0 8px",
              }}
            >
              <Select
                showSearch
                style={{ width: "100%" }}
                dropdownStyle={{ zIndex: 9999 }}
                placeholder="Chọn học kỳ"
                optionFilterProp="children"
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
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
          </Form.Item>
          <Form.Item
            label="Thời lượng học"
            style={{
              marginBottom: 0,
            }}
            rules={[
              {
                required: true,
              },
            ]}
          >
            <Space></Space>
            <Form.Item
              name="hours"
              rules={[
                {
                  required: true,
                  validator: (_, value) => {
                    if (value === null || value === undefined || value === "") {
                      return Promise.reject("Phải là số từ 1 đến 200");
                    }
                    const hours = value.toString();
                    if (Validater.isNumber(hours) && hours > 0 && hours <= 200) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error("Phải là số từ 1 đến 200"));

                    // check regex phone number viet nam
                  },
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
              }}
            >
              <Input placeholder="Số tiếng" />
            </Form.Item>
            <Form.Item
              name="days"
              rules={[
                {
                  required: true,
                  validator: (_, value) => {
                    if (value === null || value === undefined || value === "") {
                      return Promise.reject("Phải là số từ 1 đến 50");
                    }
                    const days = value.toString();
                    if (Validater.isNumber(days) && days > 0 && days <= 50) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error("Phải là số từ 1 đến 50"));

                    // check regex phone number viet nam
                  },
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
                margin: "0 8px",
              }}
            >
              <Input placeholder="Số buổi" />
            </Form.Item>
          </Form.Item>

          <Form.Item
            label="Hình thức môn học"
            style={{
              marginBottom: 0,
            }}
          >
            <Form.Item
              name="module_type"
              rules={[
                {
                  message: "Vui lòng chọn hình thức học",
                  required: true,
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
              }}
            >
              <Select
                showSearch
                style={{ width: "100%" }}
                dropdownStyle={{ zIndex: 9999 }}
                placeholder="Hình thức học"
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
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
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
                margin: "0 8px",
              }}
            >
              <Select
                showSearch
                onChange={handleChangeExamType}
                style={{ width: "100%" }}
                dropdownStyle={{ zIndex: 9999 }}
                placeholder="Hình thức thi"
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
                }
              >
                <Select.Option key="4" value={1}>
                  Lý thuyết
                </Select.Option>
                <Select.Option key="5" value={2}>
                  Thực hành
                </Select.Option>
                <Select.Option key="6" value={3}>
                  Lý thuyết và Thực hành
                </Select.Option>
                <Select.Option key="7" value={4}>
                  Không thi
                </Select.Option>
              </Select>
            </Form.Item>
          </Form.Item>
          <Form.Item
            label="Điểm thi tối đa"
            style={{
              marginBottom: 0,
            }}
          >
            <Form.Item
              name="max_theory_grade"
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
              }}
              rules={[
                {
                  required: false,
                  validator: (_, value) => {
                    if (value === null || value === undefined || value === "") {
                      return Promise.resolve();
                    }

                    const theory_grade = value.toString();
                    if (
                      Validater.isNumber(theory_grade) &&
                      theory_grade <= 100 &&
                      theory_grade >= 1
                    ) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error("Phải là số từ 1 đến 100"));
                  },
                },
              ]}
            >
              <Input
                disabled={exam_type !== 1 && exam_type !== 3}
                placeholder="Lý thuyết"
              ></Input>
            </Form.Item>
            <Form.Item
              name="max_practical_grade"
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
                margin: "0 8px",
              }}
              rules={[
                {
                  required: false,
                  validator: (_, value) => {
                    if (value === null || value === undefined || value === "") {
                      return Promise.resolve();
                    }

                    const practical_grade = value.toString();
                    if (
                      Validater.isNumber(practical_grade) &&
                      practical_grade <= 100 &&
                      practical_grade >= 1
                    ) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error("Phải là số từ 1 đến 100"));
                  },
                },
                // {
                //   whitespace: true,
                //   message: "Trường không được chứa khoảng trắng",
                // },
              ]}
            >
              <Input
                disabled={exam_type !== 2 && exam_type !== 3}
                placeholder="Thực hành"
              />
            </Form.Item>
          </Form.Item>
          <Form.Item
            label="Ấn Độ"
            style={{
              marginBottom: 0,
            }}
          >
            <Form.Item
              name="semester_name_portal"
              rules={[
                {
                  required: true,
                  validator: (_, value) => {
                    if (value === null || value === undefined) {
                      return Promise.reject("Trường phải từ 1 đến 255 ký tự");
                    }
                    if (
                      Validater.isContaintSpecialCharacterForName(value.trim())
                    ) {
                      return Promise.reject(
                        "Trường này không được chứa ký tự đặc biệt"
                      );
                    }
                    if (value.trim().length < 1 || value.trim().length > 255) {
                      return Promise.reject(
                        new Error("Trường phải từ 1 đến 255 ký tự")
                      );
                    }
                    return Promise.resolve();
                  },
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
              }}
            >
              <Input placeholder="Tên kỳ học"></Input>
            </Form.Item>
            <Form.Item
              name="module_exam_name_portal"
              rules={[
                {
                  required: true,
                  validator: (_, value) => {
                    if (value === null || value === undefined) {
                      return Promise.reject("Trường phải từ 1 đến 255 ký tự");
                    }
                    if (
                      Validater.isContaintSpecialCharacterForNameModule(
                        value.trim()
                      )
                    ) {
                      return Promise.reject(
                        "Trường này không được chứa ký tự đặc biệt"
                      );
                    }
                    if (value.trim().length < 1 || value.trim().length > 255) {
                      return Promise.reject(
                        new Error("Trường phải từ 1 đến 255 ký tự")
                      );
                    }
                    return Promise.resolve();
                  },
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
                margin: "0 8px",
              }}
            >
              <Input placeholder="Tên khóa học"></Input>
            </Form.Item>
          </Form.Item>

          <Form.Item wrapperCol={{ offset: 6, span: 10 }}>
            <Button
              auto
              flat
              css={{
                width: "120px",
              }}
              type="primary"
              htmlType="submit"
              disabled={isCreating}
            >
              {"Tạo mới"}
            </Button>
          </Form.Item>
        </Form>
      )}
      {!isCreating &&
        isFailed &&
        form.resetFields(["max_theory_grade", "max_practical_grade"])}
    </Fragment>
  );
};

export default ModuleCreate;
