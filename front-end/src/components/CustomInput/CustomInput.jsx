import { Text } from '@nextui-org/react';
import { Form, Input } from 'antd';
import { Fragment } from 'react';
import toast from 'react-hot-toast';
import { useParams } from 'react-router-dom';
import FetchApi from '../../apis/FetchApi';
import { GradeStudentApis } from '../../apis/ListApi';

const CustomInput = ({ defaultValue: defaultGrade, data }) => {
  const [form] = Form.useForm();
  const { id, moduleId } = useParams();

  const handleBlur = () => {
    const body = [
      {
        student_id: 489,
        grade_item_id: 1303,
        grade: Number.parseFloat(form.getFieldValue('grade')),
        comment: null,
      },
    ];
    FetchApi(GradeStudentApis.updateGradeStudentBySro, body, null, [
      String(id),
      String(moduleId),
    ])
      .then((res) => {
      })
      .catch((err) => {
      });
  };

  return (
    <Form form={form} initialValues={{ grade: defaultGrade.grade }}>
      <Form.Item
        name="grade"
        rules={[
          {
            pattern: new RegExp(/^[0-9]+(\.[0-9]{1,4})?$/),
            message: 'Không hợp lệ',
          },
        ]}
      >
        <Input onBlur={handleBlur} defaultValue={defaultGrade} />
      </Form.Item>
    </Form>
  );
};

export default CustomInput;
