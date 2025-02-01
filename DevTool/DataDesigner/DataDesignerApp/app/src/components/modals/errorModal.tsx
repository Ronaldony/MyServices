import { Box, Button, Modal, Stack, Typography } from '@mui/material';
import React, { Fragment } from 'react';

// ErrorModal
export default function ErrorModal({
  openErrorModal,
  errorMsg,
  setOpenErrorModal,
}) {
  return (
    <Fragment>
      {/* Error modal */}
      <Modal
        open={openErrorModal}
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
            Error
          </Typography>

          {/* Message */}
          <Typography id="modal-modal-description">{errorMsg}</Typography>

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
            <Button
              variant="contained"
              onClick={() => setOpenErrorModal(false)}
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
