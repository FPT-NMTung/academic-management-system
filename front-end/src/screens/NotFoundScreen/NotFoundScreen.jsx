import { Spacer, Text } from '@nextui-org/react';
import classes from './NotFoundScreen.module.css';
import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const NotFoundScreen = () => {
  const [time, setTime] = useState(9);
  const navigate = useNavigate();

  // useEffect(() => {
  //   const interval = setInterval(() => {
  //     if (time - 1 < 0) {
  //       navigate('/');
  //     }
  //     setTime(time - 1);
  //   }, 1000);
  //   return () => clearInterval(interval);
  // }, [time]);

  return (
    <div className={classes.main}>
      <Text h1 weight={'thin'} color={'warning'} size={60}>
        404
      </Text>
      <Text h4>Page not found</Text>
      <Spacer y={2} />
      <Text p>
        The page you are looking for might have been removed, had its name
        changed, or is temporarily unavailable.
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
