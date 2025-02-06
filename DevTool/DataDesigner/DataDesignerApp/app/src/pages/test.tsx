import React, { Fragment, useEffect } from 'react';

import { styled } from '@mui/material/styles';
import { DataGrid, GridColDef, GridRowsProp } from '@mui/x-data-grid';
import HeaderBar from './../components/bars/headerBar';

export default function TestPage() {

  return (
    <Fragment>
      <ConditionalValidationGrid/>
    </Fragment>
  );
}

const StyledBox = styled('div')(({ theme }) => ({
  height: 300,
  width: '100%',
  '& .MuiDataGrid-cell--editing': {
    backgroundColor: 'rgb(255,215,115, 0.19)',
    color: '#1a3e72',
    '& .MuiInputBase-root': {
      height: '100%',
    },
  },
  '& .Mui-error': {
    backgroundColor: 'rgb(126,10,15, 0.1)',
    color: theme.palette.error.main,
    ...theme.applyStyles('dark', {
      backgroundColor: 'rgb(126,10,15, 0)',
    }),
  },
}));

const rows: GridRowsProp = [
  {
    id: 1,
    expense: 'Light bill',
    price: 1,
    dueAt: new Date(2021, 6, 8),
    isPaid: false,
    paymentMethod: '',
  },
  {
    id: 2,
    expense: 'Rent',
    price: 2,
    dueAt: new Date(2021, 7, 1),
    isPaid: false,
    paymentMethod: '',
  },
  {
    id: 3,
    expense: 'Car insurance',
    price: 3,
    dueAt: new Date(2021, 7, 4),
    isPaid: true,
    paymentMethod: 'Wire transfer',
  },
];

 function ConditionalValidationGrid() {
  const columns: GridColDef[] = [
    { field: 'expense', headerName: 'Expense', width: 160, editable: true },
    {
      field: 'price',
      headerName: 'Price',
      type: 'number',
      width: 120,
      editable: true,
    },
    {
      field: 'dueAt',
      headerName: 'Due at',
      type: 'date',
      width: 120,
      editable: true,
    },
    {
      field: 'isPaid',
      headerName: 'Is paid?',
      type: 'boolean',
      width: 140,
      editable: true,
    },
    {
      field: 'paymentMethod',
      headerName: 'Payment method',
      type: 'singleSelect',
      valueOptions: ['Credit card', 'Wire transfer', 'Cash'],
      width: 160,
      editable: true,
    },
  ];

  return (
    <Fragment>

      <div style={{height:'100%'}} onContextMenu={() => {console.log('sd')}}>

        <DataGrid rows={rows} columns={columns} editMode="row" />
      </div>
    </Fragment>
  );
}