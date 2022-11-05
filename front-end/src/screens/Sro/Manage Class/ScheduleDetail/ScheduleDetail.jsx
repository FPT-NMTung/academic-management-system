import { useParams } from "react-router-dom";

const ScheduleDetail = () => {
  const { id, scheduleId } = useParams();

  return <div>{scheduleId}</div>
}

export default ScheduleDetail