
import Divider from '@mui/material/Divider';
import Drawer from '@mui/material/Drawer';
import Toolbar from '@mui/material/Toolbar';
import * as React from 'react';
import SideList from './sideList.tsx';

export default function PermanentDrawerLeft() {
  const drawerWidth = 240;

  return (
    <React.Fragment>
      {/* Sidebar */}
      <Drawer
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
          },
        }}
        variant="permanent"
        anchor="left"
      >
        <Toolbar />
        <Divider />
        {/* Side list */}
        <SideList/>
      </Drawer>
    </React.Fragment>
  );
}
