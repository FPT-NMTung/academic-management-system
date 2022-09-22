import { Card } from '@nextui-org/react';
import { Calendar, Select, Typography, Row, Col, Radio } from 'antd';
import { useState } from 'react';
import moment from 'moment';

const learningDay = [
  { type: 1, date: '05/09/2022' },
  { type: 1, date: '07/09/2022' },
  { type: 1, date: '09/09/2022' },
  { type: 1, date: '12/09/2022' },
  { type: 1, date: '14/09/2022' },
  { type: 1, date: '16/09/2022' },
  { type: 1, date: '19/09/2022' },
  { type: 2, date: '21/09/2022' },
  { type: 2, date: '23/09/2022' },
];

const CalendarStudent = () => {
  const [value, setValue] = useState(moment());
  const [selectedValue, setSelectedValue] = useState(moment());

  return (
    <Calendar
      fullscreen={false}
      value={value}
      onSelect={(value) => {
        setValue(value);
        setSelectedValue(value);
      }}
      headerRender={({ value, onChange }) => {
        const start = 0;
        const end = 12;
        const monthOptions = [];

        const months = [
          'Tháng 1',
          'Tháng 2',
          'Tháng 3',
          'Tháng 4',
          'Tháng 5',
          'Tháng 6',
          'Tháng 7',
          'Tháng 8',
          'Tháng 9',
          'Tháng 10',
          'Tháng 11',
          'Tháng 12',
        ];

        for (let i = start; i < end; i++) {
          monthOptions.push(
            <Select.Option key={i} value={i} className="month-item">
              {months[i]}
            </Select.Option>
          );
        }

        const year = value.year();
        const month = value.month();
        const options = [];
        for (let i = year - 10; i < year + 10; i += 1) {
          options.push(
            <Select.Option key={i} value={i} className="year-item">
              {i}
            </Select.Option>
          );
        }
        return (
          <div style={{ padding: 8 }}>
            <Row gutter={8}>
              <Col>
                <Select
                  size="small"
                  dropdownMatchSelectWidth={false}
                  className="my-year-select"
                  value={year}
                  onChange={(newYear) => {
                    const now = value.clone().year(newYear);
                    onChange(now);
                  }}
                >
                  {options}
                </Select>
              </Col>
              <Col>
                <Select
                  size="small"
                  dropdownMatchSelectWidth={false}
                  value={month}
                  onChange={(newMonth) => {
                    const now = value.clone().month(newMonth);
                    onChange(now);
                  }}
                >
                  {monthOptions}
                </Select>
              </Col>
            </Row>
          </div>
        );
      }}
      mode="month"
      dateFullCellRender={(value) => {
        console.log(value.format('DD/MM/YYYY'));
        return (
          <Card
            variant={value.day() === 0 ? 'flat' : 'bordered'}
            css={{
              width: '60px',
              height: '40px',
              margin: '3px auto',
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
              color:
                selectedValue.format('DD/MM/YYYY') ===
                value.format('DD/MM/YYYY')
                  ? 'white'
                  : selectedValue.format('MM/YYYY') === value.format('MM/YYYY')
                  ? 'black'
                  : 'lightgray',
              fontWeight:
                selectedValue.format('MM/YYYY') === value.format('MM/YYYY')
                  ? '500'
                  : '200',
              backgroundColor:
                value.format('DD/MM/YYYY') ===
                selectedValue.format('DD/MM/YYYY')
                  ? '#0072F5'
                  : learningDay.find(
                      (item) =>
                        value.format('DD/MM/YYYY') === item.date &&
                        item.type === 1
                    )
                  ? '#88F1B6'
                  : learningDay.find(
                      (item) =>
                        value.format('DD/MM/YYYY') === item.date &&
                        item.type === 2
                    )
                  ? '#F9CB80'
                  : value.day() === 0
                  ? '#F1F1F1'
                  : '#fff',
              borderRadius: '8px',
              borderColor:
                value.format('DD/MM/YYYY') === moment().format('DD/MM/YYYY')
                  ? '#0072F5'
                  : '',
            }}
            isPressable={true}
          >
            {value.date()}
          </Card>
        );
      }}
    />
  );
};

export default CalendarStudent;
