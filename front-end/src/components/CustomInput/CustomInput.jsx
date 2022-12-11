import { Text } from '@nextui-org/react';
import { Form, Input } from 'antd';
import { Fragment } from 'react';
import toast from 'react-hot-toast';
import { useParams } from 'react-router-dom';
import FetchApi from '../../apis/FetchApi';
import { GradeStudentApis } from '../../apis/ListApi';

const CustomInput = ({
  defaultValue: defaultGrade,
  data,
  max,
  min,
  type,
  role,
}) => {
  const [form] = Form.useForm();
  const { id, moduleId } = useParams();

  const handleBlur = () => {
    const regex = new RegExp(/^[0-9]+(\.[0-9]{1,2})?$/);

    const d = form.getFieldValue('grade');

    if (d !== null && d !== undefined && d?.trim() !== '' && !regex.test(d)) {
      return;
    }

    const body = [
      {
        student_id: defaultGrade.user_id,
        grade_item_id: defaultGrade.grade_item_id,
        grade: Number.parseFloat(form.getFieldValue('grade')),
        comment: null,
      },
    ];

    const api =
      localStorage.getItem('role') === 'teacher'
        ? GradeStudentApis.updateGradeStudentByTeacher
        : GradeStudentApis.updateGradeStudentBySro;

    FetchApi(api, body, null, [String(id), String(moduleId)])
      .then((res) => {})
      .catch((err) => {
        toast.error('Cập nhật không thành công');
      });
  };

  const disabled = role === 'teacher' && type >= 5 && type <= 8;

  return (
    <Fragment>
      {!disabled && (
        <Form form={form} initialValues={{ grade: defaultGrade.grade }}>
          <Form.Item
            name="grade"
            rules={[
              {
                // check max min
                validator: (rule, value) => {
                  if (
                    value === undefined ||
                    value === null ||
                    value?.trim() === ''
                  ) {
                    return Promise.resolve();
                  }

                  const re = new RegExp(/^[0-9]+(\.[0-9]{1,2})?$/);
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
      )}
      {disabled && (
        <Text p b size={14}>
          {defaultGrade.grade}
        </Text>
      )}
    </Fragment>
  );
};

export default CustomInput;
