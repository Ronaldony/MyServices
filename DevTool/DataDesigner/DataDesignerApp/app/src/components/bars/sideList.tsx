import React, { Fragment } from 'react';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import DataObjectIcon from '@mui/icons-material/DataObject';
import SaveAltIcon from '@mui/icons-material/SaveAlt';
import SettingsApplicationsIcon from '@mui/icons-material/SettingsApplications';
import TypeSpecimenIcon from '@mui/icons-material/TypeSpecimen';
import { useNavigate } from 'react-router-dom';

export default function SideList() {
  const [seletecIndex, setSeletecIndex] = React.useState(0);
  const navigate = useNavigate();

  const updateMenu = React.useCallback(
    (index, redirectTo) => {
      setSeletecIndex(index);
      navigate(redirectTo);
    },
    [setSeletecIndex, navigate]
  );

  return (
    <Fragment>
      {/* List */}
      <List>
        {/* Configuration */}
        <ListItemButton
          selected={seletecIndex === 0}
          onClick={() => updateMenu(0, '/data/configuration')}
        >
          <ListItemIcon>
            <SettingsApplicationsIcon />
          </ListItemIcon>
          <ListItemText primary={'Configuration'} />
        </ListItemButton>
        {/* Type */}
        <ListItemButton
          selected={seletecIndex === 1}
          onClick={() => updateMenu(1, '/data/type')}
        >
          <ListItemIcon>
            <TypeSpecimenIcon />
          </ListItemIcon>
          <ListItemText primary={'Type'} />
        </ListItemButton>
        {/* Table */}
        <ListItemButton
          selected={seletecIndex === 2}
          onClick={() => updateMenu(2, '/data/table')}
        >
          <ListItemIcon>
            <DataObjectIcon />
          </ListItemIcon>
          <ListItemText primary={'Table'} />
        </ListItemButton>
        {/* Generate */}
        <ListItemButton
          selected={seletecIndex === 3}
          onClick={() => updateMenu(3, '/generate')}
        >
          <ListItemIcon>
            <SaveAltIcon />
          </ListItemIcon>
          <ListItemText primary={'Generate'} />
        </ListItemButton>
        {/* Test */}
        <ListItemButton
          selected={seletecIndex === 4}
          onClick={() => updateMenu(4, '/')}
        >
          <ListItemIcon></ListItemIcon>
          <ListItemText primary={'Test'} />
        </ListItemButton>
      </List>
    </Fragment>
  );
}
