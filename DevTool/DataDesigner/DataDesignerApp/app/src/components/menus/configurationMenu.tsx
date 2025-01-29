import AddCircleIcon from '@mui/icons-material/AddCircle';
import {
  Box,
  Button,
  Checkbox,
  CircularProgress,
  Divider,
  FormControl,
  FormControlLabel,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  MenuItem,
  Modal,
  Paper,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../../apis/restfulAPI.ts';
import { RES_AllConfiguration } from '../../interfaces/api/configuration.ts';
import { ClassSchemaColumn } from '../../interfaces/classSchema.ts';
import ErrorModal from '../modals/errorModal.tsx';
import { schemaModalStyle } from './styles.ts';

// Configuration menu
export default function ConfigurationMenu({ setSelectedConfig }) {
  const [selectedData, setSelectedData] = useState('');
  const [dataListRes, setDataListRes] = useState<RES_AllConfiguration | null>();
  const [errorMsg, setErrorMsg] = useState();

  // status
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [openSchemaModal, setOpenSchemaModal] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isError, setIsError] = useState(false);

  // initialize
  useEffect(() => {
    Request_Get<RES_AllConfiguration>(
      `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_GETALL}`
    ).then((res) => {
      if (res === null) {
        // error
        onError('fail to fetch configuration list');
        setIsError(true);
      } else setDataListRes(res);

      // loading status
      setIsLoading(false);
    });
  }, []);

  // on error
  const onError = useCallback((msg) => {
    setErrorMsg(msg);
    setOpenErrorModal(true);
  }, []);

  // switch selected data
  const onSetSelectedData = useCallback((selectedData) => {
    setSelectedData(selectedData);
    setSelectedConfig(selectedData);
  }, []);

  // loading
  if (true === isLoading) {
    return (
      <Fragment>
        <Modal
          open={isLoading}
          aria-labelledby="modal-modal-title"
          aria-describedby="modal-modal-description"
        >
          <CircularProgress
            sx={{
              position: 'absolute',
              top: '50%',
              left: '50%',
            }}
          />
        </Modal>
      </Fragment>
    );
  }

  // error render
  if (true === isError) {
    return (
      <Fragment>
        <ErrorModal
          openErrorModal={openErrorModal}
          errorMsg={errorMsg}
          setOpenErrorModal={setOpenErrorModal}
        />
      </Fragment>
    );
  }

  // menu
  return (
    <Fragment>
      {/* List */}
      <List
        sx={{
          minWidth: '120px',
          overflow: 'auto',
          borderRight: '1px solid silver',
        }}
      >
        {/* Create */}
        <ListItem>
          <ListItemButton onClick={() => setOpenSchemaModal(true)}>
            <ListItemIcon>
              <AddCircleIcon />
            </ListItemIcon>
            <ListItemText primary={'Create'} />
          </ListItemButton>
        </ListItem>

        <Divider />

        {/* DataList */}
        {dataListRes?.schemaNames === undefined
          ? ''
          : dataListRes?.schemaNames.map((data) => (
              <ListItem disablePadding key={data}>
                <ListItemButton
                  selected={selectedData === data}
                  onClick={() => onSetSelectedData(data)}
                >
                  <ListItemText primary={data} />
                </ListItemButton>
              </ListItem>
            ))}
      </List>

      {/* CreateSchema */}
      <CreateSchemaModal
        openSchemaModal={openSchemaModal}
        setOpenSchemaModal={setOpenSchemaModal}
      />

      {/* Error modal */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Fragment>
  );
}

// CreateSchemaModal
let nextIdx = 1;

function CreateSchemaModal({ openSchemaModal, setOpenSchemaModal }) {
  // schema
  const [createSchema, setCreateSchema] = useState(false);

  // index
  const [index, setIndex] = useState(1);

  // columns
  const [columns, setColumns] = useState<ClassSchemaColumn[]>([
    {
      idx: nextIdx,
      name: 'Name',
      isIndexed: false,
      type: 'int',
      refType: '',
      summary: 'Schema ID',
    },
  ]);

  return (
    <Fragment>
      {/* Schema modal */}
      <Modal
        open={openSchemaModal}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={schemaModalStyle}>
          {/* Title */}
          <Typography id="modal-modal-title" variant="h6" component="h2">
            Create schema
          </Typography>

          {/* Schema title */}
          <Box
            component="form"
            sx={{ '& > :not(style)': { m: 1, width: '25ch' } }}
          >
            <TextField label="Name" variant="standard" focused required />
            <TextField label="Category" variant="standard" focused />
            <TextField label="Summary" variant="standard" focused />
          </Box>

          {/* Schema Body */}
          <Box component="form">
            <ConfigTable columns={columns} setColumns={setColumns} />
          </Box>

          {/* Buttons */}
          <Stack
            direction="row"
            spacing={2}
            sx={{
              justifyContent: 'flex-end',
              alignItems: 'center',
            }}
          >
            <Button variant="contained" onClick={() => setCreateSchema(true)}>
              Create
            </Button>
            <Button
              variant="contained"
              onClick={() => setOpenSchemaModal(false)}
            >
              Close
            </Button>
          </Stack>
        </Box>
      </Modal>
    </Fragment>
  );
}

// ConfigTable
function ConfigTable({ columns, setColumns }: any) {
  const [name, setName] = useState('');

  // handle column
  function handleColumn(handleIndex, key, value) {
    const modifiedColumns = columns.map((col, index) => {
      if (index === handleIndex) {
        // modify
        col[key] = value;
        return col;
      } else {
        return col;
      }
    });

    setColumns(modifiedColumns);
  }

  return (
    <Fragment>
      {/* Columns */}
      <Typography variant="h6" component="h2">
        Columns
      </Typography>

      {/* Table */}
      <TableContainer sx={{ maxHeight: 440 }}>
        <Table stickyHeader aria-label="sticky table">
          <TableHead>
            <TableRow>
              <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                Name
              </TableCell>
              <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                IsIndexed
              </TableCell>
              <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                Type
              </TableCell>
              <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                RefType
              </TableCell>
              <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                Summary
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {columns.map((column, index) => {
              return (
                <TableRow hover role="checkbox" tabIndex={-1} key={index}>
                  {/* Name */}
                  <TableCell>
                    <TextField
                      variant="standard"
                      focused
                      value={column.name}
                      onChange={(e) =>
                        handleColumn(index, 'name', e.target.value)
                      }
                    />
                  </TableCell>
                  {/* IsIndexed */}
                  <TableCell>
                    <Checkbox
                      checked={column.isIndexed}
                      onChange={(e) =>
                        handleColumn(index, 'isIndexed', e.target.checked)
                      }
                    />
                  </TableCell>
                  {/* Type */}
                  <TableCell>{column.type}</TableCell>
                  <FormControl sx={{ m: 1, minWidth: 120 }}>
                    <Select
                      labelId="demo-controlled-open-select-label"
                      id="demo-controlled-open-select"
                      open={true}
                      value={column.type}
                      label="Age"
                      onChange={(e) =>
                        handleColumn(index, 'type', e.target.value)}
                    >
                      <MenuItem value="">
                        <em>None</em>
                      </MenuItem>
                      <MenuItem value={'Ten'}>Ten</MenuItem>
                      <MenuItem value={'Twenty'}>Twenty</MenuItem>
                      <MenuItem value={'Thirty'}>Thirty</MenuItem>
                    </Select>
                  </FormControl>
                  {/* RefType */}
                  <TableCell>{column.refType}</TableCell>
                  {/* Summary */}
                  <TableCell>{column.summary}</TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Add button */}
      <Button variant="contained" color="error" size="small">
        Add
      </Button>
    </Fragment>
  );
}
