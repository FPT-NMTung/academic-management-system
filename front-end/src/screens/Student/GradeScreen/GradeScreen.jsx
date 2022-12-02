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
import { Calendar, Select, Row, Col, Form, Input, Tree } from "antd";
import { Fragment, useState } from "react";
import moment from "moment";
import "moment/locale/vi";
import { useEffect } from "react";
import toast from "react-hot-toast";
import { MdDelete } from "react-icons/md";
import { UserStudentApis } from "../../../apis/ListApi";
import FetchApi from "../../../apis/FetchApi";
const GradeScreen = () => {
  const [listModuleSemester, setListModuleSemester] = useState([]);
  const getModuleSemester = () => {
    FetchApi(UserStudentApis.getModuleSemester) // get list module semester
      .then((res) => {
        // res.data.map((item) => {
        //   setListModuleSemester(item);
        // });
        setListModuleSemester(res.data);
        // console.log(listModuleSemester.map((item) => item.name));
        // console.log(listModuleSemester.modules);
      })
      .catch((err) => {
        toast.error("Lỗi khi tải dữ liệu");
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
            {listModuleSemester.map((item, index) => (
              <Fragment>
                <Tree css={{ display: "block" }}>
                  <Tree.TreeNode
                    title={item.name}
                    key={item.id}
                    rootStyle={{ width: "100%" }}
                  >
                    {item.modules.map((module) => (
                      <Tree.TreeNode
                        title={module.name + " ( " + module.class.name + " )"}
                        key={module.id}
                        rootStyle={{ width: "100%" }}
                      ></Tree.TreeNode>
                    ))}
                  </Tree.TreeNode>
                </Tree>
              </Fragment>
            ))}

            {/* <Tree css={{ display: "block" }}>
              <Tree.TreeNode
                title="Học kỳ 1"
                key="0-0"
                rootStyle={{ width: "100%" }}
              >
                <Tree.TreeNode title="AngularJS" key="0-0-0" />
                <Tree.TreeNode
                  title="Logic Building and Elementary Programming"
                  key="0-0-1"
                />
              </Tree.TreeNode>
              <Tree.TreeNode title="Học kỳ 2" key="1-0">
                <Tree.TreeNode title="HTML5, CSS, and JavaScript" key="1-0-0" />
                <Tree.TreeNode
                  title="Application Programming with C#"
                  key="1-0-1"
                />
              </Tree.TreeNode>
            </Tree> */}
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
          <Card.Body>
            <Table aria-label="">
              <Table.Header>
                <Table.Column width={200}>Loại điểm</Table.Column>
                <Table.Column width={100}>Trọng số</Table.Column>
                <Table.Column width={50}>Điểm</Table.Column>
              </Table.Header>
              <Table.Body>
                <Table.Row key="2">
                  <Table.Cell>Assignment</Table.Cell>
                  <Table.Cell>20%</Table.Cell>
                  <Table.Cell>
                    <b>10</b>
                  </Table.Cell>
                </Table.Row>
                <Table.Row key="3">
                  <Table.Cell>Lab</Table.Cell>
                  <Table.Cell>20%</Table.Cell>
                  <Table.Cell>
                    <b>10</b>
                  </Table.Cell>
                </Table.Row>
                <Table.Row key="5">
                  <Table.Cell>Group Project</Table.Cell>
                  <Table.Cell>20%</Table.Cell>
                  <Table.Cell>
                    <b>10</b>
                  </Table.Cell>
                </Table.Row>
              </Table.Body>
            </Table>
          </Card.Body>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default GradeScreen;
