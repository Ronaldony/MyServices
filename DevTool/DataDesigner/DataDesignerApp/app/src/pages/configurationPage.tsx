import { Box, Tab, Tabs } from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import ConfigurationGridTable from '../components/configurationData/configurationGridTable.tsx';
import ConfigurationMenu from '../components/configurationData/configurationMenu.tsx';
import { ClassData } from '../classes/shared/classSchema.ts';
import { RES_FindConfigRows } from '../classes/shared/protocols/configuration.ts';
import { Request_Get } from '../apis/restAPI.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import ErrorModal from '../components/modals/errorModal.tsx';

export default function ConfigurationPage() {
  // configuration
  const [selectedSchema, setSelectedSchema] = useState('');

  // selected schema list
  const [schemaList, setSchemaList] = useState<Array<ClassData>>([]);

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // tab value
  const [value, setValue] = React.useState(0);

  // handle change of tab
  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  // on error
  const onError = useCallback(
    (msg) => {
      setOpenErrorModal(true);
      setErrorMsg(msg);
    },
    [setOpenErrorModal, setErrorMsg]
  );

  // create new tab when selected schema changed.
  useEffect(() => {
    if (selectedSchema !== '') {
      // add shcmea into list
      if (
        undefined ===
        schemaList.find((schema) => schema.schema.name === selectedSchema)
      ) {
        // query
        const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_FIND_ROWS}?schema=${selectedSchema}`;

        // request
        Request_Get<RES_FindConfigRows>(query).then((res) => {
          if (res instanceof RES_Error)
            onError(`[${res.errorCode}] ${res.errorMsg}`);
          else
            setSchemaList((prev) => [
              ...prev,
              new ClassData(res.schema, res.rows),
            ]);
        });
      }
    }
  }, [selectedSchema, schemaList, onError]);

  // onClickTab
  const onClickTab = useCallback(
    (schemaName) => {
      setSelectedSchema(schemaName);
      // eslint-disable-next-line react-hooks/exhaustive-deps
    },
    [setSelectedSchema]
  );

  // change tab
  useEffect(() => {
    schemaList.forEach((schema, index) => {
      if (schema.schema.name === selectedSchema) setValue(index);
    });
  }, [selectedSchema, schemaList]);

  // render
  return (
    <Box sx={{ display: 'flex', width: '100%', height: '100%'}}>
      {/* Menu */}
      <div style={{ height: '100%', width:'15%' }}>
        <ConfigurationMenu setSelectedSchema={setSelectedSchema} />
      </div>

      {/* Tabs */}
      <div
        style={{
          borderColor: 'divider',
          width: '100%',
          height: '100%',
        }}
      >
        {/* Grid */}
        <Tabs
          value={value}
          onChange={handleChange}
          aria-label="basic tabs example"
          variant="scrollable"
          scrollButtons
        >
          {schemaList.map((schema, index) => {
            return (
              <Tab
                label={schema.schema.name}
                key={index}
                {...tabProps(index)}
                onClick={() => onClickTab(schema.schema.name)}
              />
            );
          })}
        </Tabs>

        {/* Grid */}
        {schemaList.map((row, index) => {
          return (
            <CustomTabPanel value={value} index={index} key={index}>
              <ConfigurationGridTable
                schemaName={row.schema.name}
                schemaList={schemaList}
                setSchemaList={setSchemaList}
                schemaIndex={index}
              />
            </CustomTabPanel>
          );
        })}
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

// TabPanelProps
interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

// CustomTabPanel
function CustomTabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      style={{ width: '100%', height: '70%' }}
      {...other}
    >
      {value === index && (
        <Fragment>{children}</Fragment>
      )}
    </div>
  );
}

// all
function tabProps(index: number) {
  return {
    id: `simple-tab-${index}`,
    'aria-controls': `simple-tabpanel-${index}`,
  };
}
