import { Card, Grid, Button, Text } from "@nextui-org/react";
import { Form, Select, DatePicker, Input, Spin, message } from "antd";
import { Fragment } from "react";
import { useEffect, useState } from "react";
import FetchApi from "../../apis/FetchApi";
import { CourseFamilyApis } from "../../apis/ListApi";
import classes from "./CourseFamilyUpdate.module.css";
import { Validater } from "../../validater/Validater";
import moment from "moment";
import toast from "react-hot-toast";
import { ErrorCodeApi } from "../../apis/ErrorCodeApi";

const CourseFamilyUpdate = ({ data, onUpdateSuccess }) => {
  const [isUpdating, setIsUpdating] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [IsLoading, setIsLoading] = useState(true);
  const [form] = Form.useForm();
  const [listCourseFamily, setlistCourseFamily] = useState([]);
  const [messageFailed, setMessageFailed] = useState(undefined);

  const getData = () => {
    setIsLoading(true);
    const apiCourseFamily = CourseFamilyApis.getAllCourseFamily;
    console.log(apiCourseFamily);

    FetchApi(apiCourseFamily).then((res) => {
      setlistCourseFamily(res.data);
    });
  };
  useEffect(() => {
    setIsLoading(false);
    // getData();
  }, []);
  const handleSubmitForm = (e) => {
    setIsUpdating(true);

    const body = {
      name: e.coursefamilyname.trim(),
      code: e.codefamily,
      published_year: e.codefamilyyear.year(),
      is_active: true,
    };

    toast.promise(
      FetchApi(CourseFamilyApis.updateCourseFamily, body, null, [
        `${data.codefamily}`,
      ]),
      {
        loading: "Đang cập nhật...",
        success: (res) => {
          onUpdateSuccess();
          return "Cập nhật thành công !";
        },
        error: (err) => {
          setIsUpdating(false);
          setIsFailed(true);
          setMessageFailed(ErrorCodeApi[err.type_error]);
          if (err?.type_error) {
            return ErrorCodeApi[err.type_error];
          }
          return "Cập nhật thất bại !";
        },
      }
    );
  };
  //     .then((res) => {
  //       message.success("Cập nhật chương trình học thành công");
  //       onUpdateSuccess();
  //     })
  //     .catch((err) => {
  //       setIsUpdating(false);
  //       setIsFailed(true);
  //     });
  // };
  return (
    <Fragment>
      {IsLoading && (
        <div className={classes.loading}>
          <Spin />
        </div>
      )}
      {!IsLoading && (
        <Form
          labelWrap
          labelCol={{ span: 7 }}
          wrapperCol={{ span: 16 }}
          layout="horizontal"
          onFinish={handleSubmitForm}
          form={form}
          initialValues={{
            coursefamilyname: data?.namecoursefamily,
            codefamily: data?.codefamily,
            codefamilyyear: moment(data?.codefamilyyear),
          }}
        >
          <Form.Item
            name={"coursefamilyname"}
            label={"Tên"}
            rules={[
              {
                required: true,
                validator: (_, value) => {
                  if (value === null || value === undefined) {
                    return Promise.reject("Trường này không được để trống");
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
          >
            <Input />
          </Form.Item>
          <Fragment>
            <Form.Item
              name={"codefamily"}
              label={"Mã chương trình học"}
              rules={[
                {
                  required: true,
                  message: "Hãy nhập mã chương trình học",
                },
              ]}
            >
              <Input disabled />
            </Form.Item>
            <Form.Item
              name={"codefamilyyear"}
              label={"Năm áp dụng"}
              rules={[
                {
                  required: true,
                  message: "Hãy chọn năm áp dụng",
                },
              ]}
            >
              <DatePicker
                popupStyle={{ zIndex: 999999 }}
                placeholder="Năm áp dụng"
                picker="year"
              />
            </Form.Item>
          </Fragment>

          <Form.Item wrapperCol={{ offset: 7, span: 99 }}>
            <Button
              flat
              auto
              css={{
                width: "120px",
              }}
              type="primary"
              htmlType="submit"
              disabled={isUpdating}
            >
              Cập nhật
            </Button>
            {/* <Button
                flat
                auto
                css={{
                  width: '80px',
                }}
                color={'error'}
                disabled={
                  canDelete === false || canDelete === undefined || isUpdating
                }
                onPress={handleDeleteCenter}
              >
                {canDelete === undefined && <Loading size="xs" />}
                {canDelete !== undefined && 'Xoá'}
              </Button> */}
          </Form.Item>
        </Form>
      )}
      {!isUpdating  && messageFailed !== undefined && isFailed && (
        <Text
          size={14}
          css={{
            color: "red",
            textAlign: "center",
          }}
        >
          {messageFailed}, vui lòng thử lại
        </Text>
      )}
    </Fragment>
  );
};

export default CourseFamilyUpdate;
