import { Modal, Text } from '@nextui-org/react';
import { Table } from 'antd';
import { useEffect } from 'react';
import { useState } from 'react';
import FetchApi from '../../../../../apis/FetchApi';
import { ManageAttendanceApis } from '../../../../../apis/ListApi';
import { HiBadgeCheck, HiXCircle, HiMinusSm } from 'react-icons/hi';
import { Link } from 'react-router-dom';

const ViewAllAttendance = ({ open, scheduleId, onClose }) => {
  const [isLoading, setIsLoading] = useState(true);
  const [listAttendance, setListAttendance] = useState([]);
  const [columns, setColumns] = useState([]);

  const getListAttendance = () => {
    setIsLoading(true);
    FetchApi(ManageAttendanceApis.getAttendanceByScheduleId, null, null, [
      String(scheduleId),
    ])
      .then((res) => {
        const data = res.data.map((item) => item.attendances);
        let listStudent = data[0].map((item) => {
          return { student: item.student };
        });
        const objRes = [];

        listStudent.forEach((item, index) => {
          const tempObj = {};
          const dataAt = data.map((da) => {
            return da.find((st) => st.student.user_id === item.student.user_id);
          });

          dataAt.forEach((da, index) => {
            const at = da.attendance_status;
            tempObj[`slot-${index + 1}`] = at === null ? 1 : at.id;
          });

          const clone = { ...item, ...tempObj };
          objRes.push(clone);
        });

        setListAttendance(objRes);
        setColumns([
          {
            title: `Thông tin học viên`,
            dataIndex: `student`,
            key: `student`,
            fixed: `left`,
            width: 300,
            render: (student, record) => {
              return (
                <div>
                  <Link
                    target={'_blank'}
                    to={`/sro/manage/student/${student.user_id}`}
                  >
                    <Text p b size={14}>
                      {student.first_name} {student.last_name}
                    </Text>
                  </Link>
                  <Text p size={10}>
                    {student.enroll_number}
                  </Text>
                </div>
              );
            },
          },
          ...res.data.map((item, index) => {
            return {
              title: `Slot ${index + 1}`,
              dataIndex: `slot-${index + 1}`,
              key: `slot-${index + 1}`,
              width: 90,
              align: `center`,
              render: (statue, record) => {
                return statue === 1 ? (
                  <HiMinusSm color="lightgray" />
                ) : statue === 3 ? (
                  <HiBadgeCheck size={24} color={'44f255'} />
                ) : (
                  <HiXCircle size={24} color={'f52020'} />
                );
              },
            };
          }),
        ]);
        setIsLoading(false);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  useEffect(() => {
    getListAttendance();
  }, []);

  return (
    <Modal open={open} width={'90%'} blur closeButton onClose={onClose}>
      <Modal.Header>
        <Text p b size={14}>
          Xem điểm danh tất cả slot
        </Text>
      </Modal.Header>
      <Modal.Body>
        <Table
          loading={isLoading}
          pagination={false}
          scroll={{ x: 'calc(700px + 50%)', y: 700 }}
          dataSource={listAttendance}
          columns={columns}
        />
      </Modal.Body>
    </Modal>
  );
};

export default ViewAllAttendance;
