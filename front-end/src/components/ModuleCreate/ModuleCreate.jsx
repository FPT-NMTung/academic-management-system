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
        loading: "??ang t???o...",
        success: (res) => {
          onCreateSuccess();
          setIsCreating(false);
          setIsFailed(false);
          return "T???o th??nh c??ng !";
        },
        error: (err) => {
          setIsCreating(false);
          setIsFailed(true);
          setErrorValue(ErrorCodeApi[err.type_error]);
          if (err?.type_error) {
            return ErrorCodeApi[err.type_error];
          }
          return "T???o th???t b???i !";
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
            label={"M??n h???c"}
            rules={[
              {
                required: true,
                validator: (_, value) => {
                  if (value === null || value === undefined) {
                    return Promise.reject("Tr?????ng ph???i t??? 1 ?????n 255 k?? t???");
                  }
                  if (
                    Validater.isContaintSpecialCharacterForNameModule(
                      value.trim()
                    )
                  ) {
                    return Promise.reject(
                      "Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t"
                    );
                  }
                  if (value.trim().length < 1 || value.trim().length > 255) {
                    return Promise.reject(
                      new Error("Tr?????ng ph???i t??? 1 ?????n 255 k?? t???")
                    );
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <Input
              placeholder="T??n m??n h???c"
              style={{
                width: 300,
              }}
            />
          </Form.Item>
          <Form.Item
            name={"center_id"}
            label={"C?? s???"}
            rules={[
              {
                required: true,
                message: "H??y ch???n c?? s???",
              },
            ]}
          >
            <Select
              showSearch
              placeholder="Ch???n c?? s???"
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
          <span
            style={{
              color: "red",
              display: "flex",
              position: "fixed",
              alignItems: "center",
              top: "calc(50% - 90px)",
              left: "calc(78px)",
            }}
          >
            *
          </span>
          <Form.Item
            labelWrap="true"
            label="M?? kh??a h???c"
            style={{
              marginBottom: 0,
              position: "relative",
              color: "red",
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
                  message: "H??y ch???n m?? kh??a h???c",
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
              }}
            >
              <Select
                showSearch
                placeholder="Ch???n m?? kh??a h???c"
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
                  message: "H??y ch???n h???c k???",
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
                placeholder="Ch???n h???c k???"
                optionFilterProp="children"
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
                }
              >
                <Select.Option key="1" value="1">
                  H???c k??? 1
                </Select.Option>
                <Select.Option key="2" value="2">
                  H???c k??? 2
                </Select.Option>
                <Select.Option key="3" value="3">
                  H???c k??? 3
                </Select.Option>
                <Select.Option key="4" value="4">
                  H???c k??? 4
                </Select.Option>
              </Select>
            </Form.Item>
          </Form.Item>
          <span
            style={{
              color: "red",
              display: "flex",
              position: "fixed",
              alignItems: "center",
              top: "calc(50% - 25px)",
              left: "calc(64px)",
            }}
          >
            *
          </span>
          <Form.Item
            label="Th???i l?????ng h???c"
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
                      return Promise.reject("Ph???i l?? s??? t??? 1 ?????n 200");
                    }
                    const hours = value.toString();
                    if (
                      Validater.isNumber(hours) &&
                      hours > 0 &&
                      hours <= 200
                    ) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error("Ph???i l?? s??? t??? 1 ?????n 200"));

                    // check regex phone number viet nam
                  },
                },
              ]}
              style={{
                display: "inline-block",
                width: "calc(50% - 8px)",
              }}
            >
              <Input placeholder="S??? ti???ng" />
            </Form.Item>
            <Form.Item
              name="days"
              rules={[
                {
                  required: true,
                  validator: (_, value) => {
                    if (value === null || value === undefined || value === "") {
                      return Promise.reject("Ph???i l?? s??? t??? 1 ?????n 50");
                    }
                    const days = value.toString();
                    if (Validater.isNumber(days) && days > 0 && days <= 50) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error("Ph???i l?? s??? t??? 1 ?????n 50"));

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
              <Input placeholder="S??? bu???i" />
            </Form.Item>
          </Form.Item>
          <span
            style={{
              color: "red",
              display: "flex",
              position: "fixed",
              alignItems: "center",
              top: "calc(50% + 42px)",
              left: "calc(34px)",
            }}
          >
            *
          </span>
          <Form.Item
            label="H??nh th???c m??n h???c"
            style={{
              marginBottom: 0,
            }}
          >
            <Form.Item
              name="module_type"
              rules={[
                {
                  message: "Vui l??ng ch???n h??nh th???c h???c",
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
                placeholder="H??nh th???c h???c"
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
                }
              >
                <Select.Option key="1" value="1">
                  L?? thuy???t
                </Select.Option>
                <Select.Option key="2" value="2">
                  Th???c h??nh
                </Select.Option>
                <Select.Option key="3" value="3">
                  L?? thuy???t v?? Th???c h??nh
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
                placeholder="H??nh th???c thi"
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
                }
              >
                <Select.Option key="4" value={1}>
                  L?? thuy???t
                </Select.Option>
                <Select.Option key="5" value={2}>
                  Th???c h??nh
                </Select.Option>
                <Select.Option key="6" value={3}>
                  L?? thuy???t v?? Th???c h??nh
                </Select.Option>
                <Select.Option key="7" value={4}>
                  Kh??ng thi
                </Select.Option>
              </Select>
            </Form.Item>
          </Form.Item>
          {exam_type !== 4 && (
            <span
              style={{
                color: "red",
                display: "flex",
                position: "fixed",
                alignItems: "center",
                top: "calc(50% + 105px)",
                left: "calc(65px)",
              }}
            >
              *
            </span>
          )}
          <Form.Item
            label="??i???m thi t???i ??a"
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
                    return Promise.reject(new Error("Ph???i l?? s??? t??? 1 ?????n 100"));
                  },
                },
              ]}
            >
              <Input
                disabled={exam_type !== 1 && exam_type !== 3}
                placeholder="L?? thuy???t"
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
                    return Promise.reject(new Error("Ph???i l?? s??? t??? 1 ?????n 100"));
                  },
                },
                // {
                //   whitespace: true,
                //   message: "Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng",
                // },
              ]}
            >
              <Input
                disabled={exam_type !== 2 && exam_type !== 3}
                placeholder="Th???c h??nh"
              />
            </Form.Item>
          </Form.Item>
          <span
            style={{
              color: "red",
              display: "flex",
              position: "fixed",
              alignItems: "center",
              top: "calc(100% - 135px)",
              left: "calc(120px)",
            }}
          >
            *
          </span>
          <Form.Item
            label="???n ?????"
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
                      return Promise.reject("Tr?????ng ph???i t??? 1 ?????n 255 k?? t???");
                    }
                    if (
                      Validater.isContaintSpecialCharacterForName(value.trim())
                    ) {
                      return Promise.reject(
                        "Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t"
                      );
                    }
                    if (value.trim().length < 1 || value.trim().length > 255) {
                      return Promise.reject(
                        new Error("Tr?????ng ph???i t??? 1 ?????n 255 k?? t???")
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
              <Input placeholder="T??n k??? h???c"></Input>
            </Form.Item>
            <Form.Item
              name="module_exam_name_portal"
              rules={[
                {
                  required: true,
                  validator: (_, value) => {
                    if (value === null || value === undefined) {
                      return Promise.reject("Tr?????ng ph???i t??? 1 ?????n 255 k?? t???");
                    }
                    if (
                      Validater.isContaintSpecialCharacterForNameModule(
                        value.trim()
                      )
                    ) {
                      return Promise.reject(
                        "Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t"
                      );
                    }
                    if (value.trim().length < 1 || value.trim().length > 255) {
                      return Promise.reject(
                        new Error("Tr?????ng ph???i t??? 1 ?????n 255 k?? t???")
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
              <Input placeholder="T??n kh??a h???c"></Input>
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
              {"T???o m???i"}
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
