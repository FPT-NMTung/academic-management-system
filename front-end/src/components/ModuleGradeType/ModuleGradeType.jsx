import { Form, InputNumber, Select, Table, message } from 'antd';
import { useEffect } from 'react';
import { useState } from 'react';
import { IoMdTrash } from 'react-icons/io';
import FetchApi from '../../apis/FetchApi';
import { GradeType } from '../../apis/ListApi';
import { Spacer, Button, Text } from '@nextui-org/react';

// import { Table } from '@nextui-org/react';

const ModuleGradeType = ({
  typeExam,
  listGrade,
  onAddRow,
  onDeleteRow,
  onSave,
}) => {
  const [listType, setListType] = useState([]);
  const [typeGrade, setTypeGrade] = useState(undefined);

  const [form] = Form.useForm();

  const handleAddRow = () => {
    const data = form.getFieldsValue();
    const newData = {
      key: data.grade_type,
      grade_id: data.grade_type,
      grade_name: listType.find((e) => e.id === data.grade_type).name,
      weight: data.weight,
      quantity: data.quantity,
    };

    onAddRow(newData);
  };

  const handleDeleteRow = (data) => {
    onDeleteRow(data.grade_id);
  };

  const handleChangeSelectType = (value) => {
    setTypeGrade(value);
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
    setTypeGrade(undefined);
  }, [listGrade]);

  const total = listGrade
    .filter((item) => item.grade_id !== 7 && item.grade_id !== 8)
    .reduce((a, b) => a + b.weight, 0);

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
            showSearch
            placeholder="Chọn loại điểm"
            onSelect={handleChangeSelectType}
            filterOption={(input, option) =>
              option.children.toLowerCase().includes(input.toLowerCase())
            }
          >
            {listType
              .filter((item) => item.id !== 6 && item.id !== 7 && item.id !== 8)
              .map((e) => (
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
            disabled={typeGrade === 5 || typeGrade === 6}
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
          <div
            style={{
              display: 'flex',
              gap: '10px',
            }}
          >
            <Button flat auto type="primary" htmlType="submit">
              Thêm hoặc cập nhật
            </Button>
            <Button
              flat
              auto
              color={'success'}
              disabled={total !== 100}
              onPress={onSave}
            >
              Lưu
            </Button>
          </div>
        </Form.Item>
      </Form>
      <Table
        dataSource={listGrade
          .filter((item) => item.grade_id !== 6)
          .sort((a, b) => a.grade_id - b.grade_id)}
        size={'small'}
        pagination={false}
      >
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
          render={(value) => (
            <div style={total !== 100 ? { color: 'red' } : null}>
              <b>{value}%</b>
            </div>
          )}
        />
        <Table.Column
          width={50}
          title=""
          dataIndex="action"
          key="action"
          render={(_, data) => {
            if (data.grade_id >= 5 && data.grade_id <= 8) {
              return null;
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
      <Spacer y={1} />
      <Text p size={15} css={{ float: 'right', paddingRight: 10 }}>
        Tổng trọng số: <b>{total}%</b>
      </Text>
      {total > 100 && (
        <Text p size={15} css={{ float: 'right', paddingRight: 10 }}>
          <b style={{ color: 'red' }}>Tổng trọng số hợp lệ là 100%</b>
        </Text>
      )}
    </div>
  );
};

export default ModuleGradeType;
