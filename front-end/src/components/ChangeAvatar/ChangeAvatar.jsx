import AvatarEditor from 'react-avatar-editor';
import classes from './ChangeAvatar.module.css';
import Button, { Modal, Text } from '@nextui-org/react';
import { Fragment, useRef, useState } from 'react';
import { Slider } from 'antd';
import FetchApi from '../../apis/FetchApi';

const ChangeAvatar = (props) => {
  const [isEdit, setIsEdit] = useState(false);
  const [file, setFile] = useState('');
  const [imageBase64, setImageBase64] = useState('');
  const [scaleImage, setScaleImage] = useState(0);
  const [isPreview, setIsPreview] = useState(false);
  const [waiting, setWaiting] = useState(false);

  const refAvatar = useRef();
  const isHaveFile = Boolean(file !== '');

  const handleChooseFile = (event) => {
    const selected = event.target.files[0];

    if (selected.type.endsWith('jpg') || selected.type.endsWith('png')) {
      event.target.value = null;
    }

    setFile(selected);
  };

  return (
    <Modal open blur>
      <Modal.Header>
        <Text>âsd</Text>
      </Modal.Header>
      <Modal.Body>
        <AvatarEditor
          ref={refAvatar}
          borderRadius={220}
          image={file}
          width={220}
          height={220}
          border={10}
          color={[0, 0, 0, 0.3]} // RGBA
          scale={(scaleImage + 100) / 100}
          rotate={0}
          className={classes.avatar}
        />
      </Modal.Body>
    </Modal>
  );
};

export default ChangeAvatar;
