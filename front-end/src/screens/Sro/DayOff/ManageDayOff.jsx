import { Card, Grid, Text } from '@nextui-org/react';
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

  const getDetail = () => {
    const body = {
      date: selectedValue,
    };

    FetchApi(ManageDayOffApis.getDetail, body, null, null)
      .then((res) => {
        const dayOff = {
          morning: [],
          afternoon: [],
          evening: [],
        };
        const { data } = res;

        data.forEach((item) => {
          if (item.workingTimeId === 1) {
            dayOff.morning.push(item);
          } else if (item.workingTimeId === 2) {
            dayOff.afternoon.push(item);
          } else {
            dayOff.evening.push(item);
          }
        });
      })
      .catch((err) => {
        toast.error('Lỗi lấy chi tiết ngày nghỉ');
      });
  };

  useEffect(() => {
    getDetail();
  }, [selectedValue]);

  useEffect(() => {
    getListDayOff();
  }, []);

  return (
    <Grid.Container gap={2} justify={'center'}>
      <Grid xs={3}>
        <Card
          variant="bordered"
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={14}
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              Lịch nghỉ
            </Text>
          </Card.Header>
          <Card.Body
            css={{
              padding: '5px 10px',
            }}
          >
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
                          dropdownMatchSelectWidth={false}
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
                return (
                  <Card
                    variant={value.day() === 0 ? 'flat' : 'bordered'}
                    disableRipple={true}
                    css={{
                      fontSize: '12px',
                      width: '40px',
                      height: '40px',
                      margin: '1px auto',
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
                          ? '#fdd8e5'
                          : value.day() === 0
                          ? '#F1F1F1'
                          : '#fff',
                      borderRadius: '10em',
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
      <Grid xs={5}>
        <Card
          variant="bordered"
          css={{
            height: 'fit-content',
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={14}
              css={{
                width: '100%',
                textAlign: 'center',
              }}
            >
              Lịch nghỉ của ngày {selectedValue.format('DD/MM/YYYY')}
            </Text>
          </Card.Header>
          <Card.Body>
            <Grid.Container gap={2}>
              <Grid xs={12}>
                <Card
                  variant="bordered"
                  css={{
                    minHeight: '140px',
                    borderStyle: 'dashed',
                  }}
                >
                  <Card.Header>
                    <Text p b size={14}>
                      Buổi sáng
                    </Text>
                  </Card.Header>
                  <Card.Body></Card.Body>
                </Card>
              </Grid>
              <Grid xs={12}>
                <Card
                  variant="bordered"
                  css={{
                    minHeight: '140px',
                    borderStyle: 'dashed',
                  }}
                >
                  <Card.Header>
                    <Text p b size={14}>
                      Buổi chiều
                    </Text>
                  </Card.Header>
                  <Card.Body></Card.Body>
                </Card>
              </Grid>
              <Grid xs={12}>
                <Card
                  variant="bordered"
                  css={{
                    minHeight: '140px',
                    borderStyle: 'dashed',
                  }}
                >
                  <Card.Header>
                    <Text p b size={14}>
                      Buổi tối
                    </Text>
                  </Card.Header>
                  <Card.Body></Card.Body>
                </Card>
              </Grid>
            </Grid.Container>
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default ManageDayOff;
