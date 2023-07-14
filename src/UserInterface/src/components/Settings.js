import React from 'react';
import { Typography } from '@mui/material';

const Settings = () => {
  return (
    <div className="users">
      <Typography variant="h4" gutterBottom>
        Settings
      </Typography>
      <Typography variant="body1" gutterBottom>
        <ul>
            <li>Change History Retention</li>
            <li>Usage History Retention (whenever an app gets config, should we keep the history of this?) </li>
            <li>Application Token Expiration?</li>
        </ul>
      </Typography>
    </div>
  );
};

export default Settings;
