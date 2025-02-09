import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import React from 'react';
import { Route, Routes } from 'react-router-dom';
import ConfigurationPage from '../pages/configurationPage.tsx';
import GeneratePage from '../pages/generate.tsx';
import Tablepage from '../pages/tablepage.tsx';
import TestPage from '../pages/test.tsx';
import TypePage from '../pages/typePage.tsx';
import HeaderBar from './bars/headerBar.tsx';
import Sidebar from './bars/sideBar.tsx';

export default function Layout() {
  return (
    <div style={{ display: 'flex', width: '100%', height: '100%' }}>
      {/* header */}
      <HeaderBar />
      {/* css */}
      <CssBaseline />
      {/* sidebar */}
      <Sidebar />
      {/* body */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          bgcolor: 'background.default',
          p: 3,
          width: '100%',
          height: '100%',
          paddingTop: '65px',
        }}
      >
        <Routes>
          <Route path="/" element={<TestPage />}></Route>
          <Route 
            path="/data/configuration"
            element={<ConfigurationPage />}
          ></Route>
          <Route path="/data/type" element={<TypePage />}></Route>
          <Route path="/data/table" element={<Tablepage />}></Route>
          <Route path="/generate" element={<GeneratePage />}></Route>
        </Routes>
      </Box>
    </div>
  );
}
