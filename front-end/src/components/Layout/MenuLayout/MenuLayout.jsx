import { Menu } from 'antd';
import { IoHome } from 'react-icons/io5';
import { ImBooks } from 'react-icons/im';
import { MdManageAccounts } from 'react-icons/md';
import { useLocation, matchPath, useNavigate } from 'react-router-dom';

const getItem = (label, key, icon, children, url, link) => {
  return {
    key,
    icon,
    children,
    label,
    url,
    link
  };
};

const itemsAdmin = [
  getItem('Trang chủ', 'main1', <IoHome />, undefined, '/admin', '/admin'),
  getItem('Quản lý cơ sở', 'main2', <IoHome />, undefined, '/admin/center/*', '/admin/center'),
  getItem('Quản lý tài khoản', 'main3', <MdManageAccounts />, [
    getItem('Người quản lý lớp (SRO)', 'main3-sub1', undefined, undefined, '/admin/account/sro/*', '/admin/account/sro'),
    getItem('Giáo viên', 'main3-sub2', undefined, undefined, '/admin/account/teacher/*', '/admin/account/teacher'),
  ]),
  getItem('Quản lý khoá học', 'main4', <ImBooks />, [
    getItem('Chương trình học gốc', 'main4-sub1', undefined, undefined, '/admin/manage-course/course-family/*', '/admin/manage-course/course-family'),
    getItem('Khoá học', 'main4-sub2', undefined, undefined, '/admin/manage-course/course/*', '/admin/manage-course/course'),
    getItem('Môn học', 'main4-sub3', undefined, undefined, '/admin/manage-course/module/*', '/admin/manage-course/module'),
  ]),
];

const flatItemsAdmin = itemsAdmin.flatMap((item) => {
  if (item.children) {
    return [item, ...item.children];
  }
  return item;
});

const MenuLayout = () => {
  const { pathname } = useLocation();
  const navigate = useNavigate();

  const itemMatch = flatItemsAdmin.find((item) => {
    if (item.url) {
      return matchPath(item.url, pathname)
    }
  });

  const handleChangeTab = (e) => {
    navigate(e.item.props.link);
  }

  return (
    <Menu
      inlineIndent={15}
      theme="dark"
      style={{
        width: '100%',
      }}
      defaultSelectedKeys={[itemMatch?.key]}
      defaultOpenKeys={[itemMatch?.key.split('-')[0]]}
      mode="inline"
      items={itemsAdmin}
      onSelect={handleChangeTab}
    />
  );
};

export default MenuLayout;
