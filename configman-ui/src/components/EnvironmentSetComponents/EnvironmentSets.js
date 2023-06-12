import React, { useEffect, useState } from 'react';
import Button from '@mui/material/Button';
import AddEnvironmentSetDialog from './AddEnvironmentSetDialog';

import '../../App.css';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';

import SettingsClient from '../../settingsClient';
import EnvironmentSetDetail from './EnvironmentSetDetail';

const EnvironmentSets = () => {
  const [error, setError] = useState('');
  const [environments, setEnvironmentSets] = useState([]);
  const [environmentSetDialogOpen, setEnvironmentSetDialogOpen] = useState(false);
  
  
  const [deleteConfirmationOpen, setDeleteConfirmationOpen] = useState(false);
  //const [currentEnvironment, setCurrentEnvironment] = useState(null);
  //const [environmentDetailsDialogOpen, setEnvironmentDetailsDialogOpen] = useState(false);
  //const [selectedEnvironment, setSelectedEnvironment] = useState(null);
  const settingsClient = new SettingsClient();

  useEffect(() => {
    fetchEnvironmentSets();
  }, []);

  const fetchEnvironmentSets = async () => {
    const data = await settingsClient.getEnvironmentSets();
    setEnvironmentSets(data);
  };

  const handleAddEnvironmentSetDialogClose = () => {
    setEnvironmentSetDialogOpen(false);
  };

  const handleDeleteEnvironmentClick = (env) => {
    setDeleteConfirmationOpen(true);
    //setCurrentEnvironment(env);
  };

  const handleCloseDeleteConfirmation = () => {
    setDeleteConfirmationOpen(false);
  };

  const handleConfirmDeleteEnvironment = async () => {
    setDeleteConfirmationOpen(false);
    //setEnvironmentDetailsDialogOpen(false);
    //await settingsClient.deleteEnvironmentSet(currentEnvironment);
    await fetchEnvironmentSets();
  };

  return (
    <div className="environment-settings">

      <div>

        <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: '1rem', marginTop: '1rem', marginRight: '1rem' }}>
          <Button variant="contained" color="primary" onClick={() => setEnvironmentSetDialogOpen(true)}>
            Add New Environment Set
          </Button>
        </div>

        <h1>
          Environment Sets
        </h1>
        <p>
          Used to create a completely segregated set of environments. Applications are bound to an Environment Set. 
        </p>
      </div>

      <AddEnvironmentSetDialog key={environmentSetDialogOpen ? 'open' : 'closed'} open={environmentSetDialogOpen} onClose={handleAddEnvironmentSetDialogClose} onAdded={fetchEnvironmentSets} />

      {environments.map((env) => (
        <EnvironmentSetDetail key={env.name} enviornmentSet={env} />
      ))}

     
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
{/* 
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
        </Dialog> */}
      </div>
    </div>
  );
};

export default EnvironmentSets;
