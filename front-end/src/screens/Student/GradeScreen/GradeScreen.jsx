import classes from "./GradeScreen.module.css";
import {
  Button,
  Card,
  Grid,
  Loading,
  Modal,
  Table,
  Text,
  Spacer,
  Badge,
} from "@nextui-org/react";
import { Calendar, Select, Row, Col, Form, Input, Tree, Menu } from "antd";
import { Fragment, useState } from "react";
import moment from "moment";
import "moment/locale/vi";
import { useEffect } from "react";
import toast from "react-hot-toast";
import { MdDelete } from "react-icons/md";
import { UserStudentApis } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
import {
  MdMenuBook,
} from 'react-icons/md';
import { ImLibrary } from 'react-icons/im';
import Item from "antd/lib/list/Item";
const GradeScreen = () => {
  const [listModuleSemester, setListModuleSemester] = useState([]);
  const [listGrade, setListGrade] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  // const [isLoadingGrade, setIsLoadingGrade] = useState(true);

  const getModuleSemester = () => {
    setIsLoading(true);
    FetchApi(UserStudentApis.getModuleSemester) // get list module semester
      .then((res) => {
        // res.data.map((item) => {
        //   setListModuleSemester(item);
        // });
        setListModuleSemester(res.data);
        // console.log(listModuleSemester.map((item) => item.name));
        // console.log(listModuleSemester.modules);
        setIsLoading(false);
      })
      .catch((err) => {
        toast.error("Lỗi khi tải dữ liệu");
      });
  };
  const onSelectTree = (moduleid, classid) => {
    setListGrade([]);
    // setIsLoadingGrade(false);
    console.log("selected module " + moduleid + "classid " + classid);
    // console.log(FetchApi(UserStudentApis.getGradesbyclass, null, null, [String(classid),String(moduleid)]));
    FetchApi(UserStudentApis.getGradesbyclass, null, null, [
      String(classid),
      String(moduleid),
    ])
      .then((res) => {
      

        setListGrade(res.data);
        console.log(listGrade.map((item) => item));
        // console.log(listGrade.map((item) => item.name));
      })
      .catch((err) => {
        // toast.error("Lỗi khi tải điểm");
        // console.log("loi vkl");
      });
  };

  useEffect(() => {
    getModuleSemester();
  }, []);

  return (
    <Grid.Container gap={2} justify={"center"}>
      <Grid xs={4}>
        <Card
          variant="bordered"
          css={{
            height: "fit-content",
          }}
        >
          <Card.Header>
            <Text
              p
              b
              size={14}
              css={{
                width: "100%",
                textAlign: "center",
                marginBottom: "12px",
              }}
            >
              Chọn môn học
            </Text>
          </Card.Header>
          <Card.Divider />
          <Card.Body>
            {/* {listModuleSemester.map((item, index) => (
              <Fragment key={index}>
                <Tree css={{ display: "block" }}>
                  <Tree.TreeNode
                    title={item.name}
                    key={index + 1}
                    rootStyle={{ width: "100%" }}
                  >
                    {item.modules.map((modules, index) => (
                      <Tree.TreeNode
                        title={modules.name + " ( " + modules.class.name + " )"}
                        key={index + 2}
                        rootStyle={{ width: "100%" }}
                      ></Tree.TreeNode>
                    ))}
                  </Tree.TreeNode>
                </Tree>
              </Fragment>
            ))} */}
            {/* <Menu mode="inline">
              
              <Menu.SubMenu key="122"  title= "Học kỳ 1">
                
                <Menu.Item title="sssss" key="232323" onClick={() => onSelectTree(144, 53)}>
                <span>Option 1</span>
                </Menu.Item>
              </Menu.SubMenu>
            </Menu> */}
            {isLoading ? (
              <Loading />
            ) : (
              <div style={{ color: "black !important" }}>
                <Menu 
                mode="inline"
                // defaultOpenKeys={["1"]}
                style={{ width: "100%"}}
                >
                  {listModuleSemester.map((item, index) => (
                    <Fragment key={index}>
                      <Menu.SubMenu
                        style={{ color: "black!important" }}
                        title={item.name}
                        key={index + 1}
                        rootStyle={{ width: "100%" }}
                        icon={<ImLibrary />}
                        
                      >
                        {item.modules.map((modules, index) => (
                          <Menu.Item
                            // title={modules.name + " ( " + modules.class.name + " )"}
                            key={index + 2}
                            rootStyle={{ width: "100%" }}
                            onClick={() =>
                              onSelectTree(modules.id, modules.class.id)
                            }
                            icon={<MdMenuBook />}
                          >
                            <span>
                              {modules.name + " ( " + modules.class.name + " )"}
                            </span>
                          </Menu.Item>
                        ))}
                      </Menu.SubMenu>
                    </Fragment>
                  ))}
                </Menu>
              </div>
            )}
          </Card.Body>
        </Card>
      </Grid>
      <Grid xs={5}>
        <Card variant="bordered">
          <Card.Header>
            <Text
              p
              b
              size={14}
              css={{
                width: "100%",
                textAlign: "center",
              }}
            >
              Bảng điểm 
            </Text>
          </Card.Header>
          <Spacer y={0.6} />
          <Card.Divider />
          <Card.Body>
            <Table
              aria-label=""
              css={{
                height: "auto",
                minWidth: "100%",
              }}
              lined
              headerLined
              shadow={false}
            >
              <Table.Header>
                <Table.Column width={200}>Loại điểm</Table.Column>
                <Table.Column width={100}>Trọng số</Table.Column>
                <Table.Column width={50}>Điểm</Table.Column>
                <Table.Column width={80}>Chú thích</Table.Column>
              </Table.Header>
              <Table.Body>
                {listGrade.map((item, index) => (
                  <Table.Row key={index}>
                    <Table.Cell>{item.grade_item.name}</Table.Cell>
                    <Table.Cell>
                    
                        
                      {item.total_weight ? 
                        <Badge color = "warning">
                     { Math.round((item.total_weight) / (item.quantity_grade_item) * 10) / 10 }
                     % 
                      </Badge> : ""} 
                   
                      </Table.Cell>
                    {/* <Table.Cell b>{Math.round((item.grade_item?.grade) * 10) / 10}</Table.Cell> */}
                    <Table.Cell b>{item.grade_item.grade ? Math.round((item.grade_item?.grade) * 10) / 10 : " "}</Table.Cell>


                    <Table.Cell css={{ color: "#f31260" }}>
                    {item.grade_item?.comment}
                    </Table.Cell>
                  </Table.Row>
                ))}

               
              </Table.Body>
            </Table>
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default GradeScreen;
