import React, { useState, useEffect } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import ApplicationSettingsClient from '../../applicationSettingsClient';
import EnvironmentSetSettingsClient from '../../environmentSetSettingsClient';


const AddApplicationDialog = ({ open, onClose, onApplicationAdded }) => {
  const [applicationName, setApplicationName] = useState('');
  const [token, setToken] = useState('');
  const settingsClient = new ApplicationSettingsClient();
  const [environmentSets, setEnvironmentSets] = useState([]);
  const [selectedEnvironmentSet, setSelectedEnvironmentSet] = useState('');

  const handleAddApplication = async () => {
    await settingsClient.addApplication(applicationName, selectedEnvironmentSet, token);
    onClose();
    if (onApplicationAdded) {
      onApplicationAdded();
    }
  };


  
  const fetchEnvironmentSets = React.useCallback(async () => {
    const client = new EnvironmentSetSettingsClient();
    const result = await client.getEnvironmentSets(); 
    setEnvironmentSets(result);
  }, []);
  
  useEffect(() => {
    if (open) {
      fetchEnvironmentSets();
    }
  }, [open, fetchEnvironmentSets]);

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
          <Select fullWidth value={selectedEnvironmentSet} onChange={(e) => setSelectedEnvironmentSet(e.target.value)}>
            {environmentSets.map((set) => (
              <MenuItem key={set.id} value={set.id}>{set.name}</MenuItem>
            ))}
          </Select>
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
