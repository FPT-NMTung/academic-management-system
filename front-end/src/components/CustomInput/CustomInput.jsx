import { Text } from '@nextui-org/react';
import { Form, Input } from 'antd';
import { Fragment } from 'react';
import toast from 'react-hot-toast';
import { useParams } from 'react-router-dom';
import FetchApi from '../../apis/FetchApi';
import { GradeStudentApis } from '../../apis/ListApi';

const CustomInput = ({ defaultValue: defaultGrade, data, max, min }) => {
  const [form] = Form.useForm();
  const { id, moduleId } = useParams();

  const handleBlur = () => {
    const regex = new RegExp(/^[0-9]+(\.[0-9]{1,5})?$/);

    if (!regex.test(form.getFieldValue('grade'))) {
      return;
    }

    toast.success('Cập nhật');

    console.log(data);

    const body = [
      {
        student_id: defaultGrade.user_id,
        grade_item_id: defaultGrade.grade_item_id,
        grade: Number.parseFloat(form.getFieldValue('grade')),
        comment: null,
      },
    ];
    FetchApi(GradeStudentApis.updateGradeStudentBySro, body, null, [
      String(id),
      String(moduleId),
    ])
      .then((res) => {})
      .catch((err) => {});
  };

  return (
    <Form form={form} initialValues={{ grade: defaultGrade.grade }}>
      <Form.Item
        name="grade"
        rules={[
          {
            // check max min
            validator: (rule, value) => {
              const re = new RegExp(/^[0-9]+(\.[0-9]{1,5})?$/);
              if (!re.test(value)) {
                return Promise.reject('Không hợp lệ');
              }

              const number = Number.parseFloat(value);
              if (number > max) {
                return Promise.reject('Không hợp lệ');
              }

              if (number < min) {
                return Promise.reject('Không hợp lệ');
              }

              return Promise.resolve();
            },
          },
        ]}
      >
        <Input onBlur={handleBlur} defaultValue={defaultGrade} />
      </Form.Item>
    </Form>
  );
};

export default CustomInput;
