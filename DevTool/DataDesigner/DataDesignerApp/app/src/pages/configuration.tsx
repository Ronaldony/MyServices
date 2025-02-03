import {
  Box,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../apis/restAPI.ts';
import { ClassSchema } from '../classes/shared/classSchema.ts';
import {
  RES_FindConfigRows,
  RES_FindConfigSchema,
} from '../classes/shared/protocols/configuration.ts';
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

      {/* Data */}
      <GridColumns schema={schema} isEditable={false} />

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
function GridColumns({ schema, isEditable }: any) {
  // table columns
  const [tableRows, setTableRows] = useState<Array<any>>([]);

  // status.
  const [tableLoaded, setTableLoaded] = useState(false);

  // request rows.
  useEffect(() => {
    if (schema !== undefined) {
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_ROWS}?schema=${schema?.name}`;

      Request_Get<RES_FindConfigRows>(query).then((res) => {
        if (res instanceof RES_Error) {
          console.log('error');
        } else {
          setTableLoaded(true);
          setTableRows(res.rows);
        }
      });
    }
  }, [schema]);

  // schema undefined
  if (tableLoaded === false) {
    return <Fragment></Fragment>;
  }

  // Update row value
  function updateRowValue(updateIndex, key, value) {
    setTableRows(
      tableRows.map((col, index) => {
        if (index === updateIndex) {
          // modify
          col[key] = value;
          return col;
        } else {
          return col;
        }
      })
    );
  }

  return (
    <Fragment>
      {/* Table */}
      <TableContainer>
        <Table stickyHeader aria-label="sticky table">
          {/* Header */}
          <TableHead>
            <TableRow>
              {schema?.columns?.map((element, index) => {
                return (
                  <TableCell align="center" key={index}>
                    {element.name}
                  </TableCell>
                );
              })}
            </TableRow>
          </TableHead>

          {/* Body */}
          <TableBody>
            {tableRows?.map((row, index) => {
              let rowKeys = Object.keys(row);
              console.log(`index: ${index}`);
              return (
                <TableRow hover role="checkbox" tabIndex={-1} key={index}>
                  {rowKeys.map((rowKey) => {
                    console.log(`rowKey: ${rowKey}`);

                    return (
                      <TableCell
                        style={{ paddingTop: 0, paddingBottom: 0 }}
                        key={rowKey}
                      >
                        {/* Type check */}
                        <TextField
                          disabled={isEditable === false}
                          variant="standard"
                          focused
                          value={row[rowKey]}
                          required
                          onChange={(e) =>
                            updateRowValue(index, rowKey, e.target.value)
                          }
                        />
                      </TableCell>
                    );
                  })}
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>
    </Fragment>
  );
}
