import React, { useEffect, useState } from 'react';
import Button from '@mui/material/Button';
import AddEnvironmentGroupDialog from './AddEnvironmentGroupDialog';

import '../App.css';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import SettingsGrid from './SettingGrid/SettingGrid';
import SettingsClient from '../settingsClient';

const EnvironmentGroups = () => {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [environmentGroups, setEnvironmentGroups] = useState([]);
  const [environments, setEnvironments] = useState([]);

  const [transformedSettings] = useState([]);
  const [deleteConfirmationOpen, setDeleteConfirmationOpen] = useState(false);
  const [currentEnvironment, setCurrentEnvironment] = useState(null);
  const [environmentDetailsDialogOpen, setEnvironmentDetailsDialogOpen] = useState(false);
  const [selectedEnvironment, setSelectedEnvironment] = useState(null);
  const settingsClient = new SettingsClient();

  useEffect(() => {
    fetchEnvironmentGroups();
  }, []);

  const fetchEnvironmentGroups = async () => {
    const envData = await settingsClient.getEnvironmentSets();
    const data = await settingsClient.getEnvironmentGroups();
    setEnvironmentGroups(data);
    setEnvironments(envData);
    //const transformedSettings = loadGrid(data);
    //setTransformedSettings(transformedSettings);
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
  };

  const handleDeleteEnvironmentClick = (env) => {
    setDeleteConfirmationOpen(true);
    setCurrentEnvironment(env);
  };

  const handleCloseDeleteConfirmation = () => {
    setDeleteConfirmationOpen(false);
  };

  const handleConfirmDeleteEnvironment = async () => {
    setDeleteConfirmationOpen(false);
    setEnvironmentDetailsDialogOpen(false);
    await settingsClient.deleteEnvironmentSet(currentEnvironment);
    await fetchEnvironmentGroups();
  };

  // const loadGrid = (environments) => {
  //   var result = new SettingGridData();
  //   environments.forEach((env) => {
  //     result.environments.push(env.name);
  //     if (env.settings == undefined)
  //       return;
  //     env.settings.forEach((setting) => {
  //       if (!result.settings[setting.name]) {
  //         result.settings[setting.name] = [];
  //       }

  //       if (!result.settings[setting.name][env.name]) {
  //         result.settings[setting.name][env.name] = "";
  //       }
  //       result.settings[setting.name][env.name] = setting.value;
  //     });

  //   });
  //   return result;
  // }

  const handleEnvironmentDetailsClick = (env) => {
    setSelectedEnvironment(env);
    setCurrentEnvironment(env);
    setEnvironmentDetailsDialogOpen(true);
  };

  const handleAddEnvironmentSetting = async (newEnvironmentSettingName, newEnvironmentSettings) => {
    let keys = Object.keys(newEnvironmentSettings);
    let allSettings = keys.map(env => {
      return {
        environment: env,
        name: newEnvironmentSettingName,
        value: newEnvironmentSettings[env] || '',
      }
    });

    await settingsClient.addEnvironmentSettings(allSettings);
    fetchEnvironmentGroups();
  };

  const handleSettingChange = async (settingName, environment, newValue) => {
    // Update the API with the new setting value
    await settingsClient.updateEnvironmentSet(settingName, environment, newValue);
    fetchEnvironmentGroups();
  };

  return (
    <div className="environment-settings">
      <div>

        <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: '1rem', marginTop: '1rem', marginRight: '1rem' }}>
          <Button variant="contained" color="primary" onClick={() => setDialogOpen(true)}>
            Add New Environment Group
          </Button>
        </div>

        <h1>
          Environment Groups
        </h1>
        <p>
          Create a set of resuable variables with a setting per environment. These can be attached to an application
        </p>
      </div>


      {environmentGroups.map((app) => (
        <>
          <h2>{app.name}</h2>
          TODO: grid here of setting name + environments (e.g. dev/stage/preprod/prod)
          {environments.map((env) => (
            <>
              {env.name}
            </>
          ))}
        </>
      ))}

      <AddEnvironmentGroupDialog key={dialogOpen ? 'open' : 'closed'} open={dialogOpen} onClose={handleDialogClose}
        onAdded={fetchEnvironmentGroups} />

      {transformedSettings.environments && (
        <SettingsGrid
          transformedSettings={transformedSettings}
          onAddSetting={handleAddEnvironmentSetting}
          onHeaderClick={handleEnvironmentDetailsClick}
          onSettingChange={handleSettingChange}
        />
      )}

      <div>
        {/* Other components */}
        <Dialog
          open={deleteConfirmationOpen}
          onClose={handleCloseDeleteConfirmation}
          aria-labelledby="delete-confirmation-dialog-title"
          aria-describedby="delete-confirmation-dialog-description"
        >
          <DialogTitle id="delete-confirmation-dialog-title">
            {'Delete Environment'}
          </DialogTitle>
          <DialogContent>
            <DialogContentText id="delete-confirmation-dialog-description">
              Are you sure you want to delete this environment? This action cannot be undone.
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDeleteConfirmation} color="primary">
              Cancel
            </Button>
            <Button onClick={handleConfirmDeleteEnvironment} color="secondary">
              Delete
            </Button>
          </DialogActions>
        </Dialog>

        <Dialog
          open={environmentDetailsDialogOpen}
          onClose={() => setEnvironmentDetailsDialogOpen(false)}
          aria-labelledby="environment-details-dialog-title"
          aria-describedby="environment-details-dialog-description"
        >
          <DialogTitle id="environment-details-dialog-title">
            {selectedEnvironment && `Environment: ${selectedEnvironment.name}`}
          </DialogTitle>
          <DialogContent>
            <DialogContentText id="environment-details-dialog-description">
              Token: {selectedEnvironment && selectedEnvironment.token}
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setEnvironmentDetailsDialogOpen(false)} color="primary">
              Close
            </Button>
            <Button onClick={() => handleDeleteEnvironmentClick(currentEnvironment)} color="secondary">
              Delete
            </Button>
          </DialogActions>
        </Dialog>
      </div>
    </div>
  );
};

export default EnvironmentGroups;
