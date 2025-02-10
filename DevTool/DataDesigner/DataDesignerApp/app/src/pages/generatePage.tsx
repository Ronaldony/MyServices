import React, { Fragment, useCallback, useEffect, useState } from 'react';
import GenerateMenu from '../components/generate/generateMenu.tsx';
import { Box, Tab, Tabs, TextField, Typography } from '@mui/material';
import { Profile, Profile_FilePath } from '../classes/shared/profile.ts';
import { RES_FindProfile } from '../classes/shared/protocols/generate.ts';
import { Request_Get } from '../apis/restAPI.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import ErrorModal from '../components/modals/errorModal.tsx';

export default function GeneratePage() {
  // profile
  const [selectedProfile, setSelectedProfile] = useState('');
  const [profile, setProfile] = useState<Profile>(
    new Profile('', new Array<Profile_FilePath>())
  );

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // tab value
  const [value, setValue] = React.useState(0);

  // handle change of tab
  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  // onError
  const onError = useCallback(
    (msg) => {
      setErrorMsg(msg);
      setOpenErrorModal(true);
    },
    [setErrorMsg, setOpenErrorModal]
  );

  // request profile
  useEffect(() => {
    if (selectedProfile !== '') {
      // query
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_GENERATE_FIND_PROFILE}?profileName=${selectedProfile}`;
      // reqeust
      Request_Get<RES_FindProfile>(query).then((res) => {
        if (res instanceof RES_Error)
          onError(`[${res.errorCode}] ${res.errorMsg}`);
        else setProfile(res.profile);
      });
    }
  }, [selectedProfile, onError]);

  return (
    <Box sx={{ display: 'flex', height: '100%', width: '100%' }}>
      {/* Menu */}
      <div style={{ height: '100%' }}>
        {/* Menu */}
        <GenerateMenu
          selectedProfile={selectedProfile}
          setSelectedProfile={setSelectedProfile}
        />
      </div>

      {/* Generate page */}
      <Box sx={{ p: 3, width:'100%' }}>
        {/* Title */}
        <Typography gutterBottom variant="h5" component="div" color="green">
          Generate Data
        </Typography>

        {/* Profile name */}
        <TextField
          label="Profile name"
          variant="standard"
          value={profile.name}
          error={!profile.name}
          focused
          required
          onChange={(e) => setProfile({ ...profile, name: e.target.value })}
        />

        {/* Grid */}
        <Tabs value={value} onChange={handleChange} sx={{ display: 'block' }}>
          {profile.filePathList?.map((filePath, index) => {
            return (
              <Tab label={filePath.target} key={index} {...tabProps(index)} />
            );
          })}
        </Tabs>

        {/* Grid */}
        {profile.filePathList?.map((filePath, index) => {
          return (
            <CustomTabPanel value={value} index={index} key={index}>
              {/* ProtocolPath */}
                <TextField
                  label="ProtocolPath"
                  value={filePath.protocolPath}
                  variant="filled" 
                  focused
                  onChange={(e) =>
                    setProfile({
                      ...profile,
                      filePathList: profile.filePathList.map(
                        (row, rowIndex) => {
                          if (index === rowIndex)
                            row.protocolPath = e.target.value;
                          return row;
                        }
                      ),
                    })
                  }
                />

              {/* DataCodePath */}
                <TextField
                  label="DataCodePath"
                  value={filePath.dataCodePath}
                  variant="filled" 
                  focused
                  onChange={(e) =>
                    setProfile({
                      ...profile,
                      filePathList: profile.filePathList.map(
                        (row, rowIndex) => {
                          if (index === rowIndex)
                            row.dataCodePath = e.target.value;
                          return row;
                        }
                      ),
                    })
                  }
                />
              {/* DatabaseFilePath */}
                <TextField
                  label="DatabaseFilePath"
                  value={filePath.databaseFilePath}
                  variant="filled" 
                  focused
                  onChange={(e) =>
                    setProfile({
                      ...profile,
                      filePathList: profile.filePathList.map(
                        (row, rowIndex) => {
                          if (index === rowIndex)
                            row.databaseFilePath = e.target.value;
                          return row;
                        }
                      ),
                    })
                  }
                />
            </CustomTabPanel>
          );
        })}
      </Box>

      {/* Error modal */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />
    </Box>
  );
}

// TabPanelProps
interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

// CustomTabPanel
function CustomTabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      style={{ gap: '10px', display: 'flex', flexDirection:'column' }}
      {...other}
    >
      {value === index && <Fragment>{children}</Fragment>}
    </div>
  );
}

// all
function tabProps(index: number) {
  return {
    id: `simple-tab-${index}`,
    'aria-controls': `simple-tabpanel-${index}`,
  };
}
