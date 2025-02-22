import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import React from 'react';

export default function HeaderBar() {
  const drawerWidth = 200;

  return (
    <AppBar
      position="fixed"
      sx={{ width: `calc(100% - ${drawerWidth}px)`, ml: `${drawerWidth}px` }}
    >
      {/* Header */}
      <Toolbar>
        <Typography variant="h6" noWrap component="div">
          Data Designer
        </Typography>
      </Toolbar>
    </AppBar>
  );
}
