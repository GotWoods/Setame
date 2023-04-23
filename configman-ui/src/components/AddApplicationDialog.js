import React, { useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import SettingsClient from '../settingsClient';

const AddApplicationDialog = ({ open, onClose, onApplicationAdded }) => {
  const [applicationName, setApplicationName] = useState('');
  const [token, setToken] = useState('');
  const settingsClient = new SettingsClient;

  const handleAddApplication = async () => {
    await settingsClient.addApplicaiton(applicationName, token);
    onClose();
    if (onApplicationAdded) {
      onApplicationAdded();
    }
  };

  const generateToken = () => {
    // Generate a random token (change this according to your requirements)
    const newToken = Math.random().toString(36).substr(2, 10);
    setToken(newToken);
  };

  return (
    <div>
      <Dialog open={open} onClose={onClose} aria-labelledby="form-dialog-title">
        <DialogTitle id="form-dialog-title">Add Application</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="name"
            label="Application Name"
            type="text"
            fullWidth
            value={applicationName}
            onChange={(e) => setApplicationName(e.target.value)}
          />
          <TextField
            margin="dense"
            id="token"
            label="Application Token"
            type="text"
            fullWidth
            value={token}
            InputProps={{
              readOnly: true,
            }}
          />
          <Button color="primary" onClick={generateToken}>
            Generate Token
          </Button>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} color="primary">
            Cancel
          </Button>
          <Button onClick={handleAddApplication} color="primary">
            Add
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default AddApplicationDialog;
