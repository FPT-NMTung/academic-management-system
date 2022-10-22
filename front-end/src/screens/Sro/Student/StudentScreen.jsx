import { Card, Grid, Text } from '@nextui-org/react';

const StudentScreen = () => {
  return (
    <Grid.Container gap={2}>
      <Grid xs={12}>
        <Card variant="bordered">
          <Card.Body
            css={{
              padding: 10,
            }}
          >
            
          </Card.Body>
        </Card>
      </Grid>
      <Grid xs={12}>
        <Card variant="bordered">
          <Card.Header>
            <Text b p size={14} css={{ width: '100%', textAlign: 'center' }}>
              Danh sách học viên
            </Text>
          </Card.Header>
        </Card>
      </Grid>
    </Grid.Container>
  );
};

export default StudentScreen;
