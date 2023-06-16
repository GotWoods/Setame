import React, { useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import SettingsClient from '../../settingsClient';


const AddEnvironmentSetDialog = ({ open, onClose, onAdded }) => {
  const [environmentName, setEnvironmentName] = useState('');
  const settingsClient = new SettingsClient();

  const handleAddEnvironment = async () => {
    console.log("Handling add env set");
    await settingsClient.addEnvironmentSet(environmentName);
    onClose();
    if (onAdded) {
      onAdded();
    }
  };

    return (
      <div>
        <Dialog open={open} onClose={onClose} aria-labelledby="form-dialog-title">
          <DialogTitle id="form-dialog-title">Add Environment Set</DialogTitle>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              id="name"
              label="Environment Name"
              type="text"
              fullWidth
              value={environmentName}
              onChange={(e) => setEnvironmentName(e.target.value)}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={onClose} color="primary">
              Cancel
            </Button>
            <Button onClick={handleAddEnvironment} color="primary">
              Add
            </Button>
          </DialogActions>
        </Dialog>
      </div>
    );
  };

  export default AddEnvironmentSetDialog;
