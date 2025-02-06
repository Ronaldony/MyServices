import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import React, { StrictMode } from 'react';
import ReactDOM from 'react-dom/client';
import { HashRouter } from 'react-router-dom';
import Layout from './components/layout.tsx';
import './index.css';
import reportWebVitals from './reportWebVitals.ts';

// index.html에서 id가 "root"인 요소를 가져옴.
const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

// "root"를 렌더링
root.render(
  <StrictMode>
    <HashRouter>
      <Layout />
    </HashRouter>
  </StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
