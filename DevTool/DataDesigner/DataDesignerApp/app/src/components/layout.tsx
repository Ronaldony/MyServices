import React, { Fragment } from 'react';

import { Routes, Route } from 'react-router-dom';
import Sidebar from './bar/sidebar.tsx';
import TestPage from '../pages/test.tsx';

export default function Layout() {
  return (
    <div className="App">
      <Sidebar/>
      <Routes>
        <Route path="/" element={<TestPage />}></Route>
        </Routes>
    </div>
  );
}
