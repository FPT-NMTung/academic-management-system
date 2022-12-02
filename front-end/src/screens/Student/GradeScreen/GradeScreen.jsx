import classes from "./GradeScreen.module.css";
import {
  Button,
  Card,
  Grid,
  Loading,
  Modal,
  Table,
  Text,
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
  AppstoreOutlined,
  MailOutlined,
  SettingOutlined,
} from "@ant-design/icons";
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
        console.log(listGrade);
        // console.log(listGrade.map((item) => item.name));
      })
      .catch((err) => {
        // toast.error("Lỗi khi tải điểm");
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
              Chọn môn học để xem điểm
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
                <Menu mode="inline">
                  {listModuleSemester.map((item, index) => (
                    <Fragment key={index}>
                      <Menu.SubMenu
                        style={{ color: "black!important" }}
                        title={item.name}
                        key={index + 1}
                        rootStyle={{ width: "100%" }}
                      >
                        {item.modules.map((modules, index) => (
                          <Menu.Item
                            // title={modules.name + " ( " + modules.class.name + " )"}
                            key={index + 2}
                            rootStyle={{ width: "100%" }}
                            onClick={() =>
                              onSelectTree(modules.id, modules.class.id)
                            }
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
              Bảng điểm chi tiết
            </Text>
          </Card.Header>
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
                {listGrade.map((gradecate, index) => (
                  <Table.Row key={index}>
                    <Table.Cell>{gradecate.name}</Table.Cell>
                    <Table.Cell>{gradecate.total_weight}</Table.Cell>
                    <Table.Cell b>{gradecate.grade_items[0].grade}</Table.Cell>

                    <Table.Cell css={{ color: "#17c964" }}>
                      {gradecate.grade_items[0].comment}
                    </Table.Cell>
                  </Table.Row>
                ))}

                {/* <Table.Row key="211">
                    <Table.Cell>Assignment</Table.Cell>
                    <Table.Cell>20%</Table.Cell>
                    <Table.Cell>
                      <b>10</b>
                    </Table.Cell>
                  </Table.Row>
                  <Table.Row key="311">
                    <Table.Cell>Lab</Table.Cell>
                    <Table.Cell>20%</Table.Cell>
                    <Table.Cell>
                      <b>10</b>
                    </Table.Cell>
                  </Table.Row>
                  <Table.Row key="511">
                    <Table.Cell>Group Project</Table.Cell>
                    <Table.Cell>20%</Table.Cell>
                    <Table.Cell>
                      <b>10</b>
                    </Table.Cell>
                  </Table.Row> */}
              </Table.Body>
            </Table>
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default GradeScreen;
