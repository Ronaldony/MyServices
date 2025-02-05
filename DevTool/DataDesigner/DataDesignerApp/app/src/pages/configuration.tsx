import { Box, ListItemIcon, Menu, MenuItem } from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../apis/restAPI.ts';
import {
  ClassSchema,
  ClassSchemaColumn,
} from '../classes/shared/classSchema.ts';
import {
  RES_FindConfigRows,
  RES_FindConfigSchema,
} from '../classes/shared/protocols/configuration.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import ErrorModal from '../components/modals/errorModal.tsx';
import { DataGrid, GridColDef, GridColType, GridRowId, GridRowModel, useGridApiRef } from '@mui/x-data-grid';
import ConfigurationMenu from '../components/menus/configurationMenu.tsx';
import { RES_FetchTypeAndTableKeys } from '../classes/shared/protocols/common.ts';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';

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
        if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`)
        else setSchema(res.schema);
      });
    }
  }, [selectedSchema, onError, setSchema]);

  // render
  return (
    <Box sx={{ display: 'flex', height: '100%', width: '100%'}}>
      {/* Menu */}
      <ConfigurationMenu setSelectedSchema={setSelectedSchema} />

      {/* Grid columns */}
      <GridColumns
        schemaName={schema?.name}
        columns={schema?.columns}
      />

      {/* Error */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Box>
  );
}

var gridColumns: GridColDef[] = [];

// GridColumns.
function GridColumns({ schemaName, columns }: any) {
  // status
  const [dataLoaded, setDataLoaded] = useState(false);

  // grid
  const [gridRows, setGridRows] = useState<Array<any>>([]);
  const [gridRowIds, setGridRowIds] = useState<Array<number>>([]);

  // value options
  const [valueOptionsDict, setValueOptionsDict] = useState<Map<string, []>>(new Map());

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // apiRef
  const apiRef = useGridApiRef();

  // context
  const [isContextOpen, setIsContextOpen] = useState(false);
  const [contextMenuPos, setContextMenuPos] = React.useState({
    mouseX: 0,
    mouseY: 0
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
  }, [setIsContextOpen])

  // handle context menu
  const handleContextMenuPos = useCallback((event: React.MouseEvent) => {
    event.preventDefault();

    // set menu position
    setContextMenuPos({mouseX: event.clientX , mouseY: event.clientY });

    // trigge context menu
    setIsContextOpen(true);
  }, [setContextMenuPos]);

  // resetGridRowIds
  const resetGridRowIds = useCallback((rowLength) => {
    setGridRowIds([])

    // reset
    for (let cnt = 0; cnt < rowLength; cnt++)
      setGridRowIds((prev) => [...prev, cnt])
  }, [setGridRowIds])

  // delete rows
  const deleteRows = useCallback((rowMap: Map<GridRowId, GridRowModel>) => {
    rowMap.forEach((deleteRow) => {
      setGridRows(gridRows.filter(row => row.id !== deleteRow.id));
      resetGridRowIds(gridRows.length)
    })

    onContextMenuClose()
  }, [gridRows, gridRowIds, onContextMenuClose]);
  
  // add rows
  const addRow = useCallback(() => {
    setGridRows((prev) => [
      ...prev, {id: 11}
    ])
  
    setGridRowIds((prev) => [
      ...prev, gridRows.length - 1
    ])
  }, [setGridRows, gridRows]);

  ////////////////////////////////////////////////////////////////
  // request type and table keys
  useEffect(() => {
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_FETCh_TYPE_TABLE_KEYS}`;

    Request_Get<RES_FetchTypeAndTableKeys>(query).then((res) => {
      if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`)
      else setValueOptionsDict(res.schemaKeysDict)
    })
  }, [onError]);

  // 
  useEffect(() => {
    
  }, [gridRows])

  // Convert columns and request rows
  useEffect(() => {
    if (columns !== null && columns !== undefined) {
      // set grid columns
      gridColumns = ConvertColsToGridCols(columns, valueOptionsDict);

      // request grid rows.
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_ROWS}?schema=${schemaName}`;

      Request_Get<RES_FindConfigRows>(query).then((res) => {
        if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`)
        else {
          setDataLoaded(true);
          setGridRows(res.rows);
          setGridRowIds(res.rows.map(row => row.id))
        }
      });
    }
  }, [columns, onError, schemaName, valueOptionsDict]);

  // check data loaded
  if (false === dataLoaded) {
    return <></>;
  }

  function getRowId(row) {
    let getId = 0;
    
    gridRows.forEach((gridRow, index) => {
      if (gridRow.id === row.id) 
        {
          console.log("lucky")
          getId = index;
        }
        else{
          console.log("unlucky")
        }
    })

    console.log(`getId: ${getId} rowId: ${row.id}`)

    return getId;
  };
  
  // grid columns
  return (
    <Fragment>
      <div onContextMenu={handleContextMenuPos} style={{minWidth: '83%'}}>

        {/* Grid */}
        <DataGrid apiRef={apiRef} rows={gridRows} columns={gridColumns} editMode="row" disableAutosize={true} />

        {/* Context Menu */}
        <Menu
          open={isContextOpen}
          onClose={onContextMenuClose}
          anchorReference="anchorPosition"
          anchorPosition={{ top: contextMenuPos.mouseY, left: contextMenuPos.mouseX }}
        >
          {/* Add Row */}
          <MenuItem onClick={addRow}>
            <ListItemIcon><AddIcon /></ListItemIcon>
            Add Row
          </MenuItem>
          {/* Delete Row */}
          <MenuItem onClick={() => {deleteRows(apiRef.current.getSelectedRows())}}>
            <ListItemIcon><DeleteIcon /></ListItemIcon> 
            Delete Row
          </MenuItem>
        </Menu>
      </div>

      {/* Error */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal} />
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
    let field = col.name.charAt(0).toLowerCase() + col.name.slice(1);
    let valueOpts: [] | undefined = [];

    // select type
    if (type === 'singleSelect') valueOpts = valueOptionsDict[col.type];

    // add grid column
    gridColumns.push({
      field: field,
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