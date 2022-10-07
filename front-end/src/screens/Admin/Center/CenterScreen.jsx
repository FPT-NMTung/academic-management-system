import { Card, Grid, Text, Modal } from '@nextui-org/react';
import { Form, Select, Input, Divider, Button, Table } from 'antd';
import { useEffect, useState } from 'react';
import FetchApi from '../../../apis/FetchApi';
import { AddressApis, CenterApis } from '../../../apis/ListApi';
import classes from './CenterScreen.module.css';
import { MdEdit } from 'react-icons/md';
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
        <Grid xs={5}>
          <Card
            css={{
              width: '100%',
              height: 'fit-content',
            }}
          >
            <Card.Header>
              <div className={classes.headerTable}>
                <Text size={14}>Danh sách các cơ sở</Text>
                <Button
                  type="primary"
                  onClick={() => {
                    setIsCreate(!isCreate);
                  }}
                >
                  Thêm cơ sở
                </Button>
              </div>
            </Card.Header>
            <Card.Divider />
            <Table
              pagination={{ position: ['bottomCenter'] }}
              dataSource={listCenter}
              loading={isLoading}
            >
              <Table.Column title="Tên" dataIndex="name" key="name" />
              <Table.Column title="Địa chỉ" dataIndex="address" key="address" />
              <Table.Column
                title=""
                dataIndex="action"
                key="action"
                render={(_, data) => {
                  return (
                    <MdEdit
                      className={classes.editIcon}
                      onClick={() => {
                        setSelectedCenterId(data.key);

                        
                      }}
                    />
                  );
                }}
              />
            </Table>
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
          <CenterUpdate data={listCenter.find((e) => e.id === selectedCenterId)} onUpdateSuccess={handleUpdateSuccess}/>
    
        </Modal.Body>
      </Modal>
    </div>
  );
 
};

export default CenterScreen;
