import React, { Fragment, useEffect } from 'react';
import { Request_Get } from '../apis/restAPI.ts';

export default function TestPage() {
  // Initialize.
  useEffect(() => {
    fetchDataListAsync('/').then((res) => {
      console.log("res: ");
      console.log(res);
    })
    .catch(error => {
      console.log("error: ");
      console.log(error);
    })
    ;
  }, []);

  return (
    <Fragment>
      <div>Configuration page.</div>
    </Fragment>
  );
}

// fetchDataListAsync.
const fetchDataListAsync = async (path) => {
  return await Request_Get<any>(`${process.env.REACT_APP_API_BASE}${path}`);
};
