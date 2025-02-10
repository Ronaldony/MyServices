import AddCircleIcon from '@mui/icons-material/AddCircle';
import {
  Divider,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from '@mui/material';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Request_Get, Request_Post } from '../../apis/restAPI.ts';
import { RES_Error } from '../../classes/shared/protocols/error.ts';
import {
  REQ_CreateProfile,
  RES_CreateProfile,
  RES_GetProfileNames,
} from '../../classes/shared/protocols/generate.ts';
import ErrorModal from '../modals/errorModal.tsx';
import LoadingSpinner from '../modals/loadingSpinner.tsx';

//////////////////////////////////////////////////////////////////////////
// Configuration menu
export default function GenerateMenu({ selectedProfile, setSelectedProfile }) {
  // profile.
  const [profileNames, setProfileNames] = useState<Array<string>>([]);

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // status
  const [isLoading, setIsLoading] = useState(true);

  // initialize
  useEffect(() => {
    // query
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_GENERATE_GET_PROFILE_NAMES}`;

    // requet
    Request_Get<RES_GetProfileNames>(query).then((res) => {
      if (res instanceof RES_Error)
        onError(`[${res.errorCode}] ${res.errorMsg}`);
      else setProfileNames(res.profileNames);

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
    },
    [setErrorMsg, setOpenErrorModal]
  );

  // switch selected data
  const onSelectedProfile = useCallback(
    (selectedData) => {
      setSelectedProfile(selectedData);
    },
    [setSelectedProfile]
  );

  // on add profile.
  const onAddProfile = useCallback(() => {
    const randomProfileName = Math.random().toString(36).slice(2, 7);

    // request
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_GENERATE_CREATE_PROFILE}`;

    Request_Post<RES_CreateProfile>(
      query,
      new REQ_CreateProfile(randomProfileName)
    ).then((res) => {
      if (res instanceof RES_Error) {
        onError(`[${res.errorCode}] ${res.errorMsg}`);
      } else {
        setProfileNames((prev) => [...prev, randomProfileName]);
        setSelectedProfile(randomProfileName);
      }
    });

    console.log(query)
  }, [setSelectedProfile, onError]);

  // loading
  if (true === isLoading) {
    return <LoadingSpinner />;
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
          <ListItemButton onClick={onAddProfile}>
            <ListItemIcon>
              <AddCircleIcon />
            </ListItemIcon>
            <ListItemText primary={'Create'} />
          </ListItemButton>
        </ListItem>

        <Divider />

        {/* Profile list */}
        {profileNames.map((profileName) => (
          <ListItem disablePadding key={profileName}>
            <ListItemButton
              selected={selectedProfile === profileName}
              onClick={() => onSelectedProfile(profileName)}
            >
              <ListItemText primary={profileName} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>

      {/* Error modal */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Fragment>
  );
}
