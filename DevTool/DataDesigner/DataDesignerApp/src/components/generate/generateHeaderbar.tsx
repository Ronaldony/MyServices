import { Button, Stack, Typography } from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Request_Delete, Request_Patch } from '../../apis/restAPI.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import { REQ_DeleteProfile, REQ_UpdateProfile, RES_DeleteProfile, RES_UpdateProfile } from '../../classes/shared/protocols/generate.ts';
import CheckModal from '../modals/checkModal.tsx';
import ErrorModal from '../modals/errorModal.tsx';

export default function GenerateHeaderbar({ profileName, profile }: any) {
    // profile
    const [oriProfileName, setOriProfileName] = useState(profileName);

    // navigate
    const navigate = useNavigate();

    // error
    const [errorMsg, setErrorMsg] = useState('');
    const [openErrorModal, setOpenErrorModal] = useState(false);

    // delete
    const [deleteMsg, setDeleteMsg] = useState('');
    const [openDeleteModal, setOpenDeleteModal] = useState(false);

    // edit
    const [editMsg, setEditMsg] = useState('');
    const [openEditModal, setOpenEditModal] = useState(false);

    // Set ori profile name.
    useEffect(() => {
        setOriProfileName(profileName);
    }, [profileName]);

    // on error
    const onError = useCallback(
        (msg) => {
            setErrorMsg(msg);
            setOpenErrorModal(true);
        },
        [setErrorMsg, setOpenErrorModal]
    );

    // on delete 
    const onDelete = useCallback((msg) => {
        setOpenDeleteModal(true)
        setDeleteMsg(msg)
    }, [setOpenDeleteModal])

    // on edit 
    const onUpdate = useCallback((msg) => {
        setOpenEditModal(true)
        setEditMsg(msg)
    }, [setOpenEditModal])

    // on request edit
    const onRequestUpdate = useCallback(() => {
        // parameters
        const req = new REQ_UpdateProfile(oriProfileName, profile);

        // request
        Request_Patch<RES_UpdateProfile>(process.env.REACT_APP_API_SUB_GENERATE_UPDATE_PROFILE, req)
            .then((res) => {
                if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorMsg}`);
                else navigate(0)
                
                // update previous profile name.
                setOriProfileName(profile.name)
            })

        // reset modal
        setOpenEditModal(false)
    }, [profile, navigate, onError, oriProfileName])

    // on request delete
    const onRequestDelete = useCallback(() => {
        // parameters
        const req = new REQ_DeleteProfile(oriProfileName);

        // request
        Request_Delete<RES_DeleteProfile>(process.env.REACT_APP_API_SUB_GENERATE_DELETE_PROFILE, req)
            .then((res) => {
                if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorMsg}`);
                else navigate(0)
            })

        // reset modal
        setOpenDeleteModal(false)
    }, [oriProfileName, navigate, onError])

    // Render header
    return (
        <div style={{ gap: '15px', paddingBottom: '15px' }}>
            {/* Title */}
            <Typography gutterBottom variant="h5" component="div" color='green'>
                Generate Data
            </Typography>

            {/* Edit, Delete button */}
            <Stack
                direction={{ xs: 'column', sm: 'row' }}
                spacing={{ xs: 1 }}
            >
                <Button size="small" variant="outlined" onClick={() => onUpdate('Are you sure to update?')}>Update</Button>
                <Button size="small" variant="outlined" color='error' onClick={() => onDelete('Are you sure to remove?')}>Remove</Button>
            </Stack>

            {/* Error modal */}
            <ErrorModal
                openErrorModal={openErrorModal}
                errorMsg={errorMsg}
                setOpenErrorModal={setOpenErrorModal}
            />

            {/* Edit modal */}
            <CheckModal
                openCheckModal={openEditModal}
                checkMsg={editMsg}
                setOpenCheckModal={setOpenEditModal}
                setOnCheckOk={onRequestUpdate}
            />

            {/* Delete modal */}
            <CheckModal
                openCheckModal={openDeleteModal}
                checkMsg={deleteMsg}
                setOpenCheckModal={setOpenDeleteModal}
                setOnCheckOk={onRequestDelete}
            />
        </div>
    )
}