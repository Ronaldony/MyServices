import { Box, Tab, Tabs } from '@mui/material';
import React, { useEffect, useState } from 'react';
import ConfigurationGridTable from '../components/configurationData/configurationGridTable.tsx';
import ConfigurationMenu from './../components/configurationData/configurationMenu.tsx';

export default function ConfigurationPage() {
  // configuration
  const [selectedSchema, setSelectedSchema] = useState('');

  // selected schema list
  const [schemaList, setSchemaList] = useState<Array<string>>([]);

  const [value, setValue] = React.useState(0);

  // handle change of tab
  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  // create new tab when selected schema changed.
  useEffect(() => {
    if (selectedSchema !== '') {
      // add shcmea into list
      if (undefined === schemaList.find((schema) => schema === selectedSchema))
        setSchemaList((prev) => [...prev, selectedSchema]);

      // move to schema
    }
  }, [selectedSchema, schemaList]);

  // change tab
  useEffect(() => {
    schemaList.forEach((schema, index) => {
      if (schema === selectedSchema) {
        setValue(index);
      }
    });
  }, [selectedSchema, schemaList]);

  // render
  return (
    <Box sx={{ display: 'flex', height: '100%', width: '100%' }}>
      {/* Menu */}
      <div style={{ height: '100%' }}>
        <ConfigurationMenu setSelectedSchema={setSelectedSchema} />
      </div>

      {/* Tabs */}
      <div
        style={{
          borderBottom: 1,
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
            return <Tab label={schema} key={index} {...tabProps(index)} />;
          })}
        </Tabs>

        {/* Grid */}
        {schemaList.map((schema, index) => {
          return (
            <CustomTabPanel value={value} index={index} key={index}>
              <ConfigurationGridTable schemaName={schema} />
            </CustomTabPanel>
          );
        })}
      </div>
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
      style={{ width: '100%', height: '70%', padding: 0, margin: 0 }}
      {...other}
    >
      {value === index && (
        <div style={{ width: '100%', height: '100%' }}>{children}</div>
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
