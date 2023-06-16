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
          Create an environment set for each distinct set of environments your application could move through. For example if you have one set of applications that moves through Dev,Test,Prod and another that moved through Dev,Test,UAT,Prod, you would create two different Environment Sets.
</p><p>
        Variables applied to an Environment Set are inherited by any application tied to that Environment Set. This is a good spot to put global variables used by ALL applications (e.g. the URL of a central logging server)
          {/* TODO: Refresh on adding an env */}
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
