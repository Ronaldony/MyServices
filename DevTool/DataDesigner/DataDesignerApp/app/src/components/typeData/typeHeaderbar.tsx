import {
  Button,
  Card,
  CardActions,
  CardContent,
  Typography,
} from '@mui/material';
import React, { Fragment, useCallback, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Request_Delete } from '../../apis/restAPI.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import {
  REQ_DeleteTypeSchema,
  RES_DeleteTypeSchema,
} from '../../classes/shared/protocols/type.ts';
import CheckModal from '../modals/checkModal.tsx';
import ErrorModal from '../modals/errorModal.tsx';
import TypeEditSchema from './typeEditSchema.tsx';

export default function TypeHeaderbar({
  schemaName,
  description,
  dataGridApiRef,
}) {
  const navigate = useNavigate();

  // schema
  const [openSchemaModal, setOpenSchemaModal] = useState(false);

  // error
  const [errorMsg, setErrorMsg] = useState('');
  const [openErrorModal, setOpenErrorModal] = useState(false);

  // check
  const [checkMsg, setCheckMsg] = useState('');
  const [openCheckModal, setOpenCheckModal] = useState(false);

  // on error
  const onError = useCallback(
    (msg) => {
      setErrorMsg(msg);
      setOpenErrorModal(true);
    },
    [setErrorMsg, setOpenErrorModal]
  );

  // onResizeColumn
  const onResizeColumn = useCallback(() => {
    dataGridApiRef.current.autosizeColumns({
      includeHeaders: true,
      includeOutliers: true,
      outliersFactor: 1,
      expand: false,
    });
  }, [dataGridApiRef]);

  // on edit
  const onEdit = useCallback(() => {
    setOpenSchemaModal(true);
  }, [setOpenSchemaModal]);

  // on delete
  const onDelete = useCallback(
    (msg) => {
      setOpenCheckModal(true);
      setCheckMsg(msg);
    },
    [setOpenCheckModal]
  );

  // on delete
  const onRequestDelete = useCallback(() => {
    // parameters
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_TYPE_DELETE_SCHEMA}`;
    const req = new REQ_DeleteTypeSchema(schemaName);

    // request
    Request_Delete<RES_DeleteTypeSchema>(query, req).then((res) => {
      if (res instanceof RES_Error)
        onError(`[${res.errorCode}] ${res.errorMsg}`);
      else navigate(0);
    });
    setOpenCheckModal(false);
  }, [schemaName, navigate, onError]);

  // check schema name
  if (!schemaName) {
    return <Fragment></Fragment>;
  }

  // Render header
  return (
    <Fragment>
      {/* Card */}
      <Card>
        {/* Content */}
        <CardContent sx={{ height: '80px' }}>
          <Typography gutterBottom variant="h5" component="div" color="green">
            {schemaName}
          </Typography>
          {description}
        </CardContent>

        {/* CardActions */}
        <CardActions>
          {/* Resize column */}
          <Button size="small" variant="outlined" onClick={onResizeColumn}>
            Resize column
          </Button>
          {/* Edit */}
          <Button size="small" variant="outlined" onClick={onEdit}>
            Edit
          </Button>
          {/* Remove */}
          <Button
            size="small"
            variant="outlined"
            color="error"
            onClick={() => onDelete('Are you sure to remove?')}
          >
            Remove
          </Button>
        </CardActions>
      </Card>

      {/* Edit modal */}
      <TypeEditSchema
        openSchemaModal={openSchemaModal}
        setOpenSchemaModal={setOpenSchemaModal}
        schemaName={schemaName}
      />

      {/* Error modal */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />

      {/* Check modal */}
      <CheckModal
        openCheckModal={openCheckModal}
        checkMsg={checkMsg}
        setOpenCheckModal={setOpenCheckModal}
        setOnCheckOk={onRequestDelete}
      />
    </Fragment>
  );
}
