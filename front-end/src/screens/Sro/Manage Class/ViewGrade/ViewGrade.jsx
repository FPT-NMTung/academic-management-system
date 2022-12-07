import { Text } from '@nextui-org/react';
import { Form, Table, Input } from 'antd';
import React, { useEffect } from 'react';
import { useRef } from 'react';
import { useState, useContext } from 'react';
import toast from 'react-hot-toast';
import { useParams } from 'react-router-dom';
import FetchApi from '../../../../apis/FetchApi';
import { GradeStudentApis } from '../../../../apis/ListApi';
import Temp from './Temp/Temp';

const EditableContext = React.createContext(null);
const EditableRow = ({ index, ...props }) => {
  const [form] = Form.useForm();
  return (
    <Form form={form} component={false}>
      <EditableContext.Provider value={form}>
        <tr {...props} />
      </EditableContext.Provider>
    </Form>
  );
};
const EditableCell = ({
  title,
  editable,
  children,
  dataIndex,
  record,
  handleSave,
  ...restProps
}) => {
  const [editing, setEditing] = useState(false);
  const inputRef = useRef(null);
  const form = useContext(EditableContext);
  useEffect(() => {
    if (editing) {
      inputRef.current.focus();
    }
  }, [editing]);
  const toggleEdit = () => {
    setEditing(!editing);
    form.setFieldsValue({
      [dataIndex]: record[dataIndex],
    });
  };
  const save = async () => {
    try {
      const values = await form.validateFields();
      toggleEdit();
      handleSave({
        ...record,
        ...values,
      });
    } catch (errInfo) {
      console.log('Save failed:', errInfo);
    }
  };
  let childNode = children;
  if (editable) {
    childNode = editing ? (
      <Form.Item
        style={{
          margin: 0,
        }}
        name={dataIndex}
        rules={[
          {
            required: true,
            message: `${title} is required.`,
          },
        ]}
      >
        <Input ref={inputRef} onPressEnter={save} onBlur={save} />
      </Form.Item>
    ) : (
      <div
        className="editable-cell-value-wrap"
        style={{
          paddingRight: 24,
        }}
        onClick={toggleEdit}
      >
        {children}
      </div>
    );
  }
  return <td {...restProps}>{childNode}</td>;
};

const ViewGrade = ({ dataModule, dataSchedule, onSuccess }) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  const [columns, setColumns] = useState([]);

  const { id, moduleId } = useParams();

  useEffect(() => {
    setLoading(true);
    FetchApi(GradeStudentApis.getListGradeByClassIdAndModuleId, null, null, [
      String(id),
      String(moduleId),
    ])
      .then((res) => {
        const temp = res.data[0].students[0].grade_categories.map((item) => {
          if (item.grade_items.length === 1) {
            return {
              title: item.name,
              dataIndex: item.grade_items[0].id,
              key: item.grade_items[0].id,
              width: 150,
              align: 'center',
              editable: true,
            };
          }

          return {
            title: item.name,
            children: item.grade_items.map((a) => {
              return {
                title: a.name,
                dataIndex: a.id,
                key: a.id,
                width: 130,
                align: 'center',
                editable: true,
              };
            }),
          };
        });
        setColumns([
          ...[
            {
              title: 'Học viên',
              dataIndex: 'information',
              key: 'information',
              width: 250,
              fixed: 'left',
              render: (text) => (
                <div
                  style={{
                    display: 'flex',
                    flexDirection: 'column',
                  }}
                >
                  <Text p b size={14}>
                    {text.name}
                  </Text>
                  <Text p i size={10}>
                    {text.enroll_number}
                  </Text>
                </div>
              ),
            },
          ],
          ...temp,
        ]);
        const temp1 = res.data[0].students.map((item) => {
          const a = {
            key: item.user_id,
            information: {
              user_id: item.user_id,
              name: item.first_name + ' ' + item.last_name,
              enroll_number: item.enroll_number,
            },
          };

          item.grade_categories.forEach((item) => {
            item.grade_items.forEach((item) => {
              a[item.id] = item.grade ? item.grade : 0;
            });
          });

          return a;
        });
        setData(temp1);
        setLoading(false);
      })
      .catch((err) => {
        toast.error('Lỗi khi lấy dữ liệu');
      });
  }, []);

  const components = {
    body: {
      row: EditableRow,
      cell: EditableCell,
    },
  };

  const handleSave = (row) => {
    const newData = [...data];
    const index = newData.findIndex((item) => row.key === item.key);
    const item = newData[index];
    newData.splice(index, 1, {
      ...item,
      ...row,
    });
    setData(newData);
  };

  const columnsEdit = columns.map((col) => {
    if (!col.editable) {
      return col;
    }
    return {
      ...col,
      onCell: (record) => ({
        record,
        editable: col.editable,
        dataIndex: col.dataIndex,
        title: col.title,
        handleSave,
      }),
    };
  });

  return (
    <Table
      components={components}
      bordered
      size="small"
      scroll={{
        x: 'max-content',
        y: 600,
      }}
      rowClassName={() => 'editable-row'}
      columns={columnsEdit}
      dataSource={data}
      loading={loading}
    />
    // <Temp/>
  );
};

export default ViewGrade;
