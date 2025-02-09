import {
  Box,
  Button,
  Modal,
  Stack,
  TextField,
  Typography
} from '@mui/material';
import React, { Fragment, useCallback, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Request_Post } from '../../apis/restAPI.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import { REQ_CreateTypeSchema, RES_CreateTypeSchema } from '../../classes/shared/protocols/type.ts';
import CheckModal from '../modals/checkModal.tsx';
import ErrorModal from '../modals/errorModal.tsx';
import { TypeSchema } from './../../classes/shared/typeSchema.ts';

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
// ConfigurationCreateSchema
export default function TypeCreateSchema({ openSchemaModal, setOpenSchemaModal }) {
  // schema
  const [newSchema, setNewSchema] = useState<TypeSchema>(
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
  const onSuccessCreated = useCallback(() => {
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

  // onCheckOkAsync
  async function onCheckOkAsync() {
    await reqCreateSchemaAsync();
  }

  // request create schema.
  const reqCreateSchemaAsync = useCallback(async () => {
    // request
    const res = await Request_Post<RES_CreateTypeSchema>(
      `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_TYPE_CREATE_SCHEMA}`,
      new REQ_CreateTypeSchema(newSchema)
    );

    // error.
    if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorMsg}`);
    else onSuccessCreated();
  }, [newSchema, onError, onSuccessCreated]);

  // return
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
            Create schema
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
              value={newSchema.name}
              required
              error={!newSchema.name}
              onChange={(e) =>
                setNewSchema({ ...newSchema, name: e.target.value })
              }
            />

            {/* Category */}
            <TextField
              label="Category"
              variant="standard"
              focused
              value={newSchema.category}
              onChange={(e) =>
                setNewSchema({ ...newSchema, category: e.target.value })
              }
            />

            {/* description */}
            <TextField
              label="Desciption"
              variant="standard"
              focused
              value={newSchema.description}
              onChange={(e) =>
                setNewSchema({ ...newSchema, description: e.target.value })
              }
            />
          </Box>

          {/* Buttons */}
          <Stack
            direction="row"
            spacing={2}
            sx={{
              paddingTop: '10px',
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
              Create
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