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
import { ModulesApis, UserStudentApis } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
import { MdMenuBook } from "react-icons/md";
import { ImLibrary } from "react-icons/im";
import Item from "antd/lib/list/Item";
const GradeScreen = () => {
  const [listModuleSemester, setListModuleSemester] = useState([]);
  const [listGrade, setListGrade] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [listGradeFinal, setListGradeFinal] = useState([]);
  const [informationModule, setInformationModule] = useState(undefined);
  const [averagePracticeGrade, setAveragePracticeGrade] = useState(0);

  const getModuleSemester = () => {
    setIsLoading(true);
    FetchApi(UserStudentApis.getModuleSemester)
      .then((res) => {
        setListModuleSemester(res.data);
        setIsLoading(false);
      })
      .catch((err) => {
        toast.error("Lỗi khi tải dữ liệu");
      });
  };
  const onSelectTree = (moduleid, classid) => {
    setListGrade([]);
    console.log("selected module " + moduleid + "classid " + classid);
    FetchApi(UserStudentApis.getGradesbyclass, null, null, [
      String(classid),
      String(moduleid),
    ])
      .then((res) => {
        const listGradePractice = res.data.filter(
          (item) => item.grade_category_id !== 6 && item.grade_category_id !== 8
        );
        const listGradeTheory = res.data.filter(
          (item) => item.grade_category_id === 6 || item.grade_category_id === 8
        );

        setListGrade(
          listGradePractice
        );
        setListGradeFinal(
          listGradeTheory
        );

        let avgPracticeGrade = 0 ;
        for (let i = 0; i < listGradePractice.length; i++) {
          avgPracticeGrade +=
          listGradePractice[i].grade_item.grade *
            (listGradePractice[i].total_weight / listGradePractice[i].quantity_grade_item);
        }
        setAveragePracticeGrade(avgPracticeGrade / 100);
      })
      .catch((err) => {
        // toast.error("Lỗi khi tải điểm");
      });
  };

  const getInformationModule = (moduleId) => {
    FetchApi(ModulesApis.getModuleByID, null, null, [String(moduleId)])
      .then((res) => {
        setInformationModule(res.data);
      })
      .catch((err) => {});
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
            {isLoading ? (
              <Loading />
            ) : (
              <div style={{ color: "black !important" }}>
                <Menu
                  mode="inline"
                  // defaultOpenKeys={["1"]}
                  style={{ width: "100%" }}
                >
                  {listModuleSemester.map((item, index) => (
                    <Fragment key={index}>
                      <Menu.SubMenu
                        style={{ color: "black!important" }}
                        title={item.name}
                        key={item.id}
                        rootStyle={{ width: "100%" }}
                        icon={<ImLibrary />}
                      >
                        {item.modules.map((modules, index) => (
                          <Menu.Item
                            key={modules.id + modules.class.id}
                            rootStyle={{ width: "100%" }}
                            onClick={() => {
                              onSelectTree(modules.id, modules.class.id);
                              getInformationModule(modules.id);
                            }}
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
          {
            <Card.Body>
              {informationModule?.max_practical_grade && (
                <Text
                  p
                  i
                  size={12}
                  css={{
                    paddingLeft: "10px",
                  }}
                >
                  * Điểm tối đa của Practical exam:{" "}
                  {informationModule?.max_practical_grade}
                </Text>
              )}
              {informationModule?.max_theory_grade && (
                <Text
                  p
                  i
                  size={12}
                  css={{
                    paddingLeft: "10px",
                  }}
                >
                  * Điểm tối đa của Theory exam:{" "}
                  {informationModule?.max_theory_grade}
                </Text>
              )}
              {listGrade.length > 0 && (
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
                    <Table.Column width={200}>
                      Loại điểm thành phần
                    </Table.Column>
                    <Table.Column width={100}>Trọng số</Table.Column>
                    <Table.Column width={50}>Điểm</Table.Column>
                  </Table.Header>
                  <Table.Body>
                    {listGrade.map((item, index) => (
                      <Table.Row key={index}>
                        <Table.Cell>{item.grade_item.name}</Table.Cell>
                        <Table.Cell>
                          {item.total_weight ? (
                            <Badge color="warning">
                              {Math.round(
                                (item.total_weight / item.quantity_grade_item) *
                                  100
                              ) / 100}
                              %
                            </Badge>
                          ) : (
                            ""
                          )}
                        </Table.Cell>
                        <Table.Cell b>
                          {item.grade_item.grade
                            ? Math.round(item.grade_item?.grade * 100) / 100
                            : " "}
                        </Table.Cell>
                      </Table.Row>
                    ))}
                  </Table.Body>
                </Table>
              )}
              {listGrade.length > 0 && (
                <Fragment>
                  <Text
                    b
                    i
                    size={18}
                    css={{
                      paddingLeft: "10px",
                    }}
                  >
                    Tổng điểm thực hành:{" "}
                    <Badge color="success">
                      {averagePracticeGrade}{" "}
                      {informationModule?.max_practical_grade === null
                        ? ""
                        : "/ " + informationModule?.max_practical_grade}
                    </Badge>
                  </Text>
                </Fragment>
              )}
              {listGradeFinal.length > 0 && (
                <Card
                  variant="bordered"
                  css={{
                    minHeight: "140px",
                    borderStyle: "dashed",
                    marginTop: "12px",
                    borderColor: "#17c964",
                  }}
                >
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
                      <Table.Column width={200}>Điểm cuối kỳ</Table.Column>
                      <Table.Column width={100}>Trọng số</Table.Column>
                      <Table.Column width={50}>Điểm</Table.Column>
                    </Table.Header>
                    <Table.Body>
                      {listGradeFinal.map((item, index) => (
                        <Table.Row key={index}>
                          <Table.Cell>{item.grade_item.name}</Table.Cell>
                          <Table.Cell>
                            {item.total_weight ? (
                              <Badge color="warning">
                                {Math.round(
                                  (item.total_weight /
                                    item.quantity_grade_item) *
                                    10
                                ) / 10}
                                %
                              </Badge>
                            ) : (
                              ""
                            )}
                          </Table.Cell>
                          <Table.Cell b>
                            {item.grade_item.grade
                              ? Math.round(item.grade_item?.grade * 100) / 100
                              : " "}
                          </Table.Cell>
                        </Table.Row>
                      ))}
                    </Table.Body>
                  </Table>
                </Card>
              )}
            </Card.Body>
          }
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default GradeScreen;
