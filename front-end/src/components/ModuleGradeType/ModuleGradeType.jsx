import { Form, InputNumber, Select, Button, Table, message } from 'antd';
import { useEffect } from 'react';
import { useState } from 'react';
import { IoMdTrash } from 'react-icons/io';
import FetchApi from '../../apis/FetchApi';
import { GradeModuleSemesterApis, GradeType } from '../../apis/ListApi';
// import { Table } from '@nextui-org/react';

const ModuleGradeType = ({ moduleId, listGrade }) => {
  const [dataTable, setDataTable] = useState([]);
  const [listType, setListType] = useState([]);

  const [form] = Form.useForm();

  const handleAddRow = () => {
    const data = form.getFieldsValue();
    const newData = {
      grade_type: data.grade_type,
      grade_name: listType.find((e) => e.id === data.grade_type).name,
      weight: data.weight,
      quantity: data.quantity,
    };

    const temp = dataTable.filter(
      (item) => item.grade_type !== newData.grade_type
    );

    const newDataTable = [...temp, newData].sort(
      (a, b) => a.grade_type - b.grade_type
    );
    setDataTable(newDataTable);
  };

  const handleDeleteRow = (data) => {
    const newData = dataTable.filter(
      (item) => item.grade_type !== data.grade_type
    );
    setDataTable(newData);
  };

  const handleChangeSelectType = (value) => {
    const select = listGrade.find((e) => e.grade_id === value);
    if (select) {
      form.setFieldsValue({
        weight: select.weight,
        quantity: select.quantity,
      });
    } else {
      form.setFieldsValue({
        weight: null,
        quantity: null,
      });
    }
  };

  const getGradeType = () => {
    FetchApi(GradeType.getAllGradeType, null, null, null)
      .then((res) => {
        setListType(res.data);
      })
      .catch((err) => {});
  };

  useEffect(() => {
    getGradeType();
  }, []);

  useEffect(() => {
    form.resetFields(['grade_type', 'weight', 'quantity']);
  }, [listGrade]);

  return (
    <div>
      <Form
        labelCol={{ span: 6 }}
        wrapperCol={{ span: 14 }}
        form={form}
        onFinish={() => {
          handleAddRow();
        }}
      >
        <Form.Item
          name={'grade_type'}
          label="Loại điểm"
          rules={[
            {
              required: true,
              message: 'Vui lòng chọn loại điểm',
            },
          ]}
        >
          <Select
            placeholder="Chọn loại điểm"
            onSelect={handleChangeSelectType}
          >
            {listType.map((e) => (
              <Select.Option key={e.id} value={e.id}>
                {e.name}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item
          name={'quantity'}
          label="Số lượng"
          rules={[
            {
              required: true,
              message: 'Vui lòng nhập số lượng',
            },
          ]}
        >
          <InputNumber
            min={1}
            max={10}
            style={{ width: 140 }}
            placeholder="0"
          ></InputNumber>
        </Form.Item>
        <Form.Item
          label="Trọng số"
          name={'weight'}
          rules={[
            {
              required: true,
              message: 'Vui lòng nhập trọng số',
            },
          ]}
        >
          <InputNumber
            style={{ width: 140 }}
            min={1}
            max={100}
            addonAfter="%"
            placeholder="0"
          ></InputNumber>
        </Form.Item>
        <Form.Item wrapperCol={{ span: 14, offset: 6 }}>
          <Button type="primary" htmlType="submit">
            Thêm hoặc cập nhật
          </Button>
          <Button type="primary" htmlType="submit" style={{ marginLeft: 10 }}>
            Lưu
          </Button>
        </Form.Item>
      </Form>
      <Table dataSource={listGrade} size={'middle'}>
        <Table.Column
          title="Loại điểm"
          dataIndex="grade_name"
          key="grade_name"
        />
        <Table.Column title="Số lượng" dataIndex="quantity" key="quantity" />
        <Table.Column
          title="Trọng số"
          dataIndex="weight"
          key="weight"
          render={(value) => `${value}%`}
        />
        <Table.Column
          width={50}
          title=""
          dataIndex="action"
          key="action"
          render={(_, data) => {
            if (data.grade_id >= 5 && data.grade_id <= 8) {
              return null
            }

            return (
              <IoMdTrash
                size={20}
                color={'red'}
                style={{ cursor: 'pointer' }}
                onClick={() => {
                  handleDeleteRow(data);
                }}
              />
            );
          }}
        />
      </Table>
    </div>
  );
};

export default ModuleGradeType;
