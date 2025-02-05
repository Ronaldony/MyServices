import AddCircleIcon from '@mui/icons-material/AddCircle';
import {
  Box,
  Button,
  Checkbox,
  Divider,
  FormControl,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  MenuItem,
  Modal,
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
import { useNavigate } from 'react-router-dom';
import { Request_Get, Request_Post } from '../../apis/restAPI.ts';
import {
  ClassSchema,
  ClassSchemaColumn,
} from '../../classes/shared/classSchema.ts';
import {
  REQ_CreateConfigSchema,
  RES_AllConfiguration,
  RES_CreateConfigSchema,
} from '../../classes/shared/protocols/configuration.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import CheckModal from '../modals/checkModal.tsx';
import ErrorModal from '../modals/errorModal.tsx';
import LoadingSpinner from '../modals/loadingSpinner.tsx';
import { schemaModalStyle } from './styles.ts';

//////////////////////////////////////////////////////////////////////////
// Configuration menu
export default function ConfigurationMenu({ setSelectedSchema }) {
  const [selectedData, setSelectedData] = useState('');
  const [dataListRes, setDataListRes] = useState<
    RES_AllConfiguration | RES_Error | any
  >();
  const [errorMsg, setErrorMsg] = useState('');

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
      if (res instanceof RES_Error) onError('fail to fetch configuration list');
      else setDataListRes(res);

      // loading status
      setIsLoading(false);
    });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // onError
  const onError = useCallback(
    (msg) => {
      setErrorMsg(msg);
      setOpenErrorModal(true);
      setIsError(true);
    },
    [setErrorMsg, setOpenErrorModal, setIsError]
  );

  // switch selected data
  const onSelectedData = useCallback(
    (selectedData) => {
      setSelectedData(selectedData);
      setSelectedSchema(selectedData);
    },
    [setSelectedData, setSelectedSchema]
  );

  // loading
  if (true === isLoading) {
    return <LoadingSpinner />;
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
          borderRight: '1px solid silver'
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
                  onClick={() => onSelectedData(data)}
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

//////////////////////////////////////////////////////////////////////////
// CreateSchemaModal
function CreateSchemaModal({ openSchemaModal, setOpenSchemaModal }) {
  // schema
  const [newSchema, setNewSchema] = useState<ClassSchema>(
    new ClassSchema('', '', '', null)
  );

  // columns
  const [columns, setColumns] = useState<ClassSchemaColumn[]>([
    new ClassSchemaColumn(0, 'Id', false, 'int', '', 'Schema ID'),
  ]);

  // error
  const [errorMsg, setErrorMsg] = useState('');
  const [openErrorModal, setOpenErrorModal] = useState(false);

  // check
  const [checkMsg, setCheckMsg] = useState('');
  const [openCheckModal, setOpenCheckModal] = useState(false);

  // navigate
  const navigate = useNavigate();

  // close modals
  const onSuccessCreated = useCallback(() => {
    setOpenCheckModal(false)
    setOpenSchemaModal(false)
    navigate(0)
  }, [setOpenCheckModal, setOpenSchemaModal, navigate])

  // on error
  const onError = useCallback(
    (msg) => {
      setErrorMsg(msg);
      setOpenErrorModal(true);
    },
    [setErrorMsg, setOpenErrorModal]
  );
  
  // on check
  const onCheckModal = useCallback(
    (msg) => {
      setCheckMsg(msg);
      setOpenCheckModal(true);
    },
    [setCheckMsg, setOpenCheckModal]
  );

  async function onCheckOkAsync()
  {
    await reqCreateSchemaAsync();
  }

  // update schema
  useEffect(() => {
    setNewSchema((prev) => {
      return { ...prev, columns: columns };
    });
  }, [columns, setNewSchema]);

  // request create schema.
  const reqCreateSchemaAsync = useCallback(async () => {
    // request
    const res = await Request_Post<RES_CreateConfigSchema>(
      `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_CREATE_SCHEMA}`,
      new REQ_CreateConfigSchema(newSchema)
    );

    // error.
    if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorMsg}`);
    else onSuccessCreated()
  }, [newSchema, onError, onSuccessCreated]);

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
            {/* Name */}
            <TextField
              label="Name"
              variant="standard"
              focused
              value={newSchema.name}
              required
              error={!newSchema.name}
              onChange={(e) =>
                setNewSchema({ ...newSchema, name: e.target.value })
              }
            />

            {/* Category */}
            <TextField
              label="Category"
              variant="standard"
              focused
              value={newSchema.category}
              onChange={(e) =>
                setNewSchema({ ...newSchema, category: e.target.value })
              }
            />

            {/* desciption */}
            <TextField
              label="Desciption"
              variant="standard"
              focused
              value={newSchema.desciption}
              onChange={(e) =>
                setNewSchema({ ...newSchema, desciption: e.target.value })
              }
            />
          </Box>

          {/* Schema Body */}
          <Box component="form">
            <ColumnsTable columns={columns} setColumns={setColumns} />
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
            <Button
              variant="contained"
              onClick={async () => onCheckModal("Are you sure to create schema?")}
            >
              Create
            </Button>
            <Button
              variant="contained"
              onClick={() => setOpenSchemaModal(false)}
            >
              Close
            </Button>
          </Stack>

          {/* Error modal */}
          <ErrorModal
            openErrorModal={openErrorModal}
            errorMsg={errorMsg}
            setOpenErrorModal={setOpenErrorModal}
          />

          {/* Check modal */}
            <CheckModal 
            openCheckModal={openCheckModal}
            checkMsg={checkMsg}
            setOpenCheckModal={setOpenCheckModal}
            setOnCheckOk={onCheckOkAsync}/>
        </Box>
      </Modal>
    </Fragment>
  );
}

//////////////////////////////////////////////////////////////////////////
// ColumnsTable
function ColumnsTable({ columns, setColumns }: any) {
  // types
  const [types, setTypes] = useState<string[] | any>([]);
  const [refTables, setRefTables] = useState<string[] | any>([]);

  // status
  const [openTypes, setOpenTypes] = useState<boolean[] | any>([false]);
  const [openRefTables, setOpenRefTables] = useState<boolean[] | any>([false]);

  // index info
  const primaryIndex = 0;

  ////////////////////////////////////////////////////////
  // fetch types.
  useEffect(() => {
    // fetch types.
    Request_Get<String[]>(
      `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_FETCH_TYPES}`
    ).then((res) => {
      if (res instanceof RES_Error) console.log('fail to fetch types');
      else setTypes(res);
    });

    // Fetch ref types
    Request_Get<String[]>(
      `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_FETCH_REFTABLES}`
    ).then((res) => {
      if (res instanceof RES_Error) console.log('fail to fetch ref types');
      else setRefTables(res);
    });
  }, []);

  ////////////////////////////////////////////////////////
  // handle column
  function updateColumn(handleIndex, key, value) {
    setColumns(
      columns.map((col, index) => {
        if (index === handleIndex) {
          // modify
          col[key] = value;
          return col;
        } else return col;
      })
    );
  }

  // add column
  function addColumn() {
    // add column
    setColumns((prev) => [
      ...prev,
      new ClassSchemaColumn(0, '', false, 'int', '', ''),
    ]);

    // add status
    setOpenRefTables((prev) => [...prev, false]);
    setOpenTypes((prev) => [...prev, false]);
  }

  // delete column
  function deleteColumn(columnIdx) {
    // delete column
    setColumns(columns.filter((d, index) => index !== columnIdx));

    // delete status
    setOpenRefTables(openRefTables.filter((d, index) => index !== columnIdx));
    setOpenTypes(openTypes.filter((d, index) => index !== columnIdx));
  }

  ////////////////////////////////////////////////////////
  // handle open types
  function handleOpenTypes(handleIndex, value) {
    setOpenTypes(
      openTypes.map((isOpen, index) => {
        if (index === handleIndex) return value;
        else return isOpen;
      })
    );
  }

  // handle open ref types
  function handleOpenRefTables(handleIndex, value) {
    setOpenRefTables(
      openRefTables.map((isOpen, index) => {
        if (index === handleIndex) return value;
        else return isOpen;
      })
    );
  }

  ////////////////////////////////////////////////////////
  // get type status
  const getOpenType = useCallback((getIndex) =>
  {
    let isOpen = false;
    
    openTypes.forEach((element, index) => {
      if (index === getIndex) isOpen = element;
    });

    return isOpen;
  }, [openTypes]);

  const getOpenRefTable = useCallback((getIndex) =>
    {
    let isOpen = false;

    openRefTables.forEach((element, index) => {
      if (index === getIndex) isOpen = element;
    });

    return isOpen;
  }, [openRefTables])

  return (
    <Fragment>
      {/* Columns */}
      <Typography variant="h6" component="h2">
        Columns
      </Typography>

      {/* Table */}
      <TableContainer sx={{height: '350px'}}>
        <Table stickyHeader aria-label="sticky table">
          {/* Header */}
          <TableHead>
            <TableRow>
              <TableCell align='right' style={{ paddingTop: 0, paddingBottom: 0 }}>
                Name
              </TableCell>
              <TableCell align='right' style={{ paddingTop: 0, paddingBottom: 0 }}>
                Index
              </TableCell>
              <TableCell align='right' style={{ paddingTop: 0, paddingBottom: 0 }}>
                Type
              </TableCell>
              <TableCell align='right' style={{ paddingTop: 0, paddingBottom: 0 }}>
                RefTable
              </TableCell>
              <TableCell align='right' style={{ paddingTop: 0, paddingBottom: 0 }}>
                Summary
              </TableCell>
              <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}/>
            </TableRow>
          </TableHead>
          
          {/* Body */}
          <TableBody>
            {columns.map((column, index) => {
              return (
                <TableRow hover role="checkbox" tabIndex={-1} key={index}>
                  {/* Name */}
                  <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                    <TextField
                      disabled={index === primaryIndex}
                      variant="standard"
                      focused
                      value={column.name}
                      required
                      error={!column.name}
                      onChange={(e) =>
                        updateColumn(index, 'name', e.target.value)
                      }
                    />
                  </TableCell>

                  {/* IsIndexed */}
                  <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                    <Checkbox
                      disabled={index === primaryIndex}
                      checked={column.isIndexed}
                      onChange={(e) =>
                        updateColumn(index, 'isIndexed', e.target.checked)
                      }
                    />
                  </TableCell>

                  {/* Type */}
                  <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                    <FormControl sx={{ m: 1, minWidth: 120 }}>
                      <Select
                        disabled={index === primaryIndex}
                        size="small"
                        open={getOpenType(index)}
                        onOpen={() => handleOpenTypes(index, true)}
                        onClose={() => handleOpenTypes(index, false)}
                        value={column.type}
                        onChange={(e) =>
                          updateColumn(index, 'type', e.target.value)
                        }
                      >
                        {/* empty */}
                        {types?.map((d) => {
                          return (
                            <MenuItem key={d} value={d}>
                              {d}
                            </MenuItem>
                          );
                        })}
                      </Select>
                    </FormControl>
                  </TableCell>

                  {/* RefTable */}
                  <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                    <FormControl sx={{ m: 1, minWidth: 120 }}>
                      <Select
                        size="small"
                        open={getOpenRefTable(index)}
                        onOpen={() => handleOpenRefTables(index, true)}
                        onClose={() => handleOpenRefTables(index, false)}
                        value={column.refTable}
                        onChange={(e) =>
                          updateColumn(index, 'refTable', e.target.value)
                        }
                      >
                        {/* empty */}
                        <MenuItem value="">None</MenuItem>
                        {refTables?.map((d) => {
                          return (
                            <MenuItem key={d} value={d}>
                              {d}
                            </MenuItem>
                          );
                        })}
                      </Select>
                    </FormControl>
                  </TableCell>

                  {/* Summary */}
                  <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                    <TextField
                      variant="standard"
                      focused
                      value={column.summary}
                      onChange={(e) =>
                        updateColumn(index, 'summary', e.target.value)
                      }
                    />
                  </TableCell>

                  {/* Delete */}
                  <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                    <Button
                      variant="contained"
                      disabled={index === primaryIndex}
                      color="error"
                      size="small"
                      onClick={() => deleteColumn(index)}
                    >
                      Delete
                    </Button>
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Add button */}
      <Button
        variant="contained"
        color="success"
        size="small"
        onClick={() => addColumn()}
      >
        Add
      </Button>
    </Fragment>
  );
}
