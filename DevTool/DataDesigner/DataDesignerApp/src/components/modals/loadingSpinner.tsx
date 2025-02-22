import { CircularProgress, Modal } from '@mui/material';
import React, { Fragment } from 'react';

export default function LoadingSpinner() {
  return (
    <Fragment>
      <Modal
        open={true}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <CircularProgress
          sx={{
            position: 'absolute',
            top: '50%',
            left: '50%',
          }}
        />
      </Modal>
    </Fragment>
  );
}
