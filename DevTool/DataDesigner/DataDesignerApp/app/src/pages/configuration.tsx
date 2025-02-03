import { Box, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField } from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../apis/restAPI.ts';
import { ClassSchema, ClassSchemaColumn } from '../classes/shared/classSchema.ts';
import { RES_FindConfigSchema } from '../classes/shared/protocols/configuration.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import ErrorModal from '../components/modals/errorModal.tsx';
import ConfigurationMenu from './../components/menus/configurationMenu.tsx';


export default function ConfigurationPage() {
  // configuration
  const [selectedSchema, setSelectedSchema] = useState('');
  const [schema, setSchema] = useState<ClassSchema>();

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // on error
  const onError = useCallback((msg) => {
    setOpenErrorModal(true)
    setErrorMsg(msg)
  }, [setOpenErrorModal, setErrorMsg]);

  // request schema
  useEffect(() => {
    if (selectedSchema !== '') {
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_SCHEMA}?schema=${selectedConfig}`;

      Request_Get<RES_FindConfigSchema>(query)
        .then(res => {
          if (res instanceof RES_Error) onError('fail to find schema');
          else setSchema(res.schema);
        })
    }
  }, [selectedSchema, onError, setSchema])

  // render
  return (
    <Box sx={{ display: 'flex', height: '100vh' }}>

      {/* Menu */}
      <ConfigurationMenu setSelectedSchema={setSelectedSchema} />

      {/* Data */}
      <GridColumns columns={schema?.columns} isEditable={false} />

      {/* Error */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Box>
  );
}


// Grid
function GridColumns(columns: Array<ClassSchemaColumn> | any, isEditable: boolean) {
  // table columns
  const [tableColumns, setTableColumns] = useState<Array<ClassSchemaColumn> | null>(columns);
  const [tableRows, setTableRows] = useState([]);

  // Update column names.

  return (
    <Fragment>
      {/* Table */}
      <TableContainer sx={{ height: '350px' }}>
        <Table stickyHeader aria-label="sticky table">
          {/* Header */}
          <TableHead>
            {tableColumns?.map(element => {
              return (
                <TableCell align='center'>{element.name}</TableCell>
              )
            })}
          </TableHead>

          {/* Body */}
          <TableBody>
            {tableRows.map((row, index) => {
              let keys = Object.keys(row);

              return (
                <TableRow hover role="checkbox" tabIndex={-1} key={index}>
                  {keys.map(key => {
                    return (
                      <TableCell style={{ paddingTop: 0, paddingBottom: 0 }}>
                        <TextField
                          disabled={isEditable === false}
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
                    )
                  })}

                </TableRow>);
            })}


          </TableBody>
        </Table>
      </TableContainer>
    </Fragment>
  );
}