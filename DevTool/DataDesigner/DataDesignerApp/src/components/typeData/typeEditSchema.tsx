import {
  Box,
  Button,
  Modal,
  Stack,
  TextField,
  Typography
} from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Request_Get, Request_Post } from '../../apis/restAPI.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import { REQ_UpdateTypeSchema, RES_FindTypeSchema, RES_UpdateTypeSchema } from '../../classes/shared/protocols/type.ts';
import { TypeSchema } from '../../classes/shared/typeSchema.ts';
import CheckModal from '../modals/checkModal.tsx';
import ErrorModal from '../modals/errorModal.tsx';

// modal style
const modalStyle = {
  position: 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: '360px',
  height: '335px',
  bgcolor: 'background.paper',
  boxShadow: 24,
  p: 4,
}

//////////////////////////////////////////////////////////////////////////
// ConfigurationEditSchema
export default function TypeEditSchema({ openSchemaModal, setOpenSchemaModal, schemaName }) {
  // schema
  const [schema, setSchema] = useState<TypeSchema>(
    new TypeSchema('', '', '')
  );

  // error
  const [errorMsg, setErrorMsg] = useState('');
  const [openErrorModal, setOpenErrorModal] = useState(false);

  // check
  const [checkMsg, setCheckMsg] = useState('');
  const [openCheckModal, setOpenCheckModal] = useState(false);

  // navigate
  const navigate = useNavigate();

  // close modals
  const onSuccessUpdated = useCallback(() => {
    setOpenCheckModal(false);
    setOpenSchemaModal(false);
    navigate(0);
  }, [setOpenCheckModal, setOpenSchemaModal, navigate]);

  // on error
  const onError = useCallback(
    (msg) => {
      setErrorMsg(msg);
      setOpenErrorModal(true);
    },
    [setErrorMsg, setOpenErrorModal]
  );

  // on check
  const onCheckModal = useCallback(
    (msg) => {
      setCheckMsg(msg);
      setOpenCheckModal(true);
    },
    [setCheckMsg, setOpenCheckModal]
  );

  async function onCheckOkAsync() {
    await reqCreateSchemaAsync();
  }

  // request create schema.
  const reqCreateSchemaAsync = useCallback(async () => {
    // request
    const res = await Request_Post<RES_UpdateTypeSchema>(
      process.env.REACT_APP_API_SUB_TYPE_UPDATE_SCHEMA,
      new REQ_UpdateTypeSchema(schema)
    );

    // error.
    if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorMsg}`);
    else onSuccessUpdated();
  }, [schema, onError, onSuccessUpdated]);


  //////////////////////////////////////////////////
  // request schema
  useEffect(() => {
    // query
    const query = `${process.env.REACT_APP_API_SUB_TYPE_FIND_SCHEMA}?schema=${schemaName}`;

    // request
    Request_Get<RES_FindTypeSchema>(query).then((res) => {
      if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorMsg}`);
      else setSchema(res.schema);
    })
  }, [openSchemaModal, onError, schemaName])

  // render
  return (
    <Fragment>
      {/* Schema modal */}
      <Modal
        open={openSchemaModal}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={modalStyle}>
          {/* Title */}
          <Typography id="modal-modal-title" variant="h6" component="h2">
            Edit schema
          </Typography>

          {/* Schema title */}
          <Box
            component="form"
            sx={{ '& > :not(style)': { m: 1, width: '25ch' } }}
          >
            {/* Name */}
            <TextField
              label="Name"
              variant="standard"
              focused
              value={schema.name}
              required
              error={!schema.name}
              onChange={(e) =>
                setSchema({ ...schema, name: e.target.value })
              }
            />

            {/* Category */}
            <TextField
              label="Category"
              variant="standard"
              focused
              value={schema.category}
              onChange={(e) =>
                setSchema({ ...schema, category: e.target.value })
              }
            />

            {/* description */}
            <TextField
              label="Desciption"
              variant="standard"
              focused
              value={schema.description}
              onChange={(e) =>
                setSchema({ ...schema, description: e.target.value })
              }
            />
          </Box>

          {/* Buttons */}
          <Stack
            direction="row"
            spacing={2}
            sx={{
              justifyContent: 'flex-end',
              alignItems: 'center',
            }}
          >
            <Button
              variant="contained"
              onClick={async () =>
                onCheckModal('Are you sure to create schema?')
              }
            >
              Edit
            </Button>
            <Button
              variant="contained"
              color="error"
              onClick={() => setOpenSchemaModal(false)}
            >
              Close
            </Button>
          </Stack>

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
            setOnCheckOk={onCheckOkAsync}
          />
        </Box>
      </Modal>
    </Fragment>
  );
}