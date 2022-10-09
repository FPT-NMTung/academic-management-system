import { Menu } from 'antd';
import { IoHome } from 'react-icons/io5';
import { ImBooks } from 'react-icons/im';
import { MdManageAccounts } from 'react-icons/md';
import { MdMeetingRoom } from 'react-icons/md';
import { useLocation, matchPath, useNavigate } from 'react-router-dom';
import { useState } from 'react';

const getItem = (label, key, icon, children, url, link) => {
  return {
    key,
    icon,
    children,
    label,
    url,
    link,
  };
};

const itemsAdmin = [
  getItem(
    'Quản lý cơ sở',
    'main2',
    <IoHome />,
    undefined,
    '/admin/center/*',
    '/admin/center'
  ),
  getItem('Quản lý tài khoản', 'main3', <MdManageAccounts />, [
    getItem(
      'Người quản lý lớp (SRO)',
      'main3-sub1',
      undefined,
      undefined,
      '/admin/account/sro/*',
      '/admin/account/sro'
    ),
    getItem(
      'Giáo viên',
      'main3-sub2',
      undefined,
      undefined,
      '/admin/account/teacher/*',
      '/admin/account/teacher'
    ),
  ]),
  getItem('Quản lý khoá học', 'main4', <ImBooks />, [
    getItem(
      'Chương trình học gốc',
      'main4-sub1',
      undefined,
      undefined,
      '/admin/manage-course/course-family/*',
      '/admin/manage-course/course-family'
    ),
    getItem(
      'Khoá học',
      'main4-sub2',
      undefined,
      undefined,
      '/admin/manage-course/course/*',
      '/admin/manage-course/course'
    ),
    getItem(
      'Môn học',
      'main4-sub3',
      undefined,
      undefined,
      '/admin/manage-course/module/*',
      '/admin/manage-course/module'
    ),
  ]),
  getItem(
    'Quản lý phòng học',
    'main5',
    <MdMeetingRoom />,
    undefined,
    '/admin/room/*',
    '/admin/room'
  ),
];

const flatItemsAdmin = itemsAdmin.flatMap((item) => {
  if (item.children) {
    return [item, ...item.children];
  }
  return item;
});

const MenuLayout = () => {
  const [itemMatched, setItemMatched] = useState(null);
  const [openKeys, setOpenKeys] = useState([]);

  const { pathname } = useLocation();
  const navigate = useNavigate();

  const itemMatch = flatItemsAdmin.find((item) => {
    if (item.url) {
      return matchPath(item.url, pathname);
    }
  });

  const handleChangeTab = (e) => {
    setItemMatched(e.key);
    navigate(e.item.props.link);
  };

  return (
    <Menu
      inlineIndent={15}
      theme="dark"
      style={{
        width: '100%',
      }}
      selectedKeys={[itemMatch?.key]}
      openKeys={openKeys.length === 0 ? [itemMatch?.key.split('-')[0]] : openKeys}
      onSelect={handleChangeTab}
      onOpenChange={(openKeys) => {
        setOpenKeys(openKeys);
      }}
      mode="inline"
      items={itemsAdmin}
      onClick={handleChangeTab}
    />
  );
};

export default MenuLayout;
