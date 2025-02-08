import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import {
  Button,
  ListItemIcon,
  Menu,
  MenuItem,
  Stack
} from '@mui/material';
import {
  DataGrid,
  GridColDef,
  GridColType,
  GridRowId,
  GridRowModel,
  useGridApiRef,
} from '@mui/x-data-grid';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import uuid from 'react-uuid';
import { Request_Get, Request_Patch } from '../../apis/restAPI.ts';
import { ClassSchema, ClassSchemaColumn } from '../../classes/shared/classSchema.ts';
import { RES_FetchTypeAndTableKeys } from '../../classes/shared/protocols/common.ts';
import { REQ_UpdateConfigRows, RES_FindConfigRows, RES_FindConfigSchema } from '../../classes/shared/protocols/configuration.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import CheckModal from '../modals/checkModal.tsx';
import ErrorModal from '../modals/errorModal.tsx';
import ConfigurationHeaderbar from './configurationHeaderbar.tsx';

// GridColumns.
var gridColumns: GridColDef[] = [];

export default function ConfigurationGridTable({ schemaName }: any) {
  // status
  const [dataLoaded, setDataLoaded] = useState(false);

  // data
  const [gridRows, setGridRows] = useState<Array<any>>([]);
  const [schema, setSchema] = useState<ClassSchema>(new ClassSchema('', '', '', null));

  // value options
  const [valueOptionsDict, setValueOptionsDict] = useState<Map<string, []>>(
    new Map()
  );

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // apiRef
  const apiRef = useGridApiRef();

  // context
  const [isContextOpen, setIsContextOpen] = useState(false);
  const [contextMenuPos, setContextMenuPos] = React.useState({
    mouseX: 0,
    mouseY: 0,
  });

  // on error
  const onError = useCallback(
    (msg) => {
      setOpenErrorModal(true);
      setErrorMsg(msg);
    },
    [setOpenErrorModal, setErrorMsg]
  );

  // onContextTrigger
  const onContextMenuClose = useCallback(() => {
    setIsContextOpen(false);
  }, [setIsContextOpen]);

  // handle context menu
  const handleContextMenuPos = useCallback(
    (event: React.MouseEvent) => {
      event.preventDefault();

      // set menu position
      setContextMenuPos({ mouseX: event.clientX, mouseY: event.clientY });

      // trigge context menu
      setIsContextOpen(true);
    },
    [setContextMenuPos, setIsContextOpen]
  );

  // delete rows
  const onDeleteRows = useCallback(
    (rowMap: Map<GridRowId, GridRowModel>) => {
      rowMap.forEach((deleteRow) => {
        setGridRows(gridRows.filter((row) => row.Id !== deleteRow.Id));
      });

      // close context menu
      onContextMenuClose();
    },
    [gridRows, setGridRows, onContextMenuClose]
  );

  // add rows
  const onAddRow = useCallback(() => {
    const sortedRows = [...gridRows].sort((l, r) => (l.Id > r.Id ? 1 : -1));
    const last = sortedRows[gridRows.length - 1];
    let newId = last?.Id + 1;

    if (last?.Id === undefined) newId = 1;

    setGridRows((prev) => [...prev, { Id: newId, internalId: uuid() }]);
  }, [gridRows, setGridRows]);

  // get row id
  const getRowId = useCallback((row) => {
    return row.internalId;
  }, []);

  // processRowUpdate
  const processRowUpdate = (newRow: GridRowModel) => {
    const updatedRow = { ...newRow, isNew: false };
    setGridRows(
      gridRows.map((row) =>
        row.internalId === newRow.internalId ? updatedRow : row
      )
    );
    return updatedRow;
  };

  ////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////
  // request schema
  useEffect(() => {
    if ((false === dataLoaded) && (schemaName !== '')) {
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_SCHEMA}?schema=${schemaName}`;

      Request_Get<RES_FindConfigSchema>(query).then((res) => {
        if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`);
        else setSchema(res.schema);
      });
    }
  }, [dataLoaded, schemaName, onError, setSchema]);

  // request value options for select of grid table
  useEffect(() => {
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_FETCh_TYPE_TABLE_KEYS}`;

    Request_Get<RES_FetchTypeAndTableKeys>(query).then((res) => {
      if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`);
      else setValueOptionsDict(res.schemaKeysDict);
    });
  }, [onError, setValueOptionsDict]);

  // Convert columns and request rows
  useEffect(() => {
    if ((false === dataLoaded) && (schema.name !== '')) {
      // convert grid columns
      gridColumns = ConvertColsToGridCols(schema.columns, valueOptionsDict);

      // query.
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_ROWS}?schema=${schema.name}`;

      // request grid table
      Request_Get<RES_FindConfigRows>(query).then((res) => {
        if (res instanceof RES_Error) {
          onError(`[${res.errorCode}] ${res.errorCode}`);
        } else {
          setGridRows(res.rows);
          setDataLoaded(true);
        }
      });
    }
  }, [dataLoaded, onError, schema.name, schema.columns, schema, valueOptionsDict]);

  // wait for loaded data
  if (false === dataLoaded) {
    return <></>;
  }

  // grid columns
  return (
    <Fragment>
      {/* Header */}
      <ConfigurationHeaderbar schemaName={schema?.name} desciption={schema?.desciption} />

      {/* Grid table */}
      <div onContextMenu={handleContextMenuPos} style={{ height: '95%' }}>
        {/* Grid */}
        <DataGrid
          apiRef={apiRef}
          rows={gridRows}
          columns={gridColumns}
          editMode="row"
          getRowId={getRowId}
          processRowUpdate={processRowUpdate}
          hideFooter
        />

        {/* Context Menu */}
        <Menu
          open={isContextOpen}
          onClose={onContextMenuClose}
          anchorReference="anchorPosition"
          anchorPosition={{
            top: contextMenuPos.mouseY,
            left: contextMenuPos.mouseX,
          }}
        >
          {/* Add Row */}
          <MenuItem onClick={onAddRow}>
            <ListItemIcon>
              <AddIcon />
            </ListItemIcon>
            Add Row
          </MenuItem>
          {/* Delete Row */}
          <MenuItem
            onClick={() => {
              onDeleteRows(apiRef.current.getSelectedRows());
            }}
          >
            <ListItemIcon>
              <DeleteIcon />
            </ListItemIcon>
            Delete Row
          </MenuItem>
        </Menu>
      </div>

      {/* submit buttons */}
      <SubmitButtons schemaName={schemaName} gridRows={gridRows}/>

      {/* Error */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />

    </Fragment>
  );
}

// SubmitButtons
function SubmitButtons({ schemaName, gridRows }: any) {
  // navigate
  const navigate = useNavigate();

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // check
  const [checkMsg, setCheckMsg] = useState('');
  const [openCheckModal, setOpenCheckModal] = useState(false);

  // on error
  const onError = useCallback(
    (msg) => {
      setOpenErrorModal(true);
      setErrorMsg(msg);
    },
    [setOpenErrorModal, setErrorMsg]
  );

  // on check
  const onCheck = useCallback((msg) =>{
    setCheckMsg(msg);
    setOpenCheckModal(true);
  }, [setOpenCheckModal])

  // request server to save data
  const onRequestSaveData = useCallback(() => {
    // close modal
    setOpenCheckModal(false);

    // query
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_UPDATE_ROWS}?schema=${schemaName}`;

    console.log(gridRows)

    // Request protocol
    const req = new REQ_UpdateConfigRows(schemaName, gridRows);

    // request
    Request_Patch(query, req).then((res) => {
      if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`);
    });
  }, [schemaName, onError, gridRows]);

  return (
    <Fragment>
      {/* Buttons */}
      <Stack
        direction="row"
        spacing={2}
        sx={{
          justifyContent: 'flex-end',
          alignItems: 'center',
          paddingTop: '10px',
        }}
      >
        {/* Save */}
        <Button variant="contained" onClick={() => onCheck('Are you sure to update rows?')}>
          Save
        </Button>
        {/* Cancel */}
        <Button variant="contained" color="error" onClick={() => navigate(0)}>
          Cancel
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
        setOnCheckOk={onRequestSaveData}
      />
    </Fragment>
  );
}

// ConvertDataToGridData.
function ConvertColsToGridCols(
  columns: Array<ClassSchemaColumn> | null,
  valueOptionsDict: Map<string, []>
): GridColDef[] {
  let gridColumns: GridColDef[] = [];

  // convert columns to grid columns
  columns?.forEach(async (col) => {
    let type = ConvertType(col.type);
    let valueOpts: [] | undefined = [];

    // select type
    if (type === 'singleSelect') valueOpts = valueOptionsDict[col.type];

    // add grid column
    gridColumns.push({
      field: col.name,
      headerName: col.name,
      type: type,
      minWidth: 130,
      editable: true,
      headerAlign: 'center',
      align: 'right',
      valueOptions: valueOpts,
    });
  });

  return gridColumns;
}

// ConvertType
function ConvertType(type: string): GridColType {
  switch (type) {
    // numbers
    case 'int':
    case 'long':
    case 'float':
    case 'double':
      return 'number';
    // string
    case 'string':
      return 'string';
    // date
    case 'date':
      return 'dateTime';
    // boolean.
    case 'bool':
      return 'boolean';
    default:
      return 'singleSelect';
  }
}
