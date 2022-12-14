import { Loading, Spacer, Text } from '@nextui-org/react';
import { Form, Table, InputNumber, Input } from 'antd';
import React, { Fragment, useEffect } from 'react';
import { useRef } from 'react';
import { useState, useContext } from 'react';
import toast from 'react-hot-toast';
import { useParams } from 'react-router-dom';
import FetchApi from '../../../../apis/FetchApi';
import { GradeStudentApis } from '../../../../apis/ListApi';
import CustomInput from '../../../../components/CustomInput/CustomInput';

const ViewGrade = ({ role, dataModule, dataSchedule, onSuccess }) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [columns, setColumns] = useState([]);
  const [isSave, setIsSave] = useState(undefined);
  // 0: loading
  // 1: success
  // 2: error

  console.log(dataModule);

  const { id, moduleId } = useParams();

  useEffect(() => {
    setData([]);
    setColumns([]);
  }, [id, moduleId]);

  const onSave = (api, body, id, moduleId) => {
    setIsSave(0);
    FetchApi(api, body, null, [String(id), String(moduleId)])
      .then((res) => {
        setIsSave(1);
      })
      .catch((err) => {
        setIsSave(2);
      });
  };

  useEffect(() => {
    const time = setTimeout(() => {
      if (isSave === 1) {
        setIsSave(undefined);
      } else if (isSave === 2) {
        
      }
    }, 1000);
    
    return () => {
      clearTimeout(time);
    };
  }, [isSave]);

  useEffect(() => {
    setLoading(true);
    const api =
      role === 'teacher'
        ? GradeStudentApis.getListGradeByClassIdAndModuleIdByTeacher
        : GradeStudentApis.getListGradeByClassIdAndModuleIdBySro;

    FetchApi(api, null, null, [String(id), String(moduleId)])
      .then((res) => {
        console.log(res);
        const temp = res.data[0].students[0].grade_categories.map((item) => {
          if (item.grade_items.length === 1) {
            return {
              title: item.name,
              dataIndex: item.grade_items[0].id,
              key: item.grade_items[0].id,
              width: 130,
              align: 'center',
              editable: true,
              render: (source, t) => {
                console.log(t);
                return (
                  <CustomInput
                    role={role}
                    max={
                      item.id === 5 || item.id === 7
                        ? dataModule.max_practical_grade
                        : item.id === 6 || item.id === 8
                        ? dataModule.max_theory_grade
                        : 10
                    }
                    onSave={onSave}
                    type={item.id}
                    min={0}
                    data={t}
                    defaultValue={source}
                  />
                );
              },
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
                render: (source, t) => {
                  return (
                    <CustomInput
                      role={role}
                      max={
                        item.id === 5 || item.id === 7
                          ? dataModule.max_practical_grade
                          : item.id === 6 || item.id === 8
                          ? dataModule.max_theory_grade
                          : 10
                      }
                      onSave={onSave}
                      type={item.id}
                      min={0}
                      data={t}
                      defaultValue={source}
                    />
                  );
                },
              };
            }),
          };
        });
        console.log(temp);
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

          item.grade_categories.forEach((item1) => {
            item1.grade_items.forEach((item2) => {
              a[item2.id] = {
                grade: item2.grade,
                user_id: item.user_id,
                grade_item_id: item2.id,
              };
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

  return (
    <Fragment>
      <div>
        {isSave === undefined && <Text p i size={14} color={'gray'}>Đang chờ ...</Text>}
        {isSave === 0 && <Loading size={'xs'} />}
        {isSave === 1 && <Text p b size={14} color={'success'}>Lưu thành công</Text>}
        {isSave === 2 && <Text p b size={14} color={'error'}>Lưu không thành công</Text>}
        <Spacer y={0.5} />
      </div>
      <Table
        bordered
        size="small"
        scroll={{
          x: 'max-content',
          y: 600,
        }}
        rowClassName={() => 'editable-row'}
        columns={columns}
        dataSource={data}
        loading={loading}
        pagination={false}
      />
    </Fragment>
  );
};

export default ViewGrade;
