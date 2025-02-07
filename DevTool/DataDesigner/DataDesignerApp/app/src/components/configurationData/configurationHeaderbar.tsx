import { Button, Card, CardActions, CardContent, Typography } from '@mui/material';
import React, { Fragment, useCallback, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Request_Delete } from '../../apis/restAPI.ts';
import { REQ_DeleteConfigSchema, RES_DeleteConfigSchema } from '../../classes/shared/protocols/configuration.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import CheckModal from '../modals/checkModal.tsx';
import ConfigurationEditSchema from './configurationEditSchema.tsx';
import ErrorModal from '../modals/errorModal.tsx';

export default function ConfigurationHeaderbar({ schemaName, desciption }) {
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

    // on edit
    const onEdit = useCallback(() => {
        setOpenSchemaModal(true);
    }, [setOpenSchemaModal]);

    // on delete
    const onDelete = useCallback((msg) => {
        setOpenCheckModal(true)
        setCheckMsg(msg)
    }, [setOpenCheckModal])

    // on delete
    const onRequestDelete = useCallback(() => {
        // parameters
        const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_CONFIG_DELETE_SCHEMA}`;
        const req = new REQ_DeleteConfigSchema(schemaName);

        // request
        Request_Delete<RES_DeleteConfigSchema>(query, req)
            .then((res) => {
                if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorCode}`);
                else navigate(0);

            })
        setOpenCheckModal(false)

    }, [schemaName])

    // check schema name
    if (!schemaName) {
        return <Fragment></Fragment>
    }

    // Render header
    return (
        <Fragment>
            {/* Card */}
            <Card>
                {/* Content */}
                <CardContent sx={{ height: '80px' }}>
                    <Typography gutterBottom variant="h5" component="div" color='green'>
                        {schemaName}
                    </Typography>
                    {desciption}
                </CardContent>
                
                {/* CardActions */}
                <CardActions>
                    <Button size="small" variant="outlined" onClick={onEdit}>Edit</Button>
                    <Button size="small" variant="outlined" color='error' onClick={() => onDelete('Are you sure to remove?')}>Remove</Button>
                </CardActions>
            </Card>

            {/* Edit modal */}
            <ConfigurationEditSchema
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
    )
}