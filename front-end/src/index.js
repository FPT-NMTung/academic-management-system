import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import 'antd/dist/antd.min.css';
import { NextUIProvider } from '@nextui-org/react';
import { BrowserRouter } from 'react-router-dom';
import { ConfigProvider } from 'antd';
import viVN from 'antd/lib/locale/vi_VN';
import './i18n';

ReactDOM.render(
  <React.StrictMode>
    <NextUIProvider>
      <ConfigProvider locale={viVN}>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </ConfigProvider>
    </NextUIProvider>
  </React.StrictMode>,
  document.getElementById('root')
);