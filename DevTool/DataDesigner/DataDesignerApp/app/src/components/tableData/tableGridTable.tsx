import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import { Button, ListItemIcon, Menu, MenuItem, Stack } from '@mui/material';
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
import {
  ClassSchemaColumn
} from '../../classes/shared/classSchema.ts';
import { RES_FetchTypeAndTableKeys } from '../../classes/shared/protocols/common.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import { REQ_UpdateTableRows } from '../../classes/shared/protocols/table.ts';
import CheckModal from '../modals/checkModal.tsx';
import ErrorModal from '../modals/errorModal.tsx';
import TableHeaderbar from './tableHeaderbar.tsx';

// GridColumns.
var gridColumns: GridColDef[] = [];

export default function TableGridTable({
  schemaName,
  schemaList,
  setSchemaList,
  schemaIndex,
}: any) {
  // status
  const [dataLoaded, setDataLoaded] = useState(false);

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
        setSchemaList(
          schemaList.map((schema, index) => {
            if (index === schemaIndex)
              return {
                ...schema,
                rows: schema.rows.filter((row) => row.Id !== deleteRow.Id),
              };
            else return schema;
          })
        );
      });

      // close context menu
      onContextMenuClose();
    },
    [onContextMenuClose, schemaIndex, schemaList, setSchemaList]
  );

  // add rows
  const onAddRow = useCallback(() => {
    const sortedRows = [...schemaList.at(schemaIndex).rows].sort((l, r) =>
      l.Id > r.Id ? 1 : -1
    );
    const last = sortedRows[sortedRows.length - 1];
    let newId = last?.Id + 1;

    if (last?.Id === undefined) newId = 1;

    // set schema
    setSchemaList(
      schemaList.map((schema, index) => {
        if (index === schemaIndex)
          return {
            ...schema,
            rows: [...schema.rows, { Id: newId, internalId: uuid() }],
          };
        else return schema;
      })
    );
  }, [schemaIndex, setSchemaList, schemaList]);

  // get row id
  const getRowId = useCallback((row) => {
    return row.internalId;
  }, []);

  // processRowUpdate
  const processRowUpdate = (newRow: GridRowModel) => {
    const updatedRow = { ...newRow, isNew: false };
    setSchemaList(
      schemaList.map((schema, index) => {
        if (index === schemaIndex)
          return {
            ...schema,
            rows: schema.rows.map((row) =>
              row.internalId === newRow.internalId ? updatedRow : row
            ),
          };
        else return schema;
      })
    );

    return updatedRow;
  };

  ////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////
  // request value options for select of grid table
  useEffect(() => {
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_FETCH_TYPE_TABLE_KEYS}`;

    Request_Get<RES_FetchTypeAndTableKeys>(query).then((res) => {
      if (res instanceof RES_Error)
        onError(`[${res.errorCode}] ${res.errorCode}`);
      else {
        gridColumns = ConvertColsToGridCols(
          schemaList.at(schemaIndex).schema.columns,
          res.schemaKeysDict
        );
        setDataLoaded(true);
      }
    });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // wait for loaded data
  if (false === dataLoaded) {
    return <></>;
  }

  // grid columns
  return (
    <Fragment>
      {/* Header */}
      <TableHeaderbar
        schemaName={schemaName}
        description={schemaList.at(schemaIndex).schema.description}
      />

      {/* Grid table */}
      <div onContextMenu={handleContextMenuPos} style={{ height: '95%' }}>
        {/* Grid */}
        <DataGrid
          apiRef={apiRef}
          rows={schemaList.at(schemaIndex).rows}
          columns={gridColumns}
          editMode="row"
          getRowId={getRowId}
          processRowUpdate={processRowUpdate}
          hideFooter
          showCellVerticalBorder
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
      <SubmitButtons
        schemaName={schemaName}
        gridRows={schemaList.at(schemaIndex).rows}
      />

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
  const onCheck = useCallback(
    (msg) => {
      setCheckMsg(msg);
      setOpenCheckModal(true);
    },
    [setOpenCheckModal]
  );

  // request server to save data
  const onRequestSaveData = useCallback(() => {
    // close modal
    setOpenCheckModal(false);

    // query
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_TABLE_UPDATE_ROWS}?schema=${schemaName}`;

    console.log(gridRows);

    // Request protocol
    const req = new REQ_UpdateTableRows(schemaName, gridRows);

    // request
    Request_Patch(query, req).then((res) => {
      if (res instanceof RES_Error)
        onError(`[${res.errorCode}] ${res.errorCode}`);
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
        <Button
          variant="contained"
          onClick={() => onCheck('Are you sure to update rows?')}
        >
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

    // check ref table and type
    if (col.refTable) {
      valueOpts = valueOptionsDict[col.refTable];
      type = 'singleSelect';
    }
    else if (type === 'singleSelect') {
      valueOpts = valueOptionsDict[col.type];
    }

    // add grid column
    if (type === 'dateTime')
    {
      gridColumns.push({
        field: col.name,
        headerName: col.name,
        type: type,
        minWidth: 130,
        editable: true,
        headerAlign: 'center',
        align: 'right',
        valueGetter: (value) => value && new Date(value)
      });
    }
    else
    {
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
    }
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
