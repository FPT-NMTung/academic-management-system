import {
  Grid,
  Card,
  Text,
  Spacer,
  Button,
  Loading,
  Switch,
} from '@nextui-org/react';
import { Form, Input, Select, Divider, DatePicker, Descriptions } from 'antd';
import classes from './SroCreate.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import {
  CenterApis,
  GenderApis,
  AddressApis,
  ManageSroApis,
} from '../../../../apis/ListApi';
import FetchApi from '../../../../apis/FetchApi';
import { Validater } from '../../../../validater/Validater';
import moment from 'moment';
import { ErrorCodeApi } from '../../../../apis/ErrorCodeApi';
import { Fragment } from 'react';
import { FaLock } from 'react-icons/fa';
import toast from 'react-hot-toast';

const SroCreate = ({ modeUpdate }) => {
  const [listCenter, setListCenter] = useState([]);
  const [listGender, setListGender] = useState([]);
  const [listProvince, setListProvince] = useState([]);
  const [listDistrict, setListDistrict] = useState([]);
  const [listWard, setListWard] = useState([]);
  const [isGetDateUser, setIsGetDateUser] = useState(modeUpdate);
  const [isCreatingOrUpdating, setIsCreatingOrUpdating] = useState(false);
  const [messageFailed, setMessageFailed] = useState(undefined);
  const [dataUser, setDataUser] = useState(undefined);
  const [isUnlockDelete, setIsUnlockDelete] = useState(false);
  const [canDelete, setCanDelete] = useState(undefined);

  const navigate = useNavigate();
  const { id } = useParams();
  const [form] = Form.useForm();

  const getListCenter = () => {
    FetchApi(CenterApis.getAllCenter).then((res) => {
      setListCenter(res.data);
    });
  };

  const getListGender = () => {
    FetchApi(GenderApis.getAllGender).then((res) => {
      setListGender(res.data);
    });
  };

  const getListProvince = () => {
    FetchApi(AddressApis.getListProvince).then((res) => {
      setListProvince(res.data);
    });
    setListDistrict([]);
    setListWard([]);
  };

  const getListDistrict = () => {
    const provinceId = form.getFieldValue('province_id');

    FetchApi(AddressApis.getListDistrict, null, null, [`${provinceId}`]).then(
      (res) => {
        setListDistrict(res.data);
      }
    );
    setListWard([]);
    form.setFieldsValue({ district_id: null, ward_id: null });
  };

  const getListWard = () => {
    const provinceId = form.getFieldValue('province_id');
    const districtId = form.getFieldValue('district_id');

    FetchApi(AddressApis.getListWard, null, null, [
      `${provinceId}`,
      `${districtId}`,
    ]).then((res) => {
      setListWard(res.data);
    });

    form.setFieldsValue({ ward_id: null });
  };

  const getListDistrictForUpdate = () => {
    const provinceId = form.getFieldValue('province_id');

    FetchApi(AddressApis.getListDistrict, null, null, [`${provinceId}`]).then(
      (res) => {
        setListDistrict(res.data);
      }
    );
  };

  const getListWardForUpdate = () => {
    const provinceId = form.getFieldValue('province_id');
    const districtId = form.getFieldValue('district_id');

    FetchApi(AddressApis.getListWard, null, null, [
      `${provinceId}`,
      `${districtId}`,
    ]).then((res) => {
      setListWard(res.data);
    });
  };

  const handleSubmitForm = () => {
    setIsCreatingOrUpdating(true);
    setMessageFailed(undefined);
    const data = form.getFieldsValue();
    const body = {
      first_name: data.first_name.trim(),
      last_name: data.last_name.trim(),
      mobile_phone: data.mobile_phone.trim(),
      email: data.email.trim(),
      email_organization: data.email_organization.trim(),
      province_id: data.province_id,
      district_id: data.district_id,
      ward_id: data.ward_id,
      gender_id: data.gender_id,
      birthday: moment.utc(data.birthday).local().format(),
      center_id: data.center_id,
      citizen_identity_card_no: data.citizen_identity_card_no.trim(),
      citizen_identity_card_published_date: moment
        .utc(data.citizen_identity_card_published_date)
        .local()
        .format(),
      citizen_identity_card_published_place:
        data.citizen_identity_card_published_place.trim(),
    };

    const api = modeUpdate ? ManageSroApis.updateSro : ManageSroApis.createSro;
    const params = modeUpdate ? [`${id}`] : null;

    toast.promise(FetchApi(api, body, null, params), {
      loading: '??ang x??? l??...',
      success: (res) => {
        setIsCreatingOrUpdating(false);
        const user_id = res.data.user_id;
        navigate(`/admin/account/sro/${user_id}`, { replace: true });
        return 'Th??nh c??ng';
      },
      error: (err) => {
        setIsCreatingOrUpdating(false);
        if (err?.type_error) {
          return ErrorCodeApi[err.type_error];
        }
        return 'C?? l???i x???y ra';
      },
    });
  };

  const getInformationSro = () => {
    FetchApi(ManageSroApis.getDetailSro, null, null, [`${id}`])
      .then((res) => {
        const data = res.data;
        setDataUser(data);
        form.setFieldsValue({
          first_name: data.first_name,
          last_name: data.last_name,
          mobile_phone: data.mobile_phone,
          email: data.email,
          email_organization: data.email_organization,
          province_id: data.province.id,
          district_id: data.district.id,
          ward_id: data.ward.id,
          gender_id: data.gender.id,
          birthday: moment(data.birthday),
          center_id: data.center_id,
          citizen_identity_card_no: data.citizen_identity_card_no,
          citizen_identity_card_published_date: moment(
            data.citizen_identity_card_published_date
          ),
          citizen_identity_card_published_place:
            data.citizen_identity_card_published_place,
        });
        setIsGetDateUser(false);
        getListCenter();
        getListGender();
        getListDistrictForUpdate();
        getListWardForUpdate();
      })
      .catch((err) => {
        navigate('/admin/account/sro');
      });
  };

  const handleDelete = () => {
    if (!isUnlockDelete) {
      setIsUnlockDelete(true);
      return;
    }
    toast.promise(
      FetchApi(ManageSroApis.deleteSro, null, null, [String([`${id}`])]),
      {
        loading: '??ang x??a',
        success: (res) => {
          navigate('/admin/account/sro');
          return 'X??a th??nh c??ng';
        },
        error: (err) => {
          return 'X??a th???t b???i';
        },
      }
    );
  };

  const handleChangeActive = () => {
    toast.promise(FetchApi(ManageSroApis.changeActive, null, null, [`${id}`]), {
      loading: '??ang thay ?????i tr???ng th??i',
      success: () => {
        return 'Thay ?????i tr???ng th??i th??nh c??ng';
      },
      error: () => {
        return 'Thay ?????i tr???ng th??i th???t b???i';
      },
    });
  };
  const checkCanDelete = () => {
    FetchApi(ManageSroApis.checkCanDeleteSro, null, null, [`${id}`])
      .then((res) => {
        if (res.data.can_delete === true) {
          setCanDelete(true);
        } else {
          setCanDelete(false);
        }
      })
      .catch((err) => {
        toast.error('L???i ki???m tra kh??? n??ng x??a');
      });
  };

  useEffect(() => {
    getListCenter();
    getListGender();
    getListProvince();
    if (modeUpdate) {
      getInformationSro();
      checkCanDelete();
    }
  }, []);

  return (
    <Form
      labelCol={{ span: 8 }}
      wrapperCol={{ span: 15 }}
      form={form}
      onFinish={handleSubmitForm}
      disabled={isGetDateUser}
    >
      <Grid.Container justify="center" gap={2}>
        <Grid xs={7} direction={'column'} css={{ rowGap: 20 }}>
          <Card variant="bordered">
            <Card.Header>
              <Text
                b
                size={16}
                p
                css={{
                  width: '100%',
                  textAlign: 'center',
                }}
              >
                {!modeUpdate && 'T???o m???i SRO'}
                {modeUpdate && 'C???p nh???t SRO'}
              </Text>
            </Card.Header>
            <Card.Body>
              <div className={classes.layout}>
                <Form.Item
                  label="C??? s???"
                  name="center_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y ch???n c?? s???',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    disabled={modeUpdate}
                    placeholder="C?? s???"
                    loading={listCenter.length === 0}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listCenter
                      .filter((e) => e.is_active)
                      .map((e) => (
                        <Select.Option key={e.id} value={e.id}>
                          {e.name}
                        </Select.Option>
                      ))}
                  </Select>
                </Form.Item>
              </div>
              <Spacer y={1.5} />
              <div className={classes.layout}>
                <Form.Item
                  label="H???, t??n ?????m"
                  name="first_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Tr?????ng ph???i t??? 1 ?????n 255 k?? t???'
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="H??? v?? t??n ?????m" />
                </Form.Item>
                <Form.Item
                  label="T??n"
                  name="last_name"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Tr?????ng ph???i t??? 1 ?????n 255 k?? t???'
                          );
                        }
                        if (Validater.isNotHumanName(value.trim())) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="T??n" />
                </Form.Item>
                <Form.Item
                  label="Gi???i t??nh"
                  name="gender_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y nh???p gi???i t??nh',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="Gi???i t??nh"
                    loading={listGender.length === 0}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listGender.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.value}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Ng??y sinh"
                  name="birthday"
                  rules={[
                    {
                      required: true,
                      message: 'H??y nh???p ng??y sinh',
                    },
                  ]}
                >
                  <DatePicker format={'DD/MM/YYYY'} />
                </Form.Item>
                <Form.Item
                  label="S??? ??i???n tho???i"
                  name="mobile_phone"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        // check regex phone number viet nam
                        if (Validater.isPhone(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('S??? ??i???n tho???i kh??ng h???p l???')
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="0891234567" />
                </Form.Item>
                <div></div>
                <Form.Item
                  label="Email c?? nh??n"
                  name="email"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (Validater.isEmail(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Email kh??ng h???p l???'));
                      },
                    },
                  ]}
                >
                  <Input placeholder="example@gmail.com" />
                </Form.Item>
                <Form.Item
                  label="Email t??? ch???c"
                  name="email_organization"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (Validater.isEmail(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(new Error('Email kh??ng h???p l???'));
                      },
                    },
                  ]}
                >
                  <Input type="email" placeholder="example@domain.com" />
                </Form.Item>
              </div>
              <Spacer y={1.5} />
              <div className={classes.layout}>
                <Form.Item
                  label="T???nh/Th??nh ph???"
                  name="province_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y ch???n t???nh/th??nh ph???',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="T???nh/Th??nh ph???"
                    loading={listProvince.length === 0}
                    onChange={() => {
                      getListDistrict();
                    }}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listProvince.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {e.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Qu???n/Huy???n"
                  name="district_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y ch???n qu???n/huy???n',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="Qu???n/Huy???n"
                    loading={listDistrict.length === 0}
                    onChange={() => {
                      getListWard();
                    }}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listDistrict.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {`${e.prefix} ${e.name}`}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item
                  label="Ph?????ng/X??"
                  name="ward_id"
                  rules={[
                    {
                      required: true,
                      message: 'H??y ch???n ph?????ng/x??',
                    },
                  ]}
                >
                  <Select
                    showSearch
                    placeholder="Ph?????ng/X??"
                    loading={listWard.length === 0}
                    filterOption={(input, option) =>
                      option.children
                        .toLowerCase()
                        .includes(input.toLowerCase())
                    }
                  >
                    {listWard.map((e) => (
                      <Select.Option key={e.id} value={e.id}>
                        {`${e.prefix} ${e.name}`}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
              </div>
              <Spacer y={1.5} />
              <div className={classes.layout}>
                <Form.Item
                  label="S??? CMND/CCCD"
                  name="citizen_identity_card_no"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (Validater.isCitizenId(value)) {
                          return Promise.resolve();
                        }
                        return Promise.reject(
                          new Error('S??? CMND/CCCD kh??ng h???p l???')
                        );
                      },
                    },
                  ]}
                >
                  <Input placeholder="S??? CMND/CCCD" />
                </Form.Item>
                <Form.Item
                  label="Ng??y c???p"
                  name="citizen_identity_card_published_date"
                  rules={[
                    {
                      required: true,
                      message: 'H??y nh???p ng??y c???p',
                    },
                  ]}
                >
                  <DatePicker format={'DD/MM/YYYY'} />
                </Form.Item>
                <Form.Item
                  label="N??i c???p"
                  name="citizen_identity_card_published_place"
                  rules={[
                    {
                      required: true,
                      validator: (_, value) => {
                        if (value === null || value === undefined) {
                          return Promise.reject(
                            'Tr?????ng ph???i t??? 1 ?????n 255 k?? t???'
                          );
                        }
                        if (
                          Validater.isContaintSpecialCharacterForName(
                            value.trim()
                          )
                        ) {
                          return Promise.reject(
                            'Tr?????ng n??y kh??ng ???????c ch???a k?? t??? ?????c bi???t'
                          );
                        }
                        if (
                          value.trim().length < 1 ||
                          value.trim().length > 255
                        ) {
                          return Promise.reject(
                            new Error('Tr?????ng ph???i t??? 1 ?????n 255 k?? t???')
                          );
                        }
                        return Promise.resolve();
                      },
                    },
                    {
                      whitespace: true,
                      message: 'Tr?????ng kh??ng ???????c ch???a kho???ng tr???ng',
                    },
                  ]}
                >
                  <Input placeholder="N??i c???p" />
                </Form.Item>
              </div>
              <Spacer y={1.5} />
              {/* <div className={classes.buttonCreate}>
                <Button
                  flat
                  auto
                  css={{
                    width: '120px',
                  }}
                  type="primary"
                  htmlType="submit"
                  disabled={isCreatingOrUpdating}
                >
                  {!modeUpdate && 'T???o m???i'}
                  {modeUpdate && 'C???p nh???t'}
                </Button>
              </div> */}
              <div
                style={{
                  display: 'flex',
                  gap: '10px',
                  justifyContent: 'center',
                }}
              >
                <Button
                  flat
                  auto
                  css={{
                    width: '150px',
                  }}
                  type="primary"
                  htmlType="submit"
                  disabled={isCreatingOrUpdating}
                >
                  {!modeUpdate && 'T???o m???i'}
                  {modeUpdate && 'C???p nh???t'}
                </Button>
              </div>
            </Card.Body>
          </Card>
        </Grid>
        {modeUpdate && (
          <Grid xs={3}>
            <Card
              variant="bordered"
              css={{
                height: 'min-content',
              }}
            >
              <Card.Header>
                <Text
                  p
                  b
                  size={14}
                  color={'error'}
                  css={{
                    width: '100%',
                    textAlign: 'center',
                  }}
                >
                  Khu v???c nguy hi???m
                </Text>
              </Card.Header>
              <Card.Body>
                <Descriptions column={{ xs: 1, sm: 1, md: 1, lg: 1 }}>
                  <Descriptions.Item label="Tr???ng th??i k??ch ho???t">
                    <div
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                      }}
                    >
                      <Switch
                        onChange={handleChangeActive}
                        disabled={dataUser === undefined}
                        checked={dataUser?.is_active}
                        color={'success'}
                        size={'xs'}
                      />
                    </div>
                  </Descriptions.Item>
                  <Descriptions.Item label="Xo?? t??i kho???n">
                    <div
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                      }}
                    >
                      <Button
                        color={'error'}
                        flat={!isUnlockDelete}
                        auto
                        icon={isUnlockDelete ? null : <FaLock />}
                        onPress={handleDelete}
                        disabled={!canDelete}
                      >
                        {isUnlockDelete ? 'Xo?? t??i kho???n' : 'M??? kho??'}
                      </Button>
                    </div>
                  </Descriptions.Item>
                </Descriptions>
              </Card.Body>
            </Card>
          </Grid>
        )}
      </Grid.Container>
    </Form>
  );
};

export default SroCreate;
