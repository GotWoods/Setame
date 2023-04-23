import React, { useEffect, useState } from 'react';
import Button from '@mui/material/Button';
import AddEnvironmentDialog from './AddEnvironmentDialog';
import '../App.css';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { SettingGridData, SettingGridItem } from './SettingGrid/SettingGridData';
import SettingsGrid from './SettingGrid/SettingGrid';
import SettingsClient from '../settingsClient';

const EnvironmentSettings = () => {
  const [error, setError] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [transformedSettings, setTransformedSettings] = useState([]);
  const [deleteConfirmationOpen, setDeleteConfirmationOpen] = useState(false);
  const [currentEnvironment, setCurrentEnvironment] = useState(null);
  const [environmentDetailsDialogOpen, setEnvironmentDetailsDialogOpen] = useState(false);
  const [selectedEnvironment, setSelectedEnvironment] = useState(null);
  const settingsClient = new SettingsClient();

  useEffect(() => {
    fetchEnvironments();
  }, []);

  const fetchEnvironments = async () => {
      const data = await settingsClient.getEnvironments();
      const transformedSettings = loadGrid(data);
      setTransformedSettings(transformedSettings);
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
    await settingsClient.deleteEnvironment(currentEnvironment);
  };

  const loadGrid = (environments) => {
    var result = new SettingGridData();
    environments.forEach((env) => {
      result.environments.push(env.name);
      if (env.settings == undefined)
        return;
      env.settings.forEach((setting) => {
        if (!result.settings[setting.name]) {
          result.settings[setting.name] = [];
        }

        if (!result.settings[setting.name][env.name]) {
          result.settings[setting.name][env.name] = "";
        }
        result.settings[setting.name][env.name] = setting.value;
      });

      console.log("final", result);
    });
    return result;
  }

  const handleEnvironmentDetailsClick = (env) => {
    setSelectedEnvironment(env);
    setCurrentEnvironment(env.name);
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

    await settingsClient.updateEnvironmentSettings(allSettings);
    fetchEnvironments();
  };

  const handleSettingChange = async (settingName, environment, newValue) => {
    // Update the API with the new setting value
    await settingsClient.updateEnvironmentSettings2(settingName, environment, newValue);
    fetchEnvironments();
  };

  return (
    <div className="environment-settings">

      <div>

        <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: '1rem', marginTop: '1rem', marginRight: '1rem' }}>
          <Button variant="contained" color="primary" onClick={() => setDialogOpen(true)}>
            Add New Environment
          </Button>
        </div>

        <h1>
          Environments
        </h1>
        <p>
          TODO: Ability to set order of environments<br />
          TODO: Edit/Delete settings<br />
          TODO: Secret values<br />
          TODO: Edit Environment dialog sucks<br />
        </p>
      </div>

      <AddEnvironmentDialog key={dialogOpen ? 'open' : 'closed'} open={dialogOpen} onClose={handleDialogClose} onEnvironmentAdded={fetchEnvironments} />

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

export default EnvironmentSettings;
