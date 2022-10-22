import {
  Card,
  Grid,
  Text,
  Modal,
  Button,
  Table,
  Loading,
} from '@nextui-org/react';
import { Form, Select, Input, Divider } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../../apis/ListApi';
import classes from './CenterScreen.module.css';
import { FaPen } from 'react-icons/fa';
import CenterCreate from '../../../components/CenterCreate/CenterCreate';
import CenterUpdate from '../../../components/CenterUpdate/CenterUpdate';

const CenterScreen = () => {
  const [listCenter, setListCenter] = useState([]);
  const [selectedCenterId, setSelectedCenterId] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isCreate, setIsCreate] = useState(false);

  const getData = () => {
    setIsLoading(true);
    const apiCanter = CenterApis.getAllCenter;

    FetchApi(apiCanter).then((res) => {
      const data = res.data;

      const mergeAddressRes = data.map((e) => {
        return {
          key: e.id,
          ...e,
          address: `${e.ward.prefix} ${e.ward.name}, ${e.district.prefix} ${e.district.name}, ${e.province.name}`,
        };
      });

      setListCenter(mergeAddressRes);
      setIsLoading(false);
    });
  };

  const handleAddSuccess = () => {
    setIsCreate(false);
    getData();
  };

  const handleUpdateSuccess = () => {
    setSelectedCenterId(null);
    getData();
  };

  useEffect(() => {
    getData();
  }, []);

  return (
    <div>
      <Grid.Container gap={2} justify="center">
        <Grid xs={6.5}>
          <Card
            variant="bordered"
            css={{
              width: '100%',
              height: 'fit-content',
              minHeight: '200px',
            }}
          >
            <Card.Header>
              <div className={classes.headerTable}>
                <Grid.Container justify="space-between">
                  <Grid xs={4}></Grid>
                  <Grid xs={4}>
                    <Text
                      b
                      size={14}
                      p
                      css={{
                        width: '100%',
                        textAlign: 'center',
                      }}
                    >
                      Danh sách các cơ sở
                    </Text>
                  </Grid>
                  <Grid xs={4} justify="flex-end">
                    <Button
                      type="primary"
                      onPress={() => {
                        setIsCreate(!isCreate);
                      }}
                      auto
                      flat
                    >
                      + Thêm cơ sở
                    </Button>
                  </Grid>
                </Grid.Container>
              </div>
            </Card.Header>
            {isLoading && <Loading />}
            {!isLoading && (
              <Table>
                <Table.Header>
                  <Table.Column>STT</Table.Column>
                  <Table.Column>Tên</Table.Column>
                  <Table.Column>Địa chỉ</Table.Column>
                  <Table.Column width={30}></Table.Column>
                </Table.Header>
                <Table.Body>
                  {listCenter.map((data, index) => (
                    <Table.Row key={data.id}>
                      <Table.Cell>{index + 1}</Table.Cell>
                      <Table.Cell>{data.name}</Table.Cell>
                      <Table.Cell>{data.address}</Table.Cell>
                      <Table.Cell>
                        <FaPen
                          size={14}
                          color="5EA2EF"
                          style={{ cursor: 'pointer' }}
                          onClick={() => {
                            setSelectedCenterId(data.id);
                          }}
                        />
                      </Table.Cell>
                    </Table.Row>
                  ))}
                </Table.Body>
                <Table.Pagination
                  shadow
                  noMargin
                  align="center"
                  rowsPerPage={5}
                />
              </Table>
            )}
          </Card>
        </Grid>
        {isCreate && <CenterCreate onCreateSuccess={handleAddSuccess} />}
      </Grid.Container>
      <Modal
        closeButton
        aria-labelledby="modal-title"
        open={selectedCenterId !== null}
        onClose={() => {
          setSelectedCenterId(null);
        }}
        blur
        width="500px"
      >
        <Modal.Header>
          <Text size={16} b>
            Cập nhật thông tin cơ sở
          </Text>
        </Modal.Header>
        <Modal.Body>
          <CenterUpdate
            data={listCenter.find((e) => e.id === selectedCenterId)}
            onUpdateSuccess={handleUpdateSuccess}
          />
        </Modal.Body>
      </Modal>
    </div>
  );
};

export default CenterScreen;
