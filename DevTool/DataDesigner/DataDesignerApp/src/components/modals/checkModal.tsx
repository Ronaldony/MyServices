import { Box, Button, Modal, Stack, Typography } from '@mui/material';
import React, { Fragment } from 'react';

// CheckModal
export default function CheckModal({
  openCheckModal,
  checkMsg,
  setOpenCheckModal,
  setOnCheckOk
}) {
  return (
    <Fragment>
      {/* Error modal */}
      <Modal
        open={openCheckModal}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box
          sx={{
            position: 'absolute',
            top: '50%',
            left: '50%',
            transform: 'translate(-50%, -50%)',
            width: 300,
            bgcolor: 'background.paper',
            boxShadow: 24,
            p: 4,
          }}
        >
          {/* Title */}
          <Typography id="modal-modal-title" variant="h6" component="h3">
            Check
          </Typography>

          {/* Message */}
          <Typography id="modal-modal-description">{checkMsg}</Typography>

          {/* Buttons */}
          <Stack
            direction="row"
            spacing={2}
            sx={{
              justifyContent: 'flex-end',
              alignItems: 'center',
              paddingTop:'10px'
            }}
          >
            <Button variant="contained" onClick={() => setOnCheckOk()}>
              Ok
            </Button>
            <Button
              variant="contained"
              onClick={() => setOpenCheckModal(false)}
              color='error'
            >
              Close
            </Button>
          </Stack>
        </Box>
      </Modal>
    </Fragment>
  );
}
