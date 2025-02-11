import FolderIcon from '@mui/icons-material/Folder';
import { Box, Button, IconButton, Stack, Tab, Tabs, TextField, Typography } from '@mui/material';
import React, { Fragment, useCallback, useEffect, useRef, useState } from 'react';
import { Request_Get } from '../apis/restAPI.ts';
import { Profile, Profile_FilePath } from '../classes/shared/profile.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import { RES_FindProfile } from '../classes/shared/protocols/generate.ts';
import GenerateMenu from '../components/generate/generateMenu.tsx';
import ErrorModal from '../components/modals/errorModal.tsx';
import GenerateHeaderbar from '../components/generate/generateHeaderbar.tsx';

// GeneratePage
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

  // Folder button
  const inputRef = useRef<HTMLInputElement>(null);

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

  // Function to trigger the hidden file input
  const handleButtonClick = () => {
    inputRef.current?.click();
  };

  // Handler for when the folder is selected
  const handleFolderChange = useCallback((event: React.ChangeEvent<HTMLInputElement>, handleIndex: number) => {
    const files = event.target.files;
    if (files && files.length > 0) {
      // Get folder full path.
      const firstFile = files[0] as any;
      const fileLength = firstFile.name.length + 1; // +1은 '/' 포함
      const projectPath = firstFile.path.slice(0, fileLength * -1);

      setProfile({
        ...profile, filePathList: profile.filePathList.map((filePath, index) => {
          if (index === handleIndex) filePath.projectPath = projectPath;
          return filePath;
        })
      }
      )
    }
  }, [profile, setProfile]);

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

  // Profile을 선택하지 않은 경우.
  if (selectedProfile === '') {
    return <Box sx={{ display: 'flex', height: '100%', width: '100%' }}>
      {/* Menu */}
      <div style={{ height: '100%' }}>
        {/* Menu */}
        <GenerateMenu
          selectedProfile={selectedProfile}
          setSelectedProfile={setSelectedProfile}
        />
      </div>
    </Box>
  }

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
      <Box sx={{ p: 3, width: '100%' }}>
        <GenerateHeaderbar profileName={selectedProfile} profile={profile} />

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
              <Stack
                direction={{ xs: 'column', sm: 'row' }}
                spacing={{ xs: 1, sm: 2, md: 4 }}
              >
                {/* ProjectPath */}
                <TextField
                  sx={{ width: '90%' }}
                  label="ProjectPath"
                  value={filePath.projectPath ?? ""}
                  variant="filled"
                  focused
                  onChange={(e) =>
                    setProfile({
                      ...profile,
                      filePathList: profile.filePathList.map(
                        (row, rowIndex) => {
                          if (index === rowIndex)
                            row.projectPath = e.target.value;
                          return row;
                        }
                      ),
                    })
                  }
                />
                {/* Folder picker */}
                <input
                  type="file"
                  style={{ display: 'none' }}
                  ref={inputRef}
                  onChange={(e) => handleFolderChange(e, index)}
                  {...({ webkitdirectory: "true" } as any)}
                />
                <IconButton onClick={handleButtonClick} style={{ padding: 15, margin: 0 }}>
                  <FolderIcon />
                </IconButton>
              </Stack>

              {/* ProtocolPath */}
              <TextField
                sx={{ width: '90%' }}
                label="ProtocolPath"
                value={filePath.protocolPath ?? ""}
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
                sx={{ width: '90%' }}
                label="DataCodePath"
                value={filePath.dataCodePath ?? ""}
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
                sx={{ width: '90%' }}
                label="DatabaseFilePath"
                value={filePath.databaseFilePath ?? ""}
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

        {/* Save */}
        <Button
          variant="contained"
          onClick={() => onGenerate('Are you sure to generate?')}
        >
          Generate
        </Button>
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
      style={{ gap: '10px', display: 'flex', flexDirection: 'column' }}
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
