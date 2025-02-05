import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import { Box, ListItemIcon, Menu, MenuItem } from '@mui/material';
import {
  DataGrid,
  GridColDef,
  GridColType,
  GridRowId,
  GridRowModel,
  useGridApiRef,
} from '@mui/x-data-grid';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../apis/restAPI.ts';
import {
  ClassSchema,
  ClassSchemaColumn,
} from '../classes/shared/classSchema.ts';
import { RES_FetchTypeAndTableKeys } from '../classes/shared/protocols/common.ts';
import {
  RES_FindConfigRows,
  RES_FindConfigSchema,
} from '../classes/shared/protocols/configuration.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import ConfigurationMenu from '../components/menus/configurationMenu.tsx';
import ErrorModal from '../components/modals/errorModal.tsx';
import uuid from "react-uuid"

export default function ConfigurationPage() {
  // configuration
  const [selectedSchema, setSelectedSchema] = useState('');
  const [schema, setSchema] = useState<ClassSchema>();

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // on error
  const onError = useCallback(
    (msg) => {
      setOpenErrorModal(true);
      setErrorMsg(msg);
    },
    [setOpenErrorModal, setErrorMsg]
  );

  // request schema
  useEffect(() => {
    if (selectedSchema !== '') {
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_SCHEMA}?schema=${selectedSchema}`;

      Request_Get<RES_FindConfigSchema>(query).then((res) => {
        if (res instanceof RES_Error)
          onError(`[${res.errorCode}] ${res.errorCode}`);
        else setSchema(res.schema);
      });
    }
  }, [selectedSchema, onError, setSchema]);

  // render
  return (
    <Box sx={{ display: 'flex', height: '100%', width: '100%' }}>
      {/* Menu */}
      <ConfigurationMenu setSelectedSchema={setSelectedSchema} />

      {/* Grid columns */}
      <GridColumns schemaName={schema?.name} columns={schema?.columns} />

      {/* Error */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Box>
  );
}

// GridColumns.
var gridColumns: GridColDef[] = [];

function GridColumns({ schemaName, columns }: any) {
  // grid
  const [gridRows, setGridRows] = useState<Array<any>>([]);

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
    [setContextMenuPos]
  );

  // delete rows
  const deleteRows = useCallback(
    (rowMap: Map<GridRowId, GridRowModel>) => {
      rowMap.forEach((deleteRow) => {
        setGridRows(gridRows.filter((row) => row.Id !== deleteRow.Id));
      });

      onContextMenuClose();
    },
    [gridRows, setGridRows, onContextMenuClose]
  );

  // add rows
  const addRow = useCallback(() => {
    const sortedRows = [...gridRows].sort((l, r) => l.Id > r.Id ? 1: -1);
    const last = sortedRows[gridRows.length - 1];
    const newId = last.Id + 1;

    console.log(gridRows)
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
  // request type and table keys
  useEffect(() => {
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_FETCh_TYPE_TABLE_KEYS}`;

    Request_Get<RES_FetchTypeAndTableKeys>(query).then((res) => {
      if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`);
      else setValueOptionsDict(res.schemaKeysDict);
    });
  }, [onError, setValueOptionsDict]);

  // Convert columns and request rows
  useEffect(() => {
    if (columns !== null && columns !== undefined) {
      // set grid columns
      gridColumns = ConvertColsToGridCols(columns, valueOptionsDict);

      // request grid rows.
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_ROWS}?schema=${schemaName}`;

      Request_Get<RES_FindConfigRows>(query).then((res) => {
        if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`);
        else setGridRows(res.rows);
      });
    }
  }, [columns, onError, schemaName, valueOptionsDict]);

  // grid columns
  return (
    <Fragment>
      <div onContextMenu={handleContextMenuPos} style={{ minWidth: '83%' }}>
        {/* Grid */}
        <DataGrid
          apiRef={apiRef}
          rows={gridRows}
          columns={gridColumns}
          editMode="row"
          disableAutosize={true}
          getRowId={getRowId}
          processRowUpdate={processRowUpdate}
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
          <MenuItem onClick={addRow}>
            <ListItemIcon>
              <AddIcon />
            </ListItemIcon>
            Add Row
          </MenuItem>
          {/* Delete Row */}
          <MenuItem
            onClick={() => {
              deleteRows(apiRef.current.getSelectedRows());
            }}
          >
            <ListItemIcon>
              <DeleteIcon />
            </ListItemIcon>
            Delete Row
          </MenuItem>
        </Menu>
      </div>

      {/* Error */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Fragment>
  );
}

// ConvertDataToGridData.
function ConvertColsToGridCols(
  columns: Array<ClassSchemaColumn>,
  valueOptionsDict: Map<string, []>
): GridColDef[] {
  let gridColumns: GridColDef[] = [];

  // convert columns to grid columns
  columns.forEach(async (col) => {
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
