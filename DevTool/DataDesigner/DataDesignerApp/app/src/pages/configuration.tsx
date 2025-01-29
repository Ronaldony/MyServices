import { Box } from '@mui/material';
import React, { useState } from 'react';
import ConfigurationMenu from '../components/menus/configurationMenu.tsx';

export default function ConfigurationPage() {
  const [selectedConfig, setSelectedConfig] = useState('');

  return (
    <Box sx={{ display: 'flex', height: '100vh' }}>
      {/* DataList */}
      <ConfigurationMenu setSelectedConfig={setSelectedConfig}/>
      <div>Other thigs</div>
    </Box>
  );
}
