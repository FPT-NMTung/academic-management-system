import { Form, InputNumber, Select, Button, Table } from 'antd';
import { useState } from 'react';
import { IoMdTrash } from 'react-icons/io';
// import { Table } from '@nextui-org/react';

const ModuleGradeType = () => {
  const [dataTable, setDataTable] = useState([]);
  const [listType, setListType] = useState([
    {
      id: 1,
      name: 'Assignment',
    },
    {
      id: 2,
      name: 'Lab',
    },
    {
      id: 3,
      name: 'Quiz',
    },
    {
      id: 5,
      name: 'Practice Exam',
    },
    {
      id: 6,
      name: 'Final Exam',
    },
  ]);

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
    const select = dataTable.find((e) => e.grade_type === value);
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
          <Button
            type="primary"
            htmlType="submit"
            disabled
            style={{ marginLeft: 10 }}
          >
            Lưu
          </Button>
        </Form.Item>
      </Form>
      <Table dataSource={dataTable} rowKey={'grade_type'}>
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
