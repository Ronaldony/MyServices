import AddCircleIcon from '@mui/icons-material/AddCircle';
import {
  Divider,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText
} from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Request_Get } from '../../apis/restAPI.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import { RES_GetAllTable } from '../../classes/shared/protocols/table.ts';
import ErrorModal from '../modals/errorModal.tsx';
import LoadingSpinner from '../modals/loadingSpinner.tsx';
import TableCreateSchema from './tableCreateSchema.tsx';

//////////////////////////////////////////////////////////////////////////
// Configuration menu
export default function TableMenu({ setSelectedSchema }) {
  const [selectedData, setSelectedData] = useState('');
  const [schemaList, setSchemaList] = useState<Array<string>>([]);
  const [errorMsg, setErrorMsg] = useState('');

  // status
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [openSchemaModal, setOpenSchemaModal] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isError, setIsError] = useState(false);

  // initialize
  useEffect(() => {
    Request_Get<RES_GetAllTable>(
      process.env.REACT_APP_API_SUB_TABLE_GETALL
    ).then((res) => {
      if (res instanceof RES_Error)
        onError(`[${res.errorCode}] ${res.errorMsg}`);
      else setSchemaList(res.schemaNames);

      // loading status
      setIsLoading(false);
    });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // onError
  const onError = useCallback(
    (msg) => {
      setErrorMsg(msg);
      setOpenErrorModal(true);
      setIsError(true);
    },
    [setErrorMsg, setOpenErrorModal, setIsError]
  );

  // switch selected data
  const onSelectedData = useCallback(
    (selectedData) => {
      setSelectedData(selectedData);
      setSelectedSchema(selectedData);
    },
    [setSelectedData, setSelectedSchema]
  );

  // loading
  if (true === isLoading) {
    return <LoadingSpinner />;
  }

  // error render
  if (true === isError) {
    return (
      <Fragment>
        <ErrorModal
          openErrorModal={openErrorModal}
          errorMsg={errorMsg}
          setOpenErrorModal={setOpenErrorModal}
        />
      </Fragment>
    );
  }

  // menu
  return (
    <Fragment>
      {/* List */}
      <List
        sx={{
          overflow: 'auto',
          borderRight: '1px solid silver',
          height: '100%',
        }}
      >
        {/* Create */}
        <ListItem>
          <ListItemButton onClick={() => setOpenSchemaModal(true)}>
            <ListItemIcon>
              <AddCircleIcon />
            </ListItemIcon>
            <ListItemText primary={'Create'} />
          </ListItemButton>
        </ListItem>

        <Divider />

        {/* DataList */}
        {schemaList.map((data) => (
          <ListItem disablePadding key={data}>
            <ListItemButton
              selected={selectedData === data}
              onClick={() => onSelectedData(data)}
            >
              <ListItemText primary={data} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>

      {/* CreateSchema */}
      <TableCreateSchema
        openSchemaModal={openSchemaModal}
        setOpenSchemaModal={setOpenSchemaModal}
      />

      {/* Error modal */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Fragment>
  );
}

