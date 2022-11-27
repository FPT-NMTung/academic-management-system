import { Modal, Text } from '@nextui-org/react';
import { Input, Table } from 'antd';
import { useEffect } from 'react';
import { useState } from 'react';
import toast from 'react-hot-toast';
import { Link } from 'react-router-dom';
import FetchApi from '../../../../../apis/FetchApi';
import { ManageAttendanceApis } from '../../../../../apis/ListApi';

import classes from './TakeAttendance.module.css';

const TakeAttendance = ({ session, scheduleId, onClose }) => {
  const [listAttendance, setListAttendance] = useState([]);

  const getListAttendance = () => {
    FetchApi(ManageAttendanceApis.getAttendanceBySlotId, null, null, [
      String(scheduleId),
      String(session.id),
    ])
      .then((res) => {
        const data = res.data[0].attendances.map((item, index) => {
          return {
            image: item.student.avatar,
            information: {
              name: item.student.first_name + ' ' + item.student.last_name,
              user_id: item.student.user_id,
              enroll_number: item.student.enroll_number,
            },
            status: item.attendance_status,
            note: item.note,
            key: index,
          };
        });

        setListAttendance(data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy thoong tin điểm danh');
      });
  };

  useEffect(() => {
    getListAttendance();
  }, []);

  return (
    <Modal
      blur
      open={session !== undefined}
      closeButton={true}
      onClose={onClose}
      width={900}
    >
      <Modal.Header>
        <Text p b size={14}>
          Điểm danh
        </Text>
      </Modal.Header>
      <Modal.Body>
        <Table
          columns={[
            {
              title: 'Hình ảnh',
              dataIndex: 'image',
              key: 'image',
              render: (image) => (<img className={classes.imageStudent} src={image}/>)
            },
            {
              title: 'Thông tin',
              dataIndex: 'information',
              key: 'information',
              render: (information) => (
                <div className={classes.information}>
                  <Text p b size={14}>
                    {information.name}
                  </Text>
                  <Text p size={14}>
                    {information.user_id}
                  </Text>
                  <Text p size={14}>
                    {information.enroll_number}
                  </Text>
                </div>
              ),
            },
            {
              title: 'Trạng thái',
              dataIndex: 'status',
              key: 'status',
            },
            {
              title: 'Ghi chú',
              dataIndex: 'note',
              key: 'note',
              render: (note)=>(<Input/>)
            },
          ]}
          dataSource={listAttendance}
          pagination={false}
          scroll={{ y: 500 }}
        />
      </Modal.Body>
    </Modal>
  );
};

export default TakeAttendance;
