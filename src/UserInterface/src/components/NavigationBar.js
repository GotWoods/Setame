import React from 'react';
import { AppBar, Toolbar, Typography, Button } from '@mui/material';
import { styled } from '@mui/system';
import SettingsClient from '../clients/settingsClient';

const StyledAppBar = styled(AppBar)({
  flexGrow: 1,
});

const TitleTypography = styled(Typography)({
  flexGrow: 1,
});

const NavigationBar = () => {
  const settingsClient = new SettingsClient();
  let token = localStorage.getItem('authToken');
  if (settingsClient.isJwtTokenExpired())
  {
    token = null;
  }

  return (
    <StyledAppBar position="static">
      <Toolbar>
        <TitleTypography variant="h6" onClick={() => window.location.href = '/'}>Setame</TitleTypography>
        {token && (
          <>
        <Button color="inherit" href="/environmentSets">
          Environment Sets
        </Button>
        <Button color="inherit" href="/applications">
          Applications
        </Button>
        {/* <Button color="inherit" href="/environmentGroups">
          Environment Groups
        </Button>
        <Button color="inherit" href="/variableGroups">
          Variable Groups
        </Button> */}
        {/* <Button color="inherit" href="/users">
          Users
        </Button>
        <Button color="inherit" href="/settings">
          Settings
        </Button> */}
        </>
        )}
      </Toolbar>
    </StyledAppBar>
  );
};

export default NavigationBar;
