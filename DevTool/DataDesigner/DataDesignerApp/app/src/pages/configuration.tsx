import { Box } from '@mui/material';
import React, { useEffect, useState } from 'react';
import { Request_Get, Request_Post } from '../apis/restfulAPI.ts';
import DataList from '../components/list/dataList.tsx';
import { RES_AllConfiguration, RES_CreateConfigSchema } from '../interfaces/api/configuration.ts';

export default function ConfigurationPage() {
  const [selectedData, setSelectedData] = useState('Type_Test');
  const [createSchema, setCreateSchema] = useState(false);
  const [dataList, setDataList] = useState<RES_AllConfiguration | null>();

  // Initialize.
  useEffect(() => {
    fetchDataListAsync(process.env.REACT_APP_API_SUB_CONFIG_GETALL).then(
      (res) => {
        setDataList(res);
      }
    );
  }, []);

  // Create.
  useEffect(() => {
    // Reset create button.
    if (createSchema === true) {
      console.log('Reset setCreateSchema');
      setCreateSchema(false);

      // Modal.
    }
  }, [createSchema]);

  // Select
  useEffect(() => {
    console.log(`select ${selectedData}`);
  }, [selectedData]);

  return (
    <Box sx={{ display: 'flex', height: '100vh' }}>
      {/* DataList */}
      <DataList
        setSelectedData={setSelectedData}
        setCreateData={setCreateSchema}
        dataList={dataList?.schemaNames}
      />
      <div>Other thigs</div>
    </Box>
  );
}

// fetchDataListAsync.
const fetchDataListAsync = async (path) => {
  return await Request_Get<RES_AllConfiguration>(
    `${process.env.REACT_APP_API_BASE}${path}`
  );
};

// createSchemaAsync.
const createSchemaAsync = async (path, body) => {
  return await Request_Post<RES_CreateConfigSchema>(
    `${process.env.REACT_APP_API_BASE}${path}`, body
  );
};
