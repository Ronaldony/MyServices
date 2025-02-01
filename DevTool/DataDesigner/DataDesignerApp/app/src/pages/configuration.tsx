import { Box } from '@mui/material';
import React, { useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../apis/restAPI.ts';
import { ClassSchema } from '../classes/classSchema.ts';
import { RES_FindConfigSchema } from '../classes/protocols/configuration.ts';
import { RES_Error } from '../classes/protocols/error.ts';
import ErrorModal from '../components/modals/errorModal.tsx';
import ConfigurationMenu from './../components/menus/configurationMenu.tsx';

export default function ConfigurationPage() {
  // configuration
  const [selectedConfig, setSelectedConfig] = useState('');
  const [schema, setSchema] = useState<ClassSchema>();
  
  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // on error
  const onError = useCallback((msg) => {
    setOpenErrorModal(true)
    setErrorMsg(msg)
  }, [setOpenErrorModal, setErrorMsg]);

  useEffect(() => {
    if (selectedConfig !== '')
    {
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_SCHEMA}?schema=${selectedConfig}`;
      
      Request_Get<RES_FindConfigSchema>(query)
      .then(res => {
        if (res instanceof RES_Error) onError('fail to find schema');
        else setSchema(res.schema);
      })
    }
  }, [selectedConfig, onError, setSchema])

  return (
    <Box sx={{ display: 'flex', height: '100vh' }}>
      {/* ConfigurationMenu */}
      <ConfigurationMenu setSelectedConfig={setSelectedConfig}/>
      
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Box>
  );
}
