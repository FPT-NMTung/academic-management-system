import { Card, Grid } from '@nextui-org/react';
import { Calendar, Select, Row, Col } from 'antd';
import { useState } from 'react';
import moment from 'moment';
import 'moment/locale/vi';
import { useEffect } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { ManageDayOffApis } from '../../../apis/ListApi';
import toast from 'react-hot-toast';

const ManageDayOff = () => {
  const [listDayOff, setListDayOff] = useState([]);
  const [value, setValue] = useState(moment());
  const [selectedValue, setSelectedValue] = useState(moment());

  const getListDayOff = () => {
    FetchApi(ManageDayOffApis.getDayOff, null, null, null)
      .then((res) => {
        setListDayOff(res.data);
      })
      .catch((err) => {
        toast.error('Lỗi lấy danh sách ngày nghỉ');
      });
  };

  useEffect(() => {
    getListDayOff();
  }, []);

  return (
    <Grid.Container gap={2}>
      <Grid xs={4}>
        <Card variant="bordered">
          <Card.Body>
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
                    disableRipple={true}
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
                          ? '#0072F5'
                          : selectedValue.format('MM/YYYY') ===
                            value.format('MM/YYYY')
                          ? 'black'
                          : 'lightgray',
                      fontWeight:
                        selectedValue.format('MM/YYYY') ===
                        value.format('MM/YYYY')
                          ? '500'
                          : '200',
                      backgroundColor:
                        value.format('DD/MM/YYYY') ===
                        selectedValue.format('DD/MM/YYYY')
                          ? '#CEE4FE'
                          : listDayOff.find(
                              (item) =>
                                value.format('DD/MM/YYYY') ===
                                moment(item).format('DD/MM/YYYY')
                            )
                          ? '#88F1B6'
                          : value.day() === 0
                          ? '#F1F1F1'
                          : '#fff',
                      borderRadius: '8px',
                      borderColor:
                        value.format('DD/MM/YYYY') ===
                        moment().format('DD/MM/YYYY')
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
          </Card.Body>
        </Card>
      </Grid>
      <Grid xs={8}>
        <Card variant="bordered">
          <Card.Body></Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default ManageDayOff;
