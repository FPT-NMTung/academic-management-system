import { Spacer, Text } from '@nextui-org/react';
import classes from './NotFoundScreen.module.css';
import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const NotFoundScreen = () => {
  const [time, setTime] = useState(9);
  const navigate = useNavigate();

  useEffect(() => {
    const interval = setInterval(() => {
      if (time - 1 < 0) {
        navigate('/');
      }
      setTime(time - 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [time]);

  return (
    <div className={classes.main}>
      <Text h1 weight={'thin'} color={'warning'} size={60}>
        404
      </Text>
      <Text h4>Trang này không tồn tại</Text>
      <Spacer y={2}/>
      <Text p>
        Có vẻ như bạn đã đặt một liên kết không chính xác. Hoặc tài nguyên bạn đang
        tìm kiếm đã bị xóa.
      </Text>
      <Text p>Hãy kiểm tra lại đường dẫn.
      </Text>
      <Text p>
        <i>
          Back to <Link to={'/'}>home</Link> in {time} seconds
        </i>
      </Text>
    </div>
  );
};

export default NotFoundScreen;
