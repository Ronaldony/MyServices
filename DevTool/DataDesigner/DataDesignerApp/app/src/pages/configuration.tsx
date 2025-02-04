import {
  Box
} from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../apis/restAPI.ts';
import { ClassSchema, ClassSchemaColumn } from '../classes/shared/classSchema.ts';
import {
  RES_FindConfigRows,
  RES_FindConfigSchema
} from '../classes/shared/protocols/configuration.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import ErrorModal from '../components/modals/errorModal.tsx';
import ConfigurationMenu from './../components/menus/configurationMenu.tsx';
import { DataGrid, GridColDef, GridColType } from '@mui/x-data-grid';
import { RES_FindTypeOrTableKeys } from '../classes/shared/protocols/common.ts';

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
        if (res instanceof RES_Error) onError('fail to find schema');
        else setSchema(res.schema);
      });
    }
  }, [selectedSchema, onError, setSchema]);

  // render
  return (
    <Box sx={{ display: 'flex', height: '100vh' }}>
      {/* Menu */}
      <ConfigurationMenu setSelectedSchema={setSelectedSchema} />

      {/* Grid columns */}
      <GridColumns schemaName={schema?.name} columns={schema?.columns} isEditable={false} />

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
function GridColumns({ schemaName, columns, isEditable }: any) {
  // status
  const [dataLoaded, setDataLoaded] = useState(false);

  // grid
  const [gridRows, setGridRows] = useState<Array<any>>([]);

  // api
  const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_ROWS}?schema=${schemaName}`;

  // update columns
  useEffect(() => {
    if ((columns !== null) && (columns !== undefined)) {
      // set grid columns
      gridColumns = ConvertColsToGridCols(columns, schemaName);

      // request grid rows.
      Request_Get<RES_FindConfigRows>(query).then((res) => {
        if (res instanceof RES_Error) {
          console.log('error');
        } else {
          setDataLoaded(true);
          setGridRows(res.rows);
        }
      });
    }
  }, [columns]);

  // check data loaded
  if (false === dataLoaded) {
    return <></>
  }

  // grid columns
  return (
    <Fragment>
      <DataGrid rows={gridRows} columns={gridColumns} editMode="row" />
    </Fragment>
  );
}

// ConvertDataToGridData.
function ConvertColsToGridCols(columns: Array<ClassSchemaColumn>, schemaName: string): GridColDef[] {
  let gridDatas: GridColDef[] = [];

  // convert columns to grid columns
  columns.forEach(col => {
    let type = ConvertType(col.type);
    let field = col.name.charAt(0).toLowerCase() + col.name.slice(1);
    let valueOpts = GetValueOptions(schemaName);

    // add grid column
    gridDatas.push({
      field: field,
      headerName: col.name,
      type: type,
      width: 100,
      editable: true,
      valueOptions: valueOpts
    });
  });

  return gridDatas;
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

// GetValueOptions.
function GetValueOptions(schemaName: string): [] {
  const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_FIND_TYPE_TABLE_KEYS}?schema=${schemaName}`;

  Request_Get<RES_FindTypeOrTableKeys>(query)
    .then(res => {
      if (res instanceof RES_Error) {
        // error
        return [];
      }
      else {
        // value options
        return res.keys;
      }
    })

  return [];
}