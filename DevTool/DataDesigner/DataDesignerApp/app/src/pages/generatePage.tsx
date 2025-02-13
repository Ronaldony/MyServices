import FolderIcon from '@mui/icons-material/Folder';
import {
  Box,
  Button,
  IconButton,
  Stack,
  Tab,
  Tabs,
  TextField,
} from '@mui/material';
import React, {
  Fragment,
  useCallback,
  useEffect,
  useState
} from 'react';
import { Request_Get, Request_Post } from '../apis/restAPI.ts';
import { Profile, Profile_FilePath } from '../classes/shared/profile.ts';
import { RES_Error } from '../classes/shared/protocols/error.ts';
import {
  REQ_Generate,
  RES_FindProfile,
  RES_Generate,
} from '../classes/shared/protocols/generate.ts';
import GenerateHeaderbar from '../components/generate/generateHeaderbar.tsx';
import GenerateMenu from '../components/generate/generateMenu.tsx';
import CheckModal from '../components/modals/checkModal.tsx';
import ErrorModal from '../components/modals/errorModal.tsx';
import RequestingSpinner from '../components/modals/requestingSpinner.tsx';

// GeneratePage
export default function GeneratePage() {
  // profile
  const [selectedProfile, setSelectedProfile] = useState('');
  const [profile, setProfile] = useState<Profile>(
    new Profile('', new Array<Profile_FilePath>())
  );
  const [target, setTarget] = useState("Client");

  // error
  const [openErrorModal, setOpenErrorModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // generate
  const [openGenerateModal, setOpenGenerateModal] = useState(false);
  const [generateMsg, setGenerateMsg] = useState('');

  // status
  const [isRequesting, setIsRequesting] = useState(false);

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

  // onGenerate
  const onGenerate = useCallback((msg) => {
    setGenerateMsg(msg);
    setOpenGenerateModal(true);
  }, []);

  // onRequestGenerate
  const onRequestGenerate = useCallback(() => {
    // query
    const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_GENERATE_DATA}`;
    const req = new REQ_Generate(target, profile);

    // set
    setIsRequesting(true);

    // reqeust
    Request_Post<RES_Generate>(query, req).then((res) => {
      if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorMsg}`);

      // reset status
      setIsRequesting(false);
      setOpenGenerateModal(false);
    });
  }, [target, profile, onError]);

  // Function to trigger the hidden file input
  const onSelectDirectory = async (handleIndex) => {
    const directory = await (window as any).electronAPI.selectDirectory();

    // set selection directory.
    if (directory) 
    {
      setProfile({
        ...profile,
        filePathList: profile.filePathList.map((filePath, index) => {
          if (index === handleIndex) filePath.projectPath = directory;
          return filePath;
        }),
      });
    }
  };

  // request profile
  useEffect(() => {
    if (selectedProfile !== '') {
      // query
      const query = `${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_GENERATE_FIND_PROFILE}?profileName=${selectedProfile}`;
      // reqeust
      Request_Get<RES_FindProfile>(query).then((res) => {
        if (res instanceof RES_Error) onError(`[${res.errorCode}] ${res.errorMsg}`);
        else setProfile(res.profile);
      });
    }
  }, [selectedProfile, onError]);

  // Profile을 선택하지 않은 경우.
  if (selectedProfile === '') {
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
      </Box>
    );
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
              <Tab
                label={filePath.target}
                key={index}
                onClick={() => { setTarget(filePath.target) }}
                {...tabProps(index)} />
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
                  value={filePath.projectPath ?? ''}
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

                <IconButton
                  onClick={() => onSelectDirectory(index)}
                  style={{ padding: 15, margin: 0 }}
                >
                  <FolderIcon />
                </IconButton>
              </Stack>

              {/* ProtocolPath */}
              <TextField
                sx={{ width: '90%' }}
                label="ProtocolPath"
                value={filePath.protocolPath ?? ''}
                variant="filled"
                focused
                onChange={(e) =>
                  setProfile({
                    ...profile,
                    filePathList: profile.filePathList.map((row, rowIndex) => {
                      if (index === rowIndex) row.protocolPath = e.target.value;
                      return row;
                    }),
                  })
                }
              />

              {/* DataCodePath */}
              <TextField
                sx={{ width: '90%' }}
                label="DataCodePath"
                value={filePath.dataCodePath ?? ''}
                variant="filled"
                focused
                onChange={(e) =>
                  setProfile({
                    ...profile,
                    filePathList: profile.filePathList.map((row, rowIndex) => {
                      if (index === rowIndex) row.dataCodePath = e.target.value;
                      return row;
                    }),
                  })
                }
              />

              {/* DatabaseFilePath */}
              <TextField
                sx={{ width: '90%' }}
                label="DatabaseFilePath"
                value={filePath.databaseFilePath ?? ''}
                variant="filled"
                focused
                onChange={(e) =>
                  setProfile({
                    ...profile,
                    filePathList: profile.filePathList.map((row, rowIndex) => {
                      if (index === rowIndex)
                        row.databaseFilePath = e.target.value;
                      return row;
                    }),
                  })
                }
              />
            </CustomTabPanel>
          );
        })}

        {/* Edit, Delete button */}
        <Stack
          direction="row"
          sx={{
            justifyContent: 'flex-end',
            padding: '30px'
          }}
        >
          {/* Generate Button */}
          <Button
            variant="contained"
            onClick={() => onGenerate('Are you sure to generate?')}
          >
            Generate
          </Button>
        </Stack>

      </Box>

      {/* Error modal */}
      <ErrorModal
        openErrorModal={openErrorModal}
        errorMsg={errorMsg}
        setOpenErrorModal={setOpenErrorModal}
      />

      {/* Check modal */}
      <CheckModal
        openCheckModal={openGenerateModal}
        checkMsg={generateMsg}
        setOpenCheckModal={setOpenGenerateModal}
        setOnCheckOk={onRequestGenerate}
      />

      {/* Requesting */}
      <RequestingSpinner isRequesting={isRequesting} />
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
