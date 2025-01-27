import AddCircleIcon from '@mui/icons-material/AddCircle';
import {
  Divider,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from '@mui/material';
import React, { Fragment, useState } from 'react';

export default function DataList({
  setSelectedData,
  setCreateData,
  dataList,
}: any) {
  const [selectedItem, setSelectedItem] = useState();

  return (
    <Fragment>
      {/* List */}
      <List
        sx={{
          minWidth: '120px',
          overflow: 'auto',
          borderRight: '1px solid silver',
        }}
      >
        {/* Create */}
        <ListItem>
          <ListItemButton onClick={() => setCreateData(true)}>
            <ListItemIcon>
              <AddCircleIcon />
            </ListItemIcon>
            <ListItemText primary={'Create'} />
          </ListItemButton>
        </ListItem>
        <Divider />

        {/* DataList */}
        {dataList === undefined
          ? ''
          : dataList.map((data) => (
              <ListItem disablePadding key={data}>
                <ListItemButton
                  selected={selectedItem === data}
                  onClick={() => {
                    setSelectedData(data);
                    setSelectedItem(data);
                  }}
                >
                  <ListItemText primary={data} />
                </ListItemButton>
              </ListItem>
            ))}
      </List>
    </Fragment>
  );
}
