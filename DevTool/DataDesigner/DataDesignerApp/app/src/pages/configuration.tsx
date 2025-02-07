import { Box } from '@mui/material';
import React, { useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../apis/restAPI.ts';
import { ClassSchema } from '../classes/shared/classSchema.ts';
import { RES_FindConfigSchema } from '../classes/shared/protocols/configuration.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import ConfigurationGridTable from '../components/configurationData/configurationGridTable.tsx';
import ConfigurationHeaderbar from '../components/configurationData/configurationHeaderbar.tsx';
import ConfigurationMenu from '../components/configurationData/configurationMenu.tsx';
import ErrorModal from '../components/modals/errorModal.tsx';

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
        if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`);
        else setSchema(res.schema);
      });
    }
  }, [selectedSchema, onError, setSchema]);

  // render
  return (
    <Box sx={{ display: 'flex', height: '100%', width: '100%' }}>

      {/* Menu */}
      <div style={{ height: '100%' }}>
        <ConfigurationMenu setSelectedSchema={setSelectedSchema} />
      </div>

      {/* Grid table */}
      <div style={{ width: '100%', height: '80%' }}>
        <ConfigurationHeaderbar schemaName={schema?.name} desciption={schema?.desciption} />
        <ConfigurationGridTable schemaName={schema?.name} columns={schema?.columns} />
      </div>

      {/* Error */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Box>
  );
}
