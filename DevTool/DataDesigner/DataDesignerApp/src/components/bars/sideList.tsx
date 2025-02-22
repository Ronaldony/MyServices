import DataObjectIcon from '@mui/icons-material/DataObject';
import SaveAltIcon from '@mui/icons-material/SaveAlt';
import SettingsApplicationsIcon from '@mui/icons-material/SettingsApplications';
import TypeSpecimenIcon from '@mui/icons-material/TypeSpecimen';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import React, { Fragment, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

export default function SideList() {
  const [index, setIndex] = React.useState(-1);

  // navigate
  const navigate = useNavigate();
  const location = useLocation();

  // initialize index.
  useEffect(() => {
    switch (location.pathname) {
      case "/data/configuration":
        setIndex(0)
        break;
      case "/data/type":
        setIndex(1)
        break;
      case "/data/table":
        setIndex(2)
        break;
      case "/generate":
        setIndex(3)
        break;
    }
  }, [location.pathname])

  return (
    <Fragment>
      {/* List */}
      <List>
        {/* Configuration */}
        <ListItemButton
          selected={index === 0}
          onClick={() => navigate('/data/configuration')}
        >
          <ListItemIcon>
            <SettingsApplicationsIcon />
          </ListItemIcon>
          <ListItemText primary={'Configuration'} />
        </ListItemButton>
        {/* Type */}
        <ListItemButton
          selected={index === 1}
          onClick={() => navigate('/data/type')}
        >
          <ListItemIcon>
            <TypeSpecimenIcon />
          </ListItemIcon>
          <ListItemText primary={'Type'} />
        </ListItemButton>
        {/* Table */}
        <ListItemButton
          selected={index === 2}
          onClick={() => navigate('/data/table')}
        >
          <ListItemIcon>
            <DataObjectIcon />
          </ListItemIcon>
          <ListItemText primary={'Table'} />
        </ListItemButton>
        {/* Generate */}
        <ListItemButton
          selected={index === 3}
          onClick={() => navigate('/generate')}
        >
          <ListItemIcon>
            <SaveAltIcon />
          </ListItemIcon>
          <ListItemText primary={'Generate'} />
        </ListItemButton>
      </List>
    </Fragment>
  );
}
