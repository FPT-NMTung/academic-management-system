import { Text } from '@nextui-org/react';
import { Table } from 'antd';
import { useEffect } from 'react';
import { useState } from 'react';
import toast from 'react-hot-toast';
import { useParams } from 'react-router-dom';
import FetchApi from '../../../../apis/FetchApi';
import { GradeStudentApis } from '../../../../apis/ListApi';

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
                <div style={{
                  display: 'flex',
                  flexDirection: 'column',
                }}>
                  <Text p b size={14}>{text.name}</Text>
                  <Text p i size={10}>{text.enroll_number}</Text>
                </div>
              ),
            },
          ],
          ...temp,
        ]);

        const temp1 = res.data[0].students.map(item => {
          const a = {
            information: {
              user_id: item.user_id,
              name: item.first_name + ' ' + item.last_name,
              enroll_number: item.enroll_number,
            },
          }

          item.grade_categories.forEach(item => {
            item.grade_items.forEach(item => {
              a[item.id] = item.grade
            })
          })

          return a
        })
        setData(temp1);
        setLoading(false);
      })
      .catch((err) => {
        toast.error('Lỗi khi lấy dữ liệu');
      });
  }, []);

  return (
    <Table
      bordered
      size="small"
      scroll={{
        x: 'max-content',
        y: 600,
      }}
      columns={columns}
      dataSource={data}
      loading={loading}
    />
  );
};

export default ViewGrade;
